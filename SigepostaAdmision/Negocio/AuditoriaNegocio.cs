using System;
using System.Data.SqlClient;
using SigepostaAdmision.Datos;

namespace SigepostaAdmision.Negocio
{
    /// <summary>
    /// Utilidad reutilizable para registrar acciones en la tabla Auditoria con el
    /// usuario de la sesión. Pensada para operaciones simples que se agregarán
    /// más adelante (por ejemplo, cambios de estado de historias o respaldos).
    ///
    /// Decisión: si la auditoría falla, NO se interrumpe la operación principal
    /// (devuelve false). Cuando la auditoría debe ser obligatoria y atómica con
    /// la operación (como al registrar una atención), se hace dentro de la misma
    /// transacción en el DAO correspondiente.
    /// </summary>
    public class AuditoriaNegocio
    {
        // Longitudes de las columnas de dbo.Auditoria.
        private const int MaxAccion = 40;
        private const int MaxTabla = 40;
        private const int MaxDetalle = 200;

        private readonly AuditoriaDAO dao = new AuditoriaDAO();

        /// <summary>
        /// Registra una acción con el usuario de la sesión (o sin usuario si no hay
        /// sesión). Acción y tabla son obligatorias (máx. 40); el detalle se recorta
        /// a 200 caracteres. Devuelve true si se guardó.
        ///
        /// La auditoría se llama DESPUÉS de que la operación principal tuvo éxito.
        /// Si la auditoría falla (base caída, dato inválido...), NO rompe la
        /// operación: devuelve false y el detalle queda en RegistroTecnico.
        /// </summary>
        public bool Registrar(string accion, string tabla, int? idRegistro, string detalle)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accion) || accion.Trim().Length > MaxAccion)
                {
                    throw new ArgumentException("La acción es obligatoria y tiene como máximo " + MaxAccion + " caracteres.");
                }
                if (string.IsNullOrWhiteSpace(tabla) || tabla.Trim().Length > MaxTabla)
                {
                    throw new ArgumentException("La tabla es obligatoria y tiene como máximo " + MaxTabla + " caracteres.");
                }

                string textoDetalle = detalle == null ? null : detalle.Trim();
                if (textoDetalle != null && textoDetalle.Length > MaxDetalle)
                {
                    textoDetalle = textoDetalle.Substring(0, MaxDetalle);
                }

                int? idUsuario = Sesion.HaySesion ? Sesion.UsuarioActual.IdUsuario : (int?)null;
                dao.Registrar(idUsuario, accion.Trim().ToUpperInvariant(), tabla.Trim(), idRegistro, textoDetalle);
                return true;
            }
            catch (Exception ex)
            {
                RegistroTecnico.Error("Auditoría (" + accion + ")", ex);
                return false;
            }
        }
    }
}
