using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using SigepostaAdmision.Datos;
using SigepostaAdmision.Entidades;
using SigepostaAdmision.Entidades.Respaldo;
using SigepostaAdmision.Seguridad;

namespace SigepostaAdmision.Negocio
{
    /// <summary>
    /// Respaldo de información (Unidad 3): serialización, compresión y cifrado.
    /// SOLO para el rol Administrador.
    ///
    /// CADENA DE PROCESAMIENTO al exportar:
    ///   1) Base de datos → entidades → DTO (HistoriaClinicaXml / ReporteDiarioXml)
    ///   2) SERIALIZACIÓN: XmlSerializer convierte el DTO en texto XML (MemoryStream).
    ///   3) COMPRESIÓN: GZipStream reduce el XML (las etiquetas repetidas se
    ///      comprimen muy bien; suele quedar entre 80 % y 95 % más pequeño).
    ///   4) CIFRADO (opcional): CifradoRespaldo → AES-256 + HMAC con una contraseña.
    ///   5) ESCRITURA: FileStream guarda el resultado (.sigebak cifrado o .xml.gz sin cifrar).
    /// Al verificar se hace el camino inverso: leer → (autenticar y descifrar) →
    /// descomprimir → deserializar → resumen.
    ///
    /// Todos los flujos (FileStream, MemoryStream, GZipStream, CryptoStream) se usan
    /// con "using" para liberar los recursos aunque ocurra un error.
    /// </summary>
    public class RespaldoNegocio
    {
        public const int MinimoContrasena = 8;
        public const string ExtensionCifrada = ".sigebak";
        public const string ExtensionSinCifrar = ".xml.gz";
        public const string ExtensionClaves = ".sigekeys";

        // Los XmlSerializer se crean una sola vez (crearlos es costoso).
        private static readonly XmlSerializer SerializadorHistoria = new XmlSerializer(typeof(HistoriaClinicaXml));
        private static readonly XmlSerializer SerializadorReporte = new XmlSerializer(typeof(ReporteDiarioXml));

        private readonly HistoriaDAO historiaDao = new HistoriaDAO();
        private readonly AtencionDAO atencionDao = new AtencionDAO();
        private readonly CitaDAO citaDao = new CitaDAO();
        private readonly CatalogoDAO catalogoDao = new CatalogoDAO();
        private readonly MigracionDAO migracionDao = new MigracionDAO();

        // ===================== Permisos y validaciones =====================

        /// <summary>¿El usuario de la sesión es Administrador? (solo él puede hacer respaldos)</summary>
        public static bool EsAdministrador()
        {
            return Sesion.HaySesion && Sesion.UsuarioActual.Rol == RolUsuario.Administrador;
        }

        private static ResultadoRespaldo SinPermiso()
        {
            return ResultadoRespaldo.Fallo("Solo el Administrador puede realizar respaldos y administrar claves.");
        }

        /// <summary>
        /// Contraseña del respaldo: obligatoria, mínimo 8 caracteres y debe
        /// coincidir con su confirmación. (La regla vive aquí, no en el formulario.)
        /// </summary>
        public ResultadoOperacion ValidarContrasena(string contrasena, string confirmacion)
        {
            ResultadoOperacion r = new ResultadoOperacion();
            if (string.IsNullOrEmpty(contrasena))
            {
                r.AgregarError("Escriba una contraseña para el respaldo.");
                return r;
            }
            if (contrasena.Length < MinimoContrasena)
            {
                r.AgregarError("La contraseña debe tener al menos " + MinimoContrasena + " caracteres.");
            }
            if (!string.Equals(contrasena, confirmacion, StringComparison.Ordinal))
            {
                r.AgregarError("La contraseña y su confirmación no coinciden.");
            }
            return r;
        }

        // ===================== Consultas de apoyo para la pantalla =====================

        /// <summary>Postas para el ComboBox (vacío si no es Administrador).</summary>
        public List<PostaItem> Postas()
        {
            return EsAdministrador() ? catalogoDao.Postas() : new List<PostaItem>();
        }

