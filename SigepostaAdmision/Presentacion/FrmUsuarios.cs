using System;
using System.Drawing;
using System.Windows.Forms;
using SigepostaAdmision.Entidades;
using SigepostaAdmision.Negocio;

namespace SigepostaAdmision.Presentacion
{
    /// <summary>
    /// Módulo Usuarios (solo Administrador): lista, alta, edición de rol y posta,
    /// activar/desactivar y restablecer contraseña.
    ///
    /// Sin SQL ni reglas: todo lo decide UsuarioNegocio (incluido el texto del
    /// botón Activar/Desactivar y qué operación corresponde). Los usuarios no se
    /// eliminan nunca, solo se desactivan.
    /// </summary>
    public partial class FrmUsuarios : Form
    {
        private static readonly Color ColorInactivo = Tema.TextoInactivo;   // Gris #9CA3AF (mismo valor que antes)

        private readonly UsuarioNegocio negocio = new UsuarioNegocio();

        public FrmUsuarios()
        {
            InitializeComponent();
            Tema.Aplicar(this);   // Solo colores
        }

        private void FrmUsuarios_Load(object sender, EventArgs e)
        {
            CargarLista(0);
        }

        // ===================== Lista =====================

        /// <summary>Llena la grilla y, si idSeleccionar > 0, deja seleccionado ese usuario.</summary>
        private void CargarLista(int idSeleccionar)
        {
            ResultadoUsuario r = negocio.Listar();
            grdUsuarios.Rows.Clear();
            if (!r.Exito)
            {
                lblResumen.Text = r.MensajesComoTexto().Replace("• ", "");
                ActualizarBotones();
                return;
            }

            foreach (UsuarioResumen u in r.Usuarios)
            {
                int i = grdUsuarios.Rows.Add(u.NombreUsuario, u.Rol, u.Posta, u.EstadoTexto);
                DataGridViewRow fila = grdUsuarios.Rows[i];
                fila.Tag = u;
                if (!u.Estado)
                {
                    // Los inactivos se muestran en gris.
                    fila.DefaultCellStyle.ForeColor = ColorInactivo;
                    fila.DefaultCellStyle.SelectionForeColor = ColorInactivo;
                }
                if (u.IdUsuario == idSeleccionar)
                {
                    grdUsuarios.CurrentCell = fila.Cells[0];
                }
            }
            lblResumen.Text = "Usuarios: " + r.Usuarios.Count;
            ActualizarBotones();
        }

        private UsuarioResumen Seleccionado()
        {
            DataGridViewRow fila = grdUsuarios.CurrentRow;
            return fila != null ? fila.Tag as UsuarioResumen : null;
        }

        private void grdUsuarios_SelectionChanged(object sender, EventArgs e)
        {
            ActualizarBotones();
        }

        // Botones según la fila elegida; el texto Activar/Desactivar lo da Negocio.
        private void ActualizarBotones()
        {
            UsuarioResumen u = Seleccionado();
            bool hay = u != null;
            btnEditar.Enabled = hay;
            btnEstado.Enabled = hay;
            btnRestablecer.Enabled = hay;
            btnEstado.Text = hay ? UsuarioNegocio.TextoBotonEstado(u.Estado) : "Activar / Desactivar";
        }

        // ===================== Acciones =====================

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            using (FrmUsuarioEdicion dialogo = new FrmUsuarioEdicion())
            {
                if (dialogo.ShowDialog(TopLevelControl) != DialogResult.OK)
                {
                    return;
                }
                CargarLista(dialogo.Resultado.IdGenerado ?? 0);
                MostrarClaveTemporal(dialogo.NombreUsuario, dialogo.Resultado.ClaveTemporal);
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            UsuarioResumen u = Seleccionado();
            if (u == null)
            {
                return;
            }
            using (FrmUsuarioEdicion dialogo = new FrmUsuarioEdicion(u))
            {
                if (dialogo.ShowDialog(TopLevelControl) == DialogResult.OK)
                {
                    CargarLista(u.IdUsuario);
                }
            }
        }

        private void btnEstado_Click(object sender, EventArgs e)
        {
            UsuarioResumen u = Seleccionado();
            if (u == null)
            {
                return;
            }

            string accion = UsuarioNegocio.TextoBotonEstado(u.Estado);
            if (MessageBox.Show("¿" + accion + " al usuario \"" + u.NombreUsuario + "\"?" +
                    (u.Estado ? "\n\nNo podrá iniciar sesión hasta que se vuelva a activar." : ""),
                    accion + " usuario", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                return;
            }

            // Negocio decide si corresponde activar o desactivar según el estado guardado.
            ResultadoUsuario r = negocio.AlternarEstado(u.IdUsuario);
            if (!r.Exito)
            {
                MostrarErrores(r);
            }
            CargarLista(u.IdUsuario);
        }

        private void btnRestablecer_Click(object sender, EventArgs e)
        {
            UsuarioResumen u = Seleccionado();
            if (u == null)
            {
                return;
            }
            if (MessageBox.Show("¿Restablecer la contraseña de \"" + u.NombreUsuario + "\"?\n\n" +
                    "Se generará una contraseña temporal y la actual dejará de funcionar.",
                    "Restablecer contraseña", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                return;
            }

            ResultadoUsuario r = negocio.RestablecerClave(u.IdUsuario);
            if (!r.Exito)
            {
                MostrarErrores(r);
                return;
            }
            MostrarClaveTemporal(u.NombreUsuario, r.ClaveTemporal);
        }

        // La contraseña temporal se muestra UNA vez en su propio diálogo.
        private void MostrarClaveTemporal(string nombreUsuario, string clave)
        {
            if (string.IsNullOrEmpty(clave))
            {
                return;
            }
            using (FrmClaveTemporal dialogo = new FrmClaveTemporal(nombreUsuario, clave))
            {
                dialogo.ShowDialog(TopLevelControl);
            }
        }

        private static void MostrarErrores(ResultadoOperacion r)
        {
            MessageBox.Show(r.MensajesComoTexto(), "Usuarios", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
