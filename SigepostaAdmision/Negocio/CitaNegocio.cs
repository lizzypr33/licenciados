using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using SigepostaAdmision.Datos;
using SigepostaAdmision.Entidades;

namespace SigepostaAdmision.Negocio
{
    /// <summary>
    /// Reglas de negocio del módulo Citas. Los formularios no validan nada:
    /// todo pasa por aquí y los errores vuelven en un ResultadoOperacion.
    ///
    /// Reglas:
    ///   1. Horario de atención: lunes a sábado, de 08:00 a 13:00, en franjas
    ///      de 20 minutos (08:00, 08:20, ..., 12:40).
    ///   2. No se programa en el pasado (ni una hora de hoy que ya pasó).
    ///   3. El paciente debe tener historia clínica y estar Activa
    ///      (una historia Inactiva no recibe citas nuevas).
    ///   4. Un profesional no puede tener dos citas no canceladas en la misma
    ///      fecha y hora.
    ///   5. Un paciente no puede tener dos citas no canceladas del mismo
    ///      servicio el mismo día.
    ///   6. El profesional debe ser personal asistencial (Médico, Obstetra o
    ///      Enfermero) y trabajar en la posta del consultorio.
    ///   7. Solo se REPROGRAMA una cita Pendiente (una EnCola debe deshacer antes su
    ///      llegada); se CANCELA una cita Pendiente o EnCola (reglas de la entidad
    ///      Cita). Las citas no se borran: se cancelan.
    ///   8. Un usuario Admision solo ve y gestiona las citas de SU posta; el
    ///      Administrador ve todas.
    /// </summary>
    public class CitaNegocio
    {
        // ----- Regla 1: horario -----
        public static readonly TimeSpan HoraInicio = new TimeSpan(8, 0, 0);
        public static readonly TimeSpan HoraFin = new TimeSpan(13, 0, 0);
        public static readonly TimeSpan DuracionFranja = TimeSpan.FromMinutes(20);

        // ----- Regla 6: tipos de personal que atienden pacientes -----
        private static readonly string[] TiposAsistenciales = { "Medico", "Obstetra", "Enfermero" };

        private readonly CitaDAO citaDao = new CitaDAO();
        private readonly CatalogoDAO catalogoDao = new CatalogoDAO();
        private readonly HistoriaDAO historiaDao = new HistoriaDAO();
        private readonly PacienteDAO pacienteDao = new PacienteDAO();

        // ===================== Horario (regla 1) =====================

        /// <summary>Lunes a sábado.</summary>
        public static bool EsDiaLaborable(DateTime fecha)
        {
            return fecha.DayOfWeek != DayOfWeek.Sunday;
        }

        /// <summary>Todas las franjas del día (vacío si es domingo).</summary>
        public static List<TimeSpan> FranjasDelDia(DateTime fecha)
        {
            List<TimeSpan> franjas = new List<TimeSpan>();
            if (!EsDiaLaborable(fecha))
            {
                return franjas;
            }
            // La última franja empieza a las 12:40 y termina a las 13:00.
            for (TimeSpan h = HoraInicio; h + DuracionFranja <= HoraFin; h += DuracionFranja)
            {
                franjas.Add(h);
            }
            return franjas;
        }

        /// <summary>¿La hora coincide exactamente con el inicio de una franja?</summary>
        public static bool EsFranjaValida(TimeSpan hora)
        {
            return hora >= HoraInicio
                && hora + DuracionFranja <= HoraFin
                && (hora - HoraInicio).Ticks % DuracionFranja.Ticks == 0;
        }

