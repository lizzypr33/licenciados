using System;

namespace SigepostaAdmision.Entidades
{
    /// <summary>
    /// Atención realizada a un paciente a partir de una cita.
    ///
    /// Se relaciona por ASOCIACIÓN con la Cita que la originó (una cita tiene
    /// como máximo UNA atención: IdCita es UNIQUE en la tabla), con la
    /// HistoriaClinica donde se registra y con el Usuario que la registró.
    ///
    /// DiagnosticoCifrado: el diagnóstico es un DATO SENSIBLE de salud, así que la
    /// entidad nunca lo guarda en texto; solo los bytes cifrados con AES-256
    /// (CifradoAes). Se descifra únicamente cuando alguien autorizado lo pide
    /// (AtencionNegocio.VerDiagnostico), y ese acceso queda auditado.
    /// </summary>
    public class Atencion
    {
        /// <summary>Longitud máxima de la columna Observaciones VARCHAR(300).</summary>
        public const int MaxObservaciones = 300;

        /// <summary>Longitud máxima del diagnóstico en texto (antes de cifrarlo).</summary>
        public const int MaxDiagnostico = 500;

        // ----- Campos privados -----
        private int idAtencion;
        private int idCita;
        private int idHistoria;
        private int idUsuario;
        private DateTime fechaHora;
        private string observaciones;

        // ----- Constructores -----

        /// <summary>Constructor por defecto: atención con fecha y hora actuales y sin observaciones.</summary>
        public Atencion()
        {
            fechaHora = DateTime.Now;
            observaciones = string.Empty;
        }

        /// <summary>Constructor parametrizado. Reutiliza el constructor por defecto con ": this()".</summary>
        public Atencion(int idAtencion, int idCita, int idHistoria, int idUsuario,
                        DateTime fechaHora, string observaciones)
            : this()
        {
            IdAtencion = idAtencion;
            IdCita = idCita;
            IdHistoria = idHistoria;
            IdUsuario = idUsuario;
            FechaHora = fechaHora;
            Observaciones = observaciones;
        }

        // ----- Propiedades -----

        /// <summary>Identificador en la base (0 = aún no guardada).</summary>
        public int IdAtencion
        {
            get { return idAtencion; }
            set { idAtencion = NoNegativo(value, "Id de la atención"); }
        }

        public int IdCita
        {
            get { return idCita; }
            set { idCita = NoNegativo(value, "Id de la cita"); }
        }

        public int IdHistoria
        {
            get { return idHistoria; }
            set { idHistoria = NoNegativo(value, "Id de la historia"); }
        }

        /// <summary>Usuario que registró la atención (0 = aún no asignado).</summary>
        public int IdUsuario
        {
            get { return idUsuario; }
            set { idUsuario = NoNegativo(value, "Id del usuario"); }
        }

        public DateTime FechaHora
        {
            get { return fechaHora; }
            set { fechaHora = value; }
        }

        /// <summary>Observaciones opcionales, como máximo 300 caracteres.</summary>
        public string Observaciones
        {
            get { return observaciones; }
            set
            {
                string valor = value == null ? string.Empty : value.Trim();
                if (valor.Length > MaxObservaciones)
                {
                    throw new ArgumentException(
                        "Las observaciones no pueden superar los " + MaxObservaciones + " caracteres.");
                }
                observaciones = valor;
            }
        }

        /// <summary>
        /// Diagnóstico cifrado con AES-256 (IV + datos), o null si no se registró.
        /// Columna Atencion.DiagnosticoCifrado VARBINARY(MAX).
        /// </summary>
        public byte[] DiagnosticoCifrado { get; set; }

        private static int NoNegativo(int valor, string campo)
        {
            if (valor < 0)
            {
                throw new ArgumentException("El " + campo + " no puede ser negativo.");
            }
            return valor;
        }
    }
}
