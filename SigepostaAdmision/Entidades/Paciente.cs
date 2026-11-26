using System;

namespace SigepostaAdmision.Entidades
{
    /// <summary>
    /// Paciente que se atiende en la posta.
    ///
    /// ¿Por qué herencia (Paciente : Persona)? Porque un paciente "ES UNA"
    /// persona: hereda DNI, nombres, apellidos, fecha de nacimiento y teléfono,
    /// y solo agrega lo que le es propio (dirección, sexo, historia clínica, posta).
    ///
    /// Conciliación con la tabla Paciente de SQL Server:
    ///   - La tabla guarda ApellidoPaterno y ApellidoMaterno por separado, así que
    ///     el paciente también los guarda así, y SOBRESCRIBE (override) la propiedad
    ///     virtual Apellidos de Persona para devolver "Paterno Materno". De este
    ///     modo el código que ya usaba p.Apellidos (Medico, Enfermero, MostrarDatos)
    ///     sigue funcionando sin cambios: eso también es POLIMORFISMO.
    ///   - IdPaciente es la clave generada por la base de datos (0 = aún no guardado).
    ///   - Direccion es opcional (la columna admite NULL).
    ///   - NumeroHistoriaClinica y PostaAsignada no son columnas de Paciente
    ///     (la historia está en su propia tabla), por eso ahora son opcionales.
    ///
    /// La entidad solo protege su coherencia básica; las reglas del negocio
    /// (edad máxima, teléfono de 9 dígitos, DNI repetido...) están en PacienteNegocio.
    /// </summary>
    public class Paciente : Persona
    {
        // ----- Campos privados -----
        private int idPaciente;
        private string apellidoPaterno;
        private string apellidoMaterno;
        private char sexo;
        private string numeroHistoriaClinica;
        private string direccion;
        private string postaAsignada;

        // ----- Constructores -----

        /// <summary>Constructor por defecto (llama al constructor base).</summary>
        public Paciente() : base()
        {
            numeroHistoriaClinica = string.Empty;
            direccion = string.Empty;
            postaAsignada = string.Empty;
        }

        /// <summary>
        /// Constructor parametrizado con los datos de la tabla Paciente.
        /// Usa ": this()" para inicializar primero los valores por defecto y luego
        /// asigna cada dato mediante su propiedad, para que se apliquen las
        /// validaciones (las heredadas de Persona y las propias del Paciente).
        /// </summary>
        public Paciente(int idPaciente, string dni, string apellidoPaterno, string apellidoMaterno,
                        string nombres, DateTime fechaNacimiento, char sexo,
                        string direccion, string telefono)
            : this()
        {
            IdPaciente = idPaciente;
            Dni = dni;
            ApellidoPaterno = apellidoPaterno;
            ApellidoMaterno = apellidoMaterno;
            Nombres = nombres;
            FechaNacimiento = fechaNacimiento;
            Sexo = sexo;
            Direccion = direccion;
            Telefono = telefono;
        }

        // ----- Propiedades -----

        /// <summary>Identificador en la base de datos (0 si aún no se ha guardado).</summary>
        public int IdPaciente
        {
            get { return idPaciente; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("El Id del paciente no puede ser negativo.");
                }
                idPaciente = value;
            }
        }

        public string ApellidoPaterno
        {
            get { return apellidoPaterno; }
            set { apellidoPaterno = Validacion.TextoObligatorio(value, "Apellido paterno"); }
        }

        public string ApellidoMaterno
        {
            get { return apellidoMaterno; }
            set { apellidoMaterno = Validacion.TextoObligatorio(value, "Apellido materno"); }
        }