        /// <summary>Busca una historia por número y devuelve en Resumen "Paciente (DNI enmascarado)".</summary>
        public ResultadoRespaldo BuscarHistoria(string numeroHistoria)
        {
            if (!EsAdministrador()) return SinPermiso();
            string numero = (numeroHistoria ?? string.Empty).Trim().ToUpperInvariant();
            if (numero.Length == 0)
            {
                return ResultadoRespaldo.Fallo("Escriba el número de historia (por ejemplo HC-000001).");
            }

            try
            {
                HistoriaClinica h = historiaDao.ObtenerPorNumero(numero);
                if (h == null)
                {
                    return ResultadoRespaldo.Fallo("No existe la historia clínica " + numero + ".");
                }
                return new ResultadoRespaldo
                {
                    Resumen = h.Paciente.Apellidos + ", " + h.Paciente.Nombres +
                              " (DNI " + Mascara.Dni(h.Paciente.Dni) + ") — historia " + h.Estado
                };
            }
            catch (SqlException ex)
            {
                return ResultadoRespaldo.Fallo(RegistroTecnico.ErrorParaUsuario("Respaldo - buscar historia", ex));
            }
        }

        // ===================== Exportar historia clínica =====================

        /// <summary>
        /// Exporta una historia clínica completa (paciente, historia y atenciones con
        /// diagnósticos DESCIFRADOS) a un archivo comprimido y, si cifrar es true,
        /// cifrado con contraseña.
        ///
        /// Como contiene DNI completo y diagnósticos, exportarla SIN cifrar exige que
        /// el usuario haya aceptado expresamente el aviso (aceptaExportarSinCifrar).
        /// </summary>
        public ResultadoRespaldo ExportarHistoria(string numeroHistoria, string rutaArchivo, string contrasena,
                                                  string confirmacion, bool cifrar, bool aceptaExportarSinCifrar,
                                                  Action<int, string> progreso = null)
        {
            if (!EsAdministrador()) return SinPermiso();
            Action<int, string> avance = progreso ?? ((p, t) => { });

            ResultadoRespaldo previo = ValidarExportacion(rutaArchivo, contrasena, confirmacion, cifrar);
            if (previo != null) return previo;
            if (!cifrar && !aceptaExportarSinCifrar)
            {
                return ResultadoRespaldo.Fallo(
                    "La historia contiene el DNI completo y los diagnósticos del paciente. " +
                    "Para exportarla sin cifrar debe aceptar expresamente el aviso.");
            }

            try
            {
                // 1) Datos de la base → DTO.
                avance(10, "Leyendo la historia clínica...");
                string numero = (numeroHistoria ?? string.Empty).Trim().ToUpperInvariant();
                HistoriaClinica h = historiaDao.ObtenerPorNumero(numero);
                if (h == null)
                {
                    return ResultadoRespaldo.Fallo("No existe la historia clínica " + numero + ".");
                }

                HistoriaClinicaXml dto = new HistoriaClinicaXml
                {
                    NumeroHistoria = h.NumeroHistoria,
                    FechaApertura = h.FechaApertura,
                    Estado = h.Estado.ToString(),
                    FechaExportacion = DateTime.Now,
                    ExportadoPor = Sesion.UsuarioActual.NombreUsuario,
                    Paciente = new PacienteXml
                    {
                        Dni = h.Paciente.Dni,
                        ApellidoPaterno = h.Paciente.ApellidoPaterno,
                        ApellidoMaterno = h.Paciente.ApellidoMaterno,
                        Nombres = h.Paciente.Nombres,
                        FechaNacimiento = h.Paciente.FechaNacimiento,
                        Sexo = h.Paciente.Sexo.ToString(),
                        Direccion = h.Paciente.Direccion,
                        Telefono = h.Paciente.Telefono
                    }
                };

                avance(25, "Descifrando diagnósticos...");
                foreach (AtencionResumen a in atencionDao.ObtenerAtencionesPorPaciente(h.Paciente.IdPaciente)
                                                         .OrderBy(x => x.Fecha).ThenBy(x => x.Hora))
                {
                    byte[] cifrado = atencionDao.ObtenerDiagnosticoCifrado(a.IdAtencion);
                    dto.Atenciones.Add(new AtencionXml
                    {
                        Fecha = a.Fecha,
                        Hora = a.Hora.ToString(@"hh\:mm"),
                        Consultorio = a.Consultorio,
                        Profesional = a.Profesional,
                        Observaciones = a.Observaciones,
                        Diagnostico = cifrado != null ? CifradoAes.Descifrar(cifrado) : string.Empty
                    });
                }

                // 2) Serialización a XML.
                avance(40, "Serializando a XML...");
                byte[] xml = Serializar(SerializadorHistoria, dto);

                // 3 a 5) Comprimir, cifrar (opcional) y escribir.
                return Empaquetar(xml, rutaArchivo, contrasena, cifrar, avance,
                                  "EXPORTAR_HISTORIA", "HistoriaClinica", h.IdHistoria);
            }
            catch (Exception ex) when (EsErrorEsperado(ex))
            {
                return ResultadoRespaldo.Fallo(RegistroTecnico.ErrorParaUsuario("Respaldo - exportar historia", ex));
            }
        }

