using System;
using System.Drawing;
using System.Windows.Forms;
using SigepostaAdmision.Entidades;
using SigepostaAdmision.Negocio;

namespace SigepostaAdmision.Presentacion
{
    /// <summary>
    /// Formulario principal: barra superior, menú lateral, panel central donde
    /// se incrusta el módulo elegido y barra de estado.
    ///
    /// Los permisos NO se deciden aquí: se consultan a Negocio.Permisos.
    /// Este formulario solo oculta los botones sin acceso y vuelve a verificar
    /// el permiso antes de abrir un módulo (por si se invoca por código).
    ///
    /// Al cerrar:
    ///   - Con "Cerrar sesión": SesionCerrada = true y Program vuelve al login.
    ///   - Con la X de la ventana: SesionCerrada = false y la aplicación termina.
    /// </summary>
    public partial class FrmPrincipal : Form
    {
        // Colores del menú: normal y módulo activo (vienen de la paleta de Tema).
        private static readonly Color FondoMenu = Tema.MenuFondo;
        private static readonly Color TextoMenu = Tema.MenuTexto;
        private static readonly Color FondoActivo = Tema.MenuActivoFondo;   // azul claro
        private static readonly Color TextoActivo = Tema.MenuActivoTexto;   // azul oscuro

        private Form moduloActual;      // Formulario hijo incrustado en este momento
        private Button botonActivo;     // Botón resaltado en el menú
        private Font fuenteActiva;      // Negrita para el botón activo (se crea una sola vez)

        /// <summary>true si el usuario salió con "Cerrar sesión" (y no con la X).</summary>
        public bool SesionCerrada { get; private set; }

        public FrmPrincipal()
        {
            // Este formulario solo tiene sentido con un usuario autenticado.
            if (!Sesion.HaySesion)
            {
                throw new InvalidOperationException("No hay una sesión iniciada.");
            }

            InitializeComponent();
            fuenteActiva = new Font(Font, FontStyle.Bold);

            // Cada botón guarda en Tag el módulo que abre: así un solo
            // manejador de eventos sirve para todos.
            btnPacientes.Tag = Modulo.Pacientes;
            btnHistorias.Tag = Modulo.Historias;
            btnCitas.Tag = Modulo.Citas;
            btnCola.Tag = Modulo.Cola;
            btnReportes.Tag = Modulo.Reportes;
            btnRespaldo.Tag = Modulo.Respaldo;
            btnUsuarios.Tag = Modulo.Usuarios;

            MostrarDatosDeSesion();
            AplicarPermisos();
            Tema.Aplicar(this);   // Solo colores
        }

        private void FrmPrincipal_Load(object sender, EventArgs e)
        {
            // Módulo inicial: Pacientes (ambos roles tienen acceso).
            AbrirModulo(Modulo.Pacientes);
        }

        // Muestra el usuario y su rol en la barra superior y en la barra de estado.
        private void MostrarDatosDeSesion()
        {
            Usuario u = Sesion.UsuarioActual;
            lblUsuario.Text = u.NombreUsuario + "  |  " + u.Rol;
            lblEstado.Text = "Conectado a SigepostaAdmision · Usuario: " + u.NombreUsuario + " · Rol: " + u.Rol;
            Text = "SIGEPOSTA · Admisión - " + u.NombreUsuario;
        }

        // Oculta los botones de los módulos a los que el rol no tiene acceso.
        private void AplicarPermisos()
        {
            RolUsuario rol = Sesion.UsuarioActual.Rol;
            foreach (Button b in BotonesDeModulo())
            {
                b.Visible = Permisos.TieneAcceso(rol, (Modulo)b.Tag);
            }
        }

        private Button[] BotonesDeModulo()
        {
            return new[] { btnPacientes, btnHistorias, btnCitas, btnCola, btnReportes, btnRespaldo, btnUsuarios };
        }