        /// <summary>
        /// OVERRIDE de la propiedad virtual de Persona: devuelve "Paterno Materno".
        /// Al asignarla se separa por el primer espacio (paterno = primera palabra,
        /// materno = el resto). Para apellidos compuestos ("De la Cruz") conviene
        /// asignar ApellidoPaterno y ApellidoMaterno directamente.
        /// </summary>
        public override string Apellidos
        {
            get { return ((apellidoPaterno ?? string.Empty) + " " + (apellidoMaterno ?? string.Empty)).Trim(); }
            set
            {
                string valor = Validacion.TextoObligatorio(value, "Apellidos");
                int espacio = valor.IndexOf(' ');
                if (espacio < 0)
                {
                    throw new ArgumentException("Indique el apellido paterno y el materno separados por un espacio.");
                }
                ApellidoPaterno = valor.Substring(0, espacio);
                ApellidoMaterno = valor.Substring(espacio + 1);
            }
        }

        /// <summary>Sexo: 'M' (masculino) o 'F' (femenino), como en la tabla.</summary>
        public char Sexo
        {
            get { return sexo; }
            set
            {
                char valor = char.ToUpperInvariant(value);
                if (valor != 'M' && valor != 'F')
                {
                    throw new ArgumentException("El sexo debe ser 'M' o 'F'.");
                }
                sexo = valor;
            }
        }

        /// <summary>Opcional: no hay columna en la tabla Paciente.</summary>
        public string NumeroHistoriaClinica
        {
            get { return numeroHistoriaClinica; }
            set { numeroHistoriaClinica = value == null ? string.Empty : value.Trim(); }
        }

        /// <summary>Opcional (la columna Direccion admite NULL).</summary>
        public string Direccion
        {
            get { return direccion; }
            set { direccion = value == null ? string.Empty : value.Trim(); }
        }

        /// <summary>Opcional: no hay columna en la tabla Paciente.</summary>
        public string PostaAsignada
        {
            get { return postaAsignada; }
            set { postaAsignada = value == null ? string.Empty : value.Trim(); }
        }

        /// <summary>
        /// Historia clínica del paciente, si fue cargada (puede ser null).
        /// Permite que RegistrarAtencion agregue la atención a su lista en memoria.
        /// </summary>
        public HistoriaClinica Historia { get; set; }

        // ----- Métodos -----

        /// <summary>
        /// Registra (en memoria) que el paciente fue atendido en la cita indicada:
        ///   1) verifica que la cita sea de este paciente,
        ///   2) aplica la regla de dominio Cita.MarcarAtendida() (EnCola y ya llamado),
        ///   3) crea la Atencion y, si la historia está cargada, la agrega a su lista.
        /// No guarda nada en la base: eso lo hace AtencionNegocio con una transacción.
        /// El IdUsuario lo completa la capa de Negocio (la entidad no conoce la sesión).
        /// </summary>
        public Atencion RegistrarAtencion(Cita c)
        {
            if (c == null)
            {
                throw new ArgumentException("Debe indicar la cita a registrar.");
            }
            if (!ReferenceEquals(c.Paciente, this))
            {
                throw new InvalidOperationException("La cita no corresponde a este paciente.");
            }

            c.MarcarAtendida();

            Atencion atencion = new Atencion(0, c.IdCita, Historia != null ? Historia.IdHistoria : 0,
                                             0, DateTime.Now, string.Empty);
            if (Historia != null)
            {
                Historia.ListaAtenciones.Add(atencion);
            }
            return atencion;
        }

        /// <summary>
        /// POLIMORFISMO: sobrescribe el método virtual de Persona. Reutiliza
        /// el texto base (base.MostrarDatos()) y agrega los datos del paciente.
        /// </summary>
        public override string MostrarDatos()
        {
            return "[Paciente] " + base.MostrarDatos() +
                   " | Sexo: " + sexo +
                   " | Dirección: " + (direccion.Length > 0 ? direccion : "(no registrada)") +
                   (numeroHistoriaClinica.Length > 0 ? " | H.C.: " + numeroHistoriaClinica : string.Empty) +
                   (postaAsignada.Length > 0 ? " | Posta: " + postaAsignada : string.Empty);
        }
    }
}