        // ===================== Exportar reporte diario =====================

        /// <summary>
        /// Exporta el reporte de citas de un día (DNI enmascarado). idPosta null =
        /// todas las postas. Puede ir cifrado o no.
        /// </summary>
        public ResultadoRespaldo ExportarReporteDiario(DateTime fecha, int? idPosta, string rutaArchivo,
                                                       string contrasena, string confirmacion, bool cifrar,
                                                       Action<int, string> progreso = null)
        {
            if (!EsAdministrador()) return SinPermiso();
            Action<int, string> avance = progreso ?? ((p, t) => { });

            ResultadoRespaldo previo = ValidarExportacion(rutaArchivo, contrasena, confirmacion, cifrar);
            if (previo != null) return previo;

            try
            {
                avance(10, "Leyendo las citas del día...");
                List<CitaResumen> citas = citaDao.ListarPorFecha(fecha.Date, idPosta, null, null);

                string nombrePosta = "Todas las postas";
                if (idPosta.HasValue)
                {
                    PostaItem posta = catalogoDao.Postas().FirstOrDefault(p => p.IdPosta == idPosta.Value);
                    nombrePosta = posta != null ? posta.Nombre : "Posta " + idPosta.Value;
                }

                // Conteos con LINQ y DNI enmascarado en cada cita.
                ReporteDiarioXml dto = new ReporteDiarioXml
                {
                    Fecha = fecha.Date,
                    Posta = nombrePosta,
                    GeneradoPor = Sesion.UsuarioActual.NombreUsuario,
                    FechaGeneracion = DateTime.Now,
                    Resumen = new ResumenReporteXml
                    {
                        Total = citas.Count,
                        Pendientes = citas.Count(c => c.Estado == EstadoCita.Pendiente),
                        EnCola = citas.Count(c => c.Estado == EstadoCita.EnCola),
                        Atendidas = citas.Count(c => c.Estado == EstadoCita.Atendida),
                        Canceladas = citas.Count(c => c.Estado == EstadoCita.Cancelada)
                    },
                    Citas = citas.Select(c => new CitaXml
                    {
                        Turno = c.Turno,
                        Hora = c.HoraTexto,
                        Paciente = c.NombrePaciente,
                        DniEnmascarado = Mascara.Dni(c.Dni),
                        Consultorio = c.Consultorio,
                        Profesional = c.Profesional,
                        Estado = c.Estado.ToString()
                    }).ToList()
                };

                avance(40, "Serializando a XML...");
                byte[] xml = Serializar(SerializadorReporte, dto);

                return Empaquetar(xml, rutaArchivo, contrasena, cifrar, avance,
                                  "EXPORTAR_REPORTE", "Cita", null);
            }
            catch (Exception ex) when (EsErrorEsperado(ex))
            {
                return ResultadoRespaldo.Fallo(RegistroTecnico.ErrorParaUsuario("Respaldo - exportar reporte", ex));
            }
        }

        // ===================== Verificar un respaldo =====================

