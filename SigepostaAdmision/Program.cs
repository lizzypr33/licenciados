using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;
using SigepostaAdmision.Entidades;
using SigepostaAdmision.Negocio;
using SigepostaAdmision.Presentacion;

namespace SigepostaAdmision
{
    /// <summary>
    /// Clase de arranque de la aplicación:
    ///   1) crea los usuarios iniciales si la tabla está vacía,
    ///   2) muestra el inicio de sesión,
    ///   3) solo si el acceso es correcto abre el formulario principal,
    ///   4) si el usuario cierra sesión, vuelve a mostrar el login.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 1) Usuarios iniciales y migración de seguridad (DNI en texto → cifrado).
            //    Si algo falla, se informa al usuario y la aplicación termina.
            try
            {
                new AutenticacionNegocio().CrearUsuariosIniciales();
                MigracionSeguridad.MigrarDniPacientes();
            }
            catch (SqlException ex)
            {
                // El detalle técnico va al registro técnico; al usuario, una guía y el código.
                string codigo = RegistroTecnico.Error("Inicio - conexión", ex);
                MessageBox.Show(
                    "No se pudo conectar con la base de datos SigepostaAdmision.\n\n" +
                    "Verifique que el servicio de SQL Server (SQLEXPRESS01) esté iniciado " +
                    "y que la cadena de conexión 'SigepostaDB' de App.config sea correcta.\n\n" +
                    "Código: " + codigo + ". Si persiste, avise al administrador.",
                    "SIGEPOSTA - Error de conexión",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (ReglaNegocioException ex)
            {
                // Mensajes pensados para el usuario: claves de cifrado que no corresponden
                // a los datos, o un DNI inválido al migrar. No se continúa para no dañar
                // la información.
                MessageBox.Show(
                    "No se pudo iniciar SIGEPOSTA de forma segura.\n\n" + ex.Message,
                    "SIGEPOSTA - Error de seguridad",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo iniciar SIGEPOSTA.\n\n" + RegistroTecnico.ErrorParaUsuario("Inicio", ex),
                    "SIGEPOSTA - Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 2) Bucle de sesión: login -> principal -> (cerrar sesión) -> login ...
            while (true)
            {
                using (FrmLogin login = new FrmLogin())
                {
                    if (login.ShowDialog() != DialogResult.OK)
                    {
                        break;      // Pulsó Salir o agotó los 3 intentos
                    }
                }

                bool volverAlLogin;
                using (FrmPrincipal principal = new FrmPrincipal())
                {
                    principal.ShowDialog();
                    // true solo si pulsó "Cerrar sesión"; con la X es false.
                    volverAlLogin = principal.SesionCerrada;
                }

                if (!volverAlLogin)
                {
                    break;          // Cerró la ventana con la X: la aplicación termina
                }
            }

            // Al terminar la aplicación no queda ningún usuario en memoria. Si se cerró
            // con la X (sesión aún abierta), se registra LOGOUT.
            new AutenticacionNegocio().CerrarSesion("Cierre de la aplicación");
        }
    }
}
