using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SigepostaAdmision.Entidades;

namespace SigepostaAdmision.Datos
{
    /// <summary>
    /// DAO de la tabla Cita. Consultas parametrizadas y "using" en todo.
    /// Devuelve CitaResumen (clase de lectura) en vez de la jerarquía completa
    /// de entidades.
    ///
    /// Regla de los turnos: el NumeroTurno se cuenta por CONSULTORIO y FECHA
    /// (1, 2, 3...). Las citas canceladas conservan su número; los turnos no se
    /// reutilizan, así un número nunca se repite en el mismo día y consultorio.
    /// </summary>
    public class CitaDAO
    {
        private const string Cancelada = "Cancelada";   // Valor de Cita.Estado

        // SELECT común para CitaResumen (JOIN con Paciente, Consultorio, Servicio y PersonalSalud).
        private const string SelectResumen =
            "SELECT c.IdCita, c.NumeroTurno, c.Fecha, c.Hora, c.Estado, c.HoraLlegada, c.HoraLlamado, " +
            "p.IdPaciente, p.DniCifrado, p.ApellidoPaterno + ' ' + p.ApellidoMaterno + ', ' + p.Nombres AS NombrePaciente, " +
            "co.IdConsultorio, co.Nombre AS Consultorio, co.IdPosta, " +
            "s.IdServicio, s.Nombre AS Servicio, " +
            "ps.IdPersonal, ps.Apellidos + ', ' + ps.Nombres AS Profesional " +
            "FROM dbo.Cita c " +
            "INNER JOIN dbo.Paciente p ON p.IdPaciente = c.IdPaciente " +
            "INNER JOIN dbo.Consultorio co ON co.IdConsultorio = c.IdConsultorio " +
            "INNER JOIN dbo.Servicio s ON s.IdServicio = co.IdServicio " +
            "INNER JOIN dbo.PersonalSalud ps ON ps.IdPersonal = c.IdPersonal ";

        // ===================== Escritura =====================

