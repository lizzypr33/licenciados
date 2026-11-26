using System;

namespace SigepostaAdmision.Negocio
{
    /// <summary>
    /// Datos del paciente TAL COMO LOS ESCRIBIÓ el usuario, todavía sin validar.
    ///
    /// ¿Por qué no se pasa directamente un Paciente desde el formulario? Porque
    /// la entidad Paciente se protege a sí misma y lanza una excepción si recibe,
    /// por ejemplo, un DNI de 7 dígitos. Con esta clase sencilla (sin
    /// validaciones) el formulario entrega los datos "en bruto", PacienteNegocio
    /// los revisa todos, junta todos los errores en un ResultadoOperacion y solo
    /// si todo es correcto construye el Paciente.
    /// </summary>
    public class DatosPaciente
    {
        public string Dni { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Nombres { get; set; }
        public DateTime FechaNacimiento { get; set; }

        /// <summary>'M', 'F', o '\0' si no se eligió ninguno.</summary>
        public char Sexo { get; set; }

        public string Direccion { get; set; }
        public string Telefono { get; set; }
    }
}
