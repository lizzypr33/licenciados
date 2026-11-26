using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SigepostaAdmision.Entidades.Respaldo
{
    /// <summary>
    /// DTO (objeto de transferencia) para SERIALIZAR una historia clínica a XML.
    ///
    /// ¿Qué es serializar? Convertir un objeto en memoria en un formato que se
    /// puede guardar o enviar (aquí, texto XML) y luego reconstruirlo
    /// (deserializar). Se usa XmlSerializer, que exige:
    ///   - un constructor público SIN parámetros,
    ///   - propiedades públicas con get y set.
    /// Por eso NO se serializan las entidades del dominio (Persona es abstracta,
    /// Paciente valida en sus "set" y tiene constructores con parámetros): se
    /// copian sus datos a estas clases simples, pensadas solo para el archivo.
    ///
    /// Los atributos [XmlRoot], [XmlElement], [XmlArray] y [XmlArrayItem]
    /// controlan los nombres de las etiquetas para que el XML sea legible.
    /// </summary>
    [XmlRoot("HistoriaClinica")]
    public class HistoriaClinicaXml
    {
        public HistoriaClinicaXml()
        {
            Atenciones = new List<AtencionXml>();
        }

        [XmlElement("NumeroHistoria")]
        public string NumeroHistoria { get; set; }

        [XmlElement("FechaApertura")]
        public DateTime FechaApertura { get; set; }

        /// <summary>"Activa" o "Inactiva".</summary>
        [XmlElement("Estado")]
        public string Estado { get; set; }

        [XmlElement("FechaExportacion")]
        public DateTime FechaExportacion { get; set; }

        /// <summary>Nombre del usuario que exportó.</summary>
        [XmlElement("ExportadoPor")]
        public string ExportadoPor { get; set; }

        [XmlElement("Paciente")]
        public PacienteXml Paciente { get; set; }

        /// <summary>Genera &lt;Atenciones&gt;&lt;Atencion&gt;...&lt;/Atencion&gt;&lt;/Atenciones&gt;.</summary>
        [XmlArray("Atenciones")]
        [XmlArrayItem("Atencion")]
        public List<AtencionXml> Atenciones { get; set; }
    }
}
