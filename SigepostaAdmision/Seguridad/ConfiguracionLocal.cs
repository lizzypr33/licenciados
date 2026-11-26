using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;

namespace SigepostaAdmision.Seguridad
{
    /// <summary>
    /// Preferencias locales del usuario de Windows (por ejemplo, el último nombre
    /// de usuario) en un archivo "config.txt" dentro de IsolatedStorage.
    ///
    /// ¿Por qué IsolatedStorage? Cada usuario de Windows tiene su propio almacén,
    /// separado del de otros usuarios y de otras aplicaciones, y no requiere
    /// permisos de escritura en la carpeta del programa (Archivos de programa).
    /// (Se usa GetUserStoreForAssembly por la misma razón explicada en GestorClaves.)
    ///
    /// REGLA: aquí solo se guardan datos NO sensibles. NUNCA contraseñas: el
    /// archivo es texto plano.
    ///
    /// Formato: una línea "clave=valor" por preferencia; el valor se escapa para
    /// que saltos de línea o "=" no rompan el archivo. Si algo falla (por ejemplo,
    /// el almacén no está disponible) se ignora: una preferencia no debe impedir
    /// usar el sistema.
    /// </summary>
    public static class ConfiguracionLocal
    {
        private const string NombreArchivo = "config.txt";
        private static readonly object candado = new object();

        /// <summary>Guarda (o reemplaza) un valor. Devuelve false si no se pudo guardar.</summary>
        public static bool Guardar(string clave, string valor)
        {
            if (string.IsNullOrWhiteSpace(clave) || clave.Contains("=") || clave.Contains("\n"))
            {
                throw new ArgumentException("La clave de configuración no es válida.", "clave");
            }

            lock (candado)
            {
                try
                {
                    Dictionary<string, string> datos = LeerTodo();
                    datos[clave.Trim()] = valor ?? string.Empty;

                    StringBuilder sb = new StringBuilder();
                    foreach (KeyValuePair<string, string> par in datos)
                    {
                        sb.Append(par.Key).Append('=').Append(Uri.EscapeDataString(par.Value)).AppendLine();
                    }

                    using (IsolatedStorageFile almacen = IsolatedStorageFile.GetUserStoreForAssembly())
                    using (IsolatedStorageFileStream fs = new IsolatedStorageFileStream(NombreArchivo, FileMode.Create, FileAccess.Write, almacen))
                    using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                    {
                        sw.Write(sb.ToString());
                    }
                    return true;
                }
                catch (Exception ex) when (ex is IOException || ex is IsolatedStorageException || ex is UnauthorizedAccessException)
                {
                    return false;
                }
            }
        }

        /// <summary>Lee un valor; si no existe o no se puede leer, devuelve porDefecto.</summary>
        public static string Leer(string clave, string porDefecto)
        {
            lock (candado)
            {
                try
                {
                    string valor;
                    return LeerTodo().TryGetValue((clave ?? string.Empty).Trim(), out valor) ? valor : porDefecto;
                }
                catch (Exception ex) when (ex is IOException || ex is IsolatedStorageException || ex is UnauthorizedAccessException)
                {
                    return porDefecto;
                }
            }
        }

        private static Dictionary<string, string> LeerTodo()
        {
            Dictionary<string, string> datos = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            using (IsolatedStorageFile almacen = IsolatedStorageFile.GetUserStoreForAssembly())
            {
                if (!almacen.FileExists(NombreArchivo))
                {
                    return datos;
                }
                using (IsolatedStorageFileStream fs = new IsolatedStorageFileStream(NombreArchivo, FileMode.Open, FileAccess.Read, almacen))
                using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                {
                    string linea;
                    while ((linea = sr.ReadLine()) != null)
                    {
                        int igual = linea.IndexOf('=');
                        if (igual > 0)
                        {
                            datos[linea.Substring(0, igual)] = Uri.UnescapeDataString(linea.Substring(igual + 1));
                        }
                    }
                }
            }
            return datos;
        }
    }
}
