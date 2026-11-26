using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SigepostaAdmision.Entidades.Respaldo
{
    /// <summary>
    /// DTO del reporte diario de citas (para XmlSerializer: constructor sin
    /// parámetros y propiedades públicas get/set). El DNI de cada cita va
    /// ENMASCARADO, porque el reporte es para gestión, no para identificar.
    /// </summary>
    [XmlRoot("ReporteDiario")]
    public class ReporteDiarioXml
    {
        public ReporteDiarioXml()
        {
            Resumen = new ResumenReporteXml();
            Citas = new List<CitaXml>();
        }

        [XmlElement("Fecha", DataType = "date")]
        public DateTime Fecha { get; set; }

        /// <summary>Nombre de la posta, o "Todas las postas".</summary>
        [XmlElement("Posta")]
        public string Posta { get; set; }

        [XmlElement("GeneradoPor")]
        public string GeneradoPor { get; set; }

        [XmlElement("FechaGeneracion")]
        public DateTime FechaGeneracion { get; set; }

        [XmlElement("Resumen")]
        public ResumenReporteXml Resumen { get; set; }

        [XmlArray("Citas")]
        [XmlArrayItem("Cita")]
        public List<CitaXml> Citas { get; set; }
    }
}
