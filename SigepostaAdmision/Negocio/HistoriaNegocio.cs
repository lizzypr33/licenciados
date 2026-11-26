using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using SigepostaAdmision.Datos;
using SigepostaAdmision.Entidades;

namespace SigepostaAdmision.Negocio
{
    /// <summary>
    /// Reglas de negocio de las historias clínicas.
    ///
    /// Reglas:
    ///   1. Un paciente tiene UNA sola historia clínica. Lo garantizan la
    ///      restricción UNIQUE de IdPaciente en la tabla y AbrirParaPaciente,
    ///      que no crea otra si ya existe.
    ///   2. Las historias NO se eliminan (es un documento clínico que debe
    ///      conservarse); solo se pasan a Inactiva o de nuevo a Activa.
    ///   3. Una historia Inactiva NO podrá recibir citas nuevas.
    ///      (Esta regla se aplicará en el módulo de Citas, que consultará el
    ///      estado de la historia antes de programar una cita.)
    ///   4. Normalmente la historia se abre sola al registrar al paciente
    ///      (PacienteNegocio.Registrar). AbrirParaPaciente sirve para pacientes
    ///      que quedaron sin historia (por ejemplo, datos antiguos o migrados).
    /// </summary>
    public class HistoriaNegocio
    {
        private readonly HistoriaDAO dao = new HistoriaDAO();
        private readonly AtencionDAO atencionDao = new AtencionDAO();

        /// <summary>
        /// Pacientes con su historia (o sin ella), filtrando por número de
        /// historia, DNI, apellidos o nombres. Filtro vacío = todos (máximo 100).
        /// </summary>
        public List<HistoriaResumen> Buscar(string filtro)
        {
            return dao.Listar((filtro ?? string.Empty).Trim());
        }

        /// <summary>
        /// Atenciones registradas del paciente (tabla Atencion), de la más reciente
        /// a la más antigua, con profesional, observaciones y usuario que registró.
        /// </summary>
        public List<AtencionResumen> ObtenerAtenciones(int idPaciente)
        {
            return idPaciente > 0 ? atencionDao.ObtenerAtencionesPorPaciente(idPaciente) : new List<AtencionResumen>();
        }

        /// <summary>Historia del paciente, o null si no tiene.</summary>
        public HistoriaClinica ObtenerPorPaciente(int idPaciente)
        {
            return idPaciente > 0 ? dao.ObtenerPorPaciente(idPaciente) : null;
        }

        /// <summary>
        /// Abre la historia de un paciente que aún no la tiene (regla 1: si ya
        /// tiene una, no se crea otra y se informa cuál es).
        /// En el resultado: IdGenerado = IdHistoria y NumeroHistoria = "HC-000007".
        /// </summary>
        public ResultadoOperacion AbrirParaPaciente(int idPaciente)
        {
            if (idPaciente <= 0)
            {
                return ResultadoOperacion.Error("Seleccione un paciente.");
            }

            try
            {
                HistoriaClinica existente = dao.ObtenerPorPaciente(idPaciente);
                if (existente != null)
                {
                    return ResultadoOperacion.Error(
                        "El paciente ya tiene la historia clínica " + existente.NumeroHistoria +
                        ". Cada paciente tiene una sola historia.");
                }

                HistoriaClinica h = dao.AbrirParaPaciente(idPaciente);
                if (h == null)
                {
                    return ResultadoOperacion.Error("El paciente ya no existe.");
                }

                ResultadoOperacion resultado = new ResultadoOperacion();
                resultado.IdGenerado = h.IdHistoria;
                resultado.NumeroHistoria = h.NumeroHistoria;
                return resultado;
            }
            catch (SqlException ex)
            {
                return ResultadoOperacion.Error(RegistroTecnico.ErrorParaUsuario("Historias - abrir", ex));
            }
        }

        /// <summary>
        /// Regla para el botón "Activar / Inactivar": a qué estado pasa una historia
        /// según su estado actual (Activa → Inactiva; Inactiva → Activa).
        /// Vive aquí y no en el formulario para que la decisión sea de Negocio.
        /// </summary>
        public static EstadoHistoria EstadoDestino(EstadoHistoria actual)
        {
            return actual == EstadoHistoria.Activa ? EstadoHistoria.Inactiva : EstadoHistoria.Activa;
        }

        /// <summary>
        /// Activa o inactiva una historia (regla 2: nunca se elimina).
        /// Recordatorio (regla 3): una historia Inactiva no podrá recibir citas nuevas.
        /// </summary>
        public ResultadoOperacion CambiarEstado(int idHistoria, EstadoHistoria nuevo)
        {
            if (idHistoria <= 0)
            {
                return ResultadoOperacion.Error("El paciente seleccionado no tiene historia clínica.");
            }
            if (!Enum.IsDefined(typeof(EstadoHistoria), nuevo))
            {
                return ResultadoOperacion.Error("El estado indicado no es válido.");
            }

            try
            {
                if (!dao.CambiarEstado(idHistoria, nuevo))
                {
                    return ResultadoOperacion.Error("No se encontró la historia clínica.");
                }

                new AuditoriaNegocio().Registrar("CAMBIAR_ESTADO_HISTORIA", "HistoriaClinica", idHistoria,
                    "Nuevo estado: " + nuevo);

                ResultadoOperacion resultado = new ResultadoOperacion();
                resultado.IdGenerado = idHistoria;
                return resultado;
            }
            catch (SqlException ex)
            {
                return ResultadoOperacion.Error(RegistroTecnico.ErrorParaUsuario("Historias - cambiar estado", ex));
            }
        }
    }
}
