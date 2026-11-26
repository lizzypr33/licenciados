using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SigepostaAdmision.Seguridad
{
    /// <summary>
    /// Contenedor cifrado para los archivos de respaldo (".sigebak") y de claves
    /// (".sigekeys"), protegido con una CONTRASEÑA que elige el administrador.
    ///
    /// Formato del archivo (todo en bytes, uno detrás de otro):
    ///   [ "SGBK1" 5 bytes ][ sal 16 ][ IV 16 ][ datos cifrados ... ][ HMAC-SHA256 32 ]
    ///
    /// Cadena de procesamiento al exportar (RespaldoNegocio):
    ///   objeto → XmlSerializer → XML → GZipStream (comprime) → AES (cifra) → HMAC (autentica) → archivo
    ///
    /// ¿Por qué se COMPRIME ANTES de cifrar? El cifrado produce bytes que parecen
    /// aleatorios: ya no tienen patrones repetidos y no se pueden comprimir. El XML,
    /// en cambio, repite mucho (etiquetas iguales), así que se comprime muy bien.
    /// Comprimir después de cifrar no reduciría casi nada.
    ///
    /// ¿Por qué una SAL? La clave se deriva de la contraseña con PBKDF2. Con una sal
    /// aleatoria por archivo, la misma contraseña produce claves distintas en cada
    /// respaldo, y un atacante no puede usar tablas precalculadas.
    /// ¿Por qué 100 000 iteraciones? PBKDF2 repite el cálculo a propósito para que
    /// cada intento de adivinar la contraseña sea lento (fuerza bruta muy costosa).
    ///
    /// ¿Por qué un IV? Igual que en CifradoAes: el mismo contenido cifrado dos veces
    /// da resultados distintos (modo CBC con IV aleatorio).
    ///
    /// ¿Para qué el HMAC? AES-CBC oculta el contenido pero NO detecta cambios: si
    /// alguien altera bytes del archivo, el descifrado podría devolver basura sin
    /// avisar. El HMAC es una "firma" con clave secreta sobre todo el archivo:
    /// al abrir se verifica PRIMERO; si no coincide (contraseña incorrecta o archivo
    /// modificado), no se descifra nada. Este esquema se llama "Encrypt-then-MAC".
    ///
    /// De la contraseña se derivan 64 bytes: 32 para la clave AES-256 y 32 para la
    /// clave HMAC (claves distintas para cada propósito).
    /// </summary>
    public static class CifradoRespaldo
    {
        private static readonly byte[] Marca = Encoding.ASCII.GetBytes("SGBK1");
        private const int TamanoSal = 16;
        private const int TamanoIv = 16;
        private const int TamanoHmac = 32;
        private const int Iteraciones = 100000;

        private static readonly int TamanoMinimo = Marca.Length + TamanoSal + TamanoIv + 16 + TamanoHmac;

        /// <summary>¿Los bytes tienen el formato de un archivo cifrado de SIGEPOSTA (empiezan con "SGBK1")?</summary>
        public static bool EsRespaldoCifrado(byte[] archivo)
        {
            if (archivo == null || archivo.Length < TamanoMinimo)
            {
                return false;
            }
            for (int i = 0; i < Marca.Length; i++)
            {
                if (archivo[i] != Marca[i]) return false;
            }
            return true;
        }

        /// <summary>Cifra y autentica los datos (normalmente ya comprimidos) con la contraseña.</summary>
        public static byte[] Proteger(byte[] datosComprimidos, string contrasena)
        {
            if (datosComprimidos == null) throw new ArgumentNullException("datosComprimidos");
            if (string.IsNullOrEmpty(contrasena)) throw new ArgumentException("La contraseña es obligatoria.", "contrasena");

            // 1) Sal e IV aleatorios con un generador criptográfico.
            byte[] sal = Aleatorio(TamanoSal);
            byte[] iv = Aleatorio(TamanoIv);

            byte[] claveAes, claveHmac;
            DerivarClaves(contrasena, sal, out claveAes, out claveHmac);
            try
            {
                using (MemoryStream salida = new MemoryStream())
                {
                    // 2) Cabecera: marca + sal + IV (no son secretos; se necesitan para abrir).
                    salida.Write(Marca, 0, Marca.Length);
                    salida.Write(sal, 0, sal.Length);
                    salida.Write(iv, 0, iv.Length);

                    // 3) Datos cifrados con AES-256-CBC + PKCS7 mediante CryptoStream.
                    using (Aes aes = CrearAes(claveAes, iv))
                    using (ICryptoTransform cifrador = aes.CreateEncryptor())
                    using (CryptoStream cs = new CryptoStream(new NoCerrar(salida), cifrador, CryptoStreamMode.Write))
                    {
                        cs.Write(datosComprimidos, 0, datosComprimidos.Length);
                        cs.FlushFinalBlock();
                    }

                    // 4) HMAC-SHA256 sobre TODO lo anterior (cifrar y luego autenticar).
                    byte[] cuerpo = salida.ToArray();
                    using (HMACSHA256 hmac = new HMACSHA256(claveHmac))
                    {
                        byte[] firma = hmac.ComputeHash(cuerpo);
                        salida.Write(firma, 0, firma.Length);
                    }
                    return salida.ToArray();
                }
            }
            finally
            {
                // Las claves derivadas no deben quedar en memoria más de lo necesario.
                Array.Clear(claveAes, 0, claveAes.Length);
                Array.Clear(claveHmac, 0, claveHmac.Length);
            }
        }

        /// <summary>
        /// Verifica el HMAC y, solo si es correcto, descifra y devuelve los datos.
        /// Si la contraseña es incorrecta o el archivo fue alterado, lanza
        /// CryptographicException("Contraseña incorrecta o archivo alterado").
        /// </summary>
        public static byte[] Abrir(byte[] archivo, string contrasena)
        {
            if (!EsRespaldoCifrado(archivo))
            {
                throw new InvalidDataException("El archivo no es un respaldo cifrado de SIGEPOSTA.");
            }
            if (string.IsNullOrEmpty(contrasena)) throw new ArgumentException("La contraseña es obligatoria.", "contrasena");

            int inicioSal = Marca.Length;
            int inicioIv = inicioSal + TamanoSal;
            int inicioDatos = inicioIv + TamanoIv;
            int inicioHmac = archivo.Length - TamanoHmac;

            byte[] sal = Copiar(archivo, inicioSal, TamanoSal);
            byte[] iv = Copiar(archivo, inicioIv, TamanoIv);
            byte[] hmacGuardado = Copiar(archivo, inicioHmac, TamanoHmac);

            byte[] claveAes, claveHmac;
            DerivarClaves(contrasena, sal, out claveAes, out claveHmac);
            try
            {
                // 1) PRIMERO se autentica: HMAC de todo menos los últimos 32 bytes.
                byte[] hmacCalculado;
                using (HMACSHA256 hmac = new HMACSHA256(claveHmac))
                {
                    hmacCalculado = hmac.ComputeHash(archivo, 0, inicioHmac);
                }
                if (!IgualesTiempoConstante(hmacCalculado, hmacGuardado))
                {
                    throw new CryptographicException("Contraseña incorrecta o archivo alterado.");
                }

                // 2) Recién ahora se descifra.
                using (Aes aes = CrearAes(claveAes, iv))
                using (ICryptoTransform descifrador = aes.CreateDecryptor())
                using (MemoryStream entrada = new MemoryStream(archivo, inicioDatos, inicioHmac - inicioDatos))
                using (CryptoStream cs = new CryptoStream(entrada, descifrador, CryptoStreamMode.Read))
                using (MemoryStream salida = new MemoryStream())
                {
                    cs.CopyTo(salida);
                    return salida.ToArray();
                }
            }
            finally
            {
                Array.Clear(claveAes, 0, claveAes.Length);
                Array.Clear(claveHmac, 0, claveHmac.Length);
            }
        }

        // ===================== Apoyo =====================

        // PBKDF2 (Rfc2898DeriveBytes) con SHA-256 y 100 000 iteraciones → 64 bytes.
        private static void DerivarClaves(string contrasena, byte[] sal, out byte[] claveAes, out byte[] claveHmac)
        {
            using (Rfc2898DeriveBytes kdf = new Rfc2898DeriveBytes(contrasena, sal, Iteraciones, HashAlgorithmName.SHA256))
            {
                byte[] material = kdf.GetBytes(64);
                claveAes = Copiar(material, 0, 32);
                claveHmac = Copiar(material, 32, 32);
                Array.Clear(material, 0, material.Length);
            }
        }

        private static Aes CrearAes(byte[] clave, byte[] iv)
        {
            Aes aes = Aes.Create();
            aes.KeySize = 256;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = clave;
            aes.IV = iv;
            return aes;
        }

        /// <summary>
        /// Compara recorriendo SIEMPRE todos los bytes: así el tiempo no revela en
        /// qué posición falló la comparación (ataque de temporización).
        /// </summary>
        private static bool IgualesTiempoConstante(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) return false;
            int diferencia = 0;
            for (int i = 0; i < a.Length; i++)
            {
                diferencia |= a[i] ^ b[i];
            }
            return diferencia == 0;
        }

        private static byte[] Aleatorio(int tamano)
        {
            byte[] bytes = new byte[tamano];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(bytes);
            }
            return bytes;
        }

        private static byte[] Copiar(byte[] origen, int inicio, int cantidad)
        {
            byte[] destino = new byte[cantidad];
            Buffer.BlockCopy(origen, inicio, destino, 0, cantidad);
            return destino;
        }

        /// <summary>
        /// Envoltura que evita que CryptoStream cierre el MemoryStream de salida al
        /// liberarse (se necesita seguir escribiendo el HMAC después).
        /// (.NET Framework 4.7.2 no tiene el parámetro leaveOpen en CryptoStream.)
        /// </summary>
        private sealed class NoCerrar : Stream
        {
            private readonly Stream interno;
            public NoCerrar(Stream interno) { this.interno = interno; }
            public override bool CanRead { get { return false; } }
            public override bool CanSeek { get { return false; } }
            public override bool CanWrite { get { return true; } }
            public override long Length { get { return interno.Length; } }
            public override long Position { get { return interno.Position; } set { throw new NotSupportedException(); } }
            public override void Flush() { interno.Flush(); }
            public override int Read(byte[] buffer, int offset, int count) { throw new NotSupportedException(); }
            public override long Seek(long offset, SeekOrigin origin) { throw new NotSupportedException(); }
            public override void SetLength(long value) { throw new NotSupportedException(); }
            public override void Write(byte[] buffer, int offset, int count) { interno.Write(buffer, offset, count); }
            protected override void Dispose(bool disposing) { /* No se cierra el flujo interno */ }
        }
    }
}
