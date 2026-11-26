using System;
using SigepostaAdmision.Entidades;

namespace SigepostaAdmision.Negocio
{
    /// <summary>
    /// Fábrica de la entidad Cita a partir de una CitaResumen leída de la base.
    ///
    /// Antes esta reconstrucción estaba repetida en cuatro lugares (CitaNegocio
    /// dos veces, ColaNegocio y AtencionNegocio), y dos de ellos olvidaban las horas
    /// de llegada y de llamado. Centralizarla aquí evita esas diferencias: todas
    /// las reglas de dominio se aplican sobre una Cita reconstruida igual.
    /// </summary>
    public static class CitaFabrica
    {
        /// <summary>Cita con su estado real y sus horas de llegada y de llamado.</summary>
        public static Cita DesdeResumen(CitaResumen c)
        {
            if (c == null) throw new ArgumentNullException("c");
            return new Cita(c.IdCita, c.Fecha, c.Hora, c.Consultorio, c.Turno, c.Estado,
                            c.HoraLlegada, c.HoraLlamado);
        }
    }
}
