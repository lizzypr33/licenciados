using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Text.RegularExpressions;
using SigepostaAdmision.Datos;
using SigepostaAdmision.Entidades;

namespace SigepostaAdmision.Negocio
{
    /// <summary>
    /// Reglas de negocio del módulo Pacientes.
    ///
    /// Todas las validaciones están AQUÍ y no en el formulario: así cualquier
    /// pantalla futura (o una importación masiva) aplica exactamente las mismas
    /// reglas. El formulario solo recoge datos y muestra los mensajes.
    ///
    /// Reglas:
    ///   1. DNI de exactamente 8 dígitos numéricos.
    ///   2. Apellido paterno, materno y nombres obligatorios; se quitan los espacios
    ///      sobrantes y se guardan con la primera letra de cada palabra en mayúscula.
    ///   3. Fecha de nacimiento: no futura ni anterior a hace 120 años.
    ///   4. Sexo: solo 'M' o 'F'.
    ///   5. Teléfono opcional; si se escribe, 9 dígitos.
    ///   6. No se registra dos veces el mismo DNI (se informa quién lo tiene).
    ///   7. Al actualizar no se permite cambiar el DNI.
    ///   8. Al registrar un paciente se le abre automáticamente su historia clínica
    ///      (en la misma transacción que el paciente).
    /// Además se respetan las longitudes máximas de las columnas de la tabla.
    /// </summary>
    public class PacienteNegocio
    {
        private const int EdadMaxima = 120;
        private const int LargoTelefono = 9;

        // Longitudes máximas de las columnas de dbo.Paciente.
        private const int MaxApellido = 40;
        private const int MaxNombres = 60;
        private const int MaxDireccion = 120;

        // Cultura peruana para pasar a "Tipo Título" (respeta tildes y ñ).
        private static readonly TextInfo FormatoTexto = new CultureInfo("es-PE").TextInfo;

        private readonly PacienteDAO dao = new PacienteDAO();

        // ===================== Operaciones =====================

        /// <summary>
        /// Registra un paciente nuevo. Devuelve el IdGenerado si tuvo éxito,
        /// o la lista de errores de validación si no.
        /// </summary>
        public ResultadoOperacion Registrar(DatosPaciente datos)
        {
            ResultadoOperacion resultado = new ResultadoOperacion();
            Paciente p = Validar(datos, resultado, true);
            if (!resultado.Exito)
            {
                return resultado;
            }

            try
            {
                // Regla 6: un DNI ya registrado en la red no se registra de nuevo.
                if (dao.ExisteDni(p.Dni))
                {
                    return ResultadoOperacion.Error(MensajeDniExistente(p.Dni));
                }

                // Regla 8: todo paciente nuevo nace con su historia clínica.
                // Paciente e historia se guardan en UNA transacción (ver PacienteDAO):
                // o se guardan los dos, o ninguno.
                string numeroHistoria;
                resultado.IdGenerado = dao.InsertarConHistoria(p, out numeroHistoria);
                resultado.NumeroHistoria = numeroHistoria;

                // Auditoría tras el éxito, sin datos personales (el Id basta para rastrear).
                new AuditoriaNegocio().Registrar("REGISTRAR_PACIENTE", "Paciente", resultado.IdGenerado,
                    "Alta de paciente con historia clínica");
            }
            catch (RegistroDuplicadoException ex)
            {
                // Caso raro: otro usuario registró el mismo DNI entre la
                // verificación y el INSERT. La restricción UNIQUE de la tabla lo
                // detiene y aquí se informa con el mismo mensaje claro.
                // (Si lo repetido fue el número de historia, se usa el mensaje del DAO.)
                return ResultadoOperacion.Error(ex.Campo == "NumeroHistoria" ? ex.Message : MensajeDniExistente(p.Dni));
            }
            catch (Exception ex) when (ex is SqlException || ex is InvalidOperationException)
            {
                return ResultadoOperacion.Error(RegistroTecnico.ErrorParaUsuario("Pacientes - registrar", ex));
            }
            return resultado;
        }

