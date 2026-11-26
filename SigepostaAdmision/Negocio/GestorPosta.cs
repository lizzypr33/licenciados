using System;
using System.Collections.Generic;
using System.Linq;
using SigepostaAdmision.Datos;
using SigepostaAdmision.Entidades;

namespace SigepostaAdmision.Negocio
{
    /// <summary>
    /// Contenedor EN MEMORIA de los datos con los que se está trabajando en la
    /// posta (pacientes y citas del día). Se consulta la base de datos una vez
    /// y luego se filtra, agrupa y cuenta en memoria con LINQ, sin volver a la base.
    ///
    /// ¿Qué colección para qué?
    ///   - List&lt;T&gt; (PacientesPosta, CitasDelDia): conserva el ORDEN y permite
    ///     acceso por índice; ideal para mostrar en una grilla y recorrer.
    ///   - Dictionary&lt;TClave, TValor&gt; (CitasPorConsultorio, ResumenPorEstado):
    ///     búsqueda directa POR CLAVE (por ejemplo, "Consultorio 1 - Medicina")
    ///     sin recorrer toda la lista; es mucho más rápido que buscar con un bucle.
    ///   - LINQ (Where, OrderBy, GroupBy, ToDictionary, Take...): permite
    ///     CONSULTAR colecciones de forma declarativa ("qué quiero") en lugar de
    ///     escribir bucles ("cómo recorrer"). Por eso aquí no hay bucles for.
    /// </summary>
    public class GestorPosta
    {
        private const string Todos = "Todos";

        private readonly CitaNegocio citaNegocio = new CitaNegocio();
        private readonly PacienteDAO pacienteDao = new PacienteDAO();

        /// <summary>
        /// Posta del usuario de la sesión (con IdPosta 0 si es un Administrador
        /// sin posta). Su ListaPacientes se llena junto con PacientesPosta.
        /// </summary>
        public Posta Posta { get; private set; }

        /// <summary>Pacientes con citas en la posta (List: orden y acceso por índice).</summary>
        public List<Paciente> PacientesPosta { get; private set; }

        /// <summary>Todas las citas de la fecha cargada (List).</summary>
        public List<CitaResumen> CitasDelDia { get; private set; }

        /// <summary>Citas del día agrupadas por nombre de consultorio (Dictionary: búsqueda por clave).</summary>
        public Dictionary<string, List<CitaResumen>> CitasPorConsultorio { get; private set; }

        /// <summary>Fecha de las citas cargadas (null si aún no se cargó nada).</summary>
        public DateTime? FechaCargada { get; private set; }

        public GestorPosta()
        {
            Posta = new Posta();
            if (Sesion.HaySesion && Sesion.UsuarioActual.IdPosta.HasValue)
            {
                Posta.IdPosta = Sesion.UsuarioActual.IdPosta.Value;
            }
            PacientesPosta = new List<Paciente>();
            CitasDelDia = new List<CitaResumen>();
            CitasPorConsultorio = new Dictionary<string, List<CitaResumen>>(StringComparer.OrdinalIgnoreCase);
        }

        // ===================== Carga desde la base de datos =====================

        /// <summary>
        /// Carga los pacientes que tienen citas en la posta del usuario (todas las
        /// postas si es Administrador sin posta) y los asigna también a
        /// Posta.ListaPacientes.
        /// </summary>
        public void CargarPacientesPosta()
        {
            Usuario u = Sesion.UsuarioActual;
            if (u == null)
            {
                throw new InvalidOperationException("No hay una sesión iniciada.");
            }

            if (u.IdPosta.HasValue)
            {
                PacientesPosta = pacienteDao.ListarPorPosta(u.IdPosta.Value);
            }
            else if (u.Rol == RolUsuario.Administrador)
            {
                PacientesPosta = pacienteDao.ListarConCitasTodasLasPostas();
            }
            else
            {
                PacientesPosta = new List<Paciente>();   // Admision sin posta: no ve pacientes
            }

            Posta.ListaPacientes.Clear();
            Posta.ListaPacientes.AddRange(PacientesPosta);
        }

        /// <summary>
        /// Consulta UNA vez las citas de la fecha (CitaNegocio ya aplica la posta
        /// de la sesión) y arma el diccionario por consultorio con LINQ:
        /// GroupBy agrupa por nombre y ToDictionary convierte cada grupo en una lista.
        /// </summary>
        public void CargarCitasDelDia(DateTime fecha)
        {
            CitasDelDia = citaNegocio.ListarCitas(fecha.Date, null, null);
            FechaCargada = fecha.Date;

            CitasPorConsultorio = CitasDelDia
                .GroupBy(c => c.Consultorio)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderBy(c => c.Hora).ThenBy(c => c.Turno).ToList(),
                    StringComparer.OrdinalIgnoreCase);
        }

        // ===================== Consultas EN MEMORIA (LINQ) =====================

