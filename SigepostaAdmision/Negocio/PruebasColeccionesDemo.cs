using System;
using System.Collections.Generic;
using System.Diagnostics;
using SigepostaAdmision.Entidades;

namespace SigepostaAdmision.Negocio
{
    /// <summary>
    /// SOLO PARA LA SUSTENTACIÓN: demostración de la colección propia ColaAtencion
    /// y de sus dos enumeradores. No está conectada a la interfaz y no se ejecuta
    /// sola; para verla, se llama a DemoConsola() (por ejemplo, desde la ventana
    /// "Inmediato" de Visual Studio en modo Debug) y se revisa el panel "Salida".
    ///
    /// Nota: Debug.WriteLine solo escribe cuando se compila en configuración Debug.
    /// </summary>
    public static class PruebasColeccionesDemo
    {
        public static void DemoConsola()
        {
            // 1) Lista pequeña de ejemplo, DESORDENADA a propósito y con varios estados.
            //    La cola se ordena por HORA DE LLEGADA (y luego por turno), no por la hora de la cita.
            List<CitaResumen> citas = new List<CitaResumen>
            {
                Ejemplo(5, "09:00", "Quispe Mamani, Ana",     EstadoCita.EnCola,    "08:05"),  // citada tarde, llegó primero
                Ejemplo(1, "08:00", "Huamán Flores, Luis",    EstadoCita.Atendida,  "07:50"),
                Ejemplo(3, "08:40", "Rojas Vega, Carmen",     EstadoCita.EnCola,    "08:30"),
                Ejemplo(4, "08:40", "Ccori Paz, Jorge",       EstadoCita.EnCola,    "08:30"),  // misma llegada: decide el turno
                Ejemplo(2, "08:20", "Salas Ríos, Rosa",       EstadoCita.Cancelada, null),
                Ejemplo(6, "09:20", "Torres León, Miguel",    EstadoCita.Pendiente, null)
            };

            ColaAtencion cola = new ColaAtencion(citas);
            Debug.WriteLine("=== DEMO ColaAtencion (" + citas.Count + " citas, " + cola.Cantidad + " en cola) ===");

            // 2) foreach sobre la colección: usa GetEnumerator() → ColaEnumerator (escrito a mano).
            Debug.WriteLine("-- (b) foreach con el enumerador manual (ColaEnumerator):");
            foreach (CitaResumen c in cola)
            {
                Debug.WriteLine("   llegó " + c.HoraLlegada.Value.ToString("HH:mm") + "  cita " + c.HoraTexto +
                                "  turno " + c.Turno + "  " + c.NombrePaciente);
            }

            // 3) El mismo recorrido con el método que usa "yield return".
            Debug.WriteLine("-- (a) foreach con ObtenerColaAtencion() (yield return):");
            foreach (CitaResumen c in cola.ObtenerColaAtencion())
            {
                Debug.WriteLine("   llegó " + c.HoraLlegada.Value.ToString("HH:mm") + "  cita " + c.HoraTexto +
                                "  turno " + c.Turno + "  " + c.NombrePaciente);
            }

            // 4) Operaciones de la cola.
            CitaResumen siguiente = cola.Siguiente();
            Debug.WriteLine("Siguiente a atender: " + (siguiente != null ? siguiente.NombrePaciente : "(nadie)"));
            Debug.WriteLine("Cantidad en cola: " + cola.Cantidad);
            // Resultado esperado: Quispe Mamani (llegó 08:05), Rojas Vega (08:30, turno 3), Ccori Paz (08:30, turno 4).
        }

        private static CitaResumen Ejemplo(int turno, string hora, string paciente, EstadoCita estado, string llegada)
        {
            return new CitaResumen
            {
                IdCita = turno,
                Turno = turno,
                Fecha = DateTime.Today,
                Hora = TimeSpan.Parse(hora),
                NombrePaciente = paciente,
                Dni = "0000000" + turno,
                Consultorio = "Consultorio 1 - Medicina",
                Profesional = "Mendoza Rojas, Carlos",
                Estado = estado,
                HoraLlegada = llegada == null ? (DateTime?)null : DateTime.Today.Add(TimeSpan.Parse(llegada))
            };
        }
    }
}
