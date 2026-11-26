using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SigepostaAdmision.Entidades;
using SigepostaAdmision.Negocio;

namespace SigepostaAdmision.Presentacion
{
    /// <summary>
    /// Diálogo modal para PROGRAMAR una cita nueva o REPROGRAMAR una existente.
    ///
    /// Se usan dos constructores (sobrecarga de constructores):
    ///   - FrmNuevaCita()                  → modo "nueva cita".
    ///   - FrmNuevaCita(CitaResumen cita)  → modo "reprogramar" (el paciente no cambia).
    ///
    /// Las listas se encadenan: Servicio → Consultorio → Profesional (de la posta
    /// del consultorio) → franjas libres en la fecha. El formulario solo pide los
    /// datos a Negocio; las validaciones ocurren al Guardar, en CitaNegocio.
    /// </summary>
    public partial class FrmNuevaCita : Form
    {
        private static readonly Color ColorAlerta = Tema.TextoError;       // #B91C1C (mismo valor que antes)
        private static readonly Color ColorNormal = Tema.TextoPrincipal;   // Texto principal del tema

        private readonly CitaNegocio citaNegocio = new CitaNegocio();
        private readonly PacienteNegocio pacienteNegocio = new PacienteNegocio();
        private readonly HistoriaNegocio historiaNegocio = new HistoriaNegocio();

        private readonly CitaResumen citaOriginal;   // null en modo "nueva cita"
        private int idPaciente;

        /// <summary>Id de la cita guardada (para que FrmCitas la seleccione).</summary>
        public int IdCitaGuardada { get; private set; }

        /// <summary>Fecha en que quedó programada la cita.</summary>
        public DateTime FechaProgramada { get; private set; }

        /// <summary>Modo "nueva cita".</summary>
        public FrmNuevaCita()
        {
            InitializeComponent();
            Text = "Nueva cita";
            lblTitulo.Text = "Programar nueva cita";
            Tema.Aplicar(this);   // Solo colores (el constructor de reprogramación pasa por aquí)
        }

        /// <summary>Modo "reprogramar": recibe la cita a modificar.</summary>
        public FrmNuevaCita(CitaResumen cita) : this()
        {
            citaOriginal = cita;
            Text = "Reprogramar cita";
            lblTitulo.Text = "Reprogramar cita (turno " + cita.Turno + ")";
        }

        private bool EsReprogramacion
        {
            get { return citaOriginal != null; }
        }

        private void FrmNuevaCita_Load(object sender, EventArgs e)
        {
            // No se ofrecen fechas pasadas (Negocio igual lo valida al guardar).
            dtpFecha.MinDate = DateTime.Today;
            dtpFecha.Value = EsReprogramacion && citaOriginal.Fecha.Date > DateTime.Today
                ? citaOriginal.Fecha.Date
                : DateTime.Today;

            try
            {
                cboServicio.Items.AddRange(citaNegocio.ServiciosDisponibles().ToArray());

                if (EsReprogramacion)
                {
                    // El paciente de una cita no se cambia al reprogramar.
                    txtDni.Text = citaOriginal.Dni;
                    txtDni.ReadOnly = true;
                    btnBuscarPaciente.Enabled = false;
                    MostrarPaciente(pacienteNegocio.ObtenerPorId(citaOriginal.IdPaciente));

                    // Preselección en cadena: servicio → consultorio → profesional → hora.
                    SeleccionarPorId(cboServicio, citaOriginal.IdServicio);
                    SeleccionarPorId(cboConsultorio, citaOriginal.IdConsultorio);
                    SeleccionarPorId(cboProfesional, citaOriginal.IdPersonal);
                    if (cboHora.Items.Contains(citaOriginal.Hora))
                    {
                        cboHora.SelectedItem = citaOriginal.Hora;
                    }
                }
            }
            catch (Exception ex)
            {
                Interfaz.MostrarErrorBaseDatos(ex);
            }
            CargarFranjas();
        }

        // ===================== Paciente =====================

        private void btnBuscarPaciente_Click(object sender, EventArgs e)
        {
            BuscarPaciente();
        }

