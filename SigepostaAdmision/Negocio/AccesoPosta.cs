using System;
using SigepostaAdmision.Entidades;

namespace SigepostaAdmision.Negocio
{
    /// <summary>
    /// Regla de acceso por posta, compartida por CitaNegocio y ColaNegocio
    /// (antes estaba solo en CitaNegocio; se separa para no repetirla):
    ///   - Un usuario Admision solo ve y opera con datos de SU posta.
    ///   - El Administrador ve todas las postas.
    ///   - Un usuario Admision sin posta asignada no ve nada.
    /// Solo lee Sesion.UsuarioActual, así que se puede usar también desde un
    /// hilo en segundo plano (la cola de atención).
    /// </summary>
    internal static class AccesoPosta
    {
        /// <summary>null = Administrador (sin filtro); valor = posta del usuario Admision.</summary>
        public static int? PostaDelUsuario()
        {
            Usuario u = Sesion.UsuarioActual;
            if (u == null)
            {
                throw new InvalidOperationException("No hay una sesión iniciada.");
            }
            return u.Rol == RolUsuario.Administrador ? null : u.IdPosta;
        }

        /// <summary>true si no hay sesión o si es un usuario Admision sin posta asignada.</summary>
        public static bool SinPostaAsignada()
        {
            Usuario u = Sesion.UsuarioActual;
            return u == null || (u.Rol != RolUsuario.Administrador && !u.IdPosta.HasValue);
        }

        /// <summary>¿El usuario puede ver y operar con datos de esa posta?</summary>
        public static bool PuedeVerPosta(int idPosta)
        {
            Usuario u = Sesion.UsuarioActual;
            if (u == null) return false;
            if (u.Rol == RolUsuario.Administrador) return true;
            return u.IdPosta.HasValue && u.IdPosta.Value == idPosta;
        }
    }
}
