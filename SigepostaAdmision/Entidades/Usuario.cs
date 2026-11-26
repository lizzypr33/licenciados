using System;

namespace SigepostaAdmision.Entidades
{
    /// <summary>
    /// Usuario que inicia sesión en el sistema.
    ///
    /// No hereda de Persona porque representa una CUENTA de acceso (credencial y
    /// rol), no a una persona con DNI y fecha de nacimiento.
    ///
    /// Por seguridad, esta entidad NO guarda el hash ni la sal: así el objeto que
    /// queda en memoria durante la sesión (Sesion.UsuarioActual) nunca contiene
    /// información de la contraseña.
    /// </summary>
    public class Usuario
    {
        // Longitud máxima de la columna NombreUsuario VARCHAR(30) en la base de datos.
        private const int LongitudMaximaNombre = 30;

        // ----- Campos privados -----
        private int idUsuario;
        private string nombreUsuario;
        private RolUsuario rol;
        private int? idPosta;
        private bool estado;

        // ----- Constructores -----

        /// <summary>Constructor por defecto: usuario activo.</summary>
        public Usuario()
        {
            estado = true;
        }

        public Usuario(int idUsuario, string nombreUsuario, RolUsuario rol, int? idPosta, bool estado)
        {
            IdUsuario = idUsuario;
            NombreUsuario = nombreUsuario;
            Rol = rol;
            IdPosta = idPosta;
            Estado = estado;
        }

        // ----- Propiedades -----

        public int IdUsuario
        {
            get { return idUsuario; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("El Id del usuario no puede ser negativo.");
                }
                idUsuario = value;
            }
        }

        public string NombreUsuario
        {
            get { return nombreUsuario; }
            set
            {
                string valor = Validacion.TextoObligatorio(value, "Nombre de usuario");
                if (valor.Length > LongitudMaximaNombre)
                {
                    throw new ArgumentException("El nombre de usuario no puede superar los " + LongitudMaximaNombre + " caracteres.");
                }
                nombreUsuario = valor;
            }
        }

        public RolUsuario Rol
        {
            get { return rol; }
            set
            {
                if (!Enum.IsDefined(typeof(RolUsuario), value))
                {
                    throw new ArgumentException("El rol del usuario no es válido.");
                }
                rol = value;
            }
        }

        /// <summary>
        /// Posta a la que pertenece el usuario. Es int? (nullable) porque un
        /// Administrador puede no estar asignado a ninguna posta.
        /// </summary>
        public int? IdPosta
        {
            get { return idPosta; }
            set
            {
                if (value.HasValue && value.Value <= 0)
                {
                    throw new ArgumentException("El Id de la posta debe ser mayor que cero.");
                }
                idPosta = value;
            }
        }

        /// <summary>true = activo, false = inactivo (no puede iniciar sesión).</summary>
        public bool Estado
        {
            get { return estado; }
            set { estado = value; }
        }
    }
}
