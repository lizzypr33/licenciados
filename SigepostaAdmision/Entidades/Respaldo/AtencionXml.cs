using System;
using System.Xml.Serialization;

namespace SigepostaAdmision.Entidades.Respaldo
{
    /// <summary>
    /// Una atención dentro del respaldo de la historia clínica, con el diagnóstico
    /// YA DESCIFRADO (el archivo completo se protege con CifradoRespaldo).
    /// Hora es string ("08:20") porque XmlSerializer no sabe serializar TimeSpan.
    /// </summary>
    public class AtencionXml
    {
        [XmlElement("Fecha", DataType = "date")]
        public DateTime Fecha { get; set; }

        [XmlElement("Hora")]
        public string Hora { get; set; }

        [XmlElement("Consultorio")]
        public string Consultorio { get; set; }

        [XmlElement("Profesional")]
        public string Profesional { get; set; }

        [XmlElement("Observaciones")]
        public string Observaciones { get; set; }

        [XmlElement("Diagnostico")]
        public string Diagnostico { get; set; }
    }
}
