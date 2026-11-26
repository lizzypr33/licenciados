using System;

namespace SigepostaAdmision.Entidades
{
    /// <summary>
    /// Clase de SOLO LECTURA: una fila de la cola de atención, con los datos ya
    /// calculados para mostrar (posición, minutos de espera, si ya fue llamado).
    /// </summary>
    public class ItemCola
    {
        /// <summary>Posición en la cola (1 = el primero que llegó).</summary>
        public int Posicion { get; set; }
        public int IdCita { get; set; }
        public int Turno { get; set; }
        public TimeSpan HoraCita { get; set; }
        public string Paciente { get; set; }
        public int IdConsultorio { get; set; }
        public string Consultorio { get; set; }
        public string Profesional { get; set; }
        public DateTime? HoraLlegada { get; set; }

        /// <summary>Minutos que lleva esperando desde que llegó.</summary>
        public int EsperaMinutos { get; set; }

        /// <summary>Espera estimada hasta ser llamado (0 si ya fue llamado).</summary>
        public int EsperaEstimadaMinutos { get; set; }

        /// <summary>true si ya se le llamó al consultorio (HoraLlamado no es null).</summary>
        public bool Llamado { get; set; }
        public DateTime? HoraLlamado { get; set; }

        public string HoraCitaTexto
        {
            get { return HoraCita.ToString(@"hh\:mm"); }
        }
    }
}
