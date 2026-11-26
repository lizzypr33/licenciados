using System;
using System.Security.Cryptography;
using System.Text;

namespace SigepostaAdmision.Seguridad
{
    /// <summary>
    /// Solo demostración del sílabo (Unidad 4). SHA1 está obsoleto: las contraseñas
    /// usan SHA-256 con sal y el DNI usa HMAC-SHA256.
    ///
    /// (SHA1 produce 160 bits y desde 2017 se conocen colisiones prácticas —dos
    /// entradas distintas con el mismo hash—, por eso ya no se considera seguro.)
    /// No se usa en el sistema real.
    /// </summary>
    public static class HashDemoSha1
    {
        /// <summary>SHA1 del texto (UTF-8) en hexadecimal mayúsculas (40 caracteres).</summary>
        public static string DemoHash(string texto)
        {
            if (texto == null) throw new ArgumentNullException("texto");

            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(texto));
                StringBuilder sb = new StringBuilder(hash.Length * 2);
                foreach (byte b in hash)
                {
                    sb.Append(b.ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
