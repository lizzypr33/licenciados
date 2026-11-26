using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Security.Cryptography;
using System.Text;

namespace SigepostaAdmision.Seguridad
{
    /// <summary>
    /// Administra las DOS claves secretas del sistema (32 bytes = 256 bits cada una):
    ///   - ClaveAes:  para cifrar y descifrar datos (DNI, diagnóstico) con AES-256.
    ///   - ClaveHmac: para calcular el HMAC-SHA256 del DNI (búsquedas sin descifrar).
    /// Se usan claves DISTINTAS para cada propósito: si una se viera comprometida,
    /// la otra sigue protegiendo su parte (principio de separación de claves).
    ///
    /// ¿Dónde se guardan? Nunca en el código fuente ni en la base de datos (quien
    /// robe la base no debe tener también la llave). Se guardan en el archivo
    /// "claves.dat" dentro del ALMACENAMIENTO AISLADO (IsolatedStorage) del usuario
    /// de Windows, y además el contenido va protegido con DPAPI:
    ///   - DPAPI (ProtectedData, DataProtectionScope.CurrentUser) cifra el archivo
    ///     con una clave derivada de la cuenta de Windows del usuario. Otro usuario
    ///     del mismo equipo, o alguien que copie el archivo a otra PC, no puede leerlo.
    ///
    /// ¿Por qué GetUserStoreForAssembly y no GetUserStoreForApplication?
    /// GetUserStoreForApplication exige que la aplicación tenga "identidad de
    /// aplicación", algo que solo tienen las aplicaciones instaladas con ClickOnce;
    /// en un .exe de escritorio común lanza IsolatedStorageException. El almacén por
    /// ensamblado funciona en cualquier aplicación de escritorio.
    ///
    /// IMPORTANTE (limitación que hay que conocer): el almacén depende del usuario de
    /// Windows y de la identidad del ensamblado (sin firma de nombre seguro, de la
    /// RUTA del .exe). Si se ejecuta con otro usuario, en otra PC o desde otra
    /// carpeta (por ejemplo bin\Release en lugar de bin\Debug), se generarán claves
    /// NUEVAS que no descifran los datos ya guardados. MigracionSeguridad verifica
    /// esto al iniciar y detiene la aplicación con un mensaje claro.
    /// </summary>
    public static class GestorClaves
    {
        private const string NombreArchivo = "claves.dat";
        private const int TamanoClave = 32;   // 32 bytes = 256 bits

        // "Entropía" adicional para DPAPI: otro programa del mismo usuario que use
        // DPAPI sin este valor no podrá descifrar el archivo.
        private static readonly byte[] Entropia = Encoding.UTF8.GetBytes("SIGEPOSTA-Admision-Claves-v1");

        // Copia de seguridad del archivo anterior al importar claves.
        private const string NombreArchivoAnterior = "claves.anterior.dat";

        // Las claves se cargan UNA sola vez y quedan en memoria. El "lock" hace que
        // la carga (y un reemplazo por ImportarBytes) sea segura entre hilos (la
        // cola de atención y los respaldos trabajan en segundo plano).
        private static readonly object candado = new object();
        private static byte[] claves;   // null hasta la primera carga

        private static byte[] Claves
        {
            get
            {
                lock (candado)
                {
                    if (claves == null)
                    {
                        claves = CargarOCrear();
                    }
                    return claves;
                }
            }
        }

        /// <summary>
        /// Copia de las dos claves (64 bytes: AES + HMAC) para exportarlas a un
        /// archivo protegido con contraseña (RespaldoNegocio.ExportarClaves).
        /// Así los datos cifrados se pueden leer en otro equipo o usuario.
        /// </summary>
        public static byte[] ExportarBytes()
        {
            return (byte[])Claves.Clone();
        }

