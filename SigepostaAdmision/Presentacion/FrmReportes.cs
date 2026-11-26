using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using SigepostaAdmision.Entidades;
using SigepostaAdmision.Negocio;

namespace SigepostaAdmision.Presentacion
{
    /// <summary>
    /// Módulo Reportes: elige el tipo, aplica filtros, genera (en segundo plano),
    /// muestra la tabla, un resumen y un gráfico, y permite exportar a CSV o imprimir.
    ///
    /// El formulario no tiene SQL ni reglas: pide el reporte a ReporteNegocio, que
    /// valida fechas, aplica el alcance por rol y enmascara el DNI.
    /// La consulta corre en un BackgroundWorker; la interfaz solo se actualiza en
    /// RunWorkerCompleted (hilo de la interfaz).
    /// </summary>
    public partial class FrmReportes : Form
    {
        private const string Todos = "Todos";
        private const string TodasLasPostas = "Todas las postas";

        private readonly ReporteNegocio negocio = new ReporteNegocio();
        private ReporteResultado reporteActual;
        private bool cargando;

        public FrmReportes()
        {
            InitializeComponent();
            Tema.Aplicar(this);   // Solo colores
        }

        private void FrmReportes_Load(object sender, EventArgs e)
        {
            cargando = true;
            dtpDesde.Value = DateTime.Today;
            dtpHasta.Value = DateTime.Today;

            // Tipos de reporte según el rol (Auditoría solo para el Administrador).
            foreach (TipoReporte tipo in ReporteNegocio.TiposDisponibles())
            {
                cboTipo.Items.Add(tipo);
            }

            // La opción "sin filtro" la define Negocio (antes había "Todos" aquí y "Todas" allá).
            cboEstado.Items.Add(ReporteNegocio.TodosLosEstados);
            cboEstado.Items.AddRange(Enum.GetNames(typeof(EstadoCita)));
            cboEstado.SelectedIndex = 0;

            try
            {
                // Postas: el Administrador elige; un usuario Admision ve solo la suya, fija.
                List<PostaItem> postas = negocio.PostasParaFiltro();
                if (ReporteNegocio.EsAdministrador())
                {
                    cboPosta.Items.Add(TodasLasPostas);
                }
                cboPosta.Items.AddRange(postas.ToArray());
                cboPosta.Enabled = ReporteNegocio.EsAdministrador();
                if (cboPosta.Items.Count > 0) cboPosta.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Interfaz.MostrarErrorBaseDatos(ex);
            }

            cargando = false;
            cboTipo.SelectedIndex = 0;   // Dispara AjustarFiltros
            CargarConsultorios();
            MostrarSinResultados("Elija el reporte y los filtros, y pulse \"Generar\".");
        }

        // Nombre visible de cada tipo de reporte en el ComboBox.
        private void cboTipo_Format(object sender, ListControlConvertEventArgs e)
        {
            if (e.ListItem is TipoReporte)
            {
                e.Value = ReporteNegocio.Nombre((TipoReporte)e.ListItem);
            }
        }

        private TipoReporte TipoElegido
        {
            get { return cboTipo.SelectedItem is TipoReporte ? (TipoReporte)cboTipo.SelectedItem : TipoReporte.CitasDelDia; }
        }

        // ===================== Filtros visibles según el reporte =====================

        private void cboTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            AjustarFiltros();
        }

        /// <summary>Muestra solo los filtros que aplican al reporte elegido.</summary>
        private void AjustarFiltros()
        {
            TipoReporte tipo = TipoElegido;
            bool esCitas = tipo == TipoReporte.CitasDelDia;
            bool esAuditoria = tipo == TipoReporte.Auditoria;

            // Citas del día usa una sola fecha ("Desde" pasa a llamarse "Fecha").
            lblDesde.Text = esCitas ? "Fecha:" : "Desde:";
            lblHasta.Visible = dtpHasta.Visible = !esCitas;

            // Posta: no aplica a Auditoría.
            lblPosta.Visible = cboPosta.Visible = !esAuditoria;

            // Consultorio y Estado: solo en Citas del día.
            lblConsultorio.Visible = cboConsultorio.Visible = esCitas;
            lblEstado.Visible = cboEstado.Visible = esCitas;

            // Usuario y Acción: solo en Auditoría.
            lblUsuario.Visible = txtUsuario.Visible = esAuditoria;
            lblAccion.Visible = txtAccion.Visible = esAuditoria;
        }

