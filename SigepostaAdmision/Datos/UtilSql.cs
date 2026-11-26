using System;
using System.Data.SqlClient;

namespace SigepostaAdmision.Datos
{
    /// <summary>
    /// Funciones de apoyo que comparten los DAO. Se juntan aquí para no
    /// repetirlas en cada clase (es "internal": solo la usa la capa de Datos).
    /// </summary>
    internal static class UtilSql
    {
        // Números de error de SQL Server para violación de clave única:
        // 2627 = restricción UNIQUE / PRIMARY KEY, 2601 = índice único.
        private const int ErrorClaveDuplicada = 2627;
        private const int ErrorIndiceDuplicado = 2601;

        /// <summary>Máximo de filas que devuelven las búsquedas y listados.</summary>
        public const int MaximoResultados = 100;

        /// <summary>Indica si la excepción es por una clave única duplicada.</summary>
        public static bool EsClaveDuplicada(SqlException ex)
        {
            return ex.Number == ErrorClaveDuplicada || ex.Number == ErrorIndiceDuplicado;
        }

        /// <summary>
        /// En LIKE, los caracteres %, _ y [ son comodines. Se escapan para que,
        /// si el usuario los escribe, se busquen como texto normal.
        /// </summary>
        public static string EscaparLike(string texto)
        {
            return (texto ?? string.Empty).Replace("[", "[[]").Replace("%", "[%]").Replace("_", "[_]");
        }

        /// <summary>Patrón "contiene" para LIKE: %texto% (ya escapado).</summary>
        public static string PatronContiene(string texto)
        {
            return "%" + EscaparLike((texto ?? string.Empty).Trim()) + "%";
        }

        /// <summary>Los textos opcionales vacíos se guardan como NULL.</summary>
        public static object TextoONulo(string texto)
        {
            return string.IsNullOrWhiteSpace(texto) ? (object)DBNull.Value : texto.Trim();
        }

        // ===================== DNI cifrado (Unidad 4) =====================
        //
        // La columna Dni (texto plano) ya NO se lee ni se escribe. El DNI vive en:
        //   - DniCifrado VARBINARY(64): AES-256 (IV + datos), para poder mostrarlo.
        //   - DniHash CHAR(64): HMAC-SHA256, para buscar y detectar duplicados.

        /// <summary>Lee y descifra la columna DniCifrado; devuelve "" si es NULL.</summary>
        public static string LeerDni(SqlDataReader dr, string columna)
        {
            int ord = dr.GetOrdinal(columna);
            return dr.IsDBNull(ord) ? string.Empty : Seguridad.CifradoAes.Descifrar((byte[])dr.GetValue(ord));
        }

        /// <summary>
        /// Agrega el parámetro @DniHash: el HMAC del texto si son exactamente 8
        /// dígitos, o NULL si no (en SQL, "DniHash = NULL" nunca es verdadero, así
        /// que el filtro por DNI simplemente no encuentra nada).
        /// </summary>
        public static void AgregarFiltroDniHash(SqlCommand cmd, string texto)
        {
            string valor = (texto ?? string.Empty).Trim();
            cmd.Parameters.Add("@DniHash", System.Data.SqlDbType.Char, 64).Value =
                EsDniCompleto(valor) ? (object)Seguridad.HashDni.Calcular(valor) : DBNull.Value;
        }

        /// <summary>¿El texto son exactamente 8 dígitos?</summary>
        public static bool EsDniCompleto(string texto)
        {
            if (texto == null || texto.Length != 8) return false;
            foreach (char c in texto)
            {
                if (c < '0' || c > '9') return false;
            }
            return true;
        }

        /// <summary>
        /// Deshace una transacción sin ocultar el error original: si el Rollback
        /// también falla (por ejemplo, se cortó la conexión), SQL Server ya
        /// deshace por sí mismo la transacción abierta, así que se ignora.
        /// </summary>
        public static void DeshacerSeguro(SqlTransaction tx)
        {
            try
            {
                tx.Rollback();
            }
            catch (Exception)
            {
                // Se ignora a propósito: interesa relanzar la excepción original.
            }
        }
    }
}
