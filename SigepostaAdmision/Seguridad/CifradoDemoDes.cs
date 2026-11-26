using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SigepostaAdmision.Seguridad
{
    /// <summary>
    /// SOLO DEMOSTRACIÓN (lo pide el sílabo): cifrado simétrico con DES usando
    /// DESCryptoServiceProvider y CryptoStream. NO se usa en el sistema real.
    ///
    /// ¿Por qué NO se usa DES en producción?
    ///   - Su clave efectiva es de solo 56 bits: hay 2^56 claves posibles, y desde
    ///     1998 se puede romper por fuerza bruta en horas o días con hardware
    ///     dedicado (y hoy es mucho más rápido).
    ///   - Sus bloques son de 64 bits (pequeños), lo que facilita ciertos ataques
    ///     cuando se cifran muchos datos con la misma clave.
    ///   - El NIST lo retiró como estándar en 2005; lo reemplazó AES.
    /// Por eso el sistema usa AES de 256 bits (CifradoAes). El procedimiento con
    /// CryptoStream es el mismo; solo cambia el algoritmo.
    /// </summary>
    public static class CifradoDemoDes
    {
        /// <summary>
        /// Muestra un ciclo completo (clave e IV aleatorios → cifrar → descifrar)
        /// en el panel de salida con Debug.WriteLine. Devuelve el mismo texto
        /// para poder verlo también fuera de Visual Studio.
        /// </summary>
        public static string DemoCifrarDescifrar()
        {
            const string original = "DNI de prueba: 12345678";
            StringBuilder informe = new StringBuilder();

            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                // DES genera una clave de 8 bytes (64 bits, de los cuales 56 son útiles)
                // y un IV de 8 bytes (su tamaño de bloque).
                des.GenerateKey();
                des.GenerateIV();
                des.Mode = CipherMode.CBC;
                des.Padding = PaddingMode.PKCS7;

                // --- Cifrar con CryptoStream ---
                byte[] cifrado;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        byte[] datos = Encoding.UTF8.GetBytes(original);
                        cs.Write(datos, 0, datos.Length);
                        cs.FlushFinalBlock();
                    }
                    cifrado = ms.ToArray();
                }

                // --- Descifrar con CryptoStream ---
                string descifrado;
                using (MemoryStream ms = new MemoryStream(cifrado))
                using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Read))
                using (StreamReader sr = new StreamReader(cs, Encoding.UTF8))
                {
                    descifrado = sr.ReadToEnd();
                }

                informe.AppendLine("=== DEMO DES (no usar en producción) ===");
                // La clave NUNCA se escribe (ni siquiera en Debug): solo su longitud.
                informe.AppendLine("Clave:     " + des.Key.Length + " bytes (56 bits útiles; no se muestra)");
                informe.AppendLine("IV:        " + BitConverter.ToString(des.IV));
                informe.AppendLine("Original:  " + original);
                informe.AppendLine("Cifrado:   " + BitConverter.ToString(cifrado) + " (" + cifrado.Length + " bytes)");
                informe.AppendLine("Descifrado: " + descifrado);
                informe.AppendLine("¿Coincide?: " + (descifrado == original));
            }

            Debug.WriteLine(informe.ToString());
            return informe.ToString();
        }
    }
}
