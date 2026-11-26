using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using SigepostaAdmision.Datos;
using SigepostaAdmision.Entidades;
using SigepostaAdmision.Seguridad;

namespace SigepostaAdmision.Negocio
{
    /// <summary>
    /// Reglas del módulo Reportes (sin SQL ni controles).
    ///
    /// Reglas:
    ///   1. Fechas: "desde" no puede ser mayor que "hasta"; rango máximo 366 días.
    ///   2. Alcance por rol: Admision solo ve SU posta (se ignora la posta que
    ///      envíe la pantalla); Administrador elige una posta o todas.
    ///   3. El reporte de Auditoría es SOLO del Administrador.
    ///   4. El DNI nunca sale completo: si el DataTable trae DniCifrado, se descifra
    ///      y se reemplaza por una columna "DNI" enmascarada (45••••78).
    ///   5. Exportar a CSV queda registrado en la auditoría (EXPORTAR_CSV).
    /// </summary>
    public class ReporteNegocio
    {
        public const int RangoMaximoDias = 366;
        private const string Todas = "Todas";

        /// <summary>
        /// Opción "sin filtro" del combo Estado. La pantalla usa ESTA constante
        /// (antes el formulario ponía "Todos" y Negocio esperaba "Todas": el texto
        /// "Todos" llegaba al SQL como estado y el reporte salía vacío).
        /// </summary>
        public const string TodosLosEstados = "Todos";

        private readonly ReporteDAO dao = new ReporteDAO();
        private readonly CatalogoDAO catalogoDao = new CatalogoDAO();

        // ===================== Datos para los filtros de la pantalla =====================

        public static bool EsAdministrador()
        {
            return Sesion.HaySesion && Sesion.UsuarioActual.Rol == RolUsuario.Administrador;
        }

        /// <summary>Reportes que el usuario puede pedir (Auditoría solo el Administrador).</summary>
        public static List<TipoReporte> TiposDisponibles()
        {
            List<TipoReporte> tipos = new List<TipoReporte>
            {
                TipoReporte.CitasDelDia, TipoReporte.AtendidosPorPosta, TipoReporte.ProduccionAdmision
            };
            if (EsAdministrador())
            {
                tipos.Add(TipoReporte.Auditoria);
            }
            return tipos;
        }

        /// <summary>Nombre visible de cada reporte.</summary>
        public static string Nombre(TipoReporte tipo)
        {
            switch (tipo)
            {
                case TipoReporte.CitasDelDia: return "Citas del día";
                case TipoReporte.AtendidosPorPosta: return "Pacientes atendidos por posta";
                case TipoReporte.ProduccionAdmision: return "Producción de Admisión";
                default: return "Auditoría";
            }
        }

        /// <summary>
        /// Postas para el filtro: el Administrador ve todas; un usuario Admision,
        /// solo la suya (y la pantalla deja el combo fijo).
        /// </summary>
        public List<PostaItem> PostasParaFiltro()
        {
            List<PostaItem> postas = catalogoDao.Postas();
            if (EsAdministrador())
            {
                return postas;
            }
            int? propia = Sesion.HaySesion ? Sesion.UsuarioActual.IdPosta : null;
            return postas.Where(p => propia.HasValue && p.IdPosta == propia.Value).ToList();
        }

        /// <summary>Consultorios de la posta indicada (respetando el alcance por rol).</summary>
        public List<ConsultorioItem> Consultorios(int? idPosta)
        {
            int? posta;
            string error;
            if (!ResolverPosta(idPosta, out posta, out error))
            {
                return new List<ConsultorioItem>();
            }
            return catalogoDao.ConsultoriosPorPostaYServicio(posta, null);
        }

        // ===================== Reportes =====================