        /// <summary>
        /// Actualiza un paciente existente. El DNI no se puede cambiar: si el DNI
        /// recibido no coincide con el guardado, se rechaza la operación.
        /// </summary>
        public ResultadoOperacion Actualizar(int idPaciente, DatosPaciente datos)
        {
            ResultadoOperacion resultado = new ResultadoOperacion();
            Paciente p = Validar(datos, resultado, false);
            if (!resultado.Exito)
            {
                return resultado;
            }

            try
            {
                Paciente actual = dao.ObtenerPorId(idPaciente);
                if (actual == null)
                {
                    return ResultadoOperacion.Error("El paciente que intenta modificar ya no existe.");
                }

                // Regla 7: el DNI no se modifica.
                string dniRecibido = (datos.Dni ?? string.Empty).Trim();
                if (dniRecibido.Length > 0 && dniRecibido != actual.Dni)
                {
                    return ResultadoOperacion.Error("No se permite cambiar el DNI de un paciente registrado.");
                }

                p.IdPaciente = idPaciente;
                p.Dni = actual.Dni;
                if (!dao.Actualizar(p))
                {
                    return ResultadoOperacion.Error("No se encontró el paciente para actualizarlo.");
                }
                resultado.IdGenerado = idPaciente;

                new AuditoriaNegocio().Registrar("ACTUALIZAR_PACIENTE", "Paciente", idPaciente,
                    "Datos del paciente actualizados");
            }
            catch (RegistroDuplicadoException ex)
            {
                return ResultadoOperacion.Error(ex.Message);
            }
            catch (Exception ex) when (ex is SqlException || ex is InvalidOperationException)
            {
                return ResultadoOperacion.Error(RegistroTecnico.ErrorParaUsuario("Pacientes - actualizar", ex));
            }
            return resultado;
        }

        /// <summary>Busca un paciente por DNI. Devuelve null si el DNI no es válido o no existe.</summary>
        public Paciente BuscarPorDni(string dni)
        {
            string valor = (dni ?? string.Empty).Trim();
            return EsDniValido(valor) ? dao.ObtenerPorDni(valor) : null;
        }

        /// <summary>Obtiene un paciente por su Id (null si no existe).</summary>
        public Paciente ObtenerPorId(int idPaciente)
        {
            return idPaciente > 0 ? dao.ObtenerPorId(idPaciente) : null;
        }

        /// <summary>Busca por DNI exacto o por parte de apellidos o nombres. Texto vacío = recientes.</summary>
        public List<Paciente> Buscar(string texto)
        {
            string valor = NormalizarEspacios(texto);
            return valor.Length == 0 ? ListarRecientes() : dao.Buscar(valor);
        }

        /// <summary>Los 100 pacientes registrados más recientemente.</summary>
        public List<Paciente> ListarRecientes()
        {
            return dao.Listar();
        }

        /// <summary>
        /// Regla 1 expuesta para que otras capas pregunten si un texto es un DNI
        /// válido (por ejemplo, para ofrecer registrar un DNI no encontrado)
        /// sin repetir la regla.
        /// </summary>
        public static bool EsDniValido(string dni)
        {
            return dni != null && Regex.IsMatch(dni, "^[0-9]{8}$");
        }

        // ===================== Validación =====================

