using System;
using System.Security.Cryptography;
using SigepostaAdmision.Datos;
using SigepostaAdmision.Entidades;
using SigepostaAdmision.Seguridad;

namespace SigepostaAdmision.Negocio
{
    /// <summary>
    /// Migración de seguridad que se ejecuta al iniciar (Program.cs):
    ///   1) Verifica que las claves de cifrado de este equipo/usuario correspondan
    ///      a los datos ya cifrados en la base (si no, detiene la aplicación ANTES
    ///      de escribir nada con claves equivocadas).
    ///   2) Si la columna Paciente.Dni todavía existe, migra los DNI en texto plano
    ///      a DniCifrado (AES-256) + DniHash (HMAC-SHA256) y vacía la columna Dni.
    /// Es idempotente: ejecutarla varias veces no repite el trabajo.
    /// </summary>
    public static class MigracionSeguridad
    {
        /// <summary>Devuelve cuántos pacientes migró (0 si no había nada que migrar o la columna ya no existe).</summary>
        public static int MigrarDniPacientes()
        {
            MigracionDAO dao = new MigracionDAO();

            // Se cargan las claves YA (aunque no haya datos cifrados): si claves.dat
            // no se puede leer, el problema aparece al iniciar y no a mitad del trabajo.
            string huella = GestorClaves.Huella;

            VerificarClaves(dao);

            if (!dao.ExisteColumnaDni())
            {
                return 0;   // La columna Dni ya fue eliminada: no hay nada que migrar
            }
            return dao.MigrarDniPacientes();
        }

        /// <summary>
        /// Comprueba con un paciente ya cifrado que la clave AES lo descifra y que la
        /// clave HMAC produce la misma huella guardada. Si no coincide, estas claves
        /// NO son las que cifraron la base (otro usuario de Windows, otra PC u otra
        /// carpeta del programa): continuar crearía datos que nadie podría leer.
        /// </summary>
        private static void VerificarClaves(MigracionDAO dao)
        {
            Tuple<byte[], string> muestra = dao.ObtenerMuestraCifrada();
            if (muestra == null)
            {
                return;   // Aún no hay datos cifrados: cualquier clave sirve
            }

            bool coinciden;
            try
            {
                string dni = CifradoAes.Descifrar(muestra.Item1);
                coinciden = string.Equals(HashDni.Calcular(dni), muestra.Item2.Trim(), StringComparison.OrdinalIgnoreCase);
            }
            catch (CryptographicException)
            {
                coinciden = false;
            }

            if (!coinciden)
            {
                // Mensaje pensado para el usuario: Program.cs lo muestra tal cual.
                throw new ReglaNegocioException(
                    "Las claves de cifrado de este equipo no corresponden a los datos de la base " +
                    "(huella local " + GestorClaves.Huella + "). Ejecute SIGEPOSTA con el mismo usuario de " +
                    "Windows, en el mismo equipo y desde la misma carpeta en que se cifraron los datos.");
            }
        }
    }
}
