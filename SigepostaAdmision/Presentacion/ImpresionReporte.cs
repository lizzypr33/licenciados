using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using SigepostaAdmision.Negocio;

namespace SigepostaAdmision.Presentacion
{
    /// <summary>
    /// Impresión de un ReporteResultado con PrintDocument y PrintPreviewDialog
    /// (sin librerías externas):
    ///   - hoja HORIZONTAL (más columnas por fila),
    ///   - encabezado en cada página: título, filtros, usuario y fecha,
    ///   - tabla con columnas repartidas según el ancho de su contenido,
    ///   - paginación automática y "Página X de Y" al pie.
    ///
    /// Cómo funciona PrintDocument: .NET llama al evento PrintPage una vez por
    /// página y nos da un Graphics donde dibujar. Si quedan filas, se pone
    /// e.HasMorePages = true y .NET vuelve a llamar para la siguiente página.
    /// BeginPrint se llama al empezar (también al reimprimir desde la vista previa),
    /// por eso ahí se reinicia la fila actual y se calcula el total de páginas.
    ///
    /// Unidades: en PrintPage el Graphics trabaja en centésimas de pulgada, así que
    /// font.GetHeight(100) da la altura de una línea en esas mismas unidades.
    /// </summary>
    public sealed class ImpresionReporte : IDisposable
    {
        private const int MaxCaracteresColumna = 40;   // Para repartir el ancho, se ignora el texto más largo que esto

        private readonly ReporteResultado reporte;
        private readonly PrintDocument documento = new PrintDocument();

        private readonly Font fuenteTitulo = new Font("Segoe UI", 14F, FontStyle.Bold);
        private readonly Font fuenteSubtitulo = new Font("Segoe UI", 8.5F);
        private readonly Font fuenteEncabezado = new Font("Segoe UI", 8.5F, FontStyle.Bold);
        private readonly Font fuenteCelda = new Font("Segoe UI", 8.5F);
        private readonly Font fuentePie = new Font("Segoe UI", 8F);

        private int filaActual;
        private int paginaActual;
        private int totalPaginas;
        private float[] anchosRelativos;

        public ImpresionReporte(ReporteResultado reporte)
        {
            if (reporte == null || reporte.Tabla == null) throw new ArgumentNullException("reporte");
            this.reporte = reporte;

            documento.DocumentName = "SIGEPOSTA - " + reporte.Titulo;
            documento.DefaultPageSettings.Landscape = true;                       // Hoja horizontal
            documento.DefaultPageSettings.Margins = new Margins(50, 50, 50, 50);  // Media pulgada
            documento.BeginPrint += Documento_BeginPrint;
            documento.PrintPage += Documento_PrintPage;
        }