        private void txtDni_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                BuscarPaciente();
            }
        }

        // Si se cambia el DNI después de buscar, el paciente encontrado deja de valer.
        private void txtDni_TextChanged(object sender, EventArgs e)
        {
            if (!EsReprogramacion && idPaciente != 0)
            {
                MostrarPaciente(null);
            }
        }

        private void BuscarPaciente()
        {
            string dni = txtDni.Text.Trim();
            if (!PacienteNegocio.EsDniValido(dni))
            {
                MessageBox.Show("Ingrese un DNI de 8 dígitos.", "Buscar paciente",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtDni.Focus();
                return;
            }

            try
            {
                Paciente p = pacienteNegocio.BuscarPorDni(dni);
                if (p == null)
                {
                    MostrarPaciente(null);
                    MessageBox.Show("No existe un paciente con DNI " + dni +
                        ".\nRegístrelo primero en el módulo Pacientes.",
                        "Paciente no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                MostrarPaciente(p);
            }
            catch (Exception ex)
            {
                Interfaz.MostrarErrorBaseDatos(ex);
            }
        }

        // Muestra nombre, edad e historia clínica (con su estado) del paciente.
        private void MostrarPaciente(Paciente p)
        {
            if (p == null)
            {
                idPaciente = 0;
                lblNombre.Text = "—";
                lblEdad.Text = "—";
                lblHistoria.Text = "—";
                lblHistoria.ForeColor = ColorNormal;
                return;
            }

            idPaciente = p.IdPaciente;
            lblNombre.Text = p.Apellidos + ", " + p.Nombres;
            lblEdad.Text = p.ObtenerEdad() + " años";

            HistoriaClinica h = historiaNegocio.ObtenerPorPaciente(p.IdPaciente);
            if (h == null)
            {
                lblHistoria.Text = "Sin historia clínica";
                lblHistoria.ForeColor = ColorAlerta;
            }
            else
            {
                lblHistoria.Text = h.NumeroHistoria + " (" + h.Estado + ")";
                lblHistoria.ForeColor = h.Estado == EstadoHistoria.Activa ? ColorNormal : ColorAlerta;
            }
        }

        // ===================== Listas encadenadas =====================

        // Servicio → consultorios de ese servicio.
        private void cboServicio_SelectedIndexChanged(object sender, EventArgs e)
        {
            cboConsultorio.Items.Clear();
            ServicioItem servicio = cboServicio.SelectedItem as ServicioItem;
            if (servicio != null)
            {
                try
                {
                    cboConsultorio.Items.AddRange(citaNegocio.Consultorios(servicio.IdServicio).ToArray());
                }
                catch (Exception ex)
                {
                    Interfaz.MostrarErrorBaseDatos(ex);
                }
            }
            // Si el servicio tiene un solo consultorio, se elige solo.
            cboConsultorio.SelectedIndex = cboConsultorio.Items.Count == 1 ? 0 : -1;
            if (cboConsultorio.Items.Count != 1)
            {
                cboConsultorio_SelectedIndexChanged(cboConsultorio, EventArgs.Empty);
            }
        }

        // Consultorio → profesionales de la posta de ese consultorio.
        private void cboConsultorio_SelectedIndexChanged(object sender, EventArgs e)
        {
            ProfesionalItem anterior = cboProfesional.SelectedItem as ProfesionalItem;
            cboProfesional.Items.Clear();

            ConsultorioItem consultorio = cboConsultorio.SelectedItem as ConsultorioItem;
            if (consultorio != null)
            {
                try
                {
                    cboProfesional.Items.AddRange(citaNegocio.Profesionales(consultorio.IdPosta).ToArray());
                }
                catch (Exception ex)
                {
                    Interfaz.MostrarErrorBaseDatos(ex);
                }
            }

            // Se conserva el profesional si sigue disponible.
            if (anterior != null)
            {
                SeleccionarPorId(cboProfesional, anterior.IdPersonal);
            }
            if (cboProfesional.SelectedIndex < 0)
            {
                CargarFranjas();
            }
        }

        // Profesional o fecha → recalcular franjas libres.
        private void cboProfesional_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarFranjas();
        }

        private void dtpFecha_ValueChanged(object sender, EventArgs e)
        {
            CargarFranjas();
        }

        /// <summary>Muestra en cboHora solo las franjas libres del profesional en la fecha.</summary>
        private void CargarFranjas()
        {
            object horaAnterior = cboHora.SelectedItem;
            cboHora.Items.Clear();

            ProfesionalItem profesional = cboProfesional.SelectedItem as ProfesionalItem;
            if (profesional == null)
            {
                lblFranjas.Text = "Seleccione un profesional para ver los horarios libres.";
                return;
            }

            List<TimeSpan> libres;
            try
            {
                // Al reprogramar, la franja actual de la propia cita cuenta como libre.
                libres = citaNegocio.ObtenerFranjasLibres(profesional.IdPersonal, dtpFecha.Value.Date,
                    EsReprogramacion ? citaOriginal.IdCita : 0);
            }
            catch (Exception ex)
            {
                Interfaz.MostrarErrorBaseDatos(ex);
                return;
            }

            foreach (TimeSpan franja in libres)
            {
                cboHora.Items.Add(franja);
            }
            if (horaAnterior != null && cboHora.Items.Contains(horaAnterior))
            {
                cboHora.SelectedItem = horaAnterior;
            }

            if (libres.Count > 0)
            {
                lblFranjas.Text = libres.Count + " horarios libres.";
            }
            else if (!CitaNegocio.EsDiaLaborable(dtpFecha.Value))
            {
                lblFranjas.Text = "Domingo: no hay atención (lunes a sábado, 08:00 a 13:00).";
            }
            else
            {
                lblFranjas.Text = "No hay horarios libres para este profesional en esta fecha.";
            }
        }

        // Las franjas son TimeSpan; se muestran como "08:20".
        private void cboHora_Format(object sender, ListControlConvertEventArgs e)
        {
            if (e.ListItem is TimeSpan)
            {
                e.Value = ((TimeSpan)e.ListItem).ToString(@"hh\:mm");
            }
        }

        // Selecciona en un ComboBox el elemento con ese Id (servicio, consultorio o profesional).
        private static void SeleccionarPorId(ComboBox combo, int id)
        {
            for (int i = 0; i < combo.Items.Count; i++)
            {
                object item = combo.Items[i];
                int itemId =
                    item is ServicioItem ? ((ServicioItem)item).IdServicio :
                    item is ConsultorioItem ? ((ConsultorioItem)item).IdConsultorio :
                    item is ProfesionalItem ? ((ProfesionalItem)item).IdPersonal : -1;
                if (itemId == id)
                {
                    combo.SelectedIndex = i;
                    return;
                }
            }
        }

        // ===================== Guardar =====================

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            ConsultorioItem consultorio = cboConsultorio.SelectedItem as ConsultorioItem;
            ProfesionalItem profesional = cboProfesional.SelectedItem as ProfesionalItem;
            int idConsultorio = consultorio != null ? consultorio.IdConsultorio : 0;
            int idPersonal = profesional != null ? profesional.IdPersonal : 0;
            // Sin hora elegida se envía 00:00, que Negocio rechaza con un mensaje claro.
            TimeSpan hora = cboHora.SelectedItem is TimeSpan ? (TimeSpan)cboHora.SelectedItem : TimeSpan.Zero;
            DateTime fecha = dtpFecha.Value.Date;

            Cursor = Cursors.WaitCursor;
            ResultadoOperacion r;
            try
            {
                r = EsReprogramacion
                    ? citaNegocio.Reprogramar(citaOriginal.IdCita, fecha, hora, idConsultorio, idPersonal)
                    : citaNegocio.ProgramarCita(idPaciente, idConsultorio, idPersonal, fecha, hora);
            }
            finally
            {
                Cursor = Cursors.Default;
            }

            if (!r.Exito)
            {
                MessageBox.Show("No se pudo guardar la cita:\n\n" + r.MensajesComoTexto(),
                    "Revise los datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                CargarFranjas();   // Por si otro usuario ocupó la franja mientras tanto
                return;
            }

            IdCitaGuardada = r.IdGenerado.Value;
            FechaProgramada = fecha;
            MessageBox.Show(
                (EsReprogramacion ? "Cita reprogramada." : "Cita programada.") + "\n\n" +
                "Turno N° " + r.NumeroTurno + " · " + consultorio.Nombre + "\n" +
                fecha.ToString("dddd dd/MM/yyyy") + " a las " + hora.ToString(@"hh\:mm"),
                Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
        }
    }
}
