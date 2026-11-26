using System;
using System.Data;
using System.Data.SqlClient;

namespace SigepostaAdmision.Datos
{
    /// <summary>
    /// Consultas de los reportes. Cada método devuelve un DataTable llenado con
    /// SqlDataAdapter (Unidad 6).
    ///
    /// DataReader frente a DataAdapter + DataTable:
    ///   - SqlDataReader (lo usan los demás DAO) es un cursor de SOLO AVANCE y solo
    ///     lectura: lee fila por fila mientras la conexión está abierta. Es lo más
    ///     rápido y liviano para convertir filas en objetos (entidades).
    ///   - SqlDataAdapter.Fill() abre la conexión, ejecuta el comando, copia TODAS
    ///     las filas a un DataTable EN MEMORIA y cierra la conexión. El DataTable
    ///     queda "desconectado": se puede recorrer varias veces, agregar o quitar
    ///     columnas (como hace ReporteNegocio con el DNI), enlazarlo directo a un
    ///     DataGridView (DataSource) y exportarlo, sin volver a la base.
    ///   Para reportes de forma tabular y columnas variables, el DataTable es más
    ///   cómodo que crear una clase por cada reporte.
    ///
    /// Todo es parametrizado (@param) y con "using". Los filtros opcionales usan
    /// "(@Param IS NULL OR columna = @Param)". Máximo 1000 filas (Auditoría: 500).
    /// </summary>
    public class ReporteDAO
    {
        private const int MaximoFilas = 1000;
        private const int MaximoFilasAuditoria = 500;

        // ===================== Citas del día =====================

        /// <summary>Citas de una fecha: Turno, Hora, Paciente, DniCifrado, Consultorio, Profesional, Servicio, Estado.</summary>
        public DataTable CitasDelDia(DateTime fecha, int? idPosta, int? idConsultorio, string estado)
        {
            string sql =
                "SELECT TOP (" + MaximoFilas + ") c.NumeroTurno AS Turno, CONVERT(char(5), c.Hora, 108) AS Hora, " +
                "p.ApellidoPaterno + ' ' + p.ApellidoMaterno + ', ' + p.Nombres AS Paciente, p.DniCifrado, " +
                "co.Nombre AS Consultorio, ps.Apellidos + ', ' + ps.Nombres AS Profesional, " +
                "s.Nombre AS Servicio, c.Estado " +
                "FROM dbo.Cita c " +
                "INNER JOIN dbo.Paciente p ON p.IdPaciente = c.IdPaciente " +
                "INNER JOIN dbo.Consultorio co ON co.IdConsultorio = c.IdConsultorio " +
                "INNER JOIN dbo.Servicio s ON s.IdServicio = co.IdServicio " +
                "INNER JOIN dbo.PersonalSalud ps ON ps.IdPersonal = c.IdPersonal " +
                "WHERE c.Fecha = @Fecha " +
                "  AND (@IdPosta IS NULL OR co.IdPosta = @IdPosta) " +
                "  AND (@IdConsultorio IS NULL OR c.IdConsultorio = @IdConsultorio) " +
                "  AND (@Estado IS NULL OR c.Estado = @Estado) " +
                "ORDER BY c.Hora, co.Nombre, c.NumeroTurno";

            return Llenar(sql, cmd =>
            {
                cmd.Parameters.Add("@Fecha", SqlDbType.Date).Value = fecha.Date;
                cmd.Parameters.Add("@IdPosta", SqlDbType.Int).Value = Nulable(idPosta);
                cmd.Parameters.Add("@IdConsultorio", SqlDbType.Int).Value = Nulable(idConsultorio);
                cmd.Parameters.Add("@Estado", SqlDbType.VarChar, 10).Value =
                    string.IsNullOrWhiteSpace(estado) ? (object)DBNull.Value : estado.Trim();
            });
        }

        // ===================== Pacientes atendidos por posta =====================

