using System;
using System.Security.Cryptography;
using System.Text;

namespace SigepostaAdmision.Seguridad
{
    /// <summary>
    /// "Huella" del DNI para BUSCAR y detectar duplicados sin descifrar.
    ///
    /// Problema: el DNI se guarda cifrado con AES y un IV aleatorio, así que el
    /// mismo DNI produce bytes distintos cada vez y no se puede hacer
    /// "WHERE DniCifrado = ...". Solución: guardar además una huella que SIEMPRE
    /// sea la misma para el mismo DNI (columna DniHash) y buscar por ella.
    ///
    /// ¿Por qué HMAC con clave secreta y no un SHA-256 simple?
    /// Un DNI tiene solo 8 dígitos: 100 millones de combinaciones. Con un SHA-256
    /// simple, un atacante que robe la base calcularía el hash de los 100 millones
    /// de DNI en minutos (fuerza bruta) y sabría el DNI de cada paciente.
    /// HMAC-SHA256 mezcla el DNI con una CLAVE SECRETA (GestorClaves.ClaveHmac) que
    /// no está en la base: sin esa clave, el atacante no puede calcular las huellas
    /// y la fuerza bruta no sirve.
    /// </summary>
    public static class HashDni
    {
        /// <summary>HMAC-SHA256 del DNI, en hexadecimal mayúsculas (64 caracteres).</summary>
        public static string Calcular(string dni)
        {
            return Calcular(dni, GestorClaves.ClaveHmac);
        }

        /// <summary>
        /// Sobrecarga con una clave HMAC indicada (no la del sistema). La usa la
        /// importación de claves para PROBAR unas claves candidatas.
        /// </summary>
        public static string Calcular(string dni, byte[] claveHmac)
        {
            if (dni == null) throw new ArgumentNullException("dni");
            if (claveHmac == null) throw new ArgumentNullException("claveHmac");

            byte[] datos = Encoding.UTF8.GetBytes(dni.Trim());
            using (HMACSHA256 hmac = new HMACSHA256(claveHmac))
            {
                byte[] huella = hmac.ComputeHash(datos);
                StringBuilder sb = new StringBuilder(64);
                foreach (byte b in huella)
                {
                    sb.Append(b.ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
