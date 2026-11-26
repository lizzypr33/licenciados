using System;
using System.Collections.Generic;

namespace SigepostaAdmision.Entidades
{
    /// <summary>
    /// "Foto" (snapshot) de la cola en un momento dado: todo lo que la pantalla
    /// necesita mostrar, armado en segundo plano de una sola vez.
    ///
    /// ¿Por qué un objeto así? El hilo en segundo plano NO puede tocar los
    /// controles. Entonces prepara este paquete de datos y lo entrega al hilo de
    /// la interfaz (con ReportProgress), que es el único que dibuja.
    /// </summary>
    public class ColaSnapshot
    {
        public ColaSnapshot()
        {
            Cola = new List<ItemCola>();
            Pendientes = new List<CitaResumen>();
            Recordatorios = new List<string>();
        }

        /// <summary>Pacientes que llegaron (EnCola), en orden de llegada.</summary>
        public List<ItemCola> Cola { get; set; }

        /// <summary>Citas Pendientes del día (aún no llegan), ordenadas por hora.</summary>
        public List<CitaResumen> Pendientes { get; set; }

        /// <summary>
        /// Recordatorios SIMULADOS: texto de las citas Pendientes de la próxima hora.
        /// Solo se listan en pantalla; no se envía ningún mensaje.
        /// </summary>
        public List<string> Recordatorios { get; set; }

        /// <summary>Momento en que se generó la foto.</summary>
        public DateTime Generado { get; set; }
    }
}
