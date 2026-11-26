using System.Xml.Serialization;

namespace SigepostaAdmision.Entidades.Respaldo
{
    /// <summary>Una cita dentro del reporte diario (DNI enmascarado, hora como texto).</summary>
    public class CitaXml
    {
        [XmlAttribute("Turno")]
        public int Turno { get; set; }

        [XmlAttribute("Hora")]
        public string Hora { get; set; }

        [XmlElement("Paciente")]
        public string Paciente { get; set; }

        /// <summary>Por ejemplo "45••••78" (Seguridad.Mascara.Dni).</summary>
        [XmlElement("DniEnmascarado")]
        public string DniEnmascarado { get; set; }

        [XmlElement("Consultorio")]
        public string Consultorio { get; set; }

        [XmlElement("Profesional")]
        public string Profesional { get; set; }

        [XmlElement("Estado")]
        public string Estado { get; set; }
    }
}