        /// <summary>Abre la vista previa (desde ahí el usuario puede imprimir).</summary>
        public void MostrarVistaPrevia(IWin32Window propietario)
        {
            try
            {
                using (PrintPreviewDialog vista = new PrintPreviewDialog())
                {
                    vista.Document = documento;
                    vista.Width = 1000;
                    vista.Height = 720;
                    vista.UseAntiAlias = true;
                    vista.Text = "Vista previa - " + reporte.Titulo;
                    vista.ShowDialog(propietario);
                }
            }
            catch (InvalidPrinterException)
            {
                MessageBox.Show("No hay una impresora instalada. Instale una (por ejemplo \"Microsoft Print to PDF\") para ver la vista previa.",
                    "Imprimir", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ===================== Preparación =====================

        private void Documento_BeginPrint(object sender, PrintEventArgs e)
        {
            filaActual = 0;
            paginaActual = 0;
            anchosRelativos = CalcularAnchosRelativos();

            // Total de páginas = filas / filas por página (redondeando hacia arriba).
            int porPagina = FilasPorPagina(documento.DefaultPageSettings);
            int filas = reporte.Tabla.Rows.Count;
            totalPaginas = Math.Max(1, (int)Math.Ceiling(filas / (double)porPagina));
        }

        /// <summary>
        /// Ancho de cada columna proporcional a la longitud de su texto (encabezado y
        /// contenido, con un tope), para que las columnas cortas no desperdicien espacio.
        /// </summary>
        private float[] CalcularAnchosRelativos()
        {
            DataTable t = reporte.Tabla;
            float[] anchos = new float[t.Columns.Count];
            for (int c = 0; c < t.Columns.Count; c++)
            {
                int largo = t.Columns[c].ColumnName.Length;
                foreach (DataRow fila in t.Rows.Cast<DataRow>().Take(300))   // Una muestra basta
                {
                    largo = Math.Max(largo, Math.Min(MaxCaracteresColumna, ReporteNegocio.TextoValor(fila[c]).Length));
                }
                anchos[c] = Math.Max(4, largo);
            }
            float total = anchos.Sum();
            return anchos.Select(a => a / total).ToArray();
        }

        // Altura que ocupan el encabezado y el pie (para saber cuántas filas caben).
        private float AltoEncabezado()
        {
            return fuenteTitulo.GetHeight(100) + 2 * fuenteSubtitulo.GetHeight(100) + 12 + AltoFila();
        }

        private float AltoPie()
        {
            return fuentePie.GetHeight(100) + 10;
        }

        private float AltoFila()
        {
            return fuenteCelda.GetHeight(100) + 6;
        }

        private int FilasPorPagina(PageSettings pagina)
        {
            float alto = pagina.Bounds.Height - pagina.Margins.Top - pagina.Margins.Bottom;
            return Math.Max(1, (int)((alto - AltoEncabezado() - AltoPie()) / AltoFila()));
        }

        // ===================== Dibujo de cada página =====================

        private void Documento_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle area = e.MarginBounds;
            paginaActual++;

            using (StringFormat recorte = new StringFormat(StringFormatFlags.NoWrap) { Trimming = StringTrimming.EllipsisCharacter, LineAlignment = StringAlignment.Center })
            using (Pen linea = new Pen(Color.FromArgb(203, 213, 225)))
            using (SolidBrush fondoEncabezado = new SolidBrush(Color.FromArgb(226, 232, 240)))
            using (SolidBrush fondoAlterno = new SolidBrush(Color.FromArgb(248, 250, 252)))
            using (SolidBrush texto = new SolidBrush(Color.FromArgb(30, 41, 59)))
            using (SolidBrush gris = new SolidBrush(Color.FromArgb(100, 116, 139)))
            {
                float y = area.Top;

                // --- Encabezado: título, filtros, usuario y fecha ---
                g.DrawString(reporte.Titulo, fuenteTitulo, texto, area.Left, y);
                y += fuenteTitulo.GetHeight(g);
                g.DrawString(reporte.Parametros, fuenteSubtitulo, gris, new RectangleF(area.Left, y, area.Width, fuenteSubtitulo.GetHeight(g)), recorte);
                y += fuenteSubtitulo.GetHeight(g);
                g.DrawString("Generado por " + reporte.GeneradoPor + " el " + reporte.FechaGeneracion.ToString("dd/MM/yyyy HH:mm") +
                             "   ·   " + string.Join("  ·  ", reporte.Resumen.Select(p => p.Key + ": " + p.Value)),
                             fuenteSubtitulo, gris, new RectangleF(area.Left, y, area.Width, fuenteSubtitulo.GetHeight(g)), recorte);
                y += fuenteSubtitulo.GetHeight(g) + 12;

                DataTable t = reporte.Tabla;
                float altoFila = AltoFila();

                if (t.Rows.Count == 0)
                {
                    g.DrawString("No hay datos para esos filtros.", fuenteCelda, gris, area.Left, y);
                }
                else
                {
                    // --- Fila de encabezados de columna ---
                    float x = area.Left;
                    g.FillRectangle(fondoEncabezado, area.Left, y, area.Width, altoFila);
                    for (int c = 0; c < t.Columns.Count; c++)
                    {
                        float ancho = area.Width * anchosRelativos[c];
                        g.DrawString(t.Columns[c].ColumnName, fuenteEncabezado, texto, new RectangleF(x + 3, y, ancho - 6, altoFila), recorte);
                        x += ancho;
                    }
                    y += altoFila;

                    // --- Filas que caben en esta página ---
                    int porPagina = FilasPorPagina(e.PageSettings);
                    int hasta = Math.Min(t.Rows.Count, filaActual + porPagina);
                    for (int f = filaActual; f < hasta; f++)
                    {
                        if ((f - filaActual) % 2 == 1)
                        {
                            g.FillRectangle(fondoAlterno, area.Left, y, area.Width, altoFila);
                        }
                        x = area.Left;
                        for (int c = 0; c < t.Columns.Count; c++)
                        {
                            float ancho = area.Width * anchosRelativos[c];
                            g.DrawString(ReporteNegocio.TextoValor(t.Rows[f][c]), fuenteCelda, texto,
                                         new RectangleF(x + 3, y, ancho - 6, altoFila), recorte);
                            x += ancho;
                        }
                        y += altoFila;
                        g.DrawLine(linea, area.Left, y, area.Right, y);
                    }
                    filaActual = hasta;
                }

                // --- Pie: "Página X de Y" ---
                float yPie = area.Bottom - fuentePie.GetHeight(g);
                g.DrawLine(linea, area.Left, yPie - 4, area.Right, yPie - 4);
                g.DrawString("SIGEPOSTA · Admisión", fuentePie, gris, area.Left, yPie);
                string paginas = "Página " + paginaActual + " de " + totalPaginas;
                SizeF tam = g.MeasureString(paginas, fuentePie);
                g.DrawString(paginas, fuentePie, gris, area.Left + (area.Width - tam.Width) / 2, yPie);
            }

            // ¿Quedan filas? .NET volverá a llamar a PrintPage para la siguiente página.
            e.HasMorePages = filaActual < reporte.Tabla.Rows.Count;
        }

        public void Dispose()
        {
            documento.Dispose();
            fuenteTitulo.Dispose();
            fuenteSubtitulo.Dispose();
            fuenteEncabezado.Dispose();
            fuenteCelda.Dispose();
            fuentePie.Dispose();
        }
    }
}
