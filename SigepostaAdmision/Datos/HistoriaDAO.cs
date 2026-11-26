using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SigepostaAdmision.Entidades;

namespace SigepostaAdmision.Datos
{
    /// <summary>
    /// DAO de la tabla HistoriaClinica (y consultas de lectura relacionadas).
    /// Todas las consultas son parametrizadas y usan "using".
    ///
    /// Formato del número de historia: "HC-" + 6 dígitos con ceros a la izquierda
    /// (HC-000007), tomado de la secuencia dbo.SeqNumeroHistoria. Se usa una
    /// SEQUENCE (y no MAX(NumeroHistoria) + 1) porque la secuencia garantiza
    /// números distintos aunque dos usuarios registren pacientes al mismo tiempo.
    /// </summary>
    public class HistoriaDAO
    {
        private const string PrefijoHistoria = "HC-";

        // Columnas de la historia + del paciente (con los nombres que espera PacienteDAO.Mapear).
        private const string SelectHistoria =
            "SELECT h.IdHistoria, h.NumeroHistoria, h.FechaApertura, h.Estado, " +
            "p.IdPaciente, p.DniCifrado, p.ApellidoPaterno, p.ApellidoMaterno, p.Nombres, " +
            "p.FechaNacimiento, p.Sexo, p.Direccion, p.Telefono " +
            "FROM dbo.HistoriaClinica h INNER JOIN dbo.Paciente p ON p.IdPaciente = h.IdPaciente ";

        // ===================== Consultas =====================

        /// <summary>Historia del paciente, o null si aún no tiene.</summary>
        public HistoriaClinica ObtenerPorPaciente(int idPaciente)
        {
            return ConsultarUna(SelectHistoria + "WHERE h.IdPaciente = @IdPaciente", cmd =>
                cmd.Parameters.Add("@IdPaciente", SqlDbType.Int).Value = idPaciente);
        }

        /// <summary>Historia con ese número (por ejemplo "HC-000007"), o null si no existe.</summary>
        public HistoriaClinica ObtenerPorNumero(string numero)
        {
            return ConsultarUna(SelectHistoria + "WHERE h.NumeroHistoria = @NumeroHistoria", cmd =>
                cmd.Parameters.Add("@NumeroHistoria", SqlDbType.VarChar, 12).Value = (numero ?? string.Empty).Trim());
        }

