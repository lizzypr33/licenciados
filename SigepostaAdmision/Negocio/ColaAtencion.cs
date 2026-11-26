using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SigepostaAdmision.Entidades;

namespace SigepostaAdmision.Negocio
{
    /// <summary>
    /// Cola de atención del día: una COLECCIÓN PROPIA que se puede recorrer con
    /// foreach porque implementa IEnumerable&lt;CitaResumen&gt;.
    ///
    /// Recibe todas las citas del día, pero solo "expone" las que están EnCola,
    /// en ORDEN DE LLEGADA: primero por HoraLlegada (quien llegó antes, se atiende
    /// antes) y, si coinciden, por NumeroTurno. Ya no se ordena por la hora de la
    /// cita: un paciente citado a las 09:00 que llegó a las 08:30 va antes que uno
    /// citado a las 08:40 que llegó a las 08:50.
    ///
    /// El recorrido (enumerador) está implementado de DOS formas para comparar:
    ///   (a) ObtenerColaAtencion(), con "yield return": el compilador genera por
    ///       nosotros la clase enumeradora y su estado (en qué elemento va).
    ///   (b) ColaEnumerator, escrita A MANO: implementamos nosotros Current,
    ///       MoveNext, Reset y Dispose, y guardamos la posición en un campo.
    /// Las dos producen exactamente el mismo resultado. "yield" es más corto y
    /// menos propenso a errores; la versión manual muestra lo que realmente
    /// ocurre "por debajo" de cada foreach.
    /// </summary>
    public class ColaAtencion : IEnumerable<CitaResumen>
    {
        // Copia de TODAS las citas, ya ordenadas por hora de llegada y turno.
        // Es una List porque se recorre por índice (lo necesita el enumerador manual).
        private readonly List<CitaResumen> citasOrdenadas;

        /// <summary>
        /// Recibe la lista de citas del día. Se hace una copia ordenada con LINQ
        /// (OrderBy + ThenBy), así un cambio posterior en la lista original no
        /// altera la cola mientras se recorre. Una cita sin HoraLlegada (no
        /// debería estar EnCola) se envía al final.
        /// </summary>
        public ColaAtencion(List<CitaResumen> citas)
        {
            if (citas == null) throw new ArgumentNullException("citas");

            citasOrdenadas = citas
                .OrderBy(c => c.HoraLlegada ?? DateTime.MaxValue)
                .ThenBy(c => c.Turno)
                .ToList();
        }

        // ===================== Forma (a): yield return =====================

        /// <summary>
        /// Recorre la cola con "yield return". Cada vez que el foreach pide el
        /// siguiente elemento, el método continúa desde donde se quedó y entrega
        /// la próxima cita EnCola. La ejecución es DIFERIDA: no se recorre nada
        /// hasta que alguien itera el resultado.
        /// </summary>
        public IEnumerable<CitaResumen> ObtenerColaAtencion()
        {
            foreach (CitaResumen cita in citasOrdenadas)
            {
                if (cita.Estado == EstadoCita.EnCola)
                {
                    yield return cita;
                }
            }
        }

        // ===================== Forma (b): enumerador manual =====================

        /// <summary>
        /// Lo usa el foreach: "foreach (var c in cola)" llama a GetEnumerator()
        /// y luego a MoveNext()/Current hasta que MoveNext devuelve false.
        /// Aquí se devuelve el enumerador escrito a mano.
        /// </summary>
        public IEnumerator<CitaResumen> GetEnumerator()
        {
            return new ColaEnumerator(citasOrdenadas);
        }

        // Versión no genérica que exige la interfaz IEnumerable (heredada de .NET 1.0).
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Enumerador escrito a mano. Guarda en "indice" la posición actual y en
        /// MoveNext avanza saltando las citas que no están EnCola.
        /// </summary>
        private sealed class ColaEnumerator : IEnumerator<CitaResumen>
        {
            private readonly List<CitaResumen> citas;
            private int indice = -1;          // -1 = antes del primer elemento
            private CitaResumen actual;

            public ColaEnumerator(List<CitaResumen> citas)
            {
                this.citas = citas;
            }

            /// <summary>Elemento actual. Solo es válido después de un MoveNext() que devolvió true.</summary>
            public CitaResumen Current
            {
                get
                {
                    if (actual == null)
                    {
                        throw new InvalidOperationException("El enumerador no está posicionado en un elemento.");
                    }
                    return actual;
                }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            /// <summary>
            /// Avanza al siguiente elemento EnCola. Devuelve false cuando ya no
            /// quedan elementos (y entonces el foreach termina).
            /// </summary>
            public bool MoveNext()
            {
                while (++indice < citas.Count)
                {
                    if (citas[indice].Estado == EstadoCita.EnCola)
                    {
                        actual = citas[indice];
                        return true;
                    }
                }
                actual = null;
                return false;
            }

            /// <summary>Vuelve al inicio (antes del primer elemento).</summary>
            public void Reset()
            {
                indice = -1;
                actual = null;
            }

            /// <summary>
            /// IEnumerator&lt;T&gt; hereda de IDisposable. Este enumerador no usa
            /// recursos externos (archivos, conexiones), así que no hay nada que
            /// liberar; foreach lo llama igualmente al terminar.
            /// </summary>
            public void Dispose()
            {
            }
        }

        // ===================== Operaciones de la cola =====================

        /// <summary>Primera cita en cola (la que debe atenderse ahora), o null si la cola está vacía.</summary>
        public CitaResumen Siguiente()
        {
            // FirstOrDefault recorre la colección con nuestro GetEnumerator (forma b).
            return this.FirstOrDefault();
        }

        /// <summary>Cantidad de pacientes esperando (citas EnCola).</summary>
        public int Cantidad
        {
            get { return ObtenerColaAtencion().Count(); }   // Usa la forma (a)
        }
    }
}
