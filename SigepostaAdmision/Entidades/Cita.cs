using System;

namespace SigepostaAdmision.Entidades
{
    /// <summary>
    /// Cita de un paciente con un médico.
    ///
    /// Esta clase NO hereda de nadie: se relaciona con Paciente y Medico por
    /// ASOCIACIÓN (una cita "TIENE UN" paciente y "TIENE UN" médico), no porque
    /// una cita "sea" una persona.
    ///
    /// SOBRECARGA: el método Reprogramar tiene dos versiones con el mismo nombre
    /// pero distintos parámetros (DateTime o string). El compilador elige cuál
    /// usar según el tipo del argumento. Así se ofrece una sola operación
    /// ("reprogramar") con dos formas de uso, sin inventar nombres distintos.
    ///
    /// Encapsulamiento del Estado: su "set" es privado; el estado solo cambia
    /// a través de métodos (Cancelar, MarcarAtendida, PonerEnCola) que validan
    /// las reglas del negocio.
    /// </summary>
    public class Cita
    {
        // ----- Campos privados -----
        private int idCita;
        private Paciente paciente;
        private Medico medico;
        private DateTime fecha;
        private TimeSpan hora;
        private EstadoCita estado;
        private string consultorio;
        private int numeroTurno;
        private DateTime? horaLlegada;   // null = el paciente aún no llegó
        private DateTime? horaLlamado;   // null = aún no fue llamado al consultorio

        // ----- Constructores -----

        /// <summary>Constructor por defecto: cita pendiente para hoy.</summary>
        public Cita()
        {
            fecha = DateTime.Today;
            estado = EstadoCita.Pendiente;
        }

        /// <summary>Constructor parametrizado: toda cita nueva inicia como Pendiente.</summary>
        public Cita(int idCita, Paciente paciente, Medico medico, DateTime fecha,
                    TimeSpan hora, string consultorio, int numeroTurno)
        {
            IdCita = idCita;
            Paciente = paciente;
            Medico = medico;
            Fecha = fecha;
            Hora = hora;
            Consultorio = consultorio;
            NumeroTurno = numeroTurno;
            estado = EstadoCita.Pendiente;
        }

        /// <summary>
        /// Constructor de RECONSTRUCCIÓN: crea el objeto a partir de una cita ya
        /// guardada, conservando su estado real (Pendiente, Atendida, etc.).
        /// Es necesario porque el "set" de Estado es privado: sin este constructor,
        /// toda cita leída de la base parecería Pendiente y las reglas de
        /// Reprogramar/Cancelar no se aplicarían correctamente.
        /// (Paciente y Médico no se cargan aquí: la capa de Negocio trabaja con sus Id.)
        /// horaLlegada y horaLlamado son opcionales: solo se usan en la cola de atención.
        /// </summary>
        public Cita(int idCita, DateTime fecha, TimeSpan hora, string consultorio,
                    int numeroTurno, EstadoCita estado,
                    DateTime? horaLlegada = null, DateTime? horaLlamado = null)
        {
            IdCita = idCita;
            Fecha = fecha;
            Hora = hora;
            Consultorio = consultorio;
            NumeroTurno = numeroTurno;
            if (!Enum.IsDefined(typeof(EstadoCita), estado))
            {
                throw new ArgumentException("El estado de la cita no es válido.");
            }
            this.estado = estado;
            this.horaLlegada = horaLlegada;
            this.horaLlamado = horaLlamado;
        }

        // ----- Propiedades de la cola de atención -----

        /// <summary>
        /// Momento en que el paciente llegó a la posta (null si no llegó).
        /// Solo se modifica con MarcarLlegada / DeshacerLlegada.
        /// </summary>
        public DateTime? HoraLlegada
        {
            get { return horaLlegada; }
        }

        /// <summary>
        /// Momento en que se llamó al paciente al consultorio (null si no fue llamado).
        /// No existe un estado "Llamado": la cita sigue EnCola y "llamado" se
        /// representa con HoraLlamado distinta de null.
        /// </summary>
        public DateTime? HoraLlamado
        {
            get { return horaLlamado; }
        }

        // ----- Propiedades -----

        public int IdCita
        {
            get { return idCita; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("El Id de la cita no puede ser negativo.");
                }
                idCita = value;
            }
        }

        public Paciente Paciente
        {
            get { return paciente; }
            set { paciente = Validacion.ObjetoObligatorio(value, "Paciente"); }
        }

        public Medico Medico
        {
            get { return medico; }
            set { medico = Validacion.ObjetoObligatorio(value, "Médico"); }
        }

        /// <summary>Solo se guarda la fecha (sin hora); la hora va en "Hora".</summary>
        public DateTime Fecha
        {
            get { return fecha; }
            set { fecha = value.Date; }
        }

        /// <summary>Hora del día: debe estar entre 00:00 y 23:59.</summary>
        public TimeSpan Hora
        {
            get { return hora; }
            set
            {
                if (value < TimeSpan.Zero || value >= TimeSpan.FromDays(1))
                {
                    throw new ArgumentException("La hora de la cita debe estar entre 00:00 y 23:59.");
                }
                hora = value;
            }
        }

        /// <summary>Solo lectura desde fuera: se modifica con los métodos de la clase.</summary>
        public EstadoCita Estado
        {
            get { return estado; }
            private set { estado = value; }
        }

        public string Consultorio
        {
            get { return consultorio; }
            set { consultorio = Validacion.TextoObligatorio(value, "Consultorio"); }
        }

