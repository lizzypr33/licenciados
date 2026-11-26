using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using SigepostaAdmision.Datos;
using SigepostaAdmision.Entidades;

namespace SigepostaAdmision.Negocio
{
    /// <summary>
    /// Reglas de la cola de atención del día (sin SQL ni controles).
    ///
    /// Flujo de una cita en la cola:
    ///   Pendiente --(RegistrarLlegada)--> EnCola --(LlamarSiguiente: HoraLlamado)--> ...
    ///   EnCola --(DeshacerLlegada, por error)--> Pendiente
    ///
    /// Las reglas de cada cambio están en la entidad Cita (MarcarLlegada,
    /// DeshacerLlegada, MarcarLlamado); aquí se reconstruye la cita, se aplica
    /// la regla y luego se guarda.
    ///
    /// Acceso: un usuario Admision solo opera con citas de su posta; el
    /// Administrador, con todas (AccesoPosta).
    ///
    /// Esta clase se usa desde un hilo en segundo plano (BackgroundWorker), por
    /// eso no guarda estado entre llamadas: cada método crea sus propios datos.
    /// </summary>
    public class ColaNegocio
    {
        /// <summary>
        /// Minutos promedio de atención por paciente, usados para estimar la espera
        /// (posición del paciente en su consultorio × 15 min). Es una estimación
        /// simple: en un sistema real se calcularía con los tiempos históricos.
        /// </summary>
        public const int MinutosPorPaciente = 15;

        /// <summary>Ventana de los recordatorios: citas que empiezan en los próximos 60 minutos.</summary>
        public const int MinutosRecordatorio = 60;

        private readonly CitaDAO citaDao = new CitaDAO();

        // ===================== Snapshot (lectura) =====================

        /// <summary>
        /// Arma la "foto" de la cola: cola ordenada (con ColaAtencion), espera real,
        /// espera estimada, citas pendientes y recordatorios de la próxima hora.
        /// idPosta: para un usuario Admision siempre se usa SU posta (por seguridad,
        /// aunque se pida otra); para el Administrador, null = todas.
        /// Las excepciones de conexión (SqlException) se dejan pasar: el formulario
        /// las muestra como "Sin conexión" y reintenta.
        /// </summary>
        public ColaSnapshot ObtenerSnapshot(DateTime fecha, int? idPosta, int? idConsultorio)
        {
            ColaSnapshot foto = new ColaSnapshot();
            DateTime ahora = DateTime.Now;
            foto.Generado = ahora;

            if (AccesoPosta.SinPostaAsignada())
            {
                return foto;
            }
            int? posta = AccesoPosta.PostaDelUsuario() ?? idPosta;

            // 1) Cola: ColaAtencion se encarga del orden (HoraLlegada, luego turno).
            ColaAtencion cola = new ColaAtencion(citaDao.ListarCola(fecha, posta, idConsultorio));

            // Cuántos pacientes SIN llamar hay delante, por consultorio, para la espera estimada.
            Dictionary<int, int> esperandoPorConsultorio = new Dictionary<int, int>();
            int posicion = 0;

            foreach (CitaResumen c in cola)   // foreach usa el enumerador de ColaAtencion
            {
                posicion++;
                bool llamado = c.HoraLlamado.HasValue;

                int estimada = 0;
                if (!llamado)
                {
                    int delante;
                    esperandoPorConsultorio.TryGetValue(c.IdConsultorio, out delante);
                    esperandoPorConsultorio[c.IdConsultorio] = delante + 1;
                    // Posición en su consultorio (1, 2, 3...) × 15 minutos.
                    estimada = (delante + 1) * MinutosPorPaciente;
                }

                foto.Cola.Add(new ItemCola
                {
                    Posicion = posicion,
                    IdCita = c.IdCita,
                    Turno = c.Turno,
                    HoraCita = c.Hora,
                    Paciente = c.NombrePaciente,
                    IdConsultorio = c.IdConsultorio,
                    Consultorio = c.Consultorio,
                    Profesional = c.Profesional,
                    HoraLlegada = c.HoraLlegada,
                    EsperaMinutos = c.HoraLlegada.HasValue
                        ? Math.Max(0, (int)(ahora - c.HoraLlegada.Value).TotalMinutes)
                        : 0,
                    EsperaEstimadaMinutos = estimada,
                    Llamado = llamado,
                    HoraLlamado = c.HoraLlamado
                });
            }

            // 2) Pendientes del día (aún no llegan), por hora.
            foto.Pendientes = citaDao.ListarPendientesDelDia(fecha, posta, idConsultorio)
                .OrderBy(c => c.Hora)
                .ThenBy(c => c.Turno)
                .ToList();

            // 3) Recordatorios SIMULADOS: pendientes de hoy que empiezan en los próximos 60 min.
            if (fecha.Date == DateTime.Today)
            {
                TimeSpan desde = ahora.TimeOfDay;
                TimeSpan hasta = desde.Add(TimeSpan.FromMinutes(MinutosRecordatorio));
                foto.Recordatorios = foto.Pendientes
                    .Where(c => c.Hora >= desde && c.Hora <= hasta)
                    .Select(c => c.HoraTexto + " · " + c.NombrePaciente + " · " + c.Consultorio +
                                 " (en " + (int)Math.Ceiling((c.Hora - desde).TotalMinutes) + " min)")
                    .ToList();
            }

            return foto;
        }