        /// <summary>
        /// Abre un respaldo y devuelve un resumen, SIN modificar la base de datos:
        /// leer → (si es .sigebak: verificar HMAC y descifrar) → descomprimir →
        /// deserializar según el elemento raíz → resumen.
        /// </summary>
        public ResultadoRespaldo VerificarRespaldo(string rutaArchivo, string contrasena, Action<int, string> progreso = null)
        {
            if (!EsAdministrador()) return SinPermiso();
            Action<int, string> avance = progreso ?? ((p, t) => { });

            if (string.IsNullOrWhiteSpace(rutaArchivo) || !File.Exists(rutaArchivo))
            {
                return ResultadoRespaldo.Fallo("El archivo no existe.");
            }

            try
            {
                avance(10, "Leyendo el archivo...");
                byte[] archivo = Leer(rutaArchivo);

                bool cifrado = CifradoRespaldo.EsRespaldoCifrado(archivo);
                byte[] comprimido = archivo;
                if (cifrado)
                {
                    if (string.IsNullOrEmpty(contrasena))
                    {
                        return ResultadoRespaldo.Fallo(
                            "Este respaldo está cifrado: escriba su contraseña en \"Contraseña del respaldo\".");
                    }
                    avance(35, "Verificando integridad (HMAC) y descifrando...");
                    comprimido = CifradoRespaldo.Abrir(archivo, contrasena);
                }

                avance(60, "Descomprimiendo (GZip)...");
                byte[] xml = Descomprimir(comprimido);

                avance(80, "Deserializando el XML...");
                string raiz = LeerElementoRaiz(xml);
                string resumen;
                switch (raiz)
                {
                    case "HistoriaClinica":
                        resumen = ResumirHistoria(Deserializar<HistoriaClinicaXml>(SerializadorHistoria, xml));
                        break;
                    case "ReporteDiario":
                        resumen = ResumirReporte(Deserializar<ReporteDiarioXml>(SerializadorReporte, xml));
                        break;
                    default:
                        return ResultadoRespaldo.Fallo("Tipo de respaldo desconocido (elemento raíz \"" + raiz + "\").");
                }

                ResultadoRespaldo r = new ResultadoRespaldo
                {
                    RutaArchivo = rutaArchivo,
                    BytesXml = xml.Length,
                    BytesFinal = archivo.Length
                };

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Archivo:     " + Path.GetFileName(rutaArchivo));
                sb.AppendLine("Protección:  " + (cifrado
                    ? "cifrado AES-256 + HMAC-SHA256 (integridad verificada)"
                    : "SIN cifrar (solo GZip; su CRC detecta daños accidentales, no modificaciones intencionales)"));
                sb.AppendLine("Tamaños:     " + r.TextoTamanos());
                sb.AppendLine();
                sb.Append(resumen);
                r.Resumen = sb.ToString();

                new AuditoriaNegocio().Registrar("VERIFICAR_RESPALDO", "Respaldo", null,
                    "Archivo: " + Path.GetFileName(rutaArchivo));
                avance(100, "Respaldo verificado.");
                return r;
            }
            catch (CryptographicException)
            {
                // Mensaje fijo pensado para el usuario (no el texto técnico de la excepción).
                return ResultadoRespaldo.Fallo("Contraseña incorrecta o archivo alterado.");
            }
            catch (InvalidDataException)
            {
                return ResultadoRespaldo.Fallo("El archivo no es un respaldo de SIGEPOSTA válido (no está comprimido con GZip).");
            }
            catch (Exception ex) when (EsErrorEsperado(ex))
            {
                return ResultadoRespaldo.Fallo(RegistroTecnico.ErrorParaUsuario("Respaldo - verificar", ex));
            }
        }

        // ===================== Claves del sistema =====================

        /// <summary>
        /// Exporta las claves AES y HMAC del sistema a un archivo ".sigekeys"
        /// protegido con contraseña (CifradoRespaldo). Sin este archivo, si se pierde
        /// el claves.dat local (otro equipo, otro usuario, reinstalación), los DNI y
        /// diagnósticos cifrados quedarían ilegibles para siempre.
        /// </summary>
        public ResultadoRespaldo ExportarClaves(string rutaArchivo, string contrasena, string confirmacion)
        {
            if (!EsAdministrador()) return SinPermiso();

            ResultadoRespaldo previo = ValidarExportacion(rutaArchivo, contrasena, confirmacion, true);
            if (previo != null) return previo;

            byte[] claves = null;
            try
            {
                claves = GestorClaves.ExportarBytes();
                byte[] protegido = CifradoRespaldo.Proteger(claves, contrasena);
                string ruta = AjustarExtension(rutaArchivo, ExtensionClaves);
                Escribir(ruta, protegido);

                new AuditoriaNegocio().Registrar("EXPORTAR_CLAVES", "Claves", null, "Archivo: " + Path.GetFileName(ruta));
                return new ResultadoRespaldo
                {
                    RutaArchivo = ruta,
                    BytesFinal = protegido.Length,
                    Resumen = "Claves exportadas a " + Path.GetFileName(ruta) + " (huella " + GestorClaves.Huella + "). " +
                              "Guarde el archivo y su contraseña en un lugar seguro y separado de la base de datos."
                };
            }
            catch (Exception ex) when (EsErrorEsperado(ex))
            {
                return ResultadoRespaldo.Fallo(RegistroTecnico.ErrorParaUsuario("Respaldo - exportar claves", ex));
            }
            finally
            {
                if (claves != null) Array.Clear(claves, 0, claves.Length);
            }
        }

