using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SigepostaAdmision.Seguridad
{
    /// <summary>
    /// Cifrado SIMÉTRICO con AES (Advanced Encryption Standard), el estándar actual
    /// del NIST. Se usa para datos que hay que poder LEER de nuevo (DNI, diagnóstico).
    /// (A diferencia de las contraseñas, que se guardan con hash porque nunca se
    /// necesita recuperarlas.)
    ///
    /// Configuración elegida:
    ///   - Clave de 256 bits (AES-256): el tamaño más fuerte de AES.
    ///   - Modo CBC: cada bloque se mezcla con el anterior, así dos textos iguales
    ///     no producen bloques cifrados iguales.
    ///   - Relleno PKCS7: AES trabaja en bloques de 16 bytes; PKCS7 completa el
    ///     último bloque de forma estándar y reversible.
    ///   - IV (vector de inicialización) ALEATORIO y distinto en cada cifrado: el
    ///     mismo DNI cifrado dos veces da resultados distintos, y quien vea la base
    ///     no puede saber si dos pacientes tienen el mismo DNI. El IV no es secreto:
    ///     se guarda delante del texto cifrado (IV + datos).
    ///
    /// Formato del resultado: [16 bytes de IV][bytes cifrados]. Un DNI (8 bytes)
    /// ocupa 16 + 16 = 32 bytes, dentro de la columna VARBINARY(64).
    ///
    /// Mejora posible para un sistema en producción: añadir un MAC (por ejemplo
    /// HMAC sobre IV + datos, "Encrypt-then-MAC") o usar AES-GCM para detectar
    /// también si alguien modificó los bytes cifrados.
    /// </summary>
    public static class CifradoAes
    {
        private const int TamanoIv = 16;   // AES usa bloques de 128 bits = 16 bytes

        /// <summary>Cifra un texto (UTF-8) y devuelve IV + texto cifrado.</summary>
        public static byte[] Cifrar(string texto)
        {
            if (texto == null) throw new ArgumentNullException("texto");

            using (Aes aes = CrearAes())
            {
                // 1) IV aleatorio nuevo para ESTE cifrado.
                aes.GenerateIV();
                byte[] iv = aes.IV;

                using (MemoryStream salida = new MemoryStream())
                {
                    // 2) El IV va al inicio del resultado (no es secreto, se necesita para descifrar).
                    salida.Write(iv, 0, iv.Length);

                    // 3) CryptoStream: todo lo que se escribe en él sale cifrado hacia "salida".
                    using (ICryptoTransform cifrador = aes.CreateEncryptor(aes.Key, iv))
                    using (CryptoStream cs = new CryptoStream(salida, cifrador, CryptoStreamMode.Write))
                    {
                        byte[] datos = Encoding.UTF8.GetBytes(texto);
                        cs.Write(datos, 0, datos.Length);
                        // 4) FlushFinalBlock agrega el relleno PKCS7 y cifra el último bloque.
                        cs.FlushFinalBlock();
                    }
                    return salida.ToArray();
                }
            }
        }

        /// <summary>Descifra el resultado de Cifrar (IV + texto cifrado) y devuelve el texto.</summary>
        public static string Descifrar(byte[] datos)
        {
            return Descifrar(datos, GestorClaves.ClaveAes);
        }

        /// <summary>
        /// Sobrecarga que descifra con una clave AES indicada (no la del sistema).
        /// La usa la importación de claves para PROBAR unas claves candidatas antes
        /// de reemplazar las actuales.
        /// </summary>
        public static string Descifrar(byte[] datos, byte[] claveAes)
        {
            if (datos == null) throw new ArgumentNullException("datos");
            if (claveAes == null || claveAes.Length != 32) throw new ArgumentException("La clave AES debe medir 32 bytes.", "claveAes");
            if (datos.Length < TamanoIv * 2)
            {
                throw new CryptographicException("Los datos cifrados están incompletos.");
            }

            using (Aes aes = CrearAes(claveAes))
            {
                // 1) Se recupera el IV de los primeros 16 bytes.
                byte[] iv = new byte[TamanoIv];
                Buffer.BlockCopy(datos, 0, iv, 0, TamanoIv);

                // 2) Se descifra el resto con la misma clave y ese IV.
                using (ICryptoTransform descifrador = aes.CreateDecryptor(aes.Key, iv))
                using (MemoryStream entrada = new MemoryStream(datos, TamanoIv, datos.Length - TamanoIv))
                using (CryptoStream cs = new CryptoStream(entrada, descifrador, CryptoStreamMode.Read))
                using (StreamReader lector = new StreamReader(cs, Encoding.UTF8))
                {
                    // 3) Al leer, CryptoStream descifra y quita el relleno PKCS7.
                    //    Si la clave no es la correcta, aquí salta CryptographicException.
                    return lector.ReadToEnd();
                }
            }
        }

        // AES-256, CBC, PKCS7 con la clave de GestorClaves.
        private static Aes CrearAes()
        {
            return CrearAes(GestorClaves.ClaveAes);
        }

        // AES-256, CBC, PKCS7 con la clave indicada.
        private static Aes CrearAes(byte[] clave)
        {
            Aes aes = Aes.Create();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = clave;
            return aes;
        }
    }
}
