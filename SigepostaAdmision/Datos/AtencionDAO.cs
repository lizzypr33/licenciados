using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SigepostaAdmision.Entidades;

namespace SigepostaAdmision.Datos
{
    /// <summary>
    /// DAO de la tabla Atencion. Consultas parametrizadas y "using" en todo.
    ///
    /// El registro de una atención es una operación ATÓMICA (Unidad 6):
    /// cambiar la cita, crear la atención y dejar la auditoría son UNA SOLA
    /// unidad de trabajo que se guarda completa o no se guarda.
    /// </summary>
    public class AtencionDAO
    {
        private const string AccionRegistrar = "REGISTRAR_ATENCION";
        private const string TablaAtencion = "Atencion";

        /// <summary>
        /// Registra la atención de una cita dentro de UNA TRANSACCIÓN y devuelve
        /// el IdAtencion generado.
        ///
        /// ¿Por qué una transacción? Son cuatro escrituras que dependen entre sí:
        ///   (a) la cita pasa a 'Atendida',
        ///   (b) se verifica la historia clínica del paciente,
        ///   (c) se inserta la Atencion,
        ///   (d) se inserta el registro de Auditoria.
        /// SIN transacción, un fallo a mitad de camino dejaría datos INCONSISTENTES:
        ///   - si falla (b) o (c): una cita marcada 'Atendida' pero SIN atención
        ///     registrada (el paciente "desaparece" de la cola sin constancia clínica);
        ///   - si falla (d): una atención SIN rastro de auditoría (nadie sabría quién
        ///     la registró).
        /// CON transacción todo comparte la misma conexión y la misma SqlTransaction:
        /// solo el Commit final hace visibles los cambios; ante cualquier error se
        /// hace Rollback y la base queda exactamente como estaba antes (atomicidad).
        ///
        /// forzarFalloParaDemo = true lanza un error justo después de (c) y antes de
        /// (d), para DEMOSTRAR el Rollback: la atención insertada en (c) y el cambio
        /// de la cita en (a) se deshacen. (El número IDENTITY consumido no se
        /// devuelve: puede quedar un salto en IdAtencion, y eso es normal.)
        /// </summary>
        public int RegistrarAtencion(int idCita, int idUsuario, string observaciones,
                                     byte[] diagnosticoCifrado = null, bool forzarFalloParaDemo = false)
        {
            try
            {
                using (SqlConnection cn = Conexion.Crear())
                {
                    cn.Open();

                    // Desde aquí, todo comando que reciba "tx" forma parte de la misma transacción.
                    using (SqlTransaction tx = cn.BeginTransaction())
                    {
                        try
                        {
                            // (a) La cita pasa a 'Atendida'. El WHERE exige que siga EnCola y
                            //     que ya haya sido llamada: si otro puesto la atendió o la
                            //     cambió, se afectan 0 filas y no se continúa.
                            const string sqlCita =
                                "UPDATE dbo.Cita SET Estado = @Atendida " +
                                "WHERE IdCita = @IdCita AND Estado = @EnCola AND HoraLlamado IS NOT NULL";
                            using (SqlCommand cmd = new SqlCommand(sqlCita, cn, tx))
                            {
                                cmd.Parameters.Add("@Atendida", SqlDbType.VarChar, 10).Value = EstadoCita.Atendida.ToString();
                                cmd.Parameters.Add("@EnCola", SqlDbType.VarChar, 10).Value = EstadoCita.EnCola.ToString();
                                cmd.Parameters.Add("@IdCita", SqlDbType.Int).Value = idCita;
                                if (cmd.ExecuteNonQuery() == 0)
                                {
                                    throw new ReglaNegocioException(
                                        "La cita no está en cola, no fue llamada o ya fue atendida.");
                                }
                            }

                            // (b) Historia clínica del paciente de esa cita: debe existir y estar Activa.
                            //     Si no, la excepción provoca el Rollback y se deshace también (a).
                            int idHistoria;
                            const string sqlHistoria =
                                "SELECT h.IdHistoria, h.NumeroHistoria, h.Estado " +
                                "FROM dbo.Cita c INNER JOIN dbo.HistoriaClinica h ON h.IdPaciente = c.IdPaciente " +
                                "WHERE c.IdCita = @IdCita";
                            using (SqlCommand cmd = new SqlCommand(sqlHistoria, cn, tx))
                            {
                                cmd.Parameters.Add("@IdCita", SqlDbType.Int).Value = idCita;
                                using (SqlDataReader dr = cmd.ExecuteReader())
                                {
                                    if (!dr.Read())
                                    {
                                        throw new ReglaNegocioException("El paciente no tiene historia clínica.");
                                    }
                                    idHistoria = dr.GetInt32(0);
                                    string numero = dr.GetString(1);
                                    string estado = dr.GetString(2).Trim();
                                    if (!string.Equals(estado, EstadoHistoria.Activa.ToString(), StringComparison.OrdinalIgnoreCase))
                                    {
                                        throw new ReglaNegocioException(
                                            "La historia clínica " + numero + " está Inactiva: no se pueden registrar atenciones.");
                                    }
                                }
                            }

                            // (c) Se inserta la atención (con el diagnóstico YA CIFRADO por
                            //     Negocio; aquí solo viajan bytes) y se obtiene su Id.
                            int idAtencion;
                            const string sqlAtencion =
                                "INSERT INTO dbo.Atencion (IdCita, IdHistoria, IdUsuario, Observaciones, DiagnosticoCifrado) " +
                                "VALUES (@IdCita, @IdHistoria, @IdUsuario, @Observaciones, @DiagnosticoCifrado); " +
                                "SELECT CAST(SCOPE_IDENTITY() AS INT);";
                            using (SqlCommand cmd = new SqlCommand(sqlAtencion, cn, tx))
                            {
                                cmd.Parameters.Add("@IdCita", SqlDbType.Int).Value = idCita;
                                cmd.Parameters.Add("@IdHistoria", SqlDbType.Int).Value = idHistoria;
                                cmd.Parameters.Add("@IdUsuario", SqlDbType.Int).Value = idUsuario;
                                cmd.Parameters.Add("@Observaciones", SqlDbType.VarChar, Atencion.MaxObservaciones).Value =
                                    UtilSql.TextoONulo(observaciones);
                                cmd.Parameters.Add("@DiagnosticoCifrado", SqlDbType.VarBinary, -1).Value =
                                    diagnosticoCifrado != null ? (object)diagnosticoCifrado : DBNull.Value;
                                idAtencion = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            // --- DEMO: fallo provocado entre (c) y (d) para mostrar el Rollback ---
                            if (forzarFalloParaDemo)
                            {
                                throw new ReglaNegocioException(
                                    "Fallo simulado para la demostración: se interrumpió la transacción " +
                                    "después de insertar la atención N° " + idAtencion + " y antes de la auditoría.");
                            }

                            // (d) Registro de auditoría, en la MISMA conexión y transacción
                            //     (por eso aquí no se usa AuditoriaDAO, que abre su propia conexión).
                            const string sqlAuditoria =
                                "INSERT INTO dbo.Auditoria (IdUsuario, Accion, Tabla, IdRegistro, Detalle) " +
                                "VALUES (@IdUsuario, @Accion, @Tabla, @IdRegistro, @Detalle)";
                            using (SqlCommand cmd = new SqlCommand(sqlAuditoria, cn, tx))
                            {
                                cmd.Parameters.Add("@IdUsuario", SqlDbType.Int).Value = idUsuario;
                                cmd.Parameters.Add("@Accion", SqlDbType.VarChar, 40).Value = AccionRegistrar;
                                cmd.Parameters.Add("@Tabla", SqlDbType.VarChar, 40).Value = TablaAtencion;
                                cmd.Parameters.Add("@IdRegistro", SqlDbType.Int).Value = idAtencion;
                                cmd.Parameters.Add("@Detalle", SqlDbType.VarChar, 200).Value = "IdCita=" + idCita;
                                cmd.ExecuteNonQuery();
                            }

                            // Todo salió bien: recién ahora los cuatro cambios se guardan juntos.
                            tx.Commit();
                            return idAtencion;
                        }
                        catch
                        {
                            // Cualquier error en (a), (b), (c), (d) o el fallo de la demo:
                            // Rollback deshace TODO lo hecho en esta transacción y se
                            // relanza la excepción original para que Negocio la informe.
                            UtilSql.DeshacerSeguro(tx);
                            throw;
                        }
                    }
                }
            }
            catch (SqlException ex) when (UtilSql.EsClaveDuplicada(ex))
            {
                // Última defensa: IdCita es UNIQUE en Atencion. Si por algún motivo la
                // cita volviera a quedar EnCola, la base impide una segunda atención.
                throw new RegistroDuplicadoException("Esta cita ya tiene una atención registrada.", "IdCita", ex);
            }
        }

        /// <summary>
        /// Atenciones de un paciente (de la más reciente a la más antigua) con
        /// fecha y hora, consultorio, profesional, observaciones y usuario que la registró.
        /// </summary>
        public List<AtencionResumen> ObtenerAtencionesPorPaciente(int idPaciente)
        {
            const string sql =
                "SELECT a.IdAtencion, a.IdCita, a.FechaHora, a.Observaciones, " +
                "co.Nombre AS Consultorio, ps.Apellidos + ', ' + ps.Nombres AS Profesional, " +
                "u.NombreUsuario AS RegistradaPor " +
                "FROM dbo.Atencion a " +
                "INNER JOIN dbo.Cita c ON c.IdCita = a.IdCita " +
                "LEFT JOIN dbo.Consultorio co ON co.IdConsultorio = c.IdConsultorio " +
                "LEFT JOIN dbo.PersonalSalud ps ON ps.IdPersonal = c.IdPersonal " +
                "LEFT JOIN dbo.Usuario u ON u.IdUsuario = a.IdUsuario " +
                "WHERE c.IdPaciente = @IdPaciente " +
                "ORDER BY a.FechaHora DESC";

            List<AtencionResumen> lista = new List<AtencionResumen>();
            using (SqlConnection cn = Conexion.Crear())
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@IdPaciente", SqlDbType.Int).Value = idPaciente;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        DateTime fechaHora = dr.GetDateTime(dr.GetOrdinal("FechaHora"));
                        lista.Add(new AtencionResumen
                        {
                            IdAtencion = dr.GetInt32(dr.GetOrdinal("IdAtencion")),
                            IdCita = dr.GetInt32(dr.GetOrdinal("IdCita")),
                            Fecha = fechaHora.Date,
                            Hora = fechaHora.TimeOfDay,
                            Observaciones = TextoOVacio(dr, "Observaciones"),
                            Consultorio = TextoOVacio(dr, "Consultorio"),
                            Profesional = TextoOVacio(dr, "Profesional"),
                            RegistradaPor = TextoOVacio(dr, "RegistradaPor")
                        });
                    }
                }
            }
            return lista;
        }

        /// <summary>
        /// Bytes cifrados del diagnóstico de una atención (null si no tiene).
        /// No se descifra aquí: descifrar es responsabilidad de Negocio, que además
        /// verifica el acceso y lo deja auditado.
        /// </summary>
        public byte[] ObtenerDiagnosticoCifrado(int idAtencion)
        {
            const string sql = "SELECT DiagnosticoCifrado FROM dbo.Atencion WHERE IdAtencion = @IdAtencion";
            using (SqlConnection cn = Conexion.Crear())
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@IdAtencion", SqlDbType.Int).Value = idAtencion;
                cn.Open();
                object valor = cmd.ExecuteScalar();
                return valor == null || valor == DBNull.Value ? null : (byte[])valor;
            }
        }

        /// <summary>Posta donde se registró la atención (por el consultorio de su cita), o null si no existe.</summary>
        public int? ObtenerPostaDeAtencion(int idAtencion)
        {
            const string sql =
                "SELECT co.IdPosta FROM dbo.Atencion a " +
                "INNER JOIN dbo.Cita c ON c.IdCita = a.IdCita " +
                "INNER JOIN dbo.Consultorio co ON co.IdConsultorio = c.IdConsultorio " +
                "WHERE a.IdAtencion = @IdAtencion";
            using (SqlConnection cn = Conexion.Crear())
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@IdAtencion", SqlDbType.Int).Value = idAtencion;
                cn.Open();
                object valor = cmd.ExecuteScalar();
                return valor == null || valor == DBNull.Value ? (int?)null : Convert.ToInt32(valor);
            }
        }

        /// <summary>Cantidad total de filas en Atencion (la usa la demostración del Rollback).</summary>
        public int ContarAtenciones()
        {
            using (SqlConnection cn = Conexion.Crear())
            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM dbo.Atencion", cn))
            {
                cn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private static string TextoOVacio(SqlDataReader dr, string columna)
        {
            int ord = dr.GetOrdinal(columna);
            return dr.IsDBNull(ord) ? string.Empty : dr.GetString(ord);
        }
    }
}