        /// <summary>
        /// Una fila por atención registrada en [desde, hastaExclusivo):
        /// FechaHora, Posta, Servicio, Consultorio, Profesional, Paciente, DniCifrado,
        /// NumeroHistoria, Observaciones. (El diagnóstico NO se incluye.)
        /// </summary>
        public DataTable AtendidosPorPosta(DateTime desde, DateTime hastaExclusivo, int? idPosta)
        {
            string sql =
                "SELECT TOP (" + MaximoFilas + ") a.FechaHora, po.Nombre AS Posta, s.Nombre AS Servicio, " +
                "co.Nombre AS Consultorio, ps.Apellidos + ', ' + ps.Nombres AS Profesional, " +
                "p.ApellidoPaterno + ' ' + p.ApellidoMaterno + ', ' + p.Nombres AS Paciente, p.DniCifrado, " +
                "h.NumeroHistoria, a.Observaciones " +
                "FROM dbo.Atencion a " +
                "INNER JOIN dbo.Cita c ON c.IdCita = a.IdCita " +
                "INNER JOIN dbo.Consultorio co ON co.IdConsultorio = c.IdConsultorio " +
                "INNER JOIN dbo.Posta po ON po.IdPosta = co.IdPosta " +
                "INNER JOIN dbo.Servicio s ON s.IdServicio = co.IdServicio " +
                "INNER JOIN dbo.PersonalSalud ps ON ps.IdPersonal = c.IdPersonal " +
                "INNER JOIN dbo.Paciente p ON p.IdPaciente = c.IdPaciente " +
                "INNER JOIN dbo.HistoriaClinica h ON h.IdHistoria = a.IdHistoria " +
                "WHERE a.FechaHora >= @Desde AND a.FechaHora < @HastaExclusivo " +
                "  AND (@IdPosta IS NULL OR co.IdPosta = @IdPosta) " +
                "ORDER BY a.FechaHora";

            return Llenar(sql, cmd =>
            {
                AgregarRango(cmd, desde, hastaExclusivo);
                cmd.Parameters.Add("@IdPosta", SqlDbType.Int).Value = Nulable(idPosta);
            });
        }

        // ===================== Producción de Admisión =====================