        /// <summary>
        /// Revisa TODOS los datos y acumula los errores en "resultado" (no se
        /// detiene en el primero). Si no hay errores, devuelve el Paciente ya
        /// normalizado; si los hay, devuelve null.
        /// </summary>
        private Paciente Validar(DatosPaciente d, ResultadoOperacion resultado, bool validarDni)
        {
            if (d == null)
            {
                resultado.AgregarError("No se recibieron datos del paciente.");
                return null;
            }

            // Regla 1: DNI (al actualizar se ignora, porque no se puede cambiar).
            string dni = (d.Dni ?? string.Empty).Trim();
            if (validarDni && !EsDniValido(dni))
            {
                resultado.AgregarError("El DNI debe tener exactamente 8 dígitos numéricos.");
            }

            // Regla 2: textos obligatorios, normalizados.
            string paterno = ValidarNombre(d.ApellidoPaterno, "El apellido paterno", false, MaxApellido, resultado);
            string materno = ValidarNombre(d.ApellidoMaterno, "El apellido materno", false, MaxApellido, resultado);
            string nombres = ValidarNombre(d.Nombres, "Los nombres", true, MaxNombres, resultado);

            // Regla 3: fecha de nacimiento dentro de un rango razonable.
            DateTime hoy = DateTime.Today;
            DateTime fecha = d.FechaNacimiento.Date;
            if (fecha > hoy)
            {
                resultado.AgregarError("La fecha de nacimiento no puede ser futura.");
            }
            else if (fecha < hoy.AddYears(-EdadMaxima))
            {
                resultado.AgregarError("La fecha de nacimiento no puede ser anterior a hace " + EdadMaxima + " años.");
            }

            // Regla 4: sexo.
            char sexo = char.ToUpperInvariant(d.Sexo);
            if (sexo != 'M' && sexo != 'F')
            {
                resultado.AgregarError("Seleccione el sexo (Masculino o Femenino).");
            }

            // Regla 5: teléfono opcional de 9 dígitos.
            string telefono = (d.Telefono ?? string.Empty).Trim();
            if (telefono.Length > 0 && !Regex.IsMatch(telefono, "^[0-9]{" + LargoTelefono + "}$"))
            {
                resultado.AgregarError("El teléfono debe tener " + LargoTelefono + " dígitos (o dejarse vacío).");
            }

            // Dirección opcional: solo se limita su longitud.
            string direccion = NormalizarEspacios(d.Direccion);
            if (direccion.Length > MaxDireccion)
            {
                resultado.AgregarError("La dirección no puede superar los " + MaxDireccion + " caracteres.");
            }

            if (!resultado.Exito)
            {
                return null;
            }

            // Todo es válido: recién ahora se construye la entidad (no lanzará excepciones).
            return new Paciente(0, validarDni ? dni : "00000000", paterno, materno, nombres,
                                fecha, sexo, direccion, telefono);
        }

        // Valida un nombre o apellido obligatorio y lo devuelve normalizado.
        // "plural" solo ajusta la concordancia del mensaje ("es obligatorio" / "son obligatorios").
        private static string ValidarNombre(string valor, string campo, bool plural, int maximo, ResultadoOperacion resultado)
        {
            string texto = NormalizarEspacios(valor);
            if (texto.Length == 0)
            {
                resultado.AgregarError(campo + (plural ? " son obligatorios." : " es obligatorio."));
                return texto;
            }
            if (texto.Length > maximo)
            {
                resultado.AgregarError(campo + (plural ? " no pueden" : " no puede") + " superar los " + maximo + " caracteres.");
            }
            return ATitulo(texto);
        }

        // Quita espacios al inicio y al final, y deja un solo espacio entre palabras.
        private static string NormalizarEspacios(string texto)
        {
            return string.IsNullOrWhiteSpace(texto) ? string.Empty : Regex.Replace(texto.Trim(), @"\s+", " ");
        }

        // "QUISPE  mamani" -> "Quispe Mamani". Se pasa antes a minúsculas porque
        // ToTitleCase no cambia las palabras que ya están en MAYÚSCULAS.
        private static string ATitulo(string texto)
        {
            return FormatoTexto.ToTitleCase(texto.ToLower(CultureInfo.GetCultureInfo("es-PE")));
        }

        // Mensaje para un DNI repetido, con el nombre de quien ya lo tiene.
        private string MensajeDniExistente(string dni)
        {
            Paciente existente;
            try
            {
                existente = dao.ObtenerPorDni(dni);
            }
            catch (SqlException)
            {
                existente = null;   // Si falla la consulta, se da el mensaje sin el nombre
            }
            return existente == null
                ? "Ya existe un paciente con el DNI " + dni + "."
                : "Ya existe un paciente con ese DNI: " + existente.Apellidos + ", " + existente.Nombres + " (DNI " + dni + ").";
        }
    }
}
