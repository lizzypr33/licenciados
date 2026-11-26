using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SigepostaAdmision.Entidades;
using SigepostaAdmision.Negocio;

namespace SigepostaAdmision.Presentacion
{
    /// <summary>
    /// Módulo Respaldo (solo Administrador): exportar historias y reportes,
    /// verificar respaldos y exportar/importar las claves del sistema.
    ///
    /// El formulario solo recoge datos (contraseña, rutas) y muestra resultados;
    /// toda la lógica (serializar, comprimir, cifrar, validar, auditar) está en
    /// RespaldoNegocio.
    ///
    /// Las operaciones corren en un BackgroundWorker para no congelar la ventana
    /// (PBKDF2 con 100 000 iteraciones, compresión y archivos llevan su tiempo).
    /// DoWork NO toca controles: informa el avance con ReportProgress, y la interfaz
    /// solo se actualiza en ProgressChanged y RunWorkerCompleted (hilo de la interfaz).
    /// </summary>
    public partial class FrmRespaldo : Form
    {
        private static readonly Color ColorError = Tema.TextoError;        // #B91C1C (mismo valor que antes)
        private static readonly Color ColorNormal = Tema.TextoPrincipal;   // Texto principal del tema

        private enum TipoOperacion { ExportarHistoria, ExportarReporte, Verificar, ExportarClaves, ImportarClaves }

        /// <summary>Trabajo que se entrega al BackgroundWorker: qué operación es y qué ejecutar.</summary>
        private sealed class Tarea
        {
            public TipoOperacion Tipo;
            public Func<Action<int, string>, ResultadoRespaldo> Trabajo;
            public ResultadoRespaldo Resultado;
        }

        private readonly RespaldoNegocio negocio = new RespaldoNegocio();

        public FrmRespaldo()
        {
            InitializeComponent();
            Tema.Aplicar(this);   // Solo colores
        }

        private void FrmRespaldo_Load(object sender, EventArgs e)
        {
            dtpFecha.Value = DateTime.Today;

            if (!RespaldoNegocio.EsAdministrador())
            {
                Enabled = false;
                lblEstado.Text = "Solo el Administrador puede usar este módulo.";
                return;
            }

            cboPosta.Items.Add("Todas las postas");
            try
            {
                cboPosta.Items.AddRange(negocio.Postas().ToArray());
            }
            catch (Exception ex)
            {
                Interfaz.MostrarErrorBaseDatos(ex);
            }
            cboPosta.SelectedIndex = cboPosta.Items.Count > 1 ? 1 : 0;
            lblEstado.Text = "Listo.";
        }

        // ===================== Historia clínica =====================

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            ResultadoRespaldo r = negocio.BuscarHistoria(txtNumero.Text);
            lblPacienteHistoria.ForeColor = r.Exito ? ColorNormal : ColorError;
            lblPacienteHistoria.Text = r.Exito ? r.Resumen : r.MensajesComoTexto().Replace("• ", "");
        }

