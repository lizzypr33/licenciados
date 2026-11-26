using SigepostaAdmision.Datos;
using SigepostaAdmision.Entidades;
using SigepostaAdmision.Seguridad;

namespace SigepostaAdmision.Negocio
{
    /// <summary>
    /// Reglas de negocio del inicio de sesión (capa de Negocio).
    ///
    /// Se ubica entre la Presentación y los Datos: el formulario no sabe nada de
    /// SQL ni de hashes; solo llama a IniciarSesion() y recibe un Usuario o null.
    /// </summary>
    public class AutenticacionNegocio
    {
        /// <summary>
        /// Mensaje GENÉRICO para cualquier fallo de acceso. No se indica si falló
        /// el usuario o la contraseña, para no ayudar a un atacante a descubrir
        /// qué nombres de usuario existen (enumeración de usuarios).
        /// </summary>
        public const string MensajeCredencialesInvalidas = "Usuario o contraseña incorrectos.";

        private readonly UsuarioDAO usuarioDao = new UsuarioDAO();

        /// <summary>
        /// Valida las credenciales. Si son correctas, guarda el usuario en Sesion
        /// y lo devuelve; si no, devuelve null. Las excepciones de conexión a la
        /// base de datos se dejan pasar para que la Presentación las informe.
        /// </summary>
        public Usuario IniciarSesion(string usuario, string clave)
        {
            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrEmpty(clave))
            {
                return null;
            }

            string hash;
            string salt;
            Usuario u = usuarioDao.ObtenerCredenciales(usuario.Trim(), out hash, out salt);

            if (u == null)
            {
                // Aunque el usuario no exista, se calcula un hash igualmente para
                // que la respuesta tarde lo mismo que con un usuario real y no se
                // pueda deducir por el tiempo si el usuario existe.
                Hash.CalcularHash(clave, "00000000000000000000000000000000");
                AuditarLoginFallido(usuario);
                return null;
            }

            if (!Hash.Verificar(clave, salt, hash))
            {
                AuditarLoginFallido(usuario);
                return null;
            }

            Sesion.UsuarioActual = u;
            new AuditoriaNegocio().Registrar("LOGIN_OK", "Usuario", u.IdUsuario, "Inicio de sesión");
            return u;
        }

        /// <summary>
        /// Cierra la sesión y registra LOGOUT en la auditoría (si había sesión).
        /// motivo: por ejemplo "Cierre de sesión" o "Cierre de la aplicación".
        /// </summary>
        public void CerrarSesion(string motivo)
        {
            if (Sesion.HaySesion)
            {
                // Se audita ANTES de borrar la sesión, para que quede el usuario.
                new AuditoriaNegocio().Registrar("LOGOUT", "Usuario", Sesion.UsuarioActual.IdUsuario, motivo);
            }
            Sesion.CerrarSesion();
        }

        /// <summary>
        /// Intento fallido: en el detalle va el nombre de usuario ESCRITO (máx. 50
        /// caracteres) para detectar ataques; la contraseña NUNCA se registra.
        /// No hay sesión todavía, así que IdUsuario queda vacío.
        /// </summary>
        private static void AuditarLoginFallido(string usuarioEscrito)
        {
            string nombre = (usuarioEscrito ?? string.Empty).Trim();
            if (nombre.Length > 50)
            {
                nombre = nombre.Substring(0, 50);
            }
            new AuditoriaNegocio().Registrar("LOGIN_FALLIDO", "Usuario", null, "Usuario escrito: " + nombre);
        }

        /// <summary>
        /// Si la tabla Usuario está vacía, crea dos usuarios de prueba.
        ///
        /// SOLO PARA USO ACADÉMICO: las claves iniciales están escritas en el
        /// código fuente, algo que nunca debe hacerse en un sistema real. En
        /// producción el primer administrador se crearía con una clave temporal
        /// que se obliga a cambiar en el primer inicio de sesión.
        ///   - admin    / Admin123*     (rol Administrador)
        ///   - admision / Admision123*  (rol Admision)
        /// </summary>
        public void CrearUsuariosIniciales()
        {
            if (usuarioDao.ContarUsuarios() > 0)
            {
                return;
            }

            CrearUsuario("admin", RolUsuario.Administrador, "Admin123*");
            CrearUsuario("admision", RolUsuario.Admision, "Admision123*");
        }

        // Genera la sal, calcula el hash y registra el usuario. La clave en texto
        // plano solo existe en memoria durante esta llamada; nunca se guarda.
        private void CrearUsuario(string nombre, RolUsuario rol, string clave)
        {
            string salt = Hash.GenerarSalt();
            string hash = Hash.CalcularHash(clave, salt);

            Usuario u = new Usuario(0, nombre, rol, null, true);
            usuarioDao.Crear(u, hash, salt);
        }
    }
}
