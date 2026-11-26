using System;
using System.Collections.Generic;

namespace SigepostaAdmision.Entidades
{
    /// <summary>
    /// Historia clínica de un paciente.
    ///
    /// No usa herencia: se relaciona con Paciente por ASOCIACIÓN (la historia
    /// "pertenece a" un paciente) y con Cita por COMPOSICIÓN/AGREGACIÓN
    /// (la historia "contiene" una lista de atenciones).
    ///
    /// DiagnosticoCifrado guarda el diagnóstico ya cifrado. Por ahora es solo
    /// una propiedad; el cifrado se implementará en la capa de Seguridad.
    /// (En la tabla la columna es VARBINARY(MAX); su lectura y escritura se
    /// agregarán junto con el cifrado.)
    ///
    /// Reglas del negocio (aplicadas en HistoriaNegocio y en la base de datos):
    ///   - Un paciente tiene UNA sola historia (IdPaciente es UNIQUE en la tabla).
    ///   - Las historias no se eliminan: solo pasan a Activa o Inactiva.
    /// </summary>
    public class HistoriaClinica
    {
        // ----- Campos privados -----
        private int idHistoria;
        private EstadoHistoria estado;
        private string numeroHistoria;
        private Paciente paciente;
        private DateTime fechaApertura;
        private readonly List<Atencion> listaAtenciones;
        private string diagnosticoCifrado;

        // ----- Constructores -----

        /// <summary>Constructor por defecto: historia activa, abierta hoy, sin atenciones.</summary>
        public HistoriaClinica()
        {
            estado = EstadoHistoria.Activa;
            fechaApertura = DateTime.Today;
            listaAtenciones = new List<Atencion>();
            diagnosticoCifrado = string.Empty;
        }

        /// <summary>Constructor parametrizado. Reutiliza el constructor por defecto con ": this()".</summary>
        public HistoriaClinica(string numeroHistoria, Paciente paciente, DateTime fechaApertura)
            : this()
        {
            NumeroHistoria = numeroHistoria;
            Paciente = paciente;
            FechaApertura = fechaApertura;
        }

        // ----- Propiedades -----

        /// <summary>Identificador en la base de datos (0 si aún no se ha guardado).</summary>
        public int IdHistoria
        {
            get { return idHistoria; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("El Id de la historia no puede ser negativo.");
                }
                idHistoria = value;
            }
        }

        /// <summary>Activa o Inactiva. Una historia nunca se elimina, solo se inactiva.</summary>
        public EstadoHistoria Estado
        {
            get { return estado; }
            set
            {
                if (!Enum.IsDefined(typeof(EstadoHistoria), value))
                {
                    throw new ArgumentException("El estado de la historia no es válido.");
                }
                estado = value;
            }
        }

        public string NumeroHistoria
        {
            get { return numeroHistoria; }
            set { numeroHistoria = Validacion.TextoObligatorio(value, "Número de historia"); }
        }

        public Paciente Paciente
        {
            get { return paciente; }
            set { paciente = Validacion.ObjetoObligatorio(value, "Paciente"); }
        }

        /// <summary>La historia no puede abrirse en una fecha futura.</summary>
        public DateTime FechaApertura
        {
            get { return fechaApertura; }
            set
            {
                if (value.Date > DateTime.Today)
                {
                    throw new ArgumentException("La fecha de apertura no puede ser futura.");
                }
                fechaApertura = value.Date;
            }
        }

        /// <summary>
        /// Atenciones registradas en esta historia (composición: la historia
        /// "contiene" sus atenciones). Antes era una lista de Cita; desde que
        /// existe la entidad Atencion, guarda objetos Atencion.
        /// Solo tiene "get": se agregan con ListaAtenciones.Add(...), pero no se
        /// puede reemplazar la lista por null.
        /// </summary>
        public List<Atencion> ListaAtenciones
        {
            get { return listaAtenciones; }
        }

        /// <summary>Diagnóstico cifrado (el cifrado se hará en la carpeta Seguridad).</summary>
        public string DiagnosticoCifrado
        {
            get { return diagnosticoCifrado; }
            set { diagnosticoCifrado = value ?? string.Empty; }
        }
    }
}
