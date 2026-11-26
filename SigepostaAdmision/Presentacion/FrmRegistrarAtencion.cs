using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using SigepostaAdmision.Entidades;
using SigepostaAdmision.Negocio;
using SigepostaAdmision.Seguridad;

namespace SigepostaAdmision.Presentacion
{
    /// <summary>
    /// Diálogo modal para registrar la atención de un paciente llamado.
    /// Muestra los datos de la cita (solo lectura) y pide observaciones opcionales.
    /// No valida ni guarda nada por sí mismo: todo lo hace AtencionNegocio, que a
    /// su vez guarda la cita, la atención y la auditoría en UNA transacción.
    /// </summary>
    public partial class FrmRegistrarAtencion : Form
    {
        private static readonly Color ColorNormal = Tema.TextoSecundario;   // #6B7280 (mismo valor que antes)
        private static readonly Color ColorLimite = Tema.TextoError;        // #B91C1C (mismo valor que antes)

        private readonly AtencionNegocio negocio = new AtencionNegocio();
        private readonly int idCita;

        /// <summary>Id de la atención registrada (si se confirmó con éxito).</summary>
        public int IdAtencionRegistrada { get; private set; }

        public FrmRegistrarAtencion(int idCita)
        {
            InitializeComponent();
            this.idCita = idCita;
            txtObservaciones.MaxLength = Atencion.MaxObservaciones;
            txtDiagnostico.MaxLength = Atencion.MaxDiagnostico;
            ActualizarContador();
            ActualizarContadorDiagnostico();
            Tema.Aplicar(this);   // Solo colores
        }

        private void FrmRegistrarAtencion_Load(object sender, EventArgs e)
        {
            CitaResumen c;
            try
            {
                c = negocio.ObtenerCita(idCita);
            }
            catch (Exception ex)
            {
                Interfaz.MostrarErrorBaseDatos(ex);
                c = null;
            }

            if (c == null)
            {
                MessageBox.Show("No se encontró la cita o no pertenece a su posta.", Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.Cancel;   // Cierra el diálogo
                return;
            }

            lblPacienteValor.Text = c.NombrePaciente;
            lblDniValor.Text = Mascara.Dni(c.Dni);   // DNI enmascarado (no se necesita completo aquí)
            lblConsultorioValor.Text = c.Consultorio;
            lblProfesionalValor.Text = c.Profesional;
            lblLlamadoValor.Text = c.HoraLlamado.HasValue
                ? c.HoraLlamado.Value.ToString("HH:mm") + "  (turno " + c.Turno + ")"
                : "No fue llamado";
            txtObservaciones.Focus();
        }

        // Contador "n / 300" mientras se escribe.
        private void txtObservaciones_TextChanged(object sender, EventArgs e)
        {
            ActualizarContador();
        }

        private void ActualizarContador()
        {
            int n = txtObservaciones.TextLength;
            lblContador.Text = n + " / " + Atencion.MaxObservaciones;
            lblContador.ForeColor = n >= Atencion.MaxObservaciones ? ColorLimite : ColorNormal;
        }

        // Contador "n / 500" del diagnóstico.
        private void txtDiagnostico_TextChanged(object sender, EventArgs e)
        {
            ActualizarContadorDiagnostico();
        }

        private void ActualizarContadorDiagnostico()
        {
            int n = txtDiagnostico.TextLength;
            lblContadorDiagnostico.Text = n + " / " + Atencion.MaxDiagnostico;
            lblContadorDiagnostico.ForeColor = n >= Atencion.MaxDiagnostico ? ColorLimite : ColorNormal;
        }

        /// <summary>
        /// Confirma la atención. La transacción corre en otro hilo (Task.Run) para
        /// no congelar el diálogo; "await" vuelve al hilo de la interfaz.
        /// </summary>
        private async void btnConfirmar_Click(object sender, EventArgs e)
        {
            string observaciones = txtObservaciones.Text;
            string diagnostico = txtDiagnostico.Text;   // Negocio lo cifra antes de guardarlo
            btnConfirmar.Enabled = false;
            btnCancelar.Enabled = false;
            Cursor = Cursors.WaitCursor;

            ResultadoOperacion r;
            try
            {
                r = await Task.Run(() => negocio.RegistrarAtencion(idCita, observaciones, diagnostico));
            }
            catch (Exception ex)
            {
                r = ResultadoOperacion.Error(RegistroTecnico.ErrorParaUsuario("Registrar atención", ex));
            }

            if (IsDisposed)
            {
                return;
            }
            Cursor = Cursors.Default;
            btnConfirmar.Enabled = true;
            btnCancelar.Enabled = true;

            if (!r.Exito)
            {
                MessageBox.Show("No se registró la atención:\n\n" + r.MensajesComoTexto(),
                    "Registrar atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            IdAtencionRegistrada = r.IdGenerado.Value;
            MessageBox.Show("Atención N° " + IdAtencionRegistrada + " registrada para " + lblPacienteValor.Text + ".",
                "Registrar atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
        }
    }
}
