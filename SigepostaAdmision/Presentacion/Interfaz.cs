using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SigepostaAdmision.Entidades;
using SigepostaAdmision.Negocio;

namespace SigepostaAdmision.Presentacion
{
    /// <summary>
    /// Utilidades visuales que comparten varios formularios (solo apariencia,
    /// sin reglas de negocio). Se juntan aquí para no repetir código.
    /// </summary>
    internal static class Interfaz
    {
        // .NET Framework 4.7.2 no tiene TextBox.PlaceholderText. Se usa el mensaje
        // de Windows EM_SETCUEBANNER, que muestra un texto gris mientras la caja está vacía.
        private const int EM_SETCUEBANNER = 0x1501;

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, string lParam);

        /// <summary>Muestra un texto guía gris dentro de la caja mientras está vacía.</summary>
        public static void PonerTextoGuia(TextBox caja, string texto)
        {
            // wParam = 1: el texto guía se mantiene visible aunque la caja tenga el foco.
            SendMessage(caja.Handle, EM_SETCUEBANNER, (IntPtr)1, texto);
        }

        /// <summary>
        /// Mensaje estándar cuando falla una consulta. Las excepciones de reglas de
        /// negocio (mensajes pensados para el usuario) se muestran tal cual; para el
        /// resto se muestra un mensaje genérico con código y el detalle técnico va a
        /// RegistroTecnico (nunca se enseña ex.Message al usuario).
        /// </summary>
        public static void MostrarErrorBaseDatos(Exception ex)
        {
            string texto = ex is ReglaNegocioException || ex is RegistroDuplicadoException
                ? ex.Message
                : "No se pudo consultar la base de datos.\n\n" + RegistroTecnico.ErrorParaUsuario("Presentación", ex);
            MessageBox.Show(texto, "SIGEPOSTA", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