        /// <summary>
        /// Importa las claves desde un ".sigekeys". ANTES de reemplazar nada:
        ///   1) valida la contraseña (el HMAC del archivo),
        ///   2) comprueba que las claves importadas descifran un DNI REAL de la tabla
        ///      Paciente y producen su misma huella (DniHash).
        /// Si no descifran, NO reemplaza y avisa: importar claves equivocadas dejaría
        /// todos los datos cifrados ilegibles.
        /// </summary>
        public ResultadoRespaldo ImportarClaves(string rutaArchivo, string contrasena)
        {
            if (!EsAdministrador()) return SinPermiso();
            if (string.IsNullOrWhiteSpace(rutaArchivo) || !File.Exists(rutaArchivo))
            {
                return ResultadoRespaldo.Fallo("El archivo no existe.");
            }
            if (string.IsNullOrEmpty(contrasena))
            {
                return ResultadoRespaldo.Fallo("Escriba la contraseña con la que se exportaron las claves.");
            }

            byte[] candidatas = null;
            try
            {
                byte[] archivo = Leer(rutaArchivo);
                if (!CifradoRespaldo.EsRespaldoCifrado(archivo))
                {
                    return ResultadoRespaldo.Fallo("El archivo no es un archivo de claves de SIGEPOSTA.");
                }

                candidatas = CifradoRespaldo.Abrir(archivo, contrasena);   // 1) contraseña / integridad
                if (candidatas.Length != 64)
                {
                    return ResultadoRespaldo.Fallo("El archivo no contiene claves de SIGEPOSTA.");
                }

                byte[] claveAes = new byte[32], claveHmac = new byte[32];
                Buffer.BlockCopy(candidatas, 0, claveAes, 0, 32);
                Buffer.BlockCopy(candidatas, 32, claveHmac, 0, 32);

                // 2) Prueba con un DNI real de la base.
                Tuple<byte[], string> muestra = migracionDao.ObtenerMuestraCifrada();
                if (muestra != null)
                {
                    bool descifran;
                    try
                    {
                        string dni = CifradoAes.Descifrar(muestra.Item1, claveAes);
                        descifran = string.Equals(HashDni.Calcular(dni, claveHmac), muestra.Item2.Trim(),
                                                  StringComparison.OrdinalIgnoreCase);
                    }
                    catch (CryptographicException)
                    {
                        descifran = false;
                    }
                    finally
                    {
                        Array.Clear(claveAes, 0, 32);
                        Array.Clear(claveHmac, 0, 32);
                    }

                    if (!descifran)
                    {
                        return ResultadoRespaldo.Fallo(
                            "Las claves del archivo NO descifran los datos de esta base de datos. " +
                            "No se reemplazó nada.");
                    }
                }

                GestorClaves.ImportarBytes(candidatas);
                new AuditoriaNegocio().Registrar("IMPORTAR_CLAVES", "Claves", null, "Archivo: " + Path.GetFileName(rutaArchivo));

                return new ResultadoRespaldo
                {
                    RutaArchivo = rutaArchivo,
                    Resumen = muestra != null
                        ? "Claves importadas y verificadas con un DNI real de la base (huella " + GestorClaves.Huella + ")."
                        : "Claves importadas (huella " + GestorClaves.Huella + "). Aviso: la base aún no tiene " +
                          "pacientes cifrados, por lo que no se pudo comprobar con datos reales."
                };
            }
            catch (CryptographicException)
            {
                // Mensaje fijo pensado para el usuario (no el texto técnico de la excepción).
                return ResultadoRespaldo.Fallo("Contraseña incorrecta o archivo alterado.");
            }
            catch (Exception ex) when (EsErrorEsperado(ex))
            {
                return ResultadoRespaldo.Fallo(RegistroTecnico.ErrorParaUsuario("Respaldo - importar claves", ex));
            }
            finally
            {
                if (candidatas != null) Array.Clear(candidatas, 0, candidatas.Length);
            }
        }

