using System;

namespace SigepostaAdmision.Entidades
{
    /// <summary>
    /// Clase de SOLO LECTURA para mostrar en una grilla un paciente junto con su
    /// historia clínica (si la tiene) y la cantidad de atenciones.
    ///
    /// ¿Por qué no usar HistoriaClinica? Porque esta fila también representa a
    /// pacientes SIN historia (los datos de la historia quedan en null), y porque
    /// solo trae los datos que necesita la pantalla, sin validaciones: es un
    /// "objeto de consulta", no una entidad del dominio.
    /// </summary>
    public class HistoriaResumen
    {
        public int IdPaciente { get; set; }
        public string Dni { get; set; }

        /// <summary>"Paterno Materno, Nombres".</summary>
        public string NombreCompleto { get; set; }

        /// <summary>null si el paciente aún no tiene historia.</summary>
        public int? IdHistoria { get; set; }
        public string NumeroHistoria { get; set; }
        public DateTime? FechaApertura { get; set; }
        public EstadoHistoria? Estado { get; set; }

        /// <summary>Número de citas con estado 'Atendida' del paciente.</summary>
        public int TotalAtenciones { get; set; }

        public bool TieneHistoria
        {
            get { return IdHistoria.HasValue; }
        }
    }
}
