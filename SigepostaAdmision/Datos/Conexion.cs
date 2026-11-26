using System;
using System.Configuration;
using System.Data.SqlClient;

namespace SigepostaAdmision.Datos
{
    /// <summary>
    /// Clase de la capa de Datos encargada de crear conexiones a SQL Server.
    /// Lee la cadena de conexión "SigepostaDB" definida en App.config.
    /// </summary>
    public static class Conexion
    {
        // Nombre de la cadena de conexión en App.config
        private const string NombreCadena = "SigepostaDB";

        /// <summary>
        /// Crea (sin abrir) una nueva conexión a la base de datos SigepostaAdmision.
        /// Quien la use es responsable de abrirla y liberarla (idealmente con using).
        /// </summary>
        public static SqlConnection Crear()
        {
            ConnectionStringSettings cadena = ConfigurationManager.ConnectionStrings[NombreCadena];
            if (cadena == null)
            {
                throw new ConfigurationErrorsException(
                    "No se encontró la cadena de conexión '" + NombreCadena + "' en App.config.");
            }
            return new SqlConnection(cadena.ConnectionString);
        }

        /// <summary>
        /// Intenta abrir una conexión para verificar que la base de datos responde.
        /// Devuelve true si se conectó; en "mensaje" deja el resultado o el error.
        /// </summary>
        public static bool Probar(out string mensaje)
        {
            try
            {
                using (SqlConnection cn = Crear())
                {
                    cn.Open();
                    mensaje = "Conexión exitosa a la base de datos '" + cn.Database +
                              "' en el servidor '" + cn.DataSource + "'.";
                    return true;
                }
            }
            catch (Exception)
            {
                // Sin el texto técnico de la excepción (podría revelar el servidor o la
                // configuración). Este método no se usa en la interfaz actual.
                mensaje = "No se pudo conectar a la base de datos.";
                return false;
            }
        }
    }
}