        // ===================== Cadena de procesamiento =====================

        // Validación común a las exportaciones: ruta y, si se cifra, la contraseña.
        private ResultadoRespaldo ValidarExportacion(string ruta, string contrasena, string confirmacion, bool cifrar)
        {
            if (string.IsNullOrWhiteSpace(ruta))
            {
                return ResultadoRespaldo.Fallo("Elija dónde guardar el archivo.");
            }
            if (cifrar)
            {
                ResultadoOperacion v = ValidarContrasena(contrasena, confirmacion);
                if (!v.Exito)
                {
                    ResultadoRespaldo r = new ResultadoRespaldo();
                    r.Mensajes.AddRange(v.Mensajes);
                    return r;
                }
            }
            return null;
        }

        /// <summary>Pasos 3 a 5: comprimir → (cifrar) → escribir, y registrar en auditoría.</summary>
        private ResultadoRespaldo Empaquetar(byte[] xml, string ruta, string contrasena, bool cifrar,
                                             Action<int, string> avance, string accion, string tabla, int? idRegistro)
        {
            // 3) Compresión ANTES del cifrado (los datos cifrados ya no se pueden comprimir).
            avance(60, "Comprimiendo (GZip)...");
            byte[] final = Comprimir(xml);

            // 4) Cifrado + HMAC con la contraseña.
            if (cifrar)
            {
                avance(75, "Cifrando (AES-256 + HMAC)...");
                final = CifradoRespaldo.Proteger(final, contrasena);
            }

            // 5) Escritura con la extensión que corresponde.
            string rutaFinal = AjustarExtension(ruta, cifrar ? ExtensionCifrada : ExtensionSinCifrar);
            avance(90, "Escribiendo el archivo...");
            Escribir(rutaFinal, final);

            // Auditoría: nombre del archivo, NUNCA la contraseña ni el contenido.
            new AuditoriaNegocio().Registrar(accion, tabla, idRegistro,
                "Archivo: " + Path.GetFileName(rutaFinal) + (cifrar ? " (cifrado)" : " (SIN cifrar)"));

            avance(100, "Respaldo generado.");
            return new ResultadoRespaldo { RutaArchivo = rutaFinal, BytesXml = xml.Length, BytesFinal = final.Length };
        }

        /// <summary>
        /// SERIALIZACIÓN: objeto → XML (bytes UTF-8, con sangría para que sea legible).
        /// Se quitan los espacios de nombres xsi/xsd que XmlSerializer agrega por defecto.
        /// </summary>
        private static byte[] Serializar(XmlSerializer serializador, object dto)
        {
            XmlSerializerNamespaces sinEspacios = new XmlSerializerNamespaces();
            sinEspacios.Add(string.Empty, string.Empty);

            XmlWriterSettings opciones = new XmlWriterSettings
            {
                Indent = true,
                Encoding = new UTF8Encoding(false)
            };

            using (MemoryStream ms = new MemoryStream())
            {
                using (XmlWriter escritor = XmlWriter.Create(ms, opciones))
                {
                    serializador.Serialize(escritor, dto, sinEspacios);
                }
                return ms.ToArray();
            }
        }

        /// <summary>
        /// DESERIALIZACIÓN: XML → objeto. DtdProcessing.Prohibit impide que un
        /// archivo manipulado use DTD para leer archivos del equipo (ataque XXE).
        /// </summary>
        private static T Deserializar<T>(XmlSerializer serializador, byte[] xml)
        {
            using (MemoryStream ms = new MemoryStream(xml))
            using (XmlReader lector = XmlReader.Create(ms, new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit }))
            {
                return (T)serializador.Deserialize(lector);
            }
        }

