using System;
using System.IO;
using System.Linq;
using System.Text;

namespace SigepostaAdmision.Negocio
{
    /// <summary>
    /// Registro técnico de errores (para el soporte, no para el usuario).
    ///
    /// ¿Por qué? Mostrar al usuario el texto de una excepción (por ejemplo, de
    /// SQL Server) puede revelar detalles internos: nombre del servidor, tablas,
    /// rutas. Al usuario se le muestra un mensaje genérico con un CÓDIGO
    /// (ERR-AAAAMMDDHHMMSS), y el detalle completo (ex.ToString()) se guarda aquí,
    /// donde el administrador puede buscarlo por ese código.
    ///
    /// Ubicación: %LocalAppData%\SigepostaAdmision\logs\errores-AAAAMMDD.log
    /// (carpeta del usuario, fuera de la carpeta del programa y del proyecto).
    /// Se conservan solo los últimos 14 archivos.
    ///
    /// REGLA: este método NUNCA lanza excepciones (si no puede escribir, igual
    /// devuelve el código): un fallo del registro no debe empeorar el error original.
    /// </summary>
    public static class RegistroTecnico
    {
        private const int ArchivosConservados = 14;
        private static readonly object candado = new object();

        /// <summary>Carpeta de los archivos de registro.</summary>
        public static string CarpetaLogs
        {
            get
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "SigepostaAdmision", "logs");
            }
        }

        /// <summary>
        /// Escribe el error con fecha, hora, módulo y usuario, y devuelve su código
        /// "ERR-AAAAMMDDHHMMSS" para mostrárselo al usuario.
        /// </summary>
        public static string Error(string modulo, Exception ex)
        {
            DateTime ahora = DateTime.Now;
            string codigo = "ERR-" + ahora.ToString("yyyyMMddHHmmss");
            try
            {
                lock (candado)
                {
                    Directory.CreateDirectory(CarpetaLogs);
                    string ruta = Path.Combine(CarpetaLogs, "errores-" + ahora.ToString("yyyyMMdd") + ".log");
                    string usuario = Sesion.HaySesion ? Sesion.UsuarioActual.NombreUsuario : "(sin sesión)";

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("[" + ahora.ToString("yyyy-MM-dd HH:mm:ss") + "] " + codigo +
                                  " | Módulo: " + (modulo ?? "?") + " | Usuario: " + usuario);
                    sb.AppendLine(ex != null ? ex.ToString() : "(sin excepción)");
                    sb.AppendLine(new string('-', 80));
                    File.AppendAllText(ruta, sb.ToString(), Encoding.UTF8);

                    ConservarUltimos();
                }
            }
            catch
            {
                // Se ignora a propósito: el registro técnico nunca debe lanzar excepciones.
            }
            return codigo;
        }

        /// <summary>Mensaje genérico para el usuario, con el código del error.</summary>
        public static string MensajeUsuario(string codigo)
        {
            return "Ocurrió un error inesperado. Código: " + codigo + ". Si persiste, avise al administrador.";
        }

        /// <summary>Atajo: registra el error y devuelve directamente el mensaje genérico.</summary>
        public static string ErrorParaUsuario(string modulo, Exception ex)
        {
            return MensajeUsuario(Error(modulo, ex));
        }

        // Borra los archivos "errores-*.log" más antiguos, dejando los últimos 14.
        private static void ConservarUltimos()
        {
            FileInfo[] sobrantes = new DirectoryInfo(CarpetaLogs).GetFiles("errores-*.log")
                .OrderByDescending(f => f.Name)
                .Skip(ArchivosConservados)
                .ToArray();
            foreach (FileInfo f in sobrantes)
            {
                try
                {
                    f.Delete();
                }
                catch
                {
                    // Si un archivo antiguo no se puede borrar, se intenta la próxima vez.
                }
            }
        }
    }
}