        /// <summary>
        /// Franjas libres del profesional en la fecha: las del horario menos las
        /// ocupadas por citas no canceladas. Si la fecha es hoy, solo las futuras.
        /// idCitaExcluir: al reprogramar, la propia cita no ocupa su franja.
        /// </summary>
        public List<TimeSpan> ObtenerFranjasLibres(int idPersonal, DateTime fecha, int idCitaExcluir = 0)
        {
            List<TimeSpan> libres = new List<TimeSpan>();
            if (idPersonal <= 0 || fecha.Date < DateTime.Today)
            {
                return libres;
            }

            List<TimeSpan> ocupadas = citaDao.HorasOcupadas(idPersonal, fecha, idCitaExcluir);
            bool esHoy = fecha.Date == DateTime.Today;
            TimeSpan ahora = DateTime.Now.TimeOfDay;

            foreach (TimeSpan franja in FranjasDelDia(fecha))
            {
                if (ocupadas.Contains(franja)) continue;
                if (esHoy && franja <= ahora) continue;
                libres.Add(franja);
            }
            return libres;
        }

        /// <summary>Regla 7 para la interfaz: solo una cita Pendiente se puede reprogramar.</summary>
        public static bool PuedeReprogramarse(EstadoCita estado)
        {
            return Cita.EsModificable(estado);
        }

        /// <summary>Regla 7 para la interfaz: se puede cancelar una cita Pendiente o EnCola.</summary>
        public static bool PuedeCancelarse(EstadoCita estado)
        {
            return Cita.EsCancelable(estado);
        }

        // ===================== Catálogos (con filtro de posta, regla 8) =====================

        public List<ServicioItem> ServiciosDisponibles()
        {
            return SinPostaAsignada() ? new List<ServicioItem>() : catalogoDao.ServiciosConConsultorio(PostaDelUsuario());
        }

        /// <summary>Consultorios visibles para el usuario; idServicio null = todos.</summary>
        public List<ConsultorioItem> Consultorios(int? idServicio)
        {
            return SinPostaAsignada()
                ? new List<ConsultorioItem>()
                : catalogoDao.ConsultoriosPorPostaYServicio(PostaDelUsuario(), idServicio);
        }

        /// <summary>Personal asistencial de la posta indicada (normalmente, la del consultorio elegido).</summary>
        public List<ProfesionalItem> Profesionales(int idPosta)
        {
            List<ProfesionalItem> lista = new List<ProfesionalItem>();
            if (!PuedeVerPosta(idPosta))
            {
                return lista;
            }
            foreach (ProfesionalItem p in catalogoDao.PersonalPorPosta(idPosta))
            {
                if (EsAsistencial(p))
                {
                    lista.Add(p);
                }
            }
            return lista;
        }

        // ===================== Consultas de citas =====================

        /// <summary>Citas de la fecha visibles para el usuario (regla 8).</summary>
        public List<CitaResumen> ListarCitas(DateTime fecha, int? idConsultorio, EstadoCita? estado)
        {
            if (SinPostaAsignada())
            {
                return new List<CitaResumen>();
            }
            return citaDao.ListarPorFecha(fecha, PostaDelUsuario(), idConsultorio, estado);
        }

        /// <summary>Una cita, o null si no existe o no es de la posta del usuario.</summary>
        public CitaResumen ObtenerCita(int idCita)
        {
            CitaResumen c = idCita > 0 ? citaDao.ObtenerPorId(idCita) : null;
            return c != null && PuedeVerPosta(c.IdPosta) ? c : null;
        }

        // ===================== Operaciones =====================

