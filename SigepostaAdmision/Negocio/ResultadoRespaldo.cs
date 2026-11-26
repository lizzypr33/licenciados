using System;

namespace SigepostaAdmision.Negocio
{
    /// <summary>
    /// Resultado de una operación de respaldo. HEREDA de ResultadoOperacion
    /// (Exito, Mensajes) y agrega los datos propios del respaldo: ruta final,
    /// tamaños y un resumen en texto.
    /// </summary>
    public class ResultadoRespaldo : ResultadoOperacion
    {
        /// <summary>Ruta del archivo escrito o leído (con la extensión definitiva).</summary>
        public string RutaArchivo { get; set; }

        /// <summary>Tamaño del XML sin comprimir.</summary>
        public long BytesXml { get; set; }

        /// <summary>Tamaño final del archivo (comprimido y, si corresponde, cifrado).</summary>
        public long BytesFinal { get; set; }

        /// <summary>Texto para mostrar (por ejemplo, el resumen de un respaldo verificado).</summary>
        public string Resumen { get; set; }

        /// <summary>Porcentaje de reducción respecto del XML (88 = el archivo pesa 88 % menos).</summary>
        public int PorcentajeReduccion
        {
            get { return BytesXml <= 0 ? 0 : (int)Math.Round(100.0 * (BytesXml - BytesFinal) / BytesXml); }
        }

        /// <summary>"XML: 18.4 KB -> Respaldo: 2.1 KB (88% menos)".</summary>
        public string TextoTamanos()
        {
            return "XML: " + FormatearTamano(BytesXml) + " -> Respaldo: " + FormatearTamano(BytesFinal) +
                   " (" + PorcentajeReduccion + "% menos)";
        }

        /// <summary>Bytes en forma legible: "850 B", "18.4 KB", "1.2 MB".</summary>
        public static string FormatearTamano(long bytes)
        {
            System.Globalization.CultureInfo inv = System.Globalization.CultureInfo.InvariantCulture;
            if (bytes < 1024) return bytes + " B";
            if (bytes < 1024 * 1024) return (bytes / 1024.0).ToString("0.0", inv) + " KB";
            return (bytes / (1024.0 * 1024.0)).ToString("0.0", inv) + " MB";
        }

        /// <summary>Resultado fallido con un único mensaje.</summary>
        public static ResultadoRespaldo Fallo(string mensaje)
        {
            ResultadoRespaldo r = new ResultadoRespaldo();
            r.AgregarError(mensaje);
            return r;
        }
    }
}
