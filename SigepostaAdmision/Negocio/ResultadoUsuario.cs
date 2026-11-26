using System.Collections.Generic;
using SigepostaAdmision.Entidades;

namespace SigepostaAdmision.Negocio
{
    /// <summary>
    /// Resultado de una operación del módulo Usuarios. HEREDA de ResultadoOperacion
    /// (Exito, Mensajes, IdGenerado) y agrega:
    ///   - ClaveTemporal: la contraseña generada al crear o restablecer. Se entrega
    ///     UNA sola vez, solo en memoria, para mostrarla al administrador; nunca se
    ///     guarda en la base (allí va solo su hash) ni en la auditoría.
    ///   - Usuarios: la lista, cuando la operación es un listado.
    /// </summary>
    public class ResultadoUsuario : ResultadoOperacion
    {
        public string ClaveTemporal { get; set; }
        public List<UsuarioResumen> Usuarios { get; set; }

        public static ResultadoUsuario Fallo(string mensaje)
        {
            ResultadoUsuario r = new ResultadoUsuario();
            r.AgregarError(mensaje);
            return r;
        }
    }
}