        /// <summary>Citas de una fecha, con filtros opcionales de consultorio y estado.</summary>
        public ResultadoReporte CitasDelDia(DateTime fecha, int? idPosta, int? idConsultorio, string estado)
        {
            int? posta;
            string error;
            if (!ResolverPosta(idPosta, out posta, out error)) return ResultadoReporte.Fallo(error);

            // Solo se filtra por un estado REAL de EstadoCita; "Todos" (o cualquier otro
            // texto) significa "sin filtro". Así un texto de la pantalla nunca llega al SQL.
            string estadoFiltro = EstadoValido(estado);

            // La hora del DateTimePicker no importa: se compara solo el día.
            fecha = fecha.Date;
            try
            {
                DataTable tabla = dao.CitasDelDia(fecha, posta, idConsultorio, estadoFiltro);
                ReemplazarDniCifrado(tabla);

                ReporteResultado r = Nuevo(TipoReporte.CitasDelDia, tabla,
                    "Fecha: " + fecha.ToString("dd/MM/yyyy") +
                    " · Posta: " + NombrePosta(posta) +
                    " · Consultorio: " + NombreConsultorio(idConsultorio) +
                    " · Estado: " + (estadoFiltro ?? TodosLosEstados));

                // Resumen y gráfico: cantidad por estado (LINQ sobre el DataTable).
                Dictionary<string, int> porEstado = Enum.GetNames(typeof(EstadoCita))
                    .ToDictionary(e => e, e => tabla.AsEnumerable().Count(f => f.Field<string>("Estado") == e));
                r.Resumen.Add(Par("Total de citas", tabla.Rows.Count));
                foreach (KeyValuePair<string, int> e in porEstado)
                {
                    r.Resumen.Add(Par(e.Key, e.Value));
                }
                if (r.TieneDatos)
                {
                    r.TituloGrafico = "Citas por estado";
                    r.Grafico.AddRange(porEstado.Select(e => new KeyValuePair<string, double>(e.Key, e.Value)));
                }
                AvisoLimite(r, 1000);
                return Exito(r);
            }
            catch (Exception ex) when (EsErrorDeDatos(ex))
            {
                return ResultadoReporte.Fallo(RegistroTecnico.ErrorParaUsuario("Reportes", ex));
            }
        }

        /// <summary>Atenciones registradas entre dos fechas, por posta.</summary>
        public ResultadoReporte AtendidosPorPosta(DateTime desde, DateTime hasta, int? idPosta)
        {
            string error;
            if (!ValidarRango(desde, hasta, out error)) return ResultadoReporte.Fallo(error);
            int? posta;
            if (!ResolverPosta(idPosta, out posta, out error)) return ResultadoReporte.Fallo(error);

            try
            {
                DataTable tabla = dao.AtendidosPorPosta(desde.Date, FinExclusivo(hasta), posta);
                ReemplazarDniCifrado(tabla);

                ReporteResultado r = Nuevo(TipoReporte.AtendidosPorPosta, tabla,
                    TextoRango(desde, hasta) + " · Posta: " + NombrePosta(posta));

                r.Resumen.Add(Par("Atenciones", tabla.Rows.Count));
                r.Resumen.Add(Par("Pacientes distintos", tabla.AsEnumerable()
                    .Select(f => f.Field<string>("Paciente") + "|" + f.Field<string>("DNI")).Distinct().Count()));

                // Gráfico: atenciones por servicio (agrupado con LINQ, sin otra consulta).
                if (r.TieneDatos)
                {
                    r.TituloGrafico = "Atenciones por servicio";
                    r.Grafico.AddRange(tabla.AsEnumerable()
                        .GroupBy(f => f.Field<string>("Servicio"))
                        .OrderByDescending(g => g.Count())
                        .Select(g => new KeyValuePair<string, double>(g.Key, g.Count())));
                }
                AvisoLimite(r, 1000);
                return Exito(r);
            }
            catch (Exception ex) when (EsErrorDeDatos(ex))
            {
                return ResultadoReporte.Fallo(RegistroTecnico.ErrorParaUsuario("Reportes", ex));
            }
        }