        private void cboPosta_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!cargando)
            {
                CargarConsultorios();
            }
        }

        private int? PostaElegida()
        {
            PostaItem p = cboPosta.SelectedItem as PostaItem;
            return p != null ? p.IdPosta : (int?)null;
        }

        private void CargarConsultorios()
        {
            cboConsultorio.Items.Clear();
            cboConsultorio.Items.Add(Todos);
            try
            {
                cboConsultorio.Items.AddRange(negocio.Consultorios(PostaElegida()).ToArray());
            }
            catch (Exception ex)
            {
                Interfaz.MostrarErrorBaseDatos(ex);
            }
            cboConsultorio.SelectedIndex = 0;
        }

        // ===================== Generar (en segundo plano) =====================

        private void btnGenerar_Click(object sender, EventArgs e)
        {
            if (worker.IsBusy)
            {
                return;
            }

            // Se leen los filtros AQUÍ (hilo de la interfaz); DoWork no puede tocar controles.
            TipoReporte tipo = TipoElegido;
            DateTime desde = dtpDesde.Value.Date;
            DateTime hasta = dtpHasta.Value.Date;
            int? idPosta = PostaElegida();
            ConsultorioItem consultorio = cboConsultorio.SelectedItem as ConsultorioItem;
            int? idConsultorio = consultorio != null ? consultorio.IdConsultorio : (int?)null;
            string estado = cboEstado.SelectedItem as string;
            string usuario = txtUsuario.Text;
            string accion = txtAccion.Text;

            Func<ResultadoReporte> trabajo;
            switch (tipo)
            {
                case TipoReporte.CitasDelDia:
                    trabajo = () => negocio.CitasDelDia(desde, idPosta, idConsultorio, estado);
                    break;
                case TipoReporte.AtendidosPorPosta:
                    trabajo = () => negocio.AtendidosPorPosta(desde, hasta, idPosta);
                    break;
                case TipoReporte.ProduccionAdmision:
                    trabajo = () => negocio.ProduccionAdmision(desde, hasta, idPosta);
                    break;
                default:
                    trabajo = () => negocio.Auditoria(desde, hasta, usuario, accion);
                    break;
            }

            btnGenerar.Enabled = false;
            btnExportarCsv.Enabled = false;
            btnImprimir.Enabled = false;
            pbGenerando.Visible = true;
            lblResumen.Text = "Generando el reporte...";
            worker.RunWorkerAsync(trabajo);
        }

        // OTRO HILO: solo ejecuta la consulta; no toca controles.
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Func<ResultadoReporte> trabajo = (Func<ResultadoReporte>)e.Argument;
            e.Result = trabajo();
        }

        // HILO DE LA INTERFAZ: aquí se muestra el resultado.
        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (IsDisposed || Disposing)
            {
                return;
            }
            pbGenerando.Visible = false;
            btnGenerar.Enabled = true;

            if (e.Error != null)
            {
                reporteActual = null;
                MostrarSinResultados(RegistroTecnico.ErrorParaUsuario("Reportes", e.Error));
                return;
            }

            ResultadoReporte r = (ResultadoReporte)e.Result;
            if (!r.Exito)
            {
                reporteActual = null;
                MostrarSinResultados(r.MensajesComoTexto().Replace("• ", ""));
                return;
            }

            reporteActual = r.Reporte;
            MostrarReporte(reporteActual);
        }

        // ===================== Mostrar el resultado =====================

        private void MostrarReporte(ReporteResultado rep)
        {
            lblResumen.Text = rep.Titulo + "  —  " + string.Join("  ·  ", rep.Resumen.Select(p => p.Key + ": " + p.Value));

            if (!rep.TieneDatos)
            {
                MostrarSinResultados("No hay datos para esos filtros.");
                lblResumen.Text = rep.Titulo + "  —  " + rep.Parametros;
                return;
            }

            // El DataTable se enlaza directo a la grilla (DataSource).
            grdReporte.DataSource = null;
            grdReporte.Columns.Clear();
            grdReporte.DataSource = rep.Tabla;
            foreach (DataGridViewColumn col in grdReporte.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.Automatic;
                if (col.ValueType == typeof(DateTime))
                {
                    col.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                }
            }
            grdReporte.Visible = true;
            lblSinDatos.Visible = false;

            // Gráfico solo cuando el reporte lo trae (Auditoría no lleva).
            chtReporte.Series[0].Points.Clear();
            if (rep.TieneGrafico)
            {
                chtReporte.Titles[0].Text = rep.TituloGrafico;
                foreach (KeyValuePair<string, double> punto in rep.Grafico)
                {
                    chtReporte.Series[0].Points.AddXY(punto.Key, punto.Value);
                }
                chtReporte.Visible = true;
            }
            else
            {
                chtReporte.Visible = false;
            }

            btnExportarCsv.Enabled = true;
            btnImprimir.Enabled = true;
        }

        /// <summary>En lugar de una grilla vacía, un mensaje en el centro.</summary>
        private void MostrarSinResultados(string mensaje)
        {
            grdReporte.DataSource = null;
            grdReporte.Visible = false;
            chtReporte.Visible = false;
            lblSinDatos.Text = mensaje;
            lblSinDatos.Visible = true;
            btnExportarCsv.Enabled = false;
            btnImprimir.Enabled = false;
            if (reporteActual == null)
            {
                lblResumen.Text = string.Empty;
            }
        }

        // ===================== Exportar e imprimir =====================

        private void btnExportarCsv_Click(object sender, EventArgs e)
        {
            if (reporteActual == null)
            {
                return;
            }
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Title = "Exportar reporte a CSV";
                sfd.Filter = "CSV para Excel (separado por punto y coma) (*.csv)|*.csv";
                sfd.FileName = reporteActual.Titulo.Replace(" ", "_") + "_" + reporteActual.FechaGeneracion.ToString("yyyyMMdd_HHmm") + ".csv";
                if (sfd.ShowDialog(TopLevelControl) != DialogResult.OK)
                {
                    return;
                }

                ResultadoOperacion r = negocio.ExportarCsv(reporteActual, sfd.FileName);
                if (r.Exito)
                {
                    MessageBox.Show("Reporte exportado:\n" + sfd.FileName, "Exportar CSV", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(r.MensajesComoTexto(), "Exportar CSV", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            if (reporteActual == null)
            {
                return;
            }
            using (ImpresionReporte impresion = new ImpresionReporte(reporteActual))
            {
                impresion.MostrarVistaPrevia(TopLevelControl);
            }
        }
    }
}