        // Manejador común para todos los botones de módulo.
        private void btnModulo_Click(object sender, EventArgs e)
        {
            AbrirModulo((Modulo)((Button)sender).Tag);
        }

        /// <summary>
        /// Abre un módulo dentro del panel central. Antes verifica el permiso
        /// en la capa de Negocio; si no hay acceso, no abre nada.
        /// </summary>
        public void AbrirModulo(Modulo modulo)
        {
            if (!Sesion.HaySesion || !Permisos.TieneAcceso(Sesion.UsuarioActual.Rol, modulo))
            {
                MessageBox.Show("No tiene permiso para acceder a este módulo.",
                    "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Se cierra y libera el módulo anterior antes de abrir el nuevo.
            CerrarModuloActual();

            Form hijo = CrearFormulario(modulo);
            hijo.TopLevel = false;                       // Permite incrustarlo en un panel
            hijo.FormBorderStyle = FormBorderStyle.None; // Sin bordes ni barra de título
            hijo.Dock = DockStyle.Fill;                  // Ocupa todo el panel central
            pnlContenido.Controls.Add(hijo);
            hijo.Show();
            moduloActual = hijo;

            ResaltarBoton(BuscarBoton(modulo));
        }

        // Fábrica de formularios: devuelve el formulario de cada módulo.
        private static Form CrearFormulario(Modulo modulo)
        {
            switch (modulo)
            {
                case Modulo.Pacientes: return new FrmPacientes();
                case Modulo.Historias: return new FrmHistorias();
                case Modulo.Citas: return new FrmCitas();
                case Modulo.Cola: return new FrmCola();
                case Modulo.Reportes: return new FrmReportes();
                case Modulo.Respaldo: return new FrmRespaldo();
                case Modulo.Usuarios: return new FrmUsuarios();
                default: throw new ArgumentOutOfRangeException("modulo");
            }
        }

        private void CerrarModuloActual()
        {
            if (moduloActual != null)
            {
                moduloActual.Close();
                moduloActual.Dispose();
                moduloActual = null;
            }
        }

        private Button BuscarBoton(Modulo modulo)
        {
            foreach (Button b in BotonesDeModulo())
            {
                if ((Modulo)b.Tag == modulo)
                {
                    return b;
                }
            }
            return null;
        }

        // Quita el resaltado del botón anterior y resalta el nuevo.
        private void ResaltarBoton(Button boton)
        {
            if (botonActivo != null)
            {
                botonActivo.BackColor = FondoMenu;
                botonActivo.ForeColor = TextoMenu;
                botonActivo.FlatAppearance.MouseOverBackColor = Tema.MenuHover;
                botonActivo.Font = Font;            // Fuente normal del formulario
            }

            botonActivo = boton;

            if (botonActivo != null)
            {
                botonActivo.BackColor = FondoActivo;
                botonActivo.ForeColor = TextoActivo;
                // Al pasar el mouse por el módulo activo, conserva su color de activo.
                botonActivo.FlatAppearance.MouseOverBackColor = FondoActivo;
                botonActivo.Font = fuenteActiva;    // Negrita
            }
        }

        // "Cambiar contraseña": disponible para todos los roles (la regla está en UsuarioNegocio).
        private void lnkCambiarClave_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (FrmCambiarClave dialogo = new FrmCambiarClave())
            {
                dialogo.ShowDialog(this);
            }
        }

        // "Cerrar sesión": borra la sesión y cierra; Program mostrará el login otra vez.
        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            SesionCerrada = true;
            CerrarModuloActual();   // El módulo se cierra antes, con la sesión aún activa
            new AutenticacionNegocio().CerrarSesion("Cierre de sesión");   // Registra LOGOUT
            Close();
        }

        // Al cerrar (por cualquier vía) se libera el módulo incrustado.
        private void FrmPrincipal_FormClosed(object sender, FormClosedEventArgs e)
        {
            CerrarModuloActual();
            fuenteActiva.Dispose();
        }
    }
}