        /// <summary>
        /// Indicadores de producción de Admisión. La tabla muestra Indicador/Valor y,
        /// al final, las atenciones por servicio (también se grafican).
        /// </summary>
        public ResultadoReporte ProduccionAdmision(DateTime desde, DateTime hasta, int? idPosta)
        {
            string error;
            if (!ValidarRango(desde, hasta, out error)) return ResultadoReporte.Fallo(error);
            int? posta;
            if (!ResolverPosta(idPosta, out posta, out error)) return ResultadoReporte.Fallo(error);

            try
            {
                DataSet ds = dao.ProduccionAdmision(desde.Date, FinExclusivo(hasta), posta);
                DataTable indicadores = ds.Tables["Indicadores"];
                DataTable porServicio = ds.Tables["AtencionesPorServicio"];

                // Las atenciones por servicio se agregan como filas de indicadores, así la
                // grilla, el CSV y la impresión muestran todo en una sola tabla.
                foreach (DataRow f in porServicio.Rows)
                {
                    indicadores.Rows.Add("Atenciones en " + f["Servicio"], Convert.ToInt32(f["Cantidad"]).ToString());
                }

                ReporteResultado r = Nuevo(TipoReporte.ProduccionAdmision, indicadores,
                    TextoRango(desde, hasta) + " · Posta: " + NombrePosta(posta) +
                    " (pacientes nuevos e historias abiertas: toda la red)");

                int totalAtenciones = porServicio.AsEnumerable().Sum(f => Convert.ToInt32(f["Cantidad"]));
                r.Resumen.Add(Par("Atenciones en el periodo", totalAtenciones));
                r.Resumen.Add(Par("Servicios con atenciones", porServicio.Rows.Count));

                if (porServicio.Rows.Count > 0)
                {
                    r.TituloGrafico = "Atenciones por servicio";
                    r.Grafico.AddRange(porServicio.AsEnumerable()
                        .Select(f => new KeyValuePair<string, double>(f.Field<string>("Servicio"), Convert.ToDouble(f["Cantidad"]))));
                }
                return Exito(r);
            }
            catch (Exception ex) when (EsErrorDeDatos(ex))
            {
                return ResultadoReporte.Fallo(RegistroTecnico.ErrorParaUsuario("Reportes", ex));
            }
        }

        /// <summary>Registros de auditoría (SOLO Administrador).</summary>
        public ResultadoReporte Auditoria(DateTime desde, DateTime hasta, string usuario, string accion)
        {
            if (!EsAdministrador())
            {
                return ResultadoReporte.Fallo("El reporte de Auditoría es solo para el Administrador.");
            }
            string error;
            if (!ValidarRango(desde, hasta, out error)) return ResultadoReporte.Fallo(error);

            try
            {
                DataTable tabla = dao.Auditoria(desde.Date, FinExclusivo(hasta), usuario, accion);
                string filtros = TextoRango(desde, hasta) +
                    (string.IsNullOrWhiteSpace(usuario) ? "" : " · Usuario contiene: " + usuario.Trim()) +
                    (string.IsNullOrWhiteSpace(accion) ? "" : " · Acción contiene: " + accion.Trim());

                ReporteResultado r = Nuevo(TipoReporte.Auditoria, tabla, filtros);
                r.Resumen.Add(Par("Registros", tabla.Rows.Count));
                r.Resumen.Add(Par("Usuarios distintos", tabla.AsEnumerable().Select(f => f.Field<string>("Usuario")).Distinct().Count()));
                AvisoLimite(r, 500);
                return Exito(r);   // Auditoría no lleva gráfico
            }
            catch (Exception ex) when (EsErrorDeDatos(ex))
            {
                return ResultadoReporte.Fallo(RegistroTecnico.ErrorParaUsuario("Reportes", ex));
            }
        }

        // ===================== Exportar a CSV =====================

