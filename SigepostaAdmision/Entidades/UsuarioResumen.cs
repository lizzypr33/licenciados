namespace SigepostaAdmision.Entidades
{
    /// <summary>
    /// Clase de SOLO LECTURA para listar usuarios en el módulo Usuarios.
    /// Nunca lleva el hash ni la sal de la contraseña.
    /// </summary>
    public class UsuarioResumen
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public RolUsuario Rol { get; set; }
        public int? IdPosta { get; set; }

        /// <summary>Nombre de la posta (vacío si no tiene).</summary>
        public string Posta { get; set; }

        /// <summary>true = activo; false = inactivo (no puede iniciar sesión).</summary>
        public bool Estado { get; set; }

        public string EstadoTexto
        {
            get { return Estado ? "Activo" : "Inactivo"; }
        }
    }
}