        /// <summary>
        /// Registra una cita 'Pendiente' y devuelve su IdCita; en "turno" devuelve
        /// el número de turno asignado.
        ///
        /// ¿Por qué una TRANSACCIÓN con UPDLOCK, HOLDLOCK? El turno se calcula como
        /// MAX(NumeroTurno) + 1 del consultorio y fecha. Si dos recepcionistas
        /// registran a la vez, ambas podrían leer el mismo máximo (por ejemplo 4) y
        /// las dos citas tendrían el turno 5. Con UPDLOCK + HOLDLOCK, la primera
        /// transacción "reserva" esas filas (y el rango) hasta su Commit; la segunda
        /// espera, y al continuar ya ve el turno 5 y toma el 6. Lectura, cálculo e
        /// INSERT forman una sola unidad: o se hace todo, o nada.
        ///
        /// Dentro de la misma transacción también se vuelve a comprobar que el
        /// profesional no tenga otra cita a esa hora (por si otro usuario la tomó
        /// después de que Negocio verificara las franjas libres).
        /// </summary>
        public int Insertar(int idPaciente, int idConsultorio, int idPersonal, int idUsuario,
                            DateTime fecha, TimeSpan hora, out int turno)
        {
            using (SqlConnection cn = Conexion.Crear())
            {
                cn.Open();
                using (SqlTransaction tx = cn.BeginTransaction())
                {
                    try
                    {
                        VerificarHorarioLibre(cn, tx, idPersonal, fecha, hora, 0);
                        turno = SiguienteTurno(cn, tx, idConsultorio, fecha);

                        const string sql =
                            "INSERT INTO dbo.Cita (IdPaciente, IdPersonal, IdConsultorio, IdUsuario, Fecha, Hora, NumeroTurno, Estado) " +
                            "VALUES (@IdPaciente, @IdPersonal, @IdConsultorio, @IdUsuario, @Fecha, @Hora, @NumeroTurno, @Estado); " +
                            "SELECT CAST(SCOPE_IDENTITY() AS INT);";

                        int idCita;
                        using (SqlCommand cmd = new SqlCommand(sql, cn, tx))
                        {
                            cmd.Parameters.Add("@IdPaciente", SqlDbType.Int).Value = idPaciente;
                            cmd.Parameters.Add("@IdPersonal", SqlDbType.Int).Value = idPersonal;
                            cmd.Parameters.Add("@IdConsultorio", SqlDbType.Int).Value = idConsultorio;
                            cmd.Parameters.Add("@IdUsuario", SqlDbType.Int).Value = idUsuario;
                            cmd.Parameters.Add("@Fecha", SqlDbType.Date).Value = fecha.Date;
                            cmd.Parameters.Add("@Hora", SqlDbType.Time).Value = hora;
                            cmd.Parameters.Add("@NumeroTurno", SqlDbType.Int).Value = turno;
                            cmd.Parameters.Add("@Estado", SqlDbType.VarChar, 10).Value = EstadoCita.Pendiente.ToString();
                            idCita = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        tx.Commit();
                        return idCita;
                    }
                    catch
                    {
                        UtilSql.DeshacerSeguro(tx);
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Guarda la nueva programación (fecha, hora, consultorio y profesional) de
        /// una cita y devuelve su número de turno. Si cambió el consultorio o la
        /// fecha, se le asigna un turno nuevo en el destino, con el mismo bloqueo
        /// que en Insertar; si no, conserva su turno. Todo en una transacción.
        /// </summary>
        public int ActualizarProgramacion(int idCita, DateTime fecha, TimeSpan hora, int idConsultorio, int idPersonal)
        {
            using (SqlConnection cn = Conexion.Crear())
            {
                cn.Open();
                using (SqlTransaction tx = cn.BeginTransaction())
                {
                    try
                    {
                        // Datos actuales de la cita (bloqueada hasta el Commit).
                        int consultorioActual, turnoActual;
                        DateTime fechaActual;
                        const string sqlActual =
                            "SELECT IdConsultorio, Fecha, NumeroTurno FROM dbo.Cita WITH (UPDLOCK) WHERE IdCita = @IdCita";
                        using (SqlCommand cmd = new SqlCommand(sqlActual, cn, tx))
                        {
                            cmd.Parameters.Add("@IdCita", SqlDbType.Int).Value = idCita;
                            using (SqlDataReader dr = cmd.ExecuteReader())
                            {
                                if (!dr.Read())
                                {
                                    throw new ReglaNegocioException("La cita ya no existe.");
                                }
                                consultorioActual = dr.GetInt32(0);
                                fechaActual = dr.GetDateTime(1);
                                turnoActual = dr.GetInt32(2);
                            }
                        }

                        VerificarHorarioLibre(cn, tx, idPersonal, fecha, hora, idCita);

                        bool cambiaDestino = consultorioActual != idConsultorio || fechaActual.Date != fecha.Date;
                        int turno = cambiaDestino ? SiguienteTurno(cn, tx, idConsultorio, fecha) : turnoActual;

                        // Solo se actualiza si SIGUE Pendiente: si otro puesto registró la
                        // llegada (EnCola), la atendió o la canceló, se afectan 0 filas y
                        // se aborta la transacción con un mensaje claro.
                        const string sql =
                            "UPDATE dbo.Cita SET Fecha = @Fecha, Hora = @Hora, IdConsultorio = @IdConsultorio, " +
                            "IdPersonal = @IdPersonal, NumeroTurno = @NumeroTurno " +
                            "WHERE IdCita = @IdCita AND Estado = @Pendiente";
                        using (SqlCommand cmd = new SqlCommand(sql, cn, tx))
                        {
                            cmd.Parameters.Add("@Fecha", SqlDbType.Date).Value = fecha.Date;
                            cmd.Parameters.Add("@Hora", SqlDbType.Time).Value = hora;
                            cmd.Parameters.Add("@IdConsultorio", SqlDbType.Int).Value = idConsultorio;
                            cmd.Parameters.Add("@IdPersonal", SqlDbType.Int).Value = idPersonal;
                            cmd.Parameters.Add("@NumeroTurno", SqlDbType.Int).Value = turno;
                            cmd.Parameters.Add("@IdCita", SqlDbType.Int).Value = idCita;
                            cmd.Parameters.Add("@Pendiente", SqlDbType.VarChar, 10).Value = EstadoCita.Pendiente.ToString();
                            if (cmd.ExecuteNonQuery() == 0)
                            {
                                throw new ReglaNegocioException(
                                    "La cita ya no está Pendiente (otro puesto registró su llegada, la atendió o la canceló). " +
                                    "Actualice la agenda e intente de nuevo.");
                            }
                        }

                        tx.Commit();
                        return turno;
                    }
                    catch
                    {
                        UtilSql.DeshacerSeguro(tx);
                        throw;
                    }
                }
            }
        }

        /// <summary>Cambia el estado de la cita (las citas nunca se borran). true si se actualizó.</summary>
        public bool CambiarEstado(int idCita, EstadoCita estado)
        {
            const string sql = "UPDATE dbo.Cita SET Estado = @Estado WHERE IdCita = @IdCita";
            using (SqlConnection cn = Conexion.Crear())
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@Estado", SqlDbType.VarChar, 10).Value = estado.ToString();
                cmd.Parameters.Add("@IdCita", SqlDbType.Int).Value = idCita;
                cn.Open();
                return cmd.ExecuteNonQuery() == 1;
            }
        }

        // ===================== Lectura =====================

        /// <summary>Una cita por su Id, o null si no existe.</summary>
        public CitaResumen ObtenerPorId(int idCita)
        {
            List<CitaResumen> lista = Consultar(SelectResumen + "WHERE c.IdCita = @IdCita", cmd =>
                cmd.Parameters.Add("@IdCita", SqlDbType.Int).Value = idCita);
            return lista.Count > 0 ? lista[0] : null;
        }

        /// <summary>
        /// Citas de una fecha. Los filtros opcionales (null = sin filtro) se aplican
        /// con "(@Param IS NULL OR columna = @Param)", siempre parametrizados.
        /// Ordenadas por hora y consultorio.
        /// </summary>
        public List<CitaResumen> ListarPorFecha(DateTime fecha, int? idPosta, int? idConsultorio, EstadoCita? estado)
        {
            string sql = SelectResumen +
                "WHERE c.Fecha = @Fecha " +
                "  AND (@IdPosta IS NULL OR co.IdPosta = @IdPosta) " +
                "  AND (@IdConsultorio IS NULL OR c.IdConsultorio = @IdConsultorio) " +
                "  AND (@Estado IS NULL OR c.Estado = @Estado) " +
                "ORDER BY c.Hora, co.Nombre, c.NumeroTurno";

            return Consultar(sql, cmd =>
            {
                cmd.Parameters.Add("@Fecha", SqlDbType.Date).Value = fecha.Date;
                cmd.Parameters.Add("@IdPosta", SqlDbType.Int).Value =
                    idPosta.HasValue ? (object)idPosta.Value : DBNull.Value;
                cmd.Parameters.Add("@IdConsultorio", SqlDbType.Int).Value =
                    idConsultorio.HasValue ? (object)idConsultorio.Value : DBNull.Value;
                cmd.Parameters.Add("@Estado", SqlDbType.VarChar, 10).Value =
                    estado.HasValue ? (object)estado.Value.ToString() : DBNull.Value;
            });
        }

        /// <summary>Citas EnCola del día (la cola de atención). El orden final lo da ColaAtencion.</summary>
        public List<CitaResumen> ListarCola(DateTime fecha, int? idPosta, int? idConsultorio)
        {
            return ListarPorFecha(fecha, idPosta, idConsultorio, EstadoCita.EnCola);
        }

        /// <summary>Citas Pendientes del día (pacientes que aún no llegan), ordenadas por hora.</summary>
        public List<CitaResumen> ListarPendientesDelDia(DateTime fecha, int? idPosta, int? idConsultorio)
        {
            return ListarPorFecha(fecha, idPosta, idConsultorio, EstadoCita.Pendiente);
        }

        // ===================== Cola de atención (escritura) =====================
        //
        // ¿Por qué el estado se valida en el propio WHERE? Varios puestos (por
        // ejemplo, dos ventanillas) pueden actuar a la vez sobre la misma cita.
        // Si el UPDATE dijera solo "WHERE IdCita = @IdCita", el segundo puesto
        // podría pisar lo que hizo el primero. Con "AND Estado = 'Pendiente'" el
        // UPDATE solo cambia la fila si SIGUE en el estado esperado; si otro puesto
        // ya la cambió, afecta 0 filas y Negocio informa el conflicto. Es una
        // operación atómica: no hace falta una transacción explícita.
        //
        // Las horas se toman con GETDATE() del servidor: así todos los puestos usan
        // el mismo reloj para ordenar la cola, aunque sus PC tengan otra hora.

        /// <summary>Pendiente → EnCola con HoraLlegada = ahora (solo citas de hoy). Devuelve filas afectadas.</summary>
        public int MarcarLlegada(int idCita)
        {
            const string sql =
                "UPDATE dbo.Cita SET Estado = @EnCola, HoraLlegada = GETDATE(), HoraLlamado = NULL " +
                "WHERE IdCita = @IdCita AND Estado = @Pendiente AND Fecha = CAST(GETDATE() AS DATE)";
            return EjecutarCambioCola(sql, idCita);
        }

        /// <summary>EnCola → Pendiente, borrando HoraLlegada y HoraLlamado. Devuelve filas afectadas.</summary>
        public int DeshacerLlegada(int idCita)
        {
            const string sql =
                "UPDATE dbo.Cita SET Estado = @Pendiente, HoraLlegada = NULL, HoraLlamado = NULL " +
                "WHERE IdCita = @IdCita AND Estado = @EnCola";
            return EjecutarCambioCola(sql, idCita);
        }

        /// <summary>Registra HoraLlamado = ahora, solo si está EnCola y aún no fue llamado. Devuelve filas afectadas.</summary>
        public int MarcarLlamado(int idCita)
        {
            const string sql =
                "UPDATE dbo.Cita SET HoraLlamado = GETDATE() " +
                "WHERE IdCita = @IdCita AND Estado = @EnCola AND HoraLlamado IS NULL";
            return EjecutarCambioCola(sql, idCita);
        }

        // Ejecuta un UPDATE de la cola con los parámetros comunes y devuelve las filas afectadas.
        private static int EjecutarCambioCola(string sql, int idCita)
        {
            using (SqlConnection cn = Conexion.Crear())
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@IdCita", SqlDbType.Int).Value = idCita;
                cmd.Parameters.Add("@Pendiente", SqlDbType.VarChar, 10).Value = EstadoCita.Pendiente.ToString();
                cmd.Parameters.Add("@EnCola", SqlDbType.VarChar, 10).Value = EstadoCita.EnCola.ToString();
                cn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Horas ya ocupadas por el profesional en la fecha (citas no canceladas).
        /// idCitaExcluir permite ignorar la propia cita al reprogramarla.
        /// </summary>
        public List<TimeSpan> HorasOcupadas(int idPersonal, DateTime fecha, int idCitaExcluir = 0)
        {
            const string sql =
                "SELECT Hora FROM dbo.Cita " +
                "WHERE IdPersonal = @IdPersonal AND Fecha = @Fecha AND Estado <> @Cancelada AND IdCita <> @Excluir";

            List<TimeSpan> horas = new List<TimeSpan>();
            using (SqlConnection cn = Conexion.Crear())
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@IdPersonal", SqlDbType.Int).Value = idPersonal;
                cmd.Parameters.Add("@Fecha", SqlDbType.Date).Value = fecha.Date;
                cmd.Parameters.Add("@Cancelada", SqlDbType.VarChar, 10).Value = Cancelada;
                cmd.Parameters.Add("@Excluir", SqlDbType.Int).Value = idCitaExcluir;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        horas.Add(dr.GetTimeSpan(0));
                    }
                }
            }
            return horas;
        }

        /// <summary>
        /// ¿El paciente ya tiene una cita no cancelada del mismo servicio ese día?
        /// (idCitaExcluir = la cita que se está reprogramando, o 0.)
        /// </summary>
        public bool ExisteCitaPacienteServicioFecha(int idPaciente, int idServicio, DateTime fecha, int idCitaExcluir)
        {
            const string sql =
                "SELECT COUNT(*) FROM dbo.Cita c " +
                "INNER JOIN dbo.Consultorio co ON co.IdConsultorio = c.IdConsultorio " +
                "WHERE c.IdPaciente = @IdPaciente AND co.IdServicio = @IdServicio AND c.Fecha = @Fecha " +
                "  AND c.Estado <> @Cancelada AND c.IdCita <> @Excluir";

            using (SqlConnection cn = Conexion.Crear())
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@IdPaciente", SqlDbType.Int).Value = idPaciente;
                cmd.Parameters.Add("@IdServicio", SqlDbType.Int).Value = idServicio;
                cmd.Parameters.Add("@Fecha", SqlDbType.Date).Value = fecha.Date;
                cmd.Parameters.Add("@Cancelada", SqlDbType.VarChar, 10).Value = Cancelada;
                cmd.Parameters.Add("@Excluir", SqlDbType.Int).Value = idCitaExcluir;
                cn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        // ===================== Apoyo (dentro de una transacción) =====================

        // MAX(NumeroTurno) + 1 del consultorio y fecha, con bloqueo hasta el Commit.
        private static int SiguienteTurno(SqlConnection cn, SqlTransaction tx, int idConsultorio, DateTime fecha)
        {
            const string sql =
                "SELECT ISNULL(MAX(NumeroTurno), 0) + 1 FROM dbo.Cita WITH (UPDLOCK, HOLDLOCK) " +
                "WHERE IdConsultorio = @IdConsultorio AND Fecha = @Fecha";
            using (SqlCommand cmd = new SqlCommand(sql, cn, tx))
            {
                cmd.Parameters.Add("@IdConsultorio", SqlDbType.Int).Value = idConsultorio;
                cmd.Parameters.Add("@Fecha", SqlDbType.Date).Value = fecha.Date;
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        // Si el profesional ya tiene una cita no cancelada en esa fecha y hora,
        // lanza RegistroDuplicadoException (Campo = "Horario").
        private static void VerificarHorarioLibre(SqlConnection cn, SqlTransaction tx, int idPersonal,
                                                  DateTime fecha, TimeSpan hora, int idCitaExcluir)
        {
            const string sql =
                "SELECT COUNT(*) FROM dbo.Cita WITH (UPDLOCK, HOLDLOCK) " +
                "WHERE IdPersonal = @IdPersonal AND Fecha = @Fecha AND Hora = @Hora " +
                "  AND Estado <> @Cancelada AND IdCita <> @Excluir";
            using (SqlCommand cmd = new SqlCommand(sql, cn, tx))
            {
                cmd.Parameters.Add("@IdPersonal", SqlDbType.Int).Value = idPersonal;
                cmd.Parameters.Add("@Fecha", SqlDbType.Date).Value = fecha.Date;
                cmd.Parameters.Add("@Hora", SqlDbType.Time).Value = hora;
                cmd.Parameters.Add("@Cancelada", SqlDbType.VarChar, 10).Value = Cancelada;
                cmd.Parameters.Add("@Excluir", SqlDbType.Int).Value = idCitaExcluir;
                if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                {
                    throw new RegistroDuplicadoException(
                        "El profesional ya tiene otra cita en esa fecha y hora. Elija otra franja.", "Horario", null);
                }
            }
        }

        private static List<CitaResumen> Consultar(string sql, Action<SqlCommand> parametros)
        {
            List<CitaResumen> lista = new List<CitaResumen>();
            using (SqlConnection cn = Conexion.Crear())
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                parametros(cmd);
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    int ordLlegada = dr.GetOrdinal("HoraLlegada");
                    int ordLlamado = dr.GetOrdinal("HoraLlamado");
                    while (dr.Read())
                    {
                        lista.Add(new CitaResumen
                        {
                            HoraLlegada = dr.IsDBNull(ordLlegada) ? (DateTime?)null : dr.GetDateTime(ordLlegada),
                            HoraLlamado = dr.IsDBNull(ordLlamado) ? (DateTime?)null : dr.GetDateTime(ordLlamado),
                            IdCita = dr.GetInt32(dr.GetOrdinal("IdCita")),
                            Turno = dr.GetInt32(dr.GetOrdinal("NumeroTurno")),
                            Fecha = dr.GetDateTime(dr.GetOrdinal("Fecha")),
                            Hora = dr.GetTimeSpan(dr.GetOrdinal("Hora")),
                            Estado = (EstadoCita)Enum.Parse(typeof(EstadoCita), dr.GetString(dr.GetOrdinal("Estado")).Trim(), true),
                            IdPaciente = dr.GetInt32(dr.GetOrdinal("IdPaciente")),
                            Dni = UtilSql.LeerDni(dr, "DniCifrado"),   // Se descifra solo en memoria
                            NombrePaciente = dr.GetString(dr.GetOrdinal("NombrePaciente")),
                            IdConsultorio = dr.GetInt32(dr.GetOrdinal("IdConsultorio")),
                            Consultorio = dr.GetString(dr.GetOrdinal("Consultorio")),
                            IdPosta = dr.GetInt32(dr.GetOrdinal("IdPosta")),
                            IdServicio = dr.GetInt32(dr.GetOrdinal("IdServicio")),
                            Servicio = dr.GetString(dr.GetOrdinal("Servicio")),
                            IdPersonal = dr.GetInt32(dr.GetOrdinal("IdPersonal")),
                            Profesional = dr.GetString(dr.GetOrdinal("Profesional"))
                        });
                    }
                }
            }
            return lista;
        }
    }
}