        /// <summary>
        /// Lista pacientes con su historia. Usa LEFT JOIN para incluir también a
        /// los pacientes que todavía NO tienen historia (sus columnas de historia
        /// llegan en NULL). Filtra por número de historia, DNI, apellidos o
        /// nombres (LIKE parametrizado). Filtro vacío = todos. Máximo 100 filas.
        /// TotalAtenciones = cantidad de atenciones registradas (tabla Atencion) del paciente.
        /// </summary>
        public List<HistoriaResumen> Listar(string filtro)
        {
            string sql =
                "SELECT TOP (" + UtilSql.MaximoResultados + ") " +
                "p.IdPaciente, p.DniCifrado, p.ApellidoPaterno, p.ApellidoMaterno, p.Nombres, " +
                "h.IdHistoria, h.NumeroHistoria, h.FechaApertura, h.Estado, " +
                "(SELECT COUNT(*) FROM dbo.Atencion a INNER JOIN dbo.Cita c ON c.IdCita = a.IdCita " +
                " WHERE c.IdPaciente = p.IdPaciente) AS TotalAtenciones " +
                "FROM dbo.Paciente p LEFT JOIN dbo.HistoriaClinica h ON h.IdPaciente = p.IdPaciente " +
                "WHERE h.NumeroHistoria LIKE @Patron " +
                "   OR p.DniHash = @DniHash " +   // DNI: solo coincidencia exacta de 8 dígitos (huella HMAC)
                "   OR p.ApellidoPaterno LIKE @Patron " +
                "   OR p.ApellidoMaterno LIKE @Patron " +
                "   OR p.Nombres LIKE @Patron " +
                "   OR (p.ApellidoPaterno + ' ' + p.ApellidoMaterno + ' ' + p.Nombres) LIKE @Patron " +
                "ORDER BY p.ApellidoPaterno, p.ApellidoMaterno, p.Nombres";

            List<HistoriaResumen> lista = new List<HistoriaResumen>();

            using (SqlConnection cn = Conexion.Crear())
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@Patron", SqlDbType.VarChar, 200).Value = UtilSql.PatronContiene(filtro);
                UtilSql.AgregarFiltroDniHash(cmd, filtro);
                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    int ordIdHistoria = dr.GetOrdinal("IdHistoria");
                    int ordNumero = dr.GetOrdinal("NumeroHistoria");
                    int ordApertura = dr.GetOrdinal("FechaApertura");
                    int ordEstado = dr.GetOrdinal("Estado");

                    while (dr.Read())
                    {
                        bool tiene = !dr.IsDBNull(ordIdHistoria);
                        lista.Add(new HistoriaResumen
                        {
                            IdPaciente = dr.GetInt32(dr.GetOrdinal("IdPaciente")),
                            Dni = UtilSql.LeerDni(dr, "DniCifrado"),
                            NombreCompleto = dr.GetString(dr.GetOrdinal("ApellidoPaterno")) + " " +
                                             dr.GetString(dr.GetOrdinal("ApellidoMaterno")) + ", " +
                                             dr.GetString(dr.GetOrdinal("Nombres")),
                            IdHistoria = tiene ? dr.GetInt32(ordIdHistoria) : (int?)null,
                            NumeroHistoria = tiene ? dr.GetString(ordNumero) : null,
                            FechaApertura = tiene ? dr.GetDateTime(ordApertura) : (DateTime?)null,
                            Estado = tiene ? ConvertirEstado(dr.GetString(ordEstado)) : (EstadoHistoria?)null,
                            TotalAtenciones = dr.GetInt32(dr.GetOrdinal("TotalAtenciones"))
                        });
                    }
                }
            }
            return lista;
        }

        // Las atenciones del paciente se obtienen ahora con
        // AtencionDAO.ObtenerAtencionesPorPaciente (tabla Atencion).

        // ===================== Escritura =====================

        /// <summary>
        /// Abre la historia de un paciente que no la tiene, dentro de una transacción.
        ///   - Si ya tiene historia, no hace nada y devuelve la existente.
        ///   - Si el paciente no existe, devuelve null.
        ///
        /// La lectura previa usa WITH (UPDLOCK, HOLDLOCK): bloquea ese "hueco" hasta
        /// el Commit, para que dos usuarios que pulsan "Abrir historia" a la vez no
        /// intenten crear dos historias para el mismo paciente. Aun así, la
        /// restricción UNIQUE de IdPaciente es la garantía final.
        /// </summary>
        public HistoriaClinica AbrirParaPaciente(int idPaciente)
        {
            try
            {
                using (SqlConnection cn = Conexion.Crear())
                {
                    cn.Open();
                    using (SqlTransaction tx = cn.BeginTransaction())
                    {
                        try
                        {
                            if (!TieneHistoria(cn, tx, idPaciente))
                            {
                                if (!ExistePaciente(cn, tx, idPaciente))
                                {
                                    tx.Rollback();
                                    return null;
                                }
                                InsertarHistoria(cn, tx, idPaciente);
                            }
                            tx.Commit();
                        }
                        catch
                        {
                            UtilSql.DeshacerSeguro(tx);
                            throw;
                        }
                    }
                }
            }
            catch (SqlException ex) when (UtilSql.EsClaveDuplicada(ex))
            {
                // Otro usuario la abrió justo antes: se devuelve la que ya existe.
            }
            return ObtenerPorPaciente(idPaciente);
        }

        /// <summary>
        /// Cambia el estado (Activa / Inactiva). Las historias nunca se eliminan.
        /// Devuelve true si se actualizó la historia.
        /// </summary>
        public bool CambiarEstado(int idHistoria, EstadoHistoria nuevo)
        {
            const string sql = "UPDATE dbo.HistoriaClinica SET Estado = @Estado WHERE IdHistoria = @IdHistoria";

            using (SqlConnection cn = Conexion.Crear())
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@Estado", SqlDbType.VarChar, 10).Value = nuevo.ToString();
                cmd.Parameters.Add("@IdHistoria", SqlDbType.Int).Value = idHistoria;
                cn.Open();
                return cmd.ExecuteNonQuery() == 1;
            }
        }

        /// <summary>
        /// Toma el siguiente número de la secuencia e inserta la historia 'Activa'
        /// usando la conexión y la transacción RECIBIDAS. Así puede formar parte de
        /// una transacción mayor (PacienteDAO.InsertarConHistoria). Devuelve el número.
        /// </summary>
        internal static string InsertarHistoria(SqlConnection cn, SqlTransaction tx, int idPaciente)
        {
            int siguiente;
            using (SqlCommand cmd = new SqlCommand("SELECT NEXT VALUE FOR dbo.SeqNumeroHistoria", cn, tx))
            {
                siguiente = Convert.ToInt32(cmd.ExecuteScalar());
            }

            string numero = FormatearNumero(siguiente);

            const string sql =
                "INSERT INTO dbo.HistoriaClinica (NumeroHistoria, IdPaciente, Estado) " +
                "VALUES (@NumeroHistoria, @IdPaciente, @Estado)";

            using (SqlCommand cmd = new SqlCommand(sql, cn, tx))
            {
                cmd.Parameters.Add("@NumeroHistoria", SqlDbType.VarChar, 12).Value = numero;
                cmd.Parameters.Add("@IdPaciente", SqlDbType.Int).Value = idPaciente;
                cmd.Parameters.Add("@Estado", SqlDbType.VarChar, 10).Value = EstadoHistoria.Activa.ToString();
                cmd.ExecuteNonQuery();
            }
            return numero;
        }

        /// <summary>7 → "HC-000007".</summary>
        public static string FormatearNumero(int valor)
        {
            return PrefijoHistoria + valor.ToString("D6");
        }

        // ===================== Apoyo =====================

        private static bool TieneHistoria(SqlConnection cn, SqlTransaction tx, int idPaciente)
        {
            const string sql =
                "SELECT COUNT(*) FROM dbo.HistoriaClinica WITH (UPDLOCK, HOLDLOCK) WHERE IdPaciente = @IdPaciente";
            using (SqlCommand cmd = new SqlCommand(sql, cn, tx))
            {
                cmd.Parameters.Add("@IdPaciente", SqlDbType.Int).Value = idPaciente;
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private static bool ExistePaciente(SqlConnection cn, SqlTransaction tx, int idPaciente)
        {
            const string sql = "SELECT COUNT(*) FROM dbo.Paciente WHERE IdPaciente = @IdPaciente";
            using (SqlCommand cmd = new SqlCommand(sql, cn, tx))
            {
                cmd.Parameters.Add("@IdPaciente", SqlDbType.Int).Value = idPaciente;
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private static HistoriaClinica ConsultarUna(string sql, Action<SqlCommand> parametros)
        {
            using (SqlConnection cn = Conexion.Crear())
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                parametros(cmd);
                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (!dr.Read())
                    {
                        return null;
                    }

                    // El paciente se arma con el mismo método que usa PacienteDAO.
                    Paciente p = PacienteDAO.Mapear(dr);

                    HistoriaClinica h = new HistoriaClinica(
                        dr.GetString(dr.GetOrdinal("NumeroHistoria")),
                        p,
                        dr.GetDateTime(dr.GetOrdinal("FechaApertura")));
                    h.IdHistoria = dr.GetInt32(dr.GetOrdinal("IdHistoria"));
                    h.Estado = ConvertirEstado(dr.GetString(dr.GetOrdinal("Estado")));
                    return h;
                }
            }
        }

        // 'Activa' / 'Inactiva' (texto de la tabla) → enum.
        private static EstadoHistoria ConvertirEstado(string texto)
        {
            return (EstadoHistoria)Enum.Parse(typeof(EstadoHistoria), texto.Trim(), true);
        }
    }
}
