using System.Xml.Serialization;

namespace SigepostaAdmision.Entidades.Respaldo
{
    /// <summary>
    /// Totales del reporte diario. Se usan atributos XML ([XmlAttribute]) para que
    /// quede compacto: &lt;Resumen Total="12" Pendientes="5" ... /&gt;.
    /// </summary>
    public class ResumenReporteXml
    {
        [XmlAttribute("Total")]
        public int Total { get; set; }

        [XmlAttribute("Pendientes")]
        public int Pendientes { get; set; }

        [XmlAttribute("EnCola")]
        public int EnCola { get; set; }

        [XmlAttribute("Atendidas")]
        public int Atendidas { get; set; }

        [XmlAttribute("Canceladas")]
        public int Canceladas { get; set; }
    }
}