        /// <summary>
        /// Devuelve un DataSet con DOS tablas:
        ///   "Indicadores" (Indicador, Valor) y "AtencionesPorServicio" (Servicio, Cantidad).
        ///
        /// IMPORTANTE: "Pacientes nuevos registrados" e "Historias abiertas" son de
        /// TODA LA RED: la tabla Paciente y la tabla HistoriaClinica no guardan en qué
        /// posta se registró el paciente, así que no se pueden filtrar por posta.
        /// Los demás indicadores (citas, atenciones, espera) sí respetan la posta.
        /// </summary>
        public DataSet ProduccionAdmision(DateTime desde, DateTime hastaExclusivo, int? idPosta)
        {
            // 1) Todos los conteos en UNA consulta (una fila con varias columnas).
            const string sqlConteos =
                "SELECT " +
                // Toda la red (sin filtro de posta):
                " (SELECT COUNT(*) FROM dbo.Paciente WHERE FechaRegistro >= @Desde AND FechaRegistro < @HastaExclusivo) AS PacientesNuevos, " +
                " (SELECT COUNT(*) FROM dbo.HistoriaClinica WHERE FechaApertura >= @Desde AND FechaApertura < @HastaExclusivo) AS HistoriasAbiertas, " +
                // Con filtro de posta (por el consultorio de la cita):
                " COUNT(*) AS CitasProgramadas, " +
                " SUM(CASE WHEN c.Estado = 'Atendida' THEN 1 ELSE 0 END) AS CitasAtendidas, " +
                " SUM(CASE WHEN c.Estado = 'Cancelada' THEN 1 ELSE 0 END) AS CitasCanceladas, " +
                // Espera: minutos entre la llegada y el llamado, solo donde existen ambas horas.
                " AVG(CASE WHEN c.HoraLlegada IS NOT NULL AND c.HoraLlamado IS NOT NULL " +
                "          THEN DATEDIFF(SECOND, c.HoraLlegada, c.HoraLlamado) / 60.0 END) AS EsperaPromedio, " +
                " SUM(CASE WHEN c.HoraLlegada IS NOT NULL AND c.HoraLlamado IS NOT NULL THEN 1 ELSE 0 END) AS CitasConEspera " +
                "FROM dbo.Cita c INNER JOIN dbo.Consultorio co ON co.IdConsultorio = c.IdConsultorio " +
                "WHERE c.Fecha >= @DesdeFecha AND c.Fecha <= @HastaFecha " +
                "  AND (@IdPosta IS NULL OR co.IdPosta = @IdPosta)";

            DataTable conteos = Llenar(sqlConteos, cmd =>
            {
                AgregarRango(cmd, desde, hastaExclusivo);
                // Cita.Fecha es DATE (sin hora): comparación inclusiva normal con el último
                // día elegido, que es el día anterior al límite exclusivo.
                cmd.Parameters.Add("@DesdeFecha", SqlDbType.Date).Value = desde.Date;
                cmd.Parameters.Add("@HastaFecha", SqlDbType.Date).Value = hastaExclusivo.Date.AddDays(-1);
                cmd.Parameters.Add("@IdPosta", SqlDbType.Int).Value = Nulable(idPosta);
            });

            // 2) Atenciones por servicio (agrupadas en SQL con GROUP BY).
            const string sqlServicios =
                "SELECT s.Nombre AS Servicio, COUNT(*) AS Cantidad " +
                "FROM dbo.Atencion a " +
                "INNER JOIN dbo.Cita c ON c.IdCita = a.IdCita " +
                "INNER JOIN dbo.Consultorio co ON co.IdConsultorio = c.IdConsultorio " +
                "INNER JOIN dbo.Servicio s ON s.IdServicio = co.IdServicio " +
                "WHERE a.FechaHora >= @Desde AND a.FechaHora < @HastaExclusivo " +
                "  AND (@IdPosta IS NULL OR co.IdPosta = @IdPosta) " +
                "GROUP BY s.Nombre ORDER BY Cantidad DESC, s.Nombre";

            DataTable porServicio = Llenar(sqlServicios, cmd =>
            {
                AgregarRango(cmd, desde, hastaExclusivo);
                cmd.Parameters.Add("@IdPosta", SqlDbType.Int).Value = Nulable(idPosta);
            });
            porServicio.TableName = "AtencionesPorServicio";

            // 3) Se da forma a la tabla de indicadores (Indicador / Valor).
            DataRow f = conteos.Rows[0];
            int programadas = Entero(f["CitasProgramadas"]);
            int canceladas = Entero(f["CitasCanceladas"]);
            double porcentajeCancelacion = programadas == 0 ? 0 : 100.0 * canceladas / programadas;

            DataTable indicadores = new DataTable("Indicadores");
            indicadores.Columns.Add("Indicador", typeof(string));
            indicadores.Columns.Add("Valor", typeof(string));
            indicadores.Rows.Add("Pacientes nuevos registrados (toda la red)", Entero(f["PacientesNuevos"]).ToString());
            indicadores.Rows.Add("Historias abiertas (toda la red)", Entero(f["HistoriasAbiertas"]).ToString());
            indicadores.Rows.Add("Citas programadas", programadas.ToString());
            indicadores.Rows.Add("Citas atendidas", Entero(f["CitasAtendidas"]).ToString());
            indicadores.Rows.Add("Citas canceladas", canceladas.ToString());
            indicadores.Rows.Add("% de cancelación", porcentajeCancelacion.ToString("0.0") + " %");
            indicadores.Rows.Add("Espera promedio (minutos entre llegada y llamado)",
                f["EsperaPromedio"] == DBNull.Value
                    ? "Sin datos"
                    : Convert.ToDouble(f["EsperaPromedio"]).ToString("0.0") + " min (" + Entero(f["CitasConEspera"]) + " citas)");

            DataSet ds = new DataSet("ProduccionAdmision");
            ds.Tables.Add(indicadores);
            ds.Tables.Add(porServicio);
            return ds;
        }