        /// <summary>
        /// Reemplaza las claves por las indicadas (64 bytes) y las guarda protegidas
        /// con DPAPI. El archivo anterior se conserva como "claves.anterior.dat".
        /// ATENCIÓN: quien llame debe verificar ANTES que las nuevas claves descifran
        /// los datos existentes (RespaldoNegocio.ImportarClaves lo hace).
        /// </summary>
        public static void ImportarBytes(byte[] nuevas)
        {
            if (nuevas == null || nuevas.Length != TamanoClave * 2)
            {
                throw new ArgumentException("Las claves importadas deben medir " + (TamanoClave * 2) + " bytes.", "nuevas");
            }

            lock (candado)
            {
                byte[] protegidas = ProtectedData.Protect(nuevas, Entropia, DataProtectionScope.CurrentUser);
                using (IsolatedStorageFile almacen = IsolatedStorageFile.GetUserStoreForAssembly())
                {
                    if (almacen.FileExists(NombreArchivo))
                    {
                        if (almacen.FileExists(NombreArchivoAnterior))
                        {
                            almacen.DeleteFile(NombreArchivoAnterior);
                        }
                        almacen.MoveFile(NombreArchivo, NombreArchivoAnterior);
                    }
                    using (IsolatedStorageFileStream fs = new IsolatedStorageFileStream(NombreArchivo, FileMode.Create, FileAccess.Write, almacen))
                    {
                        fs.Write(protegidas, 0, protegidas.Length);
                    }
                }
                claves = (byte[])nuevas.Clone();
            }
        }

        /// <summary>Clave AES de 256 bits (se devuelve una copia para que nadie altere la original).</summary>
        public static byte[] ClaveAes
        {
            get { return Parte(0); }
        }

        /// <summary>Clave HMAC-SHA256 de 256 bits (copia).</summary>
        public static byte[] ClaveHmac
        {
            get { return Parte(TamanoClave); }
        }

        /// <summary>
        /// Huella corta (8 caracteres) de las claves, para diagnosticar si dos
        /// ejecuciones usan las mismas claves SIN mostrar las claves.
        /// </summary>
        public static string Huella
        {
            get
            {
                using (SHA256 sha = SHA256.Create())
                {
                    return BitConverter.ToString(sha.ComputeHash(Claves), 0, 4).Replace("-", "");
                }
            }
        }

        private static byte[] Parte(int inicio)
        {
            byte[] copia = new byte[TamanoClave];
            Buffer.BlockCopy(Claves, inicio, copia, 0, TamanoClave);
            return copia;
        }

        /// <summary>
        /// Lee "claves.dat" si existe (descifrándolo con DPAPI); si no existe,
        /// genera las dos claves al azar y las guarda protegidas.
        /// </summary>
        private static byte[] CargarOCrear()
        {
            using (IsolatedStorageFile almacen = IsolatedStorageFile.GetUserStoreForAssembly())
            {
                if (almacen.FileExists(NombreArchivo))
                {
                    byte[] protegido;
                    using (IsolatedStorageFileStream fs = new IsolatedStorageFileStream(NombreArchivo, FileMode.Open, FileAccess.Read, almacen))
                    using (MemoryStream ms = new MemoryStream())
                    {
                        fs.CopyTo(ms);
                        protegido = ms.ToArray();
                    }

                    try
                    {
                        byte[] datos = ProtectedData.Unprotect(protegido, Entropia, DataProtectionScope.CurrentUser);
                        if (datos.Length != TamanoClave * 2)
                        {
                            throw new CryptographicException("Tamaño de claves inesperado.");
                        }
                        return datos;
                    }
                    catch (CryptographicException ex)
                    {
                        // Mensaje pensado para el usuario (se muestra tal cual al iniciar).
                        throw new Entidades.ReglaNegocioException(
                            "No se pudieron leer las claves de cifrado (el archivo está dañado o pertenece a otro usuario de Windows).", ex);
                    }
                }

                // Primera vez: dos claves aleatorias con un generador CRIPTOGRÁFICO
                // (RNGCryptoServiceProvider), no con Random, que es predecible.
                byte[] nuevas = new byte[TamanoClave * 2];
                using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(nuevas);
                }

                byte[] cifradas = ProtectedData.Protect(nuevas, Entropia, DataProtectionScope.CurrentUser);
                using (IsolatedStorageFileStream fs = new IsolatedStorageFileStream(NombreArchivo, FileMode.CreateNew, FileAccess.Write, almacen))
                {
                    fs.Write(cifradas, 0, cifradas.Length);
                }
                return nuevas;
            }
        }
    }
}
