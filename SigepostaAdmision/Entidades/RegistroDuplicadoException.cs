using System;

namespace SigepostaAdmision.Entidades
{
    /// <summary>
    /// Excepción propia del sistema que indica que se intentó guardar un registro
    /// que ya existe (por ejemplo, un paciente con un DNI ya registrado).
    ///
    /// ¿Por qué una excepción propia? La capa de Datos recibe de SQL Server una
    /// SqlException con un número técnico (2627 o 2601 = clave única duplicada).
    /// Las capas superiores no deberían depender de esos números ni de
    /// System.Data.SqlClient, así que Datos la "traduce" a esta excepción con un
    /// mensaje claro en español.
    ///
    /// ¿Por qué en Entidades? Porque Datos no puede referenciar a Negocio (las
    /// dependencias van Presentación → Negocio → Datos). Entidades la comparten
    /// todas las capas, así Datos la lanza y Negocio la captura.
    /// </summary>
    [Serializable]
    public class RegistroDuplicadoException : Exception
    {
        /// <summary>
        /// Dato que se repitió (por ejemplo "Dni" o "NumeroHistoria"), para que
        /// Negocio pueda dar un mensaje distinto según el caso.
        /// </summary>
        public string Campo { get; private set; }

        public RegistroDuplicadoException(string mensaje)
            : base(mensaje)
        {
        }

        public RegistroDuplicadoException(string mensaje, Exception interna)
            : base(mensaje, interna)
        {
        }

        public RegistroDuplicadoException(string mensaje, string campo, Exception interna)
            : base(mensaje, interna)
        {
            Campo = campo;
        }
    }
}
