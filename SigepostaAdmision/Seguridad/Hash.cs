using System;
using System.Security.Cryptography;
using System.Text;

namespace SigepostaAdmision.Seguridad
{
    /// <summary>
    /// Utilidades para proteger las contraseñas mediante hash con sal (salt).
    ///
    /// ¿Por qué no se guarda la contraseña? Si alguien roba la base de datos no
    /// debe poder leer las claves. Un hash es una función de un solo sentido:
    /// desde la clave se obtiene el hash, pero desde el hash no se recupera la clave.
    ///
    /// ¿Por qué SHA-256 y no SHA-1 (o MD5)? SHA-1 está OBSOLETO: desde 2017 se
    /// conocen colisiones prácticas (ataque "SHAttered") y organismos como el
    /// NIST desaconsejan su uso. SHA-256 (familia SHA-2) produce 256 bits y no
    /// tiene ataques prácticos conocidos.
    ///
    /// ¿Por qué con sal? La sal es un valor aleatorio distinto para cada usuario
    /// que se combina con la clave antes de calcular el hash. Así:
    ///   - Dos usuarios con la misma clave tienen hashes diferentes.
    ///   - Las "tablas arcoíris" (hashes precalculados de claves comunes) no sirven,
    ///     porque el atacante tendría que recalcularlas para cada sal.
    /// La sal no es secreta: se guarda junto al hash en la tabla Usuario.
    ///
    /// Nota académica: en sistemas de producción se recomiendan además algoritmos
    /// lentos a propósito (PBKDF2, bcrypt, Argon2) que dificultan la fuerza bruta.
    /// </summary>
    public static class Hash
    {
        private const int BytesSalt = 16;   // 16 bytes = 32 caracteres hexadecimales

        /// <summary>
        /// Genera una sal aleatoria de 16 bytes con un generador criptográficamente
        /// seguro (RNGCryptoServiceProvider), no con Random, que es predecible.
        /// </summary>
        public static string GenerarSalt()
        {
            byte[] bytes = new byte[BytesSalt];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(bytes);
            }
            return AHexadecimal(bytes);
        }

        /// <summary>
        /// Calcula SHA-256 de (salt + clave) y lo devuelve como 64 caracteres
        /// hexadecimales en mayúsculas.
        /// </summary>
        public static string CalcularHash(string clave, string salt)
        {
            if (clave == null) throw new ArgumentNullException("clave");
            if (salt == null) throw new ArgumentNullException("salt");

            byte[] entrada = Encoding.UTF8.GetBytes(salt + clave);
            using (SHA256 sha = SHA256.Create())
            {
                return AHexadecimal(sha.ComputeHash(entrada));
            }
        }

        /// <summary>
        /// Verifica si una clave corresponde al hash guardado.
        /// La comparación se hace en TIEMPO CONSTANTE: se recorren todos los
        /// caracteres aunque haya una diferencia al inicio. Una comparación normal
        /// (==) se detiene en el primer carácter distinto, y midiendo cuánto tarda
        /// un atacante podría deducir información (ataque de temporización).
        /// </summary>
        public static bool Verificar(string clave, string salt, string hashGuardado)
        {
            if (clave == null || salt == null || hashGuardado == null)
            {
                return false;
            }

            string calculado = CalcularHash(clave, salt);
            string guardado = hashGuardado.Trim().ToUpperInvariant();

            if (calculado.Length != guardado.Length)
            {
                return false;
            }

            int diferencia = 0;
            for (int i = 0; i < calculado.Length; i++)
            {
                // XOR: da 0 solo si los caracteres son iguales; OR acumula cualquier diferencia.
                diferencia |= calculado[i] ^ guardado[i];
            }
            return diferencia == 0;
        }

        // Convierte bytes a texto hexadecimal en mayúsculas (2 caracteres por byte).
        private static string AHexadecimal(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