        /// <summary>
        /// Programa una cita nueva. Valida todas las reglas, guarda la cita
        /// 'Pendiente' con el IdUsuario de la sesión y devuelve el turno asignado.
        /// </summary>
        public ResultadoOperacion ProgramarCita(int idPaciente, int idConsultorio, int idPersonal,
                                                DateTime fecha, TimeSpan hora)
        {
            try
            {
                ResultadoOperacion r = new ResultadoOperacion();
                ValidarProgramacion(r, idPaciente, idConsultorio, idPersonal, fecha, hora, 0);
                if (!r.Exito)
                {
                    return r;
                }

                int turno;
                r.IdGenerado = citaDao.Insertar(idPaciente, idConsultorio, idPersonal,
                    Sesion.UsuarioActual.IdUsuario, fecha.Date, hora, out turno);
                r.NumeroTurno = turno;

                // Auditoría solo tras el éxito (si falla, no rompe la operación).
                new AuditoriaNegocio().Registrar("PROGRAMAR_CITA", "Cita", r.IdGenerado,
                    "Turno " + turno + " para el " + fecha.ToString("dd/MM/yyyy") + " " + hora.ToString(@"hh\:mm"));
                return r;
            }
            catch (RegistroDuplicadoException ex)
            {
                // Otro usuario tomó la franja justo antes (detectado dentro de la transacción).
                return ResultadoOperacion.Error(ex.Message);
            }
            catch (Exception ex) when (ex is SqlException || ex is InvalidOperationException)
            {
                return ResultadoOperacion.Error(RegistroTecnico.ErrorParaUsuario("Citas - programar", ex));
            }
        }

        /// <summary>
        /// Reprograma una cita: puede cambiar fecha, hora, consultorio y profesional.
        ///
        /// Solo se reprograma una cita PENDIENTE: una cita EnCola ya está en la cola
        /// del día (primero hay que deshacer su llegada), y una Atendida o Cancelada
        /// está cerrada.
        ///
        /// Se reconstruye la entidad Cita (CitaFabrica) y se usan sus métodos
        /// SOBRECARGADOS para aplicar la regla de dominio:
        ///   - Reprogramar(DateTime) → cambia la fecha (no acepta fechas pasadas).
        ///   - Reprogramar(string)   → cambia el consultorio.
        /// El compilador elige cuál ejecutar según el tipo del argumento.
        /// Luego se revalidan los cupos con las MISMAS reglas que al programar
        /// (excluyendo la propia cita) y se guarda con ActualizarProgramacion.
        /// </summary>
        public ResultadoOperacion Reprogramar(int idCita, DateTime nuevaFecha, TimeSpan nuevaHora,
                                              int idConsultorio, int idPersonal)
        {
            try
            {
                CitaResumen actual = ObtenerCita(idCita);
                if (actual == null)
                {
                    return ResultadoOperacion.Error("La cita no existe o no pertenece a su posta.");
                }

                if (actual.Fecha.Date == nuevaFecha.Date && actual.Hora == nuevaHora &&
                    actual.IdConsultorio == idConsultorio && actual.IdPersonal == idPersonal)
                {
                    return ResultadoOperacion.Error("No se realizó ningún cambio en la programación.");
                }

                if (actual.Estado == EstadoCita.EnCola)
                {
                    return ResultadoOperacion.Error(
                        "Esta cita ya está en cola. Deshaga la llegada en Cola de atención antes de reprogramarla.");
                }

                // Regla de dominio en la entidad (sobrecargas de Reprogramar).
                Cita cita = CitaFabrica.DesdeResumen(actual);
                try
                {
                    cita.Reprogramar(nuevaFecha.Date);                    // Sobrecarga 1: DateTime

                    ConsultorioItem nuevoConsultorio = idConsultorio > 0 ? catalogoDao.ObtenerConsultorio(idConsultorio) : null;
                    if (nuevoConsultorio != null && nuevoConsultorio.IdConsultorio != actual.IdConsultorio)
                    {
                        cita.Reprogramar(nuevoConsultorio.Nombre);       // Sobrecarga 2: string
                    }
                }
                catch (InvalidOperationException ex)
                {
                    // Regla de la entidad: la cita no está Pendiente (mensaje pensado para el usuario).
                    return ResultadoOperacion.Error(ex.Message);
                }
                catch (ArgumentException ex)
                {
                    return ResultadoOperacion.Error(ex.Message);   // Por ejemplo: fecha pasada
                }

                ResultadoOperacion r = new ResultadoOperacion();
                ValidarProgramacion(r, actual.IdPaciente, idConsultorio, idPersonal, cita.Fecha, nuevaHora, idCita);
                if (!r.Exito)
                {
                    return r;
                }

                r.NumeroTurno = citaDao.ActualizarProgramacion(idCita, cita.Fecha, nuevaHora, idConsultorio, idPersonal);
                r.IdGenerado = idCita;

                new AuditoriaNegocio().Registrar("REPROGRAMAR_CITA", "Cita", idCita,
                    "Nueva programación: " + cita.Fecha.ToString("dd/MM/yyyy") + " " + nuevaHora.ToString(@"hh\:mm") +
                    ", turno " + r.NumeroTurno);
                return r;
            }
            catch (RegistroDuplicadoException ex)
            {
                return ResultadoOperacion.Error(ex.Message);   // Franja tomada por otro puesto
            }
            catch (ReglaNegocioException ex)
            {
                return ResultadoOperacion.Error(ex.Message);   // La cita cambió de estado en otro puesto
            }
            catch (Exception ex) when (ex is SqlException || ex is InvalidOperationException)
            {
                return ResultadoOperacion.Error(RegistroTecnico.ErrorParaUsuario("Citas - reprogramar", ex));
            }
        }

