using System;
using System.Data;
using System.Data.SqlClient;

namespace SigepostaAdmision.Datos
{
    /// <summary>
    /// DAO de la tabla Auditoria: deja constancia de quién hizo qué y cuándo.
    ///
    /// Es una utilidad reutilizable para operaciones SIMPLES (abre su propia
    /// conexión). Cuando la auditoría debe ser parte de una transacción (como
    /// en AtencionDAO.RegistrarAtencion), se hace el INSERT con la conexión y la
    /// transacción de esa operación, para que se deshaga junto con ella.
    /// </summary>
    public class AuditoriaDAO
    {
        /// <summary>Inserta un registro de auditoría y devuelve su IdAuditoria.</summary>
        public int Registrar(int? idUsuario, string accion, string tabla, int? idRegistro, string detalle)
        {
            const string sql =
                "INSERT INTO dbo.Auditoria (IdUsuario, Accion, Tabla, IdRegistro, Detalle) " +
                "VALUES (@IdUsuario, @Accion, @Tabla, @IdRegistro, @Detalle); " +
                "SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (SqlConnection cn = Conexion.Crear())
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@IdUsuario", SqlDbType.Int).Value = idUsuario.HasValue ? (object)idUsuario.Value : DBNull.Value;
                cmd.Parameters.Add("@Accion", SqlDbType.VarChar, 40).Value = accion;
                cmd.Parameters.Add("@Tabla", SqlDbType.VarChar, 40).Value = tabla;
                cmd.Parameters.Add("@IdRegistro", SqlDbType.Int).Value = idRegistro.HasValue ? (object)idRegistro.Value : DBNull.Value;
                cmd.Parameters.Add("@Detalle", SqlDbType.VarChar, 200).Value = UtilSql.TextoONulo(detalle);
                cn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        /// <summary>Cantidad total de filas en Auditoria (la usa la demostración del Rollback).</summary>
        public int Contar()
        {
            using (SqlConnection cn = Conexion.Crear())
            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM dbo.Auditoria", cn))
            {
                cn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
    }
}
