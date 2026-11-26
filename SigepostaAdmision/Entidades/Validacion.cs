using System;

namespace SigepostaAdmision.Entidades
{
    /// <summary>
    /// Clase auxiliar (estática e interna) con validaciones que se repiten en
    /// varias entidades. Se centralizan aquí para no duplicar código en cada
    /// "set" (principio DRY). Es "internal" porque solo la usan las entidades.
    /// </summary>
    internal static class Validacion
    {
        /// <summary>
        /// Verifica que un texto obligatorio no sea nulo ni vacío.
        /// Devuelve el texto sin espacios al inicio ni al final.
        /// </summary>
        public static string TextoObligatorio(string valor, string nombreCampo)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                throw new ArgumentException("El campo '" + nombreCampo + "' es obligatorio y no puede estar vacío.");
            }
            return valor.Trim();
        }

        /// <summary>
        /// Verifica que un objeto obligatorio (por ejemplo un Paciente) no sea nulo.
        /// </summary>
        public static T ObjetoObligatorio<T>(T valor, string nombreCampo) where T : class
        {
            if (valor == null)
            {
                throw new ArgumentException("El campo '" + nombreCampo + "' es obligatorio.");
            }
            return valor;
        }
    }
}
