using SigepostaAdmision.Entidades;

namespace SigepostaAdmision.Negocio
{
    /// <summary>
    /// Guarda quién inició sesión mientras la aplicación está abierta.
    ///
    /// Es ESTÁTICA porque en una aplicación de escritorio hay una sola sesión a
    /// la vez y cualquier formulario debe poder consultarla (por ejemplo, para
    /// mostrar opciones según el rol) sin tener que pasar el usuario de mano en mano.
    ///
    /// El "set" de UsuarioActual es internal: solo la capa de Negocio
    /// (AutenticacionNegocio) establece el usuario tras verificar la contraseña.
    /// </summary>
    public static class Sesion
    {
        /// <summary>Usuario autenticado, o null si no hay sesión iniciada.</summary>
        public static Usuario UsuarioActual { get; internal set; }

        /// <summary>Indica si hay un usuario con sesión iniciada.</summary>
        public static bool HaySesion
        {
            get { return UsuarioActual != null; }
        }

        /// <summary>Cierra la sesión actual.</summary>
        public static void CerrarSesion()
        {
            UsuarioActual = null;
        }
    }
}
