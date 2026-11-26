using System;
using System.Xml.Serialization;

namespace SigepostaAdmision.Entidades.Respaldo
{
    /// <summary>
    /// Datos del paciente dentro del respaldo de su historia clínica.
    /// Contiene el DNI COMPLETO (por eso el respaldo de historias debería ir
    /// cifrado). Sexo es string ("M"/"F") porque XmlSerializer guardaría un char
    /// como número.
    /// </summary>
    public class PacienteXml
    {
        [XmlElement("Dni")]
        public string Dni { get; set; }

        [XmlElement("ApellidoPaterno")]
        public string ApellidoPaterno { get; set; }

        [XmlElement("ApellidoMaterno")]
        public string ApellidoMaterno { get; set; }

        [XmlElement("Nombres")]
        public string Nombres { get; set; }

        /// <summary>DataType = "date": se escribe solo la fecha (2000-05-10), sin hora.</summary>
        [XmlElement("FechaNacimiento", DataType = "date")]
        public DateTime FechaNacimiento { get; set; }

        [XmlElement("Sexo")]
        public string Sexo { get; set; }

        [XmlElement("Direccion")]
        public string Direccion { get; set; }

        [XmlElement("Telefono")]
        public string Telefono { get; set; }
    }
}