        // Nombre del elemento raíz (HistoriaClinica / ReporteDiario) para saber qué tipo deserializar.
        private static string LeerElementoRaiz(byte[] xml)
        {
            using (MemoryStream ms = new MemoryStream(xml))
            using (XmlReader lector = XmlReader.Create(ms, new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit }))
            {
                lector.MoveToContent();
                return lector.LocalName;
            }
        }

        /// <summary>COMPRESIÓN con GZipStream (nivel óptimo).</summary>
        private static byte[] Comprimir(byte[] datos)
        {
            using (MemoryStream salida = new MemoryStream())
            {
                // leaveOpen = true: al cerrar el GZipStream se escribe el final del
                // formato GZip, pero el MemoryStream sigue abierto para leerlo.
                using (GZipStream gz = new GZipStream(salida, CompressionLevel.Optimal, true))
                {
                    gz.Write(datos, 0, datos.Length);
                }
                return salida.ToArray();
            }
        }

        /// <summary>DESCOMPRESIÓN con GZipStream.</summary>
        private static byte[] Descomprimir(byte[] datos)
        {
            using (MemoryStream entrada = new MemoryStream(datos))
            using (GZipStream gz = new GZipStream(entrada, CompressionMode.Decompress))
            using (MemoryStream salida = new MemoryStream())
            {
                gz.CopyTo(salida);
                return salida.ToArray();
            }
        }

        private static void Escribir(string ruta, byte[] datos)
        {
            using (FileStream fs = new FileStream(ruta, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                fs.Write(datos, 0, datos.Length);
            }
        }

        private static byte[] Leer(string ruta)
        {
            using (FileStream fs = new FileStream(ruta, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (MemoryStream ms = new MemoryStream())
            {
                fs.CopyTo(ms);
                return ms.ToArray();
            }
        }

        // Quita extensiones conocidas y pone la correcta ("x.xml.gz" → "x.sigebak").
        private static string AjustarExtension(string ruta, string extension)
        {
            string[] conocidas = { ExtensionCifrada, ExtensionSinCifrar, ExtensionClaves, ".gz", ".xml" };
            string resultado = ruta.Trim();
            bool quitada = true;
            while (quitada)
            {
                quitada = false;
                foreach (string ext in conocidas)
                {
                    if (resultado.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
                    {
                        resultado = resultado.Substring(0, resultado.Length - ext.Length);
                        quitada = true;
                    }
                }
            }
            return resultado + extension;
        }

        private static string ResumirHistoria(HistoriaClinicaXml h)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Tipo:        Historia clínica");
            sb.AppendLine("Historia:    " + h.NumeroHistoria + " (" + h.Estado + "), abierta el " + h.FechaApertura.ToString("dd/MM/yyyy"));
            if (h.Paciente != null)
            {
                sb.AppendLine("Paciente:    " + h.Paciente.ApellidoPaterno + " " + h.Paciente.ApellidoMaterno + ", " +
                              h.Paciente.Nombres + " — DNI " + Mascara.Dni(h.Paciente.Dni));
            }
            sb.AppendLine("Atenciones:  " + h.Atenciones.Count + " (" +
                          h.Atenciones.Count(a => !string.IsNullOrEmpty(a.Diagnostico)) + " con diagnóstico)");
            sb.AppendLine("Exportado:   " + h.FechaExportacion.ToString("dd/MM/yyyy HH:mm") + " por " + h.ExportadoPor);
            return sb.ToString();
        }

        private static string ResumirReporte(ReporteDiarioXml r)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Tipo:        Reporte diario de citas");
            sb.AppendLine("Fecha:       " + r.Fecha.ToString("dd/MM/yyyy") + " — " + r.Posta);
            sb.AppendLine("Citas:       " + r.Resumen.Total + " (Pendientes " + r.Resumen.Pendientes + ", En cola " +
                          r.Resumen.EnCola + ", Atendidas " + r.Resumen.Atendidas + ", Canceladas " + r.Resumen.Canceladas + ")");
            sb.AppendLine("Generado:    " + r.FechaGeneracion.ToString("dd/MM/yyyy HH:mm") + " por " + r.GeneradoPor);
            return sb.ToString();
        }

        // Errores que se informan al usuario en lugar de cerrar la aplicación.
        private static bool EsErrorEsperado(Exception ex)
        {
            return ex is IOException || ex is UnauthorizedAccessException || ex is SqlException ||
                   ex is CryptographicException || ex is InvalidDataException || ex is XmlException ||
                   ex is InvalidOperationException || ex is ArgumentException;
        }
    }
}
