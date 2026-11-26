using System;

namespace SigepostaAdmision.Entidades
{
    /// <summary>
    /// Clase de SOLO LECTURA con los datos de una cita listos para mostrar
    /// (nombres ya unidos con JOIN) y los Id necesarios para operar con ella.
    ///
    /// ¿Por qué no armar Cita + Paciente + Medico completos? Porque para una
    /// grilla basta con textos, y construir toda la jerarquía de entidades por
    /// cada fila sería más lento y obligaría a cargar datos que no se usan.
    /// </summary>
    public class CitaResumen
    {
        public int IdCita { get; set; }
        public int Turno { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }
        public EstadoCita Estado { get; set; }

        public int IdPaciente { get; set; }
        public string NombrePaciente { get; set; }
        public string Dni { get; set; }

        public int IdConsultorio { get; set; }
        public string Consultorio { get; set; }
        public int IdPosta { get; set; }

        public int IdServicio { get; set; }
        public string Servicio { get; set; }

        public int IdPersonal { get; set; }
        public string Profesional { get; set; }

        /// <summary>Hora en que el paciente llegó (null si aún no llegó).</summary>
        public DateTime? HoraLlegada { get; set; }

        /// <summary>Hora en que se le llamó al consultorio (null si no fue llamado).</summary>
        public DateTime? HoraLlamado { get; set; }

        /// <summary>"08:20" (formato corto de la hora).</summary>
        public string HoraTexto
        {
            get { return Hora.ToString(@"hh\:mm"); }
        }
    }
}
