using System;

namespace SigepostaAdmision.Entidades
{
    /// <summary>
    /// Clase base ABSTRACTA que representa a cualquier persona del sistema.
    ///
    /// ¿Por qué abstracta? Porque en la posta nunca existe "una persona" a secas:
    /// siempre es un Paciente o un miembro del PersonalSalud. Al ser abstracta
    /// no se puede instanciar con "new Persona()", solo sirve como molde común.
    ///
    /// ¿Por qué herencia? Pacientes, médicos y enfermeros comparten DNI, nombres,
    /// apellidos, fecha de nacimiento y teléfono. Se definen una sola vez aquí y
    /// las clases hijas los heredan (reutilización de código).
    ///
    /// Encapsulamiento: los campos son privados y solo se accede a ellos mediante
    /// propiedades públicas que validan los datos en el "set".
    /// </summary>
    public abstract class Persona
    {
        // ----- Campos privados (encapsulamiento) -----
        private string dni;
        private string nombres;
        private string apellidos;
        private DateTime fechaNacimiento;
        private string telefono;

        // ----- Constructores -----

        /// <summary>
        /// Constructor por defecto. Es "protected" porque solo lo pueden usar
        /// las clases hijas (una clase abstracta no se instancia directamente).
        /// </summary>
        protected Persona()
        {
            telefono = string.Empty;
        }

        /// <summary>
        /// Constructor parametrizado: asigna los valores pasando por las
        /// propiedades, para que se apliquen las mismas validaciones.
        /// </summary>
        protected Persona(string dni, string nombres, string apellidos,
                          DateTime fechaNacimiento, string telefono)
        {
            Dni = dni;
            Nombres = nombres;
            Apellidos = apellidos;
            FechaNacimiento = fechaNacimiento;
            Telefono = telefono;
        }

        // ----- Propiedades públicas con validación -----

        /// <summary>DNI peruano: exactamente 8 dígitos numéricos.</summary>
        public string Dni
        {
            get { return dni; }
            set
            {
                string valor = value == null ? null : value.Trim();
                if (valor == null || valor.Length != 8 || !SoloDigitos(valor))
                {
                    throw new ArgumentException("El DNI debe tener exactamente 8 dígitos numéricos.");
                }
                dni = valor;
            }
        }

        public string Nombres
        {
            get { return nombres; }
            set { nombres = Validacion.TextoObligatorio(value, "Nombres"); }
        }

        /// <summary>
        /// Apellidos completos. Es "virtual" para que una clase hija pueda
        /// guardarlos de otra forma: Paciente los separa en paterno y materno
        /// (como en la tabla) y sobrescribe esta propiedad para devolverlos unidos.
        /// </summary>
        public virtual string Apellidos
        {
            get { return apellidos; }
            set { apellidos = Validacion.TextoObligatorio(value, "Apellidos"); }
        }

        /// <summary>No se permite una fecha de nacimiento futura.</summary>
        public DateTime FechaNacimiento
        {
            get { return fechaNacimiento; }
            set
            {
                if (value.Date > DateTime.Today)
                {
                    throw new ArgumentException("La fecha de nacimiento no puede ser posterior a la fecha actual.");
                }
                fechaNacimiento = value.Date;
            }
        }

        /// <summary>Teléfono opcional: si se ingresa, solo puede contener dígitos.</summary>
        public string Telefono
        {
            get { return telefono; }
            set
            {
                string valor = value == null ? string.Empty : value.Trim();
                if (valor.Length > 0 && !SoloDigitos(valor))
                {
                    throw new ArgumentException("El teléfono solo puede contener dígitos numéricos.");
                }
                telefono = valor;
            }
        }

        // ----- Métodos -----

        /// <summary>
        /// Calcula la edad en años cumplidos a la fecha de hoy.
        /// </summary>
        public int ObtenerEdad()
        {
            DateTime hoy = DateTime.Today;
            int edad = hoy.Year - fechaNacimiento.Year;
            // Si aún no cumple años este año, se resta uno.
            if (fechaNacimiento.Date > hoy.AddYears(-edad))
            {
                edad--;
            }
            return edad;
        }

        /// <summary>
        /// Muestra los datos comunes de la persona. Es "virtual" para que cada
        /// clase hija pueda sobrescribirlo (override) y agregar sus propios datos:
        /// esto es POLIMORFISMO.
        /// </summary>
        public virtual string MostrarDatos()
        {
            return "DNI: " + dni +
                   " | Nombre: " + Nombres + " " + Apellidos +   // propiedad (no campo) para respetar el override
                   " | Edad: " + ObtenerEdad() + " años" +
                   " | Teléfono: " + (telefono.Length > 0 ? telefono : "(no registrado)");
        }

        // Verifica que todos los caracteres sean dígitos del 0 al 9.
        private static bool SoloDigitos(string texto)
        {
            foreach (char c in texto)
            {
                if (c < '0' || c > '9')
                {
                    return false;
                }
            }
            return true;
        }
    }
}
