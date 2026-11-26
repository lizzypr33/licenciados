using System;
using System.Windows.Forms;
using SigepostaAdmision.Negocio;

namespace SigepostaAdmision.Presentacion
{
    /// <summary>
    /// Cambio de la contraseña propia (cualquier rol). Las reglas se muestran en
    /// una etiqueta (el texto viene de UsuarioNegocio) y se aplican en
    /// UsuarioNegocio.CambiarMiClave; el formulario solo recoge los datos.
    /// </summary>
    public partial class FrmCambiarClave : Form
    {
        private readonly UsuarioNegocio negocio = new UsuarioNegocio();

        public FrmCambiarClave()
        {
            InitializeComponent();
            lblReglas.Text = UsuarioNegocio.TextoReglasClave;
            Tema.Aplicar(this);   // Solo colores
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            ResultadoOperacion r = negocio.CambiarMiClave(txtActual.Text, txtNueva.Text, txtConfirmacion.Text);
            if (!r.Exito)
            {
                MessageBox.Show("No se cambió la contraseña:\n\n" + r.MensajesComoTexto(),
                    Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show("Su contraseña se cambió correctamente.", Text,
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
        }

        // Al cerrar, no se dejan contraseñas en los controles.
        private void FrmCambiarClave_FormClosed(object sender, FormClosedEventArgs e)
        {
            txtActual.Clear();
            txtNueva.Clear();
            txtConfirmacion.Clear();
        }
    }
}