        /// <summary>
        /// Exporta el reporte a CSV para Excel en español:
        ///   - UTF-8 CON BOM: Excel reconoce así las tildes y la ñ.
        ///   - Separador ";": en configuración regional en español la coma es el
        ///     separador decimal, y Excel espera ";" entre columnas.
        ///   - Comillas: un valor con ";", comillas o saltos de línea va entre
        ///     comillas, y las comillas internas se duplican ("").
        ///   - Valores que empiezan con = + - @ se anteponen con un apóstrofo para que
        ///     Excel no los ejecute como fórmula (inyección de fórmulas).
        /// Encabezado: título, filtros, usuario y fecha. Queda en la auditoría.
        /// </summary>
        public ResultadoOperacion ExportarCsv(ReporteResultado reporte, string ruta)
        {
            if (reporte == null || reporte.Tabla == null)
            {
                return ResultadoOperacion.Error("Primero genere un reporte.");
            }
            if (string.IsNullOrWhiteSpace(ruta))
            {
                return ResultadoOperacion.Error("Elija dónde guardar el archivo.");
            }

            try
            {
                using (StreamWriter sw = new StreamWriter(ruta, false, new UTF8Encoding(true)))
                {
                    sw.WriteLine(Linea("Reporte", reporte.Titulo));
                    sw.WriteLine(Linea("Filtros", reporte.Parametros));
                    sw.WriteLine(Linea("Generado por", reporte.GeneradoPor));
                    sw.WriteLine(Linea("Fecha de generación", reporte.FechaGeneracion.ToString("dd/MM/yyyy HH:mm")));
                    foreach (KeyValuePair<string, string> par in reporte.Resumen)
                    {
                        sw.WriteLine(Linea(par.Key, par.Value));
                    }
                    sw.WriteLine();

                    DataTable t = reporte.Tabla;
                    sw.WriteLine(string.Join(";", t.Columns.Cast<DataColumn>().Select(c => Celda(c.ColumnName))));
                    foreach (DataRow fila in t.Rows)
                    {
                        sw.WriteLine(string.Join(";", fila.ItemArray.Select(v => Celda(TextoValor(v)))));
                    }
                }

                new AuditoriaNegocio().Registrar("EXPORTAR_CSV", "Reporte", null,
                    "Tipo: " + reporte.Titulo + " · Archivo: " + Path.GetFileName(ruta));
                return new ResultadoOperacion();
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                // Sin el texto técnico: una pista útil y el código para buscar el detalle.
                string codigo = RegistroTecnico.Error("Reportes - exportar CSV", ex);
                return ResultadoOperacion.Error("No se pudo guardar el archivo (¿está abierto en otro programa o la carpeta " +
                                                "no permite escribir?). Código: " + codigo + ".");
            }
        }

        /// <summary>Texto de un valor para mostrar, exportar o imprimir (fechas en dd/MM/yyyy HH:mm).</summary>
        public static string TextoValor(object valor)
        {
            if (valor == null || valor == DBNull.Value) return string.Empty;
            if (valor is DateTime) return ((DateTime)valor).ToString("dd/MM/yyyy HH:mm");
            if (valor is IFormattable) return ((IFormattable)valor).ToString(null, CultureInfo.CurrentCulture);
            return valor.ToString();
        }

        // ===================== Reglas de apoyo =====================

        // Regla 1.
        private static bool ValidarRango(DateTime desde, DateTime hasta, out string error)
        {
            error = null;
            if (desde.Date > hasta.Date)
            {
                error = "La fecha \"Desde\" no puede ser mayor que la fecha \"Hasta\".";
            }
            else if ((hasta.Date - desde.Date).TotalDays > RangoMaximoDias)
            {
                error = "El rango de fechas no puede superar los " + RangoMaximoDias + " días.";
            }
            return error == null;
        }

        /// <summary>
        /// Fin del rango como límite EXCLUSIVO: las 00:00 del día siguiente a "hasta".
        /// Las columnas DATETIME guardan la hora (p. ej. 15/11 23:35); comparar
        /// "columna &lt;= 15/11" las dejaría fuera porque 15/11 equivale a 15/11 00:00.
        /// Con "columna &lt; 16/11 00:00" entra TODO el último día, sin importar la hora
        /// que traiga el DateTimePicker.
        /// </summary>
        private static DateTime FinExclusivo(DateTime hasta)
        {
            return hasta.Date.AddDays(1);
        }

        // Devuelve el nombre del estado si es uno real de EstadoCita; si no, null (= sin filtro).
        private static string EstadoValido(string estado)
        {
            if (string.IsNullOrWhiteSpace(estado))
            {
                return null;
            }
            string limpio = estado.Trim();
            return Enum.GetNames(typeof(EstadoCita)).Contains(limpio) ? limpio : null;
        }

        // Regla 2: Admision → su posta (se ignora la pedida); Administrador → la pedida (null = todas).
        private static bool ResolverPosta(int? pedida, out int? efectiva, out string error)
        {
            efectiva = null;
            error = null;
            if (!Sesion.HaySesion)
            {
                error = "No hay una sesión iniciada.";
                return false;
            }
            if (EsAdministrador())
            {
                efectiva = pedida;
                return true;
            }
            if (!Sesion.UsuarioActual.IdPosta.HasValue)
            {
                error = "Su usuario no tiene una posta asignada.";
                return false;
            }
            efectiva = Sesion.UsuarioActual.IdPosta.Value;
            return true;
        }

