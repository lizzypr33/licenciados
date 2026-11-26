using SigepostaAdmision.Entidades;

namespace SigepostaAdmision.Negocio
{
    /// <summary>
    /// Módulos (opciones del menú) que existen en el sistema.
    /// </summary>
    public enum Modulo
    {
        Pacientes,
        Historias,
        Citas,
        Cola,
        Reportes,
        Respaldo,
        Usuarios
    }

    /// <summary>
    /// Reglas de autorización: qué rol puede entrar a qué módulo.
    ///
    /// ¿Por qué en la capa de Negocio y no en el formulario? Porque es una REGLA
    /// DEL NEGOCIO, no un detalle visual. Si mañana se agrega otro formulario
    /// o se cambia el menú, la regla sigue en un solo lugar y todos la consultan.
    /// El formulario solo la usa para ocultar botones y para bloquear el acceso.
    ///
    /// Reglas:
    ///   - Admision:      Pacientes, Historias, Citas, Cola y Reportes.
    ///   - Administrador: todos los módulos (Respaldo y Usuarios son exclusivos suyos).
    /// </summary>
    public static class Permisos
    {
        /// <summary>
        /// Indica si el rol indicado puede acceder al módulo.
        /// Cualquier combinación no prevista se niega (principio de "denegar por defecto").
        /// </summary>
        /// <summary>
        /// ¿El rol puede LEER los diagnósticos? Solo el Administrador.
        /// El diagnóstico es un dato clínico sensible: el rol Admision puede
        /// ESCRIBIRLO al registrar una atención (lo dicta el personal de salud),
        /// pero no consultarlo después (principio de mínimo privilegio).
        /// </summary>
        public static bool PuedeVerDiagnostico(RolUsuario rol)
        {
            return rol == RolUsuario.Administrador;
        }

        public static bool TieneAcceso(RolUsuario rol, Modulo modulo)
        {
            switch (rol)
            {
                case RolUsuario.Administrador:
                    return true;

                case RolUsuario.Admision:
                    return modulo == Modulo.Pacientes
                        || modulo == Modulo.Historias
                        || modulo == Modulo.Citas
                        || modulo == Modulo.Cola
                        || modulo == Modulo.Reportes;

                default:
                    return false;
            }
        }
    }
}
