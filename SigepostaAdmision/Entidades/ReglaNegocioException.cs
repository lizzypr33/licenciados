using System;

namespace SigepostaAdmision.Entidades
{
    /// <summary>
    /// Excepción propia que indica que una REGLA DEL NEGOCIO impidió completar
    /// una operación (por ejemplo: "la cita no fue llamada", "la historia está
    /// Inactiva"). La lanza la capa de Datos dentro de una transacción para
    /// provocar el Rollback, y Negocio la convierte en un mensaje para el usuario.
    ///
    /// Está en Entidades por el mismo motivo que RegistroDuplicadoException:
    /// Datos no puede depender de Negocio, y todas las capas ven Entidades.
    /// </summary>
    [Serializable]
    public class ReglaNegocioException : Exception
    {
        public ReglaNegocioException(string mensaje)
            : base(mensaje)
        {
        }

        public ReglaNegocioException(string mensaje, Exception interna)
            : base(mensaje, interna)
        {
        }
    }
}