        // ===================== Auditoría =====================

        /// <summary>
        /// Registros de auditoría entre las fechas, de la más reciente a la más antigua
        /// (máximo 500). usuario y accion filtran por coincidencia parcial (LIKE).
        /// </summary>
        public DataTable Auditoria(DateTime desde, DateTime hastaExclusivo, string usuario, string accion)
        {
            string sql =
                "SELECT TOP (" + MaximoFilasAuditoria + ") a.IdAuditoria, a.FechaHora, " +
                "ISNULL(u.NombreUsuario, '(sin usuario)') AS Usuario, a.Accion, a.Tabla, a.IdRegistro, a.Detalle " +
                "FROM dbo.Auditoria a LEFT JOIN dbo.Usuario u ON u.IdUsuario = a.IdUsuario " +
                "WHERE a.FechaHora >= @Desde AND a.FechaHora < @HastaExclusivo " +
                "  AND (@Usuario IS NULL OR u.NombreUsuario LIKE @Usuario) " +
                "  AND (@Accion IS NULL OR a.Accion LIKE @Accion) " +
                "ORDER BY a.FechaHora DESC, a.IdAuditoria DESC";

            return Llenar(sql, cmd =>
            {
                AgregarRango(cmd, desde, hastaExclusivo);
                cmd.Parameters.Add("@Usuario", SqlDbType.VarChar, 100).Value =
                    string.IsNullOrWhiteSpace(usuario) ? (object)DBNull.Value : UtilSql.PatronContiene(usuario);
                cmd.Parameters.Add("@Accion", SqlDbType.VarChar, 100).Value =
                    string.IsNullOrWhiteSpace(accion) ? (object)DBNull.Value : UtilSql.PatronContiene(accion);
            });
        }

        // ===================== Apoyo =====================

        /// <summary>
        /// Llena un DataTable con SqlDataAdapter. Fill() abre y cierra la conexión
        /// por sí mismo; los "using" garantizan que se liberen conexión, comando y adaptador.
        /// </summary>
        private static DataTable Llenar(string sql, Action<SqlCommand> parametros)
        {
            using (SqlConnection cn = Conexion.Crear())
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            using (SqlDataAdapter adaptador = new SqlDataAdapter(cmd))
            {
                parametros(cmd);
                DataTable tabla = new DataTable();
                adaptador.Fill(tabla);
                return tabla;
            }
        }

        /// <summary>
        /// Rango SEMIABIERTO para columnas DATETIME (Atencion.FechaHora, Paciente.FechaRegistro,
        /// HistoriaClinica.FechaApertura, Auditoria.FechaHora): "columna &gt;= @Desde AND
        /// columna &lt; @HastaExclusivo". ReporteNegocio ya envía @Desde a las 00:00 y
        /// @HastaExclusivo a las 00:00 del día SIGUIENTE al último elegido: así entra todo
        /// el último día (también un registro de las 23:59:59) y nada del día siguiente.
        /// </summary>
        private static void AgregarRango(SqlCommand cmd, DateTime desde, DateTime hastaExclusivo)
        {
            cmd.Parameters.Add("@Desde", SqlDbType.DateTime).Value = desde.Date;
            cmd.Parameters.Add("@HastaExclusivo", SqlDbType.DateTime).Value = hastaExclusivo.Date;
        }

        private static object Nulable(int? valor)
        {
            return valor.HasValue ? (object)valor.Value : DBNull.Value;
        }

        private static int Entero(object valor)
        {
            return valor == null || valor == DBNull.Value ? 0 : Convert.ToInt32(valor);
        }
    }
}
