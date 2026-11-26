using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using SigepostaAdmision.Datos;
using SigepostaAdmision.Entidades;
using SigepostaAdmision.Seguridad;

namespace SigepostaAdmision.Negocio
{
    /// <summary>
    /// Reglas para registrar la atención de un paciente.
    ///
    /// Reglas:
    ///   1. La cita debe existir y ser de la posta del usuario (el Administrador ve todas).
    ///   2. Solo se atiende una cita EnCola que ya fue LLAMADA (Cita.MarcarAtendida()).
    ///   3. La historia clínica del paciente debe existir y estar Activa.
    ///   4. Una cita tiene como máximo una atención.
    ///   5. Observaciones opcionales, como máximo 300 caracteres.
    ///   6. Diagnóstico opcional, como máximo 500 caracteres; se guarda CIFRADO
    ///      (AES-256) y cada consulta queda registrada en la auditoría.
    /// Las reglas 1, 2 y 5 se validan aquí ANTES de tocar la base; las reglas 2, 3
    /// y 4 se vuelven a garantizar DENTRO de la transacción (AtencionDAO), por si
    /// otro puesto cambió algo entre la validación y el guardado.
    /// </summary>
    public class AtencionNegocio
    {
        private readonly AtencionDAO atencionDao = new AtencionDAO();
        private readonly AuditoriaDAO auditoriaDao = new AuditoriaDAO();
        private readonly CitaDAO citaDao = new CitaDAO();

        /// <summary>Datos de la cita para mostrar en el diálogo (null si no existe o no es de su posta).</summary>
        public CitaResumen ObtenerCita(int idCita)
        {
            CitaResumen c = idCita > 0 ? citaDao.ObtenerPorId(idCita) : null;
            return c != null && AccesoPosta.PuedeVerPosta(c.IdPosta) ? c : null;
        }

        /// <summary>Atenciones del paciente (para la historia clínica).</summary>
        public List<AtencionResumen> ObtenerAtencionesPorPaciente(int idPaciente)
        {
            return idPaciente > 0 ? atencionDao.ObtenerAtencionesPorPaciente(idPaciente) : new List<AtencionResumen>();
        }

        /// <summary>Registra la atención sin diagnóstico.</summary>
        public ResultadoOperacion RegistrarAtencion(int idCita, string observaciones)
        {
            return RegistrarAtencion(idCita, observaciones, null);
        }

        /// <summary>
        /// Registra la atención de la cita. Devuelve un ResultadoOperacion: si tuvo
        /// éxito, IdGenerado = IdAtencion; si no, los mensajes en español.
        /// No lanza excepciones de validación a la interfaz.
        ///
        /// El diagnóstico (opcional, máx. 500 caracteres) se CIFRA AQUÍ con AES-256
        /// antes de llamar al DAO: así el texto del diagnóstico nunca viaja a la capa
        /// de Datos ni se guarda legible en la base. Se guarda dentro de la misma
        /// transacción que la cita, la atención y la auditoría.
        /// </summary>
        public ResultadoOperacion RegistrarAtencion(int idCita, string observaciones, string diagnostico)
        {
            ResultadoOperacion r = new ResultadoOperacion();
            if (!Sesion.HaySesion)
            {
                return ResultadoOperacion.Error("No hay una sesión iniciada.");
            }

            // Regla 5: observaciones.
            string texto = (observaciones ?? string.Empty).Trim();
            if (texto.Length > Atencion.MaxObservaciones)
            {
                r.AgregarError("Las observaciones no pueden superar los " + Atencion.MaxObservaciones +
                               " caracteres (tiene " + texto.Length + ").");
            }

            // Regla 6: diagnóstico opcional de hasta 500 caracteres.
            string textoDiagnostico = (diagnostico ?? string.Empty).Trim();
            if (textoDiagnostico.Length > Atencion.MaxDiagnostico)
            {
                r.AgregarError("El diagnóstico no puede superar los " + Atencion.MaxDiagnostico +
                               " caracteres (tiene " + textoDiagnostico.Length + ").");
            }

            try
            {
                // Regla 1: la cita existe y es de la posta del usuario.
                CitaResumen actual = ObtenerCita(idCita);
                if (actual == null)
                {
                    r.AgregarError("La cita no existe o no pertenece a su posta.");
                    return r;
                }

                // Regla 2: regla de dominio en la entidad (EnCola y ya llamado).
                Cita cita = CitaFabrica.DesdeResumen(actual);
                try
                {
                    cita.MarcarAtendida();
                }
                catch (InvalidOperationException ex)
                {
                    r.AgregarError(ex.Message);
                }

                if (!r.Exito)
                {
                    return r;
                }

                // Cifrado del diagnóstico ANTES de ir a Datos (null si no se escribió).
                byte[] diagnosticoCifrado = textoDiagnostico.Length > 0 ? CifradoAes.Cifrar(textoDiagnostico) : null;

                // Reglas 2, 3 y 4 garantizadas de nuevo dentro de UNA transacción.
                r.IdGenerado = atencionDao.RegistrarAtencion(idCita, Sesion.UsuarioActual.IdUsuario, texto,
                                                             diagnosticoCifrado);
                return r;
            }
            catch (ReglaNegocioException ex)
            {
                return ResultadoOperacion.Error(ex.Message);
            }
            catch (RegistroDuplicadoException ex)
            {
                return ResultadoOperacion.Error(ex.Message);
            }
            catch (Exception ex) when (ex is SqlException || ex is InvalidOperationException || ex is CryptographicException)
            {
                return ResultadoOperacion.Error(RegistroTecnico.ErrorParaUsuario("Atención - registrar", ex));
            }
        }