        /// <summary>
        /// Regla 4: si la tabla trae DniCifrado, se agrega "DNI" (descifrado y
        /// ENMASCARADO) en la misma posición y se quita la columna cifrada.
        /// El DNI completo nunca llega a la pantalla, al CSV ni a la impresión.
        /// </summary>
        private static void ReemplazarDniCifrado(DataTable tabla)
        {
            if (!tabla.Columns.Contains("DniCifrado"))
            {
                return;
            }
            int posicion = tabla.Columns["DniCifrado"].Ordinal;
            DataColumn dni = tabla.Columns.Add("DNI", typeof(string));

            foreach (DataRow fila in tabla.Rows)
            {
                object cifrado = fila["DniCifrado"];
                string texto;
                if (cifrado == DBNull.Value)
                {
                    texto = string.Empty;
                }
                else
                {
                    try
                    {
                        texto = Mascara.Dni(CifradoAes.Descifrar((byte[])cifrado));
                    }
                    catch (CryptographicException)
                    {
                        texto = "(no legible)";
                    }
                }
                fila[dni] = texto;
            }

            tabla.Columns.Remove("DniCifrado");
            dni.SetOrdinal(posicion);
        }

        private static ReporteResultado Nuevo(TipoReporte tipo, DataTable tabla, string parametros)
        {
            return new ReporteResultado
            {
                Tipo = tipo,
                Titulo = Nombre(tipo),
                Parametros = parametros,
                Tabla = tabla,
                GeneradoPor = Sesion.UsuarioActual.NombreUsuario + " (" + Sesion.UsuarioActual.Rol + ")",
                FechaGeneracion = DateTime.Now
            };
        }

        private static ResultadoReporte Exito(ReporteResultado r)
        {
            return new ResultadoReporte { Reporte = r };
        }

        private static void AvisoLimite(ReporteResultado r, int limite)
        {
            if (r.Tabla.Rows.Count >= limite)
            {
                r.Resumen.Add(new KeyValuePair<string, string>("Aviso", "se muestran solo las primeras " + limite + " filas"));
            }
        }

        private static KeyValuePair<string, string> Par(string texto, int valor)
        {
            return new KeyValuePair<string, string>(texto, valor.ToString());
        }

        private static string TextoRango(DateTime desde, DateTime hasta)
        {
            // Se muestra el "hasta" que eligió el usuario (inclusivo), no el límite exclusivo.
            return "Desde: " + desde.ToString("dd/MM/yyyy") + " · Hasta: " + hasta.ToString("dd/MM/yyyy");
        }

        private string NombrePosta(int? idPosta)
        {
            if (!idPosta.HasValue) return "Todas";
            PostaItem p = catalogoDao.Postas().FirstOrDefault(x => x.IdPosta == idPosta.Value);
            return p != null ? p.Nombre : "Posta " + idPosta.Value;
        }

        private string NombreConsultorio(int? idConsultorio)
        {
            if (!idConsultorio.HasValue) return "Todos";
            ConsultorioItem c = catalogoDao.ObtenerConsultorio(idConsultorio.Value);
            return c != null ? c.Nombre : "Consultorio " + idConsultorio.Value;
        }

        private static string Linea(string etiqueta, string valor)
        {
            return Celda(etiqueta) + ";" + Celda(valor);
        }

        // Una celda CSV con el escape correcto.
        private static string Celda(string valor)
        {
            string v = valor ?? string.Empty;
            if (v.Length > 0 && "=+-@".IndexOf(v[0]) >= 0)
            {
                v = "'" + v;   // Evita que Excel lo interprete como fórmula
            }
            bool requiereComillas = v.IndexOfAny(new[] { ';', '"', '\r', '\n' }) >= 0 || v != v.Trim();
            return requiereComillas ? "\"" + v.Replace("\"", "\"\"") + "\"" : v;
        }

        private static bool EsErrorDeDatos(Exception ex)
        {
            return ex is SqlException || ex is InvalidOperationException;
        }
    }
}
