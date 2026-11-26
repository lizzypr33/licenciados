using System;
using System.Windows.Forms;
using SigepostaAdmision.Entidades;
using SigepostaAdmision.Negocio;

namespace SigepostaAdmision.Presentacion
{
    /// <summary>
    /// Diálogo para crear un usuario o editar su rol y posta.
    /// Dos constructores (sobrecarga): FrmUsuarioEdicion() = nuevo;
    /// FrmUsuarioEdicion(UsuarioResumen) = editar (el nombre no se cambia).
    ///
    /// Las reglas (formato del nombre, posta obligatoria para Admision, último
    /// Administrador, etc.) las aplica UsuarioNegocio al guardar. Habilitar o no
    /// el combo de posta se consulta a UsuarioNegocio.RequierePosta.
    /// </summary>
    public partial class FrmUsuarioEdicion : Form
    {
        private readonly UsuarioNegocio negocio = new UsuarioNegocio();
        private readonly UsuarioResumen original;   // null = usuario nuevo

        /// <summary>Resultado de la operación (incluye la contraseña temporal al crear).</summary>
        public ResultadoUsuario Resultado { get; private set; }

        /// <summary>Nombre del usuario creado o editado.</summary>
        public string NombreUsuario { get; private set; }

        public FrmUsuarioEdicion()
        {
            InitializeComponent();
            Text = "Nuevo usuario";
            lblTitulo.Text = "Nuevo usuario";
            Tema.Aplicar(this);   // Solo colores (el constructor de edición pasa por aquí)
        }

        public FrmUsuarioEdicion(UsuarioResumen usuario) : this()
        {
            original = usuario;
            Text = "Editar usuario";
            lblTitulo.Text = "Editar usuario";
        }

        private void FrmUsuarioEdicion_Load(object sender, EventArgs e)
        {
            cboRol.Items.Add(RolUsuario.Admision);
            cboRol.Items.Add(RolUsuario.Administrador);

            try
            {
                cboPosta.Items.AddRange(negocio.Postas().ToArray());
            }
            catch (Exception ex)
            {
                Interfaz.MostrarErrorBaseDatos(ex);
            }

            if (original != null)
            {
                // Editar: el nombre de usuario no se cambia.
                txtUsuario.Text = original.NombreUsuario;
                txtUsuario.Enabled = false;
                cboRol.SelectedItem = original.Rol;
                SeleccionarPosta(original.IdPosta);
            }
            else
            {
                cboRol.SelectedItem = RolUsuario.Admision;
                if (cboPosta.Items.Count == 1)
                {
                    cboPosta.SelectedIndex = 0;
                }
            }
            AjustarPosta();
        }

        // Nombre visible de cada rol.
        private void cboRol_Format(object sender, ListControlConvertEventArgs e)
        {
            if (e.ListItem is RolUsuario)
            {
                e.Value = (RolUsuario)e.ListItem == RolUsuario.Admision ? "Admisión" : "Administrador";
            }
        }

        private void cboRol_SelectedIndexChanged(object sender, EventArgs e)
        {
            AjustarPosta();
        }

        // La posta solo se usa si el rol la requiere (lo dice Negocio); si no, se vacía y bloquea.
        private void AjustarPosta()
        {
            bool requiere = cboRol.SelectedItem is RolUsuario && UsuarioNegocio.RequierePosta((RolUsuario)cboRol.SelectedItem);
            cboPosta.Enabled = requiere;
            if (!requiere)
            {
                cboPosta.SelectedIndex = -1;
            }
        }

        private void SeleccionarPosta(int? idPosta)
        {
            for (int i = 0; i < cboPosta.Items.Count; i++)
            {
                PostaItem p = cboPosta.Items[i] as PostaItem;
                if (p != null && idPosta.HasValue && p.IdPosta == idPosta.Value)
                {
                    cboPosta.SelectedIndex = i;
                    return;
                }
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            RolUsuario rol = cboRol.SelectedItem is RolUsuario ? (RolUsuario)cboRol.SelectedItem : RolUsuario.Admision;
            PostaItem posta = cboPosta.SelectedItem as PostaItem;
            int? idPosta = posta != null ? posta.IdPosta : (int?)null;

            ResultadoUsuario r = original == null
                ? negocio.Crear(txtUsuario.Text, rol, idPosta)
                : negocio.ActualizarRolYPosta(original.IdUsuario, rol, idPosta);

            if (!r.Exito)
            {
                // Todos los errores juntos, en un solo mensaje.
                MessageBox.Show("No se pudo guardar el usuario:\n\n" + r.MensajesComoTexto(),
                    Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Resultado = r;
            NombreUsuario = original != null ? original.NombreUsuario : txtUsuario.Text.Trim();
            DialogResult = DialogResult.OK;
        }
    }
}