        /// <summary>
        /// Filtra las citas ya cargadas, SIN volver a consultar la base.
        /// consultorio / estado: null, vacío o "Todos" = sin filtro.
        /// Si hay consultorio, se toma directamente del diccionario (búsqueda por clave).
        /// </summary>
        public List<CitaResumen> FiltrarCitas(string consultorio, string estado)
        {
            IEnumerable<CitaResumen> origen = CitasDelDia;

            if (!EsTodos(consultorio))
            {
                List<CitaResumen> delConsultorio;
                origen = CitasPorConsultorio.TryGetValue(consultorio, out delConsultorio)
                    ? delConsultorio
                    : Enumerable.Empty<CitaResumen>();
            }

            EstadoCita estadoBuscado;
            if (!EsTodos(estado) && Enum.TryParse(estado.Replace(" ", ""), true, out estadoBuscado))
            {
                origen = origen.Where(c => c.Estado == estadoBuscado);
            }

            return origen
                .OrderBy(c => c.Hora)
                .ThenBy(c => c.Consultorio)
                .ThenBy(c => c.Turno)
                .ToList();
        }

        /// <summary>
        /// Cantidad de citas por estado, con GroupBy. Incluye todos los estados
        /// (con 0 si no hay citas en ese estado), para que quien lo use pueda leer
        /// cualquier clave sin comprobar si existe.
        /// </summary>
        public Dictionary<string, int> ResumenPorEstado()
        {
            Dictionary<EstadoCita, int> conteo = CitasDelDia
                .GroupBy(c => c.Estado)
                .ToDictionary(g => g.Key, g => g.Count());

            return Enum.GetValues(typeof(EstadoCita))
                .Cast<EstadoCita>()
                .ToDictionary(
                    e => e.ToString(),
                    e => conteo.ContainsKey(e) ? conteo[e] : 0);
        }

        /// <summary>
        /// Próxima cita Pendiente: si la fecha cargada es hoy, la primera con hora
        /// igual o posterior a la actual; si es un día futuro, la primera del día;
        /// si es un día pasado, null.
        /// </summary>
        public CitaResumen ProximaCita()
        {
            if (!FechaCargada.HasValue || FechaCargada.Value < DateTime.Today)
            {
                return null;
            }

            TimeSpan desde = FechaCargada.Value == DateTime.Today ? DateTime.Now.TimeOfDay : TimeSpan.Zero;

            return CitasDelDia
                .Where(c => c.Estado == EstadoCita.Pendiente && c.Hora >= desde)
                .OrderBy(c => c.Hora)
                .ThenBy(c => c.Turno)
                .FirstOrDefault();
        }

        /// <summary>Cola de atención (citas EnCola) de las citas cargadas.</summary>
        public ColaAtencion Cola()
        {
            return new ColaAtencion(CitasDelDia);
        }

        /// <summary>
        /// Busca en PacientesPosta por DNI exacto, apellidos o nombres; ordena por
        /// apellidos y nombres y devuelve como máximo 50.
        /// </summary>
        public List<Paciente> BuscarPacientes(string texto)
        {
            return FiltrarPacientes(PacientesPosta, texto)
                .OrderBy(p => p.ApellidoPaterno)
                .ThenBy(p => p.ApellidoMaterno)
                .ThenBy(p => p.Nombres)
                .Take(50)
                .ToList();
        }

        /// <summary>
        /// Filtro en memoria reutilizable (también lo usa FrmPacientes para filtrar
        /// la lista mientras se escribe). Conserva el orden original de la lista.
        /// Texto vacío = todos.
        /// </summary>
        public static List<Paciente> FiltrarPacientes(IEnumerable<Paciente> pacientes, string texto)
        {
            if (pacientes == null)
            {
                return new List<Paciente>();
            }

            string t = (texto ?? string.Empty).Trim();
            if (t.Length == 0)
            {
                return pacientes.ToList();
            }

            // DNI: solo coincidencia EXACTA (como la búsqueda en la base por DniHash).
            // Permitir buscar por parte del DNI dejaría deducir dígitos que la
            // pantalla oculta con la máscara (45••••78).
            return pacientes
                .Where(p => string.Equals(p.Dni, t, StringComparison.Ordinal)
                            || Contiene(p.Apellidos, t) || Contiene(p.Nombres, t)
                            || Contiene(p.Apellidos + " " + p.Nombres, t))
                .ToList();
        }

        // ===================== Apoyo =====================

        /// <summary>
        /// "Contiene" sin distinguir mayúsculas y minúsculas.
        /// En .NET Core existe string.Contains(texto, StringComparison), pero en
        /// .NET Framework 4.7.2 no; el equivalente es IndexOf con
        /// StringComparison.OrdinalIgnoreCase (devuelve -1 si no lo encuentra).
        /// Nota: no ignora tildes ("Ángel" no coincide con "angel").
        /// </summary>
        private static bool Contiene(string origen, string texto)
        {
            return origen != null && origen.IndexOf(texto, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool EsTodos(string valor)
        {
            return string.IsNullOrWhiteSpace(valor) || string.Equals(valor, Todos, StringComparison.OrdinalIgnoreCase);
        }
    }
}
