using System;

namespace SigepostaAdmision.Entidades
{
    /// <summary>
    /// Clase de SOLO LECTURA con los datos de una atención registrada (tabla
    /// Atencion unida con Cita, Consultorio, PersonalSalud y Usuario) que se
    /// muestran en la historia clínica.
    /// </summary>
    public class AtencionResumen
    {
        public int IdAtencion { get; set; }
        public int IdCita { get; set; }

        /// <summary>Fecha en que se registró la atención.</summary>
        public DateTime Fecha { get; set; }

        /// <summary>Hora en que se registró la atención.</summary>
        public TimeSpan Hora { get; set; }

        public string Consultorio { get; set; }
        public string Profesional { get; set; }
        public string Observaciones { get; set; }

        /// <summary>Nombre del usuario que registró la atención.</summary>
        public string RegistradaPor { get; set; }
    }
}