        /// <summary>
        /// Cancela una cita Pendiente o EnCola usando Cita.Cancelar() (regla de la
        /// entidad). La cita NO se borra: queda guardada con Estado 'Cancelada',
        /// para conservar el historial y liberar la franja del profesional.
        /// </summary>
        public ResultadoOperacion Cancelar(int idCita)
        {
            try
            {
                CitaResumen actual = ObtenerCita(idCita);
                if (actual == null)
                {
                    return ResultadoOperacion.Error("La cita no existe o no pertenece a su posta.");
                }

                Cita cita = CitaFabrica.DesdeResumen(actual);
                try
                {
                    cita.Cancelar();
                }
                catch (InvalidOperationException)
                {
                    return ResultadoOperacion.Error(
                        "Solo se pueden cancelar citas Pendientes o En cola. Esta cita está " + actual.Estado + ".");
                }

                if (!citaDao.CambiarEstado(idCita, cita.Estado))
                {
                    return ResultadoOperacion.Error("No se encontró la cita.");
                }

                new AuditoriaNegocio().Registrar("CANCELAR_CITA", "Cita", idCita,
                    "Estado anterior: " + actual.Estado);

                ResultadoOperacion r = new ResultadoOperacion();
                r.IdGenerado = idCita;
                return r;
            }
            catch (Exception ex) when (ex is SqlException || ex is InvalidOperationException)
            {
                return ResultadoOperacion.Error(RegistroTecnico.ErrorParaUsuario("Citas - cancelar", ex));
            }
        }

        // ===================== Validación común (programar y reprogramar) =====================