        // ===================== Acciones =====================

        /// <summary>
        /// El paciente llegó: Pendiente → EnCola. La regla (solo Pendiente y solo
        /// citas de hoy) la valida Cita.MarcarLlegada(); luego se guarda.
        /// </summary>
        public ResultadoOperacion RegistrarLlegada(int idCita)
        {
            return CambiarEnCola(idCita,
                cita => cita.MarcarLlegada(),
                () => citaDao.MarcarLlegada(idCita),
                "La cita ya no está Pendiente (otro puesto pudo registrar la llegada). Se actualizará la lista.",
                "REGISTRAR_LLEGADA", "Llegada del paciente: pasa a la cola");
        }

        /// <summary>
        /// Corrige una llegada registrada por error: EnCola → Pendiente
        /// (Cita.DeshacerLlegada() valida la regla).
        /// </summary>
        public ResultadoOperacion DeshacerLlegada(int idCita)
        {
            return CambiarEnCola(idCita,
                cita => cita.DeshacerLlegada(),
                () => citaDao.DeshacerLlegada(idCita),
                "La cita ya no está En cola (otro puesto pudo cambiarla). Se actualizará la lista.",
                null, null);   // Deshacer no está en la lista de acciones auditadas
        }

        /// <summary>
        /// Llama al primer paciente de la cola que aún NO fue llamado (del
        /// consultorio indicado, o de toda la posta si idConsultorio es null),
        /// registra su HoraLlamado y lo devuelve. Si no hay nadie, devuelve null.
        ///
        /// Si otro puesto llamó a ese mismo paciente un instante antes, el UPDATE
        /// afecta 0 filas (el WHERE exige HoraLlamado IS NULL) y se intenta con el
        /// siguiente de la cola.
        /// </summary>
        public CitaResumen LlamarSiguiente(int? idConsultorio)
        {
            if (AccesoPosta.SinPostaAsignada())
            {
                return null;
            }
            int? posta = AccesoPosta.PostaDelUsuario();

            const int intentos = 3;
            for (int i = 0; i < intentos; i++)
            {
                ColaAtencion cola = new ColaAtencion(citaDao.ListarCola(DateTime.Today, posta, idConsultorio));
                CitaResumen siguiente = cola.FirstOrDefault(c => !c.HoraLlamado.HasValue);
                if (siguiente == null)
                {
                    return null;   // Nadie esperando
                }

                // Regla de dominio (EnCola y sin llamar).
                CitaFabrica.DesdeResumen(siguiente).MarcarLlamado();

                if (citaDao.MarcarLlamado(siguiente.IdCita) == 1)
                {
                    new AuditoriaNegocio().Registrar("LLAMAR_PACIENTE", "Cita", siguiente.IdCita,
                        "Llamado al consultorio, turno " + siguiente.Turno);
                    return citaDao.ObtenerPorId(siguiente.IdCita);
                }
                // 0 filas: otro puesto lo llamó primero; se intenta con el siguiente.
            }
            return null;
        }

        // ===================== Apoyo =====================

        /// <summary>
        /// Patrón común de RegistrarLlegada y DeshacerLlegada:
        ///   1) leer la cita y comprobar que sea de la posta del usuario,
        ///   2) aplicar la regla en la entidad (lanza excepción si no se cumple),
        ///   3) guardar; si el UPDATE afecta 0 filas, otro puesto se adelantó,
        ///   4) si tuvo éxito y se indicó una acción, registrarla en la auditoría.
        /// </summary>
        private ResultadoOperacion CambiarEnCola(int idCita, Action<Cita> reglaDominio,
                                                 Func<int> guardar, string mensajeConflicto,
                                                 string accionAuditoria, string detalleAuditoria)
        {
            try
            {
                CitaResumen actual = idCita > 0 ? citaDao.ObtenerPorId(idCita) : null;
                if (actual == null || !AccesoPosta.PuedeVerPosta(actual.IdPosta))
                {
                    return ResultadoOperacion.Error("La cita no existe o no pertenece a su posta.");
                }

                try
                {
                    reglaDominio(CitaFabrica.DesdeResumen(actual));
                }
                catch (InvalidOperationException ex)
                {
                    return ResultadoOperacion.Error(ex.Message);
                }
                catch (ArgumentException ex)
                {
                    return ResultadoOperacion.Error(ex.Message);
                }

                if (guardar() != 1)
                {
                    return ResultadoOperacion.Error(mensajeConflicto);
                }

                if (accionAuditoria != null)
                {
                    new AuditoriaNegocio().Registrar(accionAuditoria, "Cita", idCita, detalleAuditoria);
                }

                ResultadoOperacion r = new ResultadoOperacion();
                r.IdGenerado = idCita;
                return r;
            }
            catch (SqlException ex)
            {
                return ResultadoOperacion.Error(RegistroTecnico.ErrorParaUsuario("Cola de atención", ex));
            }
        }
    }
}
