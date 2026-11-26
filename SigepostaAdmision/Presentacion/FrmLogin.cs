using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using SigepostaAdmision.Entidades;
using SigepostaAdmision.Negocio;
using SigepostaAdmision.Seguridad;

namespace SigepostaAdmision.Presentacion
{
    /// <summary>
    /// Formulario de inicio de sesión (capa de Presentación).
    /// Solo recoge los datos y muestra mensajes; la verificación la hace
    /// AutenticacionNegocio. Permite como máximo 3 intentos fallidos.
    ///
    /// Resultado: DialogResult.OK si el acceso es correcto,
    ///            DialogResult.Cancel si el usuario sale o agota los intentos.
    /// </summary>
    public partial class FrmLogin : Form
    {
        private const int MaximoIntentos = 3;

        // Clave de la preferencia local (IsolatedStorage). Solo el NOMBRE de usuario:
        // la contraseña NUNCA se guarda en ningún lado.
        private const string ClaveUltimoUsuario = "UltimoUsuario";

        private readonly AutenticacionNegocio autenticacion = new AutenticacionNegocio();
        private int intentosFallidos;

        public FrmLogin()
        {
            InitializeComponent();
            Tema.Aplicar(this);   // Solo colores
        }

        // Si se eligió "Recordar usuario", se rellena el nombre y el foco va a la contraseña.
        private void FrmLogin_Load(object sender, EventArgs e)
        {
            string recordado = ConfiguracionLocal.Leer(ClaveUltimoUsuario, string.Empty);
            if (recordado.Length > 0)
            {
                txtUsuario.Text = recordado;
                chkRecordar.Checked = true;
                ActiveControl = txtClave;
            }
        }

        // Guarda (o borra) el nombre de usuario recordado. Un fallo aquí no impide ingresar.
        private void GuardarPreferenciaUsuario()
        {
            ConfiguracionLocal.Guardar(ClaveUltimoUsuario, chkRecordar.Checked ? txtUsuario.Text.Trim() : string.Empty);
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            lblMensaje.Text = string.Empty;

            // Campos vacíos: se avisa sin contar como intento fallido.
            if (string.IsNullOrWhiteSpace(txtUsuario.Text) || txtClave.Text.Length == 0)
            {
                lblMensaje.Text = "Ingrese su usuario y contraseña.";
                (txtUsuario.Text.Trim().Length == 0 ? (Control)txtUsuario : txtClave).Focus();
                return;
            }

            Usuario usuario;
            try
            {
                Cursor = Cursors.WaitCursor;
                usuario = autenticacion.IniciarSesion(txtUsuario.Text, txtClave.Text);
            }
            catch (SqlException)
            {
                // Un problema de conexión no es culpa del usuario: no cuenta como intento.
                lblMensaje.Text = "No se pudo conectar con la base de datos. Intente nuevamente.";
                return;
            }
            finally
            {
                Cursor = Cursors.Default;
            }

            if (usuario != null)
            {
                GuardarPreferenciaUsuario();      // Solo tras un ingreso correcto
                DialogResult = DialogResult.OK;   // Cierra el formulario con OK
                return;
            }

            intentosFallidos++;
            txtClave.Clear();   // La contraseña nunca se conserva tras un fallo

            if (intentosFallidos >= MaximoIntentos)
            {
                MessageBox.Show(
                    "Se superó el número máximo de intentos (" + MaximoIntentos + "). La aplicación se cerrará.",
                    Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.Cancel;
                return;
            }

            lblMensaje.Text = AutenticacionNegocio.MensajeCredencialesInvalidas +
                              " Intentos restantes: " + (MaximoIntentos - intentosFallidos) + ".";
            txtClave.Focus();
        }
    }
}