        /// <summary>
        /// SOLO PARA LA SUSTENTACIÓN (rol Administrador): intenta registrar la
        /// atención con un fallo provocado a mitad de la transacción y devuelve un
        /// informe del estado ANTES y DESPUÉS, para evidenciar que el Rollback no
        /// dejó nada guardado (ni el cambio de la cita, ni la atención, ni la auditoría).
        /// </summary>
        public string DemoRollback(int idCita)
        {
            if (!Sesion.HaySesion || Sesion.UsuarioActual.Rol != RolUsuario.Administrador)
            {
                return "Solo el Administrador puede ejecutar la demostración de Rollback.";
            }

            try
            {
                if (citaDao.ObtenerPorId(idCita) == null)
                {
                    return "La cita indicada no existe.";
                }

                string antes = Foto(idCita);

                string error;
                try
                {
                    atencionDao.RegistrarAtencion(idCita, Sesion.UsuarioActual.IdUsuario,
                        "Demostración de Rollback", forzarFalloParaDemo: true);
                    error = "(no hubo error: la transacción se confirmó)";
                }
                catch (ReglaNegocioException ex)
                {
                    error = ex.Message;   // El fallo simulado (o una regla) es un mensaje pensado para mostrar
                }
                catch (Exception ex)
                {
                    error = RegistroTecnico.ErrorParaUsuario("Atención - demo rollback", ex);
                }

                string despues = Foto(idCita);

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("DEMOSTRACIÓN DE ROLLBACK — cita N° " + idCita);
                sb.AppendLine();
                sb.AppendLine("ANTES:    " + antes);
                sb.AppendLine("ERROR:    " + error);
                sb.AppendLine("DESPUÉS:  " + despues);
                sb.AppendLine();
                sb.AppendLine(antes == despues
                    ? "Resultado: NO quedó nada guardado. El Rollback deshizo el cambio de estado de la cita " +
                      "y la atención insertada; la auditoría nunca llegó a escribirse."
                    : "Resultado: los datos cambiaron (revise: la transacción no se deshizo por completo).");
                return sb.ToString();
            }
            catch (SqlException ex)
            {
                return RegistroTecnico.ErrorParaUsuario("Atención - demo rollback", ex);
            }
        }

        /// <summary>
        /// Descifra y devuelve el diagnóstico de una atención, dejando constancia en
        /// Auditoria (acción VER_DIAGNOSTICO, tabla Atencion, IdRegistro = idAtencion,
        /// usuario de la sesión).
        ///
        /// Decisiones de seguridad:
        ///   - Permiso por rol: solo el Administrador puede LEER diagnósticos
        ///     (Permisos.PuedeVerDiagnostico). Un intento denegado también se audita
        ///     (VER_DIAGNOSTICO_DENEGADO).
        ///   - Control de acceso: solo usuarios de la posta de la atención (el
        ///     Administrador, todas).
        ///   - Trazabilidad: todo acceso a un dato de salud queda registrado.
        ///   - "Falla cerrado": si la auditoría NO se puede registrar, el diagnóstico
        ///     NO se muestra (un acceso sin rastro no está permitido).
        /// accesoRegistrado indica si el acceso quedó en la auditoría.
        /// </summary>
        public string VerDiagnostico(int idAtencion, out bool accesoRegistrado)
        {
            accesoRegistrado = false;
            if (!Sesion.HaySesion)
            {
                return "No hay una sesión iniciada.";
            }

            // Permiso por rol, en la capa de negocio (aunque la pantalla ya deshabilite el botón).
            if (!Permisos.PuedeVerDiagnostico(Sesion.UsuarioActual.Rol))
            {
                new AuditoriaNegocio().Registrar("VER_DIAGNOSTICO_DENEGADO", "Atencion", idAtencion,
                    "Rol " + Sesion.UsuarioActual.Rol + " sin permiso");
                return "Solo el Administrador puede ver diagnósticos. El intento quedó registrado en la auditoría.";
            }

            try
            {
                int? idPosta = idAtencion > 0 ? atencionDao.ObtenerPostaDeAtencion(idAtencion) : null;
                if (!idPosta.HasValue || !AccesoPosta.PuedeVerPosta(idPosta.Value))
                {
                    return "La atención no existe o no pertenece a su posta.";
                }

                byte[] cifrado = atencionDao.ObtenerDiagnosticoCifrado(idAtencion);

                // Primero se audita; solo si quedó registrado se muestra el contenido.
                string detalle = cifrado != null ? "Diagnóstico consultado" : "Consulta sin diagnóstico registrado";
                accesoRegistrado = new AuditoriaNegocio().Registrar("VER_DIAGNOSTICO", "Atencion", idAtencion, detalle);
                if (!accesoRegistrado)
                {
                    return "No se pudo registrar el acceso en la auditoría; por seguridad, el diagnóstico no se muestra.";
                }

                return cifrado == null ? "Sin diagnóstico registrado" : CifradoAes.Descifrar(cifrado);
            }
            catch (CryptographicException)
            {
                return "No se pudo descifrar el diagnóstico (las claves de este equipo no corresponden).";
            }
            catch (SqlException ex)
            {
                return RegistroTecnico.ErrorParaUsuario("Atención - ver diagnóstico", ex);
            }
        }

        // Estado de la cita y cantidad de filas en Atencion y Auditoria.
        private string Foto(int idCita)
        {
            CitaResumen c = citaDao.ObtenerPorId(idCita);
            return "Estado de la cita = " + (c != null ? c.Estado.ToString() : "?") +
                   " | Filas en Atencion = " + atencionDao.ContarAtenciones() +
                   " | Filas en Auditoria = " + auditoriaDao.Contar();
        }
    }
}
