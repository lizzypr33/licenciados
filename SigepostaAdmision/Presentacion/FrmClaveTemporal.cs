using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SigepostaAdmision.Presentacion
{
    /// <summary>
    /// Muestra UNA vez la contraseña temporal generada al crear un usuario o al
    /// restablecer su contraseña. No se guarda en ningún lado (en la base solo
    /// queda su hash); al cerrar este diálogo ya no se puede volver a ver.
    /// </summary>
    public partial class FrmClaveTemporal : Form
    {
        public FrmClaveTemporal(string nombreUsuario, string clave)
        {
            InitializeComponent();
            lblUsuarioValor.Text = nombreUsuario;
            txtClave.Text = clave;
            Tema.Aplicar(this);   // Solo colores
        }

        private void btnCopiar_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(txtClave.Text);
                lblCopiado.Text = "Copiada al portapapeles.";
            }
            catch (ExternalException)
            {
                // Otro programa está usando el portapapeles en ese instante.
                lblCopiado.Text = "No se pudo copiar; selecciónela y cópiela manualmente.";
            }
        }
    }
}