        /// <summary>
        /// Aplica las reglas 1 a 6 y 8, acumulando TODOS los errores en "r".
        /// Las comprobaciones que consultan la base de datos (cupos) solo se hacen
        /// si los datos básicos son correctos.
        /// </summary>
        private void ValidarProgramacion(ResultadoOperacion r, int idPaciente, int idConsultorio, int idPersonal,
                                         DateTime fecha, TimeSpan hora, int idCitaExcluir)
        {
            if (!Sesion.HaySesion)
            {
                r.AgregarError("No hay una sesión iniciada.");
                return;
            }

            // Regla 3: paciente con historia clínica Activa.
            bool pacienteOk = false;
            if (idPaciente <= 0 || pacienteDao.ObtenerPorId(idPaciente) == null)
            {
                r.AgregarError("Busque y seleccione un paciente registrado.");
            }
            else
            {
                HistoriaClinica historia = historiaDao.ObtenerPorPaciente(idPaciente);
                if (historia == null)
                {
                    r.AgregarError("El paciente no tiene historia clínica. Ábrala en el módulo Historias.");
                }
                else if (historia.Estado != EstadoHistoria.Activa)
                {
                    r.AgregarError("La historia clínica " + historia.NumeroHistoria +
                                   " está Inactiva: no puede recibir citas nuevas.");
                }
                else
                {
                    pacienteOk = true;
                }
            }

            // Consultorio (y regla 8: de la posta del usuario).
            ConsultorioItem consultorio = null;
            if (idConsultorio <= 0)
            {
                r.AgregarError("Seleccione un consultorio.");
            }
            else
            {
                consultorio = catalogoDao.ObtenerConsultorio(idConsultorio);
                if (consultorio == null)
                {
                    r.AgregarError("El consultorio seleccionado no existe.");
                }
                else if (!PuedeVerPosta(consultorio.IdPosta))
                {
                    r.AgregarError("El consultorio no pertenece a su posta.");
                    consultorio = null;
                }
            }

            // Regla 6: profesional asistencial de la misma posta que el consultorio.
            bool profesionalOk = false;
            if (idPersonal <= 0)
            {
                r.AgregarError("Seleccione un profesional.");
            }
            else
            {
                ProfesionalItem profesional = catalogoDao.ObtenerPersonal(idPersonal);
                if (profesional == null)
                {
                    r.AgregarError("El profesional seleccionado no existe.");
                }
                else if (!EsAsistencial(profesional))
                {
                    r.AgregarError(profesional.NombreCompleto + " no es personal asistencial (" + profesional.Tipo + ").");
                }
                else if (consultorio != null && profesional.IdPosta != consultorio.IdPosta)
                {
                    r.AgregarError("El profesional no trabaja en la posta del consultorio seleccionado.");
                }
                else
                {
                    profesionalOk = true;
                }
            }

            // Reglas 1 y 2: fecha y hora.
            bool horarioOk = false;
            if (fecha.Date < DateTime.Today)
            {
                r.AgregarError("No se puede programar una cita en una fecha pasada.");
            }
            else if (!EsDiaLaborable(fecha))
            {
                r.AgregarError("Solo se atiende de lunes a sábado.");
            }
            else if (!EsFranjaValida(hora))
            {
                r.AgregarError("Seleccione una hora disponible (franjas de 20 minutos entre 08:00 y 13:00).");
            }
            else if (fecha.Date == DateTime.Today && hora <= DateTime.Now.TimeOfDay)
            {
                r.AgregarError("La hora " + hora.ToString(@"hh\:mm") + " de hoy ya pasó.");
            }
            else
            {
                horarioOk = true;
            }

            // Regla 4: el profesional no puede tener dos citas a la misma hora.
            if (profesionalOk && horarioOk &&
                citaDao.HorasOcupadas(idPersonal, fecha, idCitaExcluir).Contains(hora))
            {
                r.AgregarError("El profesional ya tiene una cita el " + fecha.ToString("dd/MM/yyyy") +
                               " a las " + hora.ToString(@"hh\:mm") + ".");
            }

            // Regla 5: una cita por servicio y día para el paciente.
            if (pacienteOk && consultorio != null && fecha.Date >= DateTime.Today &&
                citaDao.ExisteCitaPacienteServicioFecha(idPaciente, consultorio.IdServicio, fecha, idCitaExcluir))
            {
                r.AgregarError("El paciente ya tiene una cita de " + consultorio.Servicio +
                               " el " + fecha.ToString("dd/MM/yyyy") + ".");
            }
        }

        // ===================== Regla 8: posta del usuario =====================
        // La regla vive en AccesoPosta (compartida con ColaNegocio); aquí solo se delega.

        private static int? PostaDelUsuario()
        {
            return AccesoPosta.PostaDelUsuario();
        }

        private static bool SinPostaAsignada()
        {
            return AccesoPosta.SinPostaAsignada();
        }

        private static bool PuedeVerPosta(int idPosta)
        {
            return AccesoPosta.PuedeVerPosta(idPosta);
        }

        private static bool EsAsistencial(ProfesionalItem p)
        {
            return Array.IndexOf(TiposAsistenciales, p.Tipo) >= 0;
        }
    }
}
