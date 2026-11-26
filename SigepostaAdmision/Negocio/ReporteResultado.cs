using System;
using System.Collections.Generic;
using System.Data;

namespace SigepostaAdmision.Negocio
{
    /// <summary>Reportes disponibles en el módulo.</summary>
    public enum TipoReporte
    {
        CitasDelDia,
        AtendidosPorPosta,
        ProduccionAdmision,
        Auditoria
    }

    /// <summary>
    /// Un reporte ya generado, listo para mostrar, exportar a CSV o imprimir.
    /// La pantalla, el CSV y la impresión usan este mismo objeto, así los tres
    /// muestran exactamente lo mismo.
    /// </summary>
    public class ReporteResultado
    {
        public ReporteResultado()
        {
            Resumen = new List<KeyValuePair<string, string>>();
            Grafico = new List<KeyValuePair<string, double>>();
        }

        public TipoReporte Tipo { get; set; }
        public string Titulo { get; set; }

        /// <summary>Filtros aplicados, en texto (por ejemplo "Fecha: 16/11/2026 · Posta: Todas").</summary>
        public string Parametros { get; set; }

        /// <summary>Filas del reporte (DataTable desconectado). Nunca trae el DNI completo.</summary>
        public DataTable Tabla { get; set; }

        /// <summary>Totales para mostrar (texto / valor).</summary>
        public List<KeyValuePair<string, string>> Resumen { get; set; }

        /// <summary>Datos del gráfico (etiqueta / valor); vacío si el reporte no lleva gráfico.</summary>
        public List<KeyValuePair<string, double>> Grafico { get; set; }

        public string TituloGrafico { get; set; }

        public string GeneradoPor { get; set; }
        public DateTime FechaGeneracion { get; set; }

        public bool TieneDatos
        {
            get { return Tabla != null && Tabla.Rows.Count > 0; }
        }

        public bool TieneGrafico
        {
            get { return Grafico.Count > 0; }
        }
    }

    /// <summary>
    /// Resultado de pedir un reporte: hereda de ResultadoOperacion (Exito, Mensajes)
    /// y, si tuvo éxito, trae el ReporteResultado.
    /// </summary>
    public class ResultadoReporte : ResultadoOperacion
    {
        public ReporteResultado Reporte { get; set; }

        public static ResultadoReporte Fallo(string mensaje)
        {
            ResultadoReporte r = new ResultadoReporte();
            r.AgregarError(mensaje);
            return r;
        }
    }
}