        public int NumeroTurno
        {
            get { return numeroTurno; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("El número de turno debe ser mayor que cero.");
                }
                numeroTurno = value;
            }
        }

        // ----- Métodos -----

        /// <summary>
        /// SOBRECARGA 1: reprograma la cita a otra fecha (se mantiene la hora).
        /// La nueva fecha no puede ser anterior a hoy.
        /// </summary>
        public void Reprogramar(DateTime nuevaFecha)
        {
            ValidarQueSePuedeModificar();
            if (nuevaFecha.Date < DateTime.Today)
            {
                throw new ArgumentException("No se puede reprogramar a una fecha pasada.");
            }
            Fecha = nuevaFecha;
        }

        /// <summary>
        /// SOBRECARGA 2: mismo nombre, distinto parámetro. Cambia el consultorio.
        /// </summary>
        public void Reprogramar(string nuevoConsultorio)
        {
            ValidarQueSePuedeModificar();
            Consultorio = nuevoConsultorio;
        }

        /// <summary>Cancela la cita, salvo que ya esté atendida o cancelada.</summary>
        public void Cancelar()
        {
            ValidarQueSePuedeCancelar();
            Estado = EstadoCita.Cancelada;
        }

        /// <summary>Pasa la cita a la cola de atención (el paciente ya llegó).</summary>
        public void PonerEnCola()
        {
            ValidarQueSePuedeModificar();
            Estado = EstadoCita.EnCola;
        }

        /// <summary>
        /// Regla de dominio: el paciente llegó a la posta.
        /// Solo desde Pendiente y solo si la cita es de HOY. Pasa a EnCola y
        /// registra la hora de llegada (que define su lugar en la cola).
        /// </summary>
        public void MarcarLlegada()
        {
            if (estado != EstadoCita.Pendiente)
            {
                throw new InvalidOperationException(
                    "Solo se puede registrar la llegada de una cita Pendiente. Esta cita está " + estado + ".");
            }
            if (fecha.Date != DateTime.Today)
            {
                throw new ArgumentException(
                    "Solo se registra la llegada de citas de hoy. Esta cita es del " + fecha.ToString("dd/MM/yyyy") + ".");
            }
            estado = EstadoCita.EnCola;
            horaLlegada = DateTime.Now;
            horaLlamado = null;
        }

        /// <summary>
        /// Corrige una llegada registrada por error: solo desde EnCola. Vuelve a
        /// Pendiente y borra la hora de llegada y la de llamado.
        /// </summary>
        public void DeshacerLlegada()
        {
            if (estado != EstadoCita.EnCola)
            {
                throw new InvalidOperationException(
                    "Solo se puede deshacer la llegada de una cita En cola. Esta cita está " + estado + ".");
            }
            estado = EstadoCita.Pendiente;
            horaLlegada = null;
            horaLlamado = null;
        }

        /// <summary>
        /// Llama al paciente al consultorio: solo si está EnCola y aún no fue llamado.
        /// El estado no cambia (sigue EnCola hasta ser atendido).
        /// </summary>
        public void MarcarLlamado()
        {
            if (estado != EstadoCita.EnCola)
            {
                throw new InvalidOperationException(
                    "Solo se puede llamar a un paciente En cola. Esta cita está " + estado + ".");
            }
            if (horaLlamado.HasValue)
            {
                throw new InvalidOperationException(
                    "El paciente ya fue llamado a las " + horaLlamado.Value.ToString("HH:mm") + ".");
            }
            horaLlamado = DateTime.Now;
        }

        /// <summary>
        /// Regla de dominio: se atiende al paciente. Solo desde EnCola y solo si
        /// ya fue LLAMADO (HoraLlamado no es null): no se atiende a quien no fue
        /// llamado al consultorio. Pasa a Atendida.
        /// La usan Paciente.RegistrarAtencion() y AtencionNegocio.
        /// </summary>
        public void MarcarAtendida()
        {
            if (estado != EstadoCita.EnCola)
            {
                throw new InvalidOperationException(
                    "Solo se puede atender una cita En cola. Esta cita está " + estado + ".");
            }
            if (!horaLlamado.HasValue)
            {
                throw new InvalidOperationException(
                    "No se puede atender a un paciente que aún no fue llamado al consultorio.");
            }
            estado = EstadoCita.Atendida;
        }

        /// <summary>
        /// Regla de dominio: solo una cita PENDIENTE se puede reprogramar (o poner
        /// en cola). Una cita EnCola ya tiene hora de llegada en la cola del día:
        /// moverla a otra fecha la dejaría "en cola" en un día equivocado; primero
        /// hay que deshacer su llegada. Atendida y Cancelada ya están cerradas.
        /// Es estática para que otras capas (por ejemplo, para habilitar botones)
        /// la consulten sin repetirla.
        /// </summary>
        public static bool EsModificable(EstadoCita estado)
        {
            return estado == EstadoCita.Pendiente;
        }

        /// <summary>
        /// Regla de dominio: se puede CANCELAR una cita Pendiente o EnCola (el
        /// paciente puede retirarse después de llegar). Atendida y Cancelada no.
        /// </summary>
        public static bool EsCancelable(EstadoCita estado)
        {
            return estado == EstadoCita.Pendiente || estado == EstadoCita.EnCola;
        }

        // Lanza una excepción si la cita no está Pendiente (no se puede reprogramar).
        private void ValidarQueSePuedeModificar()
        {
            if (!EsModificable(estado))
            {
                throw new InvalidOperationException(
                    "Solo se puede reprogramar una cita Pendiente. Esta cita está " + estado + ".");
            }
        }

        // Lanza una excepción si la cita ya está cerrada (Atendida o Cancelada).
        private void ValidarQueSePuedeCancelar()
        {
            if (!EsCancelable(estado))
            {
                throw new InvalidOperationException(
                    "No se puede cancelar una cita en estado " + estado + ".");
            }
        }
    }
}