        private void btnExportarHistoria_Click(object sender, EventArgs e)
        {
            string numero = txtNumero.Text.Trim().ToUpperInvariant();
            bool cifrar = chkCifrar.Checked;
            if (cifrar && !ContrasenaValida())
            {
                return;
            }

            // Sin cifrar: aviso fuerte, la historia contiene DNI completo y diagnósticos.
            bool aceptaSinCifrar = false;
            if (!cifrar)
            {
                DialogResult aviso = MessageBox.Show(
                    "ATENCIÓN: la historia clínica contiene el DNI completo y los DIAGNÓSTICOS del paciente.\n\n" +
                    "Exportarla SIN cifrar deja esos datos legibles para cualquiera que obtenga el archivo.\n\n" +
                    "¿Desea exportarla SIN cifrar de todos modos?",
                    "Exportar sin cifrar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                if (aviso != DialogResult.Yes)
                {
                    return;
                }
                aceptaSinCifrar = true;
            }

            string ruta = PedirRutaGuardar(cifrar, (numero.Length > 0 ? numero : "historia"));
            if (ruta == null)
            {
                return;
            }

            string contrasena = txtContrasena.Text, confirmacion = txtConfirmacion.Text;
            Iniciar(TipoOperacion.ExportarHistoria, avance =>
                negocio.ExportarHistoria(numero, ruta, contrasena, confirmacion, cifrar, aceptaSinCifrar, avance));
        }

        // ===================== Reporte diario =====================

        private void btnExportarReporte_Click(object sender, EventArgs e)
        {
            bool cifrar = chkCifrar.Checked;
            if (cifrar && !ContrasenaValida())
            {
                return;
            }

            DateTime fecha = dtpFecha.Value.Date;
            PostaItem posta = cboPosta.SelectedItem as PostaItem;
            int? idPosta = posta != null ? posta.IdPosta : (int?)null;

            string ruta = PedirRutaGuardar(cifrar, "Reporte_" + fecha.ToString("yyyyMMdd"));
            if (ruta == null)
            {
                return;
            }

            string contrasena = txtContrasena.Text, confirmacion = txtConfirmacion.Text;
            Iniciar(TipoOperacion.ExportarReporte, avance =>
                negocio.ExportarReporteDiario(fecha, idPosta, ruta, contrasena, confirmacion, cifrar, avance));
        }

        // ===================== Verificar respaldo =====================

        private void btnAbrirRespaldo_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Abrir respaldo de SIGEPOSTA";
                ofd.Filter = "Respaldos SIGEPOSTA (*.sigebak;*.xml.gz)|*.sigebak;*.xml.gz|Todos los archivos (*.*)|*.*";
                if (ofd.ShowDialog(TopLevelControl) != DialogResult.OK)
                {
                    return;
                }
                string ruta = ofd.FileName;
                string contrasena = txtContrasena.Text;   // Solo se usa si el respaldo está cifrado
                txtResumen.Clear();
                Iniciar(TipoOperacion.Verificar, avance => negocio.VerificarRespaldo(ruta, contrasena, avance));
            }
        }

        // ===================== Claves del sistema =====================

        private void btnExportarClaves_Click(object sender, EventArgs e)
        {
            if (!ContrasenaValida())
            {
                return;
            }
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Title = "Exportar claves del sistema";
                sfd.Filter = "Claves de SIGEPOSTA (*.sigekeys)|*.sigekeys";
                sfd.FileName = "claves_sigeposta" + RespaldoNegocio.ExtensionClaves;
                if (sfd.ShowDialog(TopLevelControl) != DialogResult.OK)
                {
                    return;
                }
                string ruta = sfd.FileName;
                string contrasena = txtContrasena.Text, confirmacion = txtConfirmacion.Text;
                Iniciar(TipoOperacion.ExportarClaves, avance => negocio.ExportarClaves(ruta, contrasena, confirmacion));
            }
        }

        private void btnImportarClaves_Click(object sender, EventArgs e)
        {
            DialogResult aviso = MessageBox.Show(
                "ADVERTENCIA: importar claves REEMPLAZA las claves de cifrado de este equipo.\n\n" +
                "Si se importan claves equivocadas, los DNI y diagnósticos cifrados dejarían de poder leerse.\n" +
                "Antes de reemplazar nada se comprobará que las claves del archivo descifran un DNI real de la base; " +
                "si no lo hacen, no se cambiará nada. La clave actual se conserva como copia (claves.anterior.dat).\n\n" +
                "Escriba en \"Contraseña del respaldo\" la contraseña con la que se exportaron las claves.\n\n" +
                "¿Desea continuar?",
                "Importar claves", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (aviso != DialogResult.Yes)
            {
                return;
            }

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Importar claves del sistema";
                ofd.Filter = "Claves de SIGEPOSTA (*.sigekeys)|*.sigekeys";
                if (ofd.ShowDialog(TopLevelControl) != DialogResult.OK)
                {
                    return;
                }
                string ruta = ofd.FileName;
                string contrasena = txtContrasena.Text;
                Iniciar(TipoOperacion.ImportarClaves, avance => negocio.ImportarClaves(ruta, contrasena));
            }
        }

        // ===================== Ejecución en segundo plano =====================

        private void Iniciar(TipoOperacion tipo, Func<Action<int, string>, ResultadoRespaldo> trabajo)
        {
            if (worker.IsBusy)
            {
                return;
            }
            HabilitarControles(false);
            pbProgreso.Value = 0;
            lblEstado.ForeColor = ColorNormal;
            lblEstado.Text = "Procesando...";
            worker.RunWorkerAsync(new Tarea { Tipo = tipo, Trabajo = trabajo });
        }

        /// <summary>
        /// OTRO HILO: ejecuta la operación de Negocio. Prohibido tocar controles;
        /// el avance se comunica con ReportProgress (porcentaje + texto).
        /// </summary>
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker w = (BackgroundWorker)sender;
            Tarea tarea = (Tarea)e.Argument;
            tarea.Resultado = tarea.Trabajo((porcentaje, texto) => w.ReportProgress(porcentaje, texto));
            e.Result = tarea;
        }

        // HILO DE LA INTERFAZ: aquí sí se actualizan la barra y el texto de estado.
        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (IsDisposed || Disposing)
            {
                return;
            }
            pbProgreso.Value = Math.Max(0, Math.Min(100, e.ProgressPercentage));
            lblEstado.Text = e.UserState as string ?? lblEstado.Text;
        }

        // HILO DE LA INTERFAZ: muestra el resultado de la operación.
        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (IsDisposed || Disposing)
            {
                return;
            }
            HabilitarControles(true);

            if (e.Error != null)
            {
                pbProgreso.Value = 0;
                MostrarEstado(RegistroTecnico.ErrorParaUsuario("Respaldo", e.Error), true);
                return;
            }

            Tarea tarea = (Tarea)e.Result;
            ResultadoRespaldo r = tarea.Resultado;
            if (!r.Exito)
            {
                pbProgreso.Value = 0;
                MostrarEstado(r.MensajesComoTexto().Replace("\n", " ").Replace("• ", ""), true);
                if (tarea.Tipo == TipoOperacion.Verificar)
                {
                    txtResumen.Text = "No se pudo verificar el respaldo:" + Environment.NewLine + Lineas(r.MensajesComoTexto());
                }
                else
                {
                    MessageBox.Show(r.MensajesComoTexto(), "Respaldo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                return;
            }

            switch (tarea.Tipo)
            {
                case TipoOperacion.ExportarHistoria:
                case TipoOperacion.ExportarReporte:
                    MostrarEstado(r.TextoTamanos(), false);
                    txtResumen.Text = "Respaldo generado:" + Environment.NewLine + r.RutaArchivo +
                                      Environment.NewLine + Environment.NewLine + r.TextoTamanos();
                    MessageBox.Show("Respaldo generado:\n" + r.RutaArchivo + "\n\n" + r.TextoTamanos(),
                        "Respaldo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;

                case TipoOperacion.Verificar:
                    MostrarEstado("Respaldo verificado. " + r.TextoTamanos(), false);
                    txtResumen.Text = Lineas(r.Resumen);
                    break;

                default:   // Claves
                    MostrarEstado(r.Resumen, false);
                    MessageBox.Show(r.Resumen, "Claves del sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
            }
        }

        // ===================== Apoyo (solo interfaz) =====================

        // La regla de la contraseña la aplica Negocio; aquí solo se muestra el resultado.
        private bool ContrasenaValida()
        {
            ResultadoOperacion v = negocio.ValidarContrasena(txtContrasena.Text, txtConfirmacion.Text);
            if (!v.Exito)
            {
                MessageBox.Show(v.MensajesComoTexto(), "Contraseña del respaldo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtContrasena.Focus();
            }
            return v.Exito;
        }

        // SaveFileDialog con la extensión según se cifre o no; null si se canceló.
        private string PedirRutaGuardar(bool cifrar, string nombreSugerido)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Title = cifrar ? "Guardar respaldo cifrado" : "Guardar respaldo SIN cifrar";
                sfd.Filter = cifrar
                    ? "Respaldo cifrado (*.sigebak)|*.sigebak"
                    : "Respaldo comprimido sin cifrar (*.xml.gz)|*.xml.gz";
                sfd.FileName = nombreSugerido + (cifrar ? RespaldoNegocio.ExtensionCifrada : RespaldoNegocio.ExtensionSinCifrar);
                return sfd.ShowDialog(TopLevelControl) == DialogResult.OK ? sfd.FileName : null;
            }
        }

        private void HabilitarControles(bool habilitar)
        {
            grpContrasena.Enabled = habilitar;
            grpHistoria.Enabled = habilitar;
            grpReporte.Enabled = habilitar;
            grpVerificar.Enabled = habilitar;
            grpClaves.Enabled = habilitar;
        }

        private void MostrarEstado(string texto, bool esError)
        {
            lblEstado.ForeColor = esError ? ColorError : ColorNormal;
            lblEstado.Text = texto;
        }

        // El TextBox multilínea necesita "\r\n" para cambiar de línea.
        private static string Lineas(string texto)
        {
            return (texto ?? string.Empty).Replace("\r\n", "\n").Replace("\n", Environment.NewLine);
        }
    }
}
