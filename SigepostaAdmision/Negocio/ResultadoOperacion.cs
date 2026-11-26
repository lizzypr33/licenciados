using System.Collections.Generic;

namespace SigepostaAdmision.Negocio
{
    /// <summary>
    /// Resultado de una operación de negocio (registrar, actualizar, etc.).
    ///
    /// ¿Por qué no lanzar excepciones para los errores de validación? Porque un
    /// dato mal escrito por el usuario es algo ESPERADO, no excepcional. Devolver
    /// un objeto con la LISTA de mensajes permite mostrar todos los errores de una
    /// sola vez (y no uno por uno), y el formulario no necesita try/catch para
    /// las validaciones. Las excepciones se reservan para fallos reales
    /// (por ejemplo, que la base de datos no responda).
    /// </summary>
    public class ResultadoOperacion
    {
        private readonly List<string> mensajes = new List<string>();

        /// <summary>true si la operación se completó sin errores.</summary>
        public bool Exito
        {
            get { return mensajes.Count == 0; }
        }

        /// <summary>Mensajes de error en español (vacío si hubo éxito).</summary>
        public List<string> Mensajes
        {
            get { return mensajes; }
        }

        /// <summary>Id del registro creado o actualizado (null si no aplica o si falló).</summary>
        public int? IdGenerado { get; set; }

        /// <summary>
        /// Número de historia clínica asignado (por ejemplo "HC-000007") cuando la
        /// operación abrió una historia; null en los demás casos.
        /// </summary>
        public string NumeroHistoria { get; set; }

        /// <summary>Número de turno asignado al programar o reprogramar una cita; null en los demás casos.</summary>
        public int? NumeroTurno { get; set; }

        /// <summary>Agrega un mensaje de error (la operación deja de ser exitosa).</summary>
        public void AgregarError(string mensaje)
        {
            mensajes.Add(mensaje);
        }

        /// <summary>Une todos los mensajes, uno por línea con viñeta, para mostrarlos juntos.</summary>
        public string MensajesComoTexto()
        {
            return mensajes.Count == 0 ? string.Empty : "• " + string.Join("\n• ", mensajes);
        }

        /// <summary>Crea un resultado fallido con un único mensaje.</summary>
        public static ResultadoOperacion Error(string mensaje)
        {
            ResultadoOperacion r = new ResultadoOperacion();
            r.AgregarError(mensaje);
            return r;
        }
    }
}
