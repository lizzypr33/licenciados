using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SigepostaAdmision.Entidades;
using SigepostaAdmision.Negocio;
using SigepostaAdmision.Seguridad;

namespace SigepostaAdmision.Presentacion
{
    /// <summary>
    /// Módulo Citas: agenda del día con tarjetas de resumen, filtros, y acciones
    /// Nueva cita, Reprogramar y Cancelar.
    ///
    /// Decisión de diseño (Unidad 2): la base de datos se consulta UNA vez por
    /// fecha (GestorPosta.CargarCitasDelDia). Cambiar el consultorio o el estado
    /// filtra EN MEMORIA con LINQ (GestorPosta.FiltrarCitas), sin volver a la
    /// base. Solo el botón Actualizar, el cambio de fecha o una acción que
    /// modifica citas vuelven a consultar.
    ///
    /// El formulario no valida reglas ni escribe SQL: todo pasa por Negocio.
    /// </summary>
    public partial class FrmCitas : Form
    {
        private const string Todos = "Todos";
        private static readonly Color ColorCancelada = Tema.TextoInactivo;   // Gris #9CA3AF (mismo valor que antes)

        private readonly CitaNegocio negocio = new CitaNegocio();
        private readonly GestorPosta gestor = new GestorPosta();
        private bool cargandoFiltros;   // Evita recargar la grilla mientras se llenan los filtros

        public FrmCitas()
        {
            InitializeComponent();
            Tema.Aplicar(this);   // Solo colores
        }

        private void FrmCitas_Load(object sender, EventArgs e)
        {
            cargandoFiltros = true;
            try
            {
                dtpFecha.Value = DateTime.Today;

                cboEstado.Items.Add(Todos);
                foreach (EstadoCita estado in Enum.GetValues(typeof(EstadoCita)))
                {
                    cboEstado.Items.Add(estado);
                }
                cboEstado.SelectedIndex = 0;

                cboConsultorio.Items.Add(Todos);
                try
                {
                    cboConsultorio.Items.AddRange(negocio.Consultorios(null).ToArray());
                }
                catch (Exception ex)
                {
                    Interfaz.MostrarErrorBaseDatos(ex);
                }
                cboConsultorio.SelectedIndex = 0;
            }
            finally
            {
                cargandoFiltros = false;
            }
            ConsultarYMostrar(0);
        }

        // ===================== Consulta a la base (solo cuando hace falta) =====================

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            ConsultarYMostrar(IdSeleccionado());
        }

        // Otra fecha = otros datos: se vuelve a consultar la base.
        private void dtpFecha_ValueChanged(object sender, EventArgs e)
        {
            if (!cargandoFiltros)
            {
                ConsultarYMostrar(0);
            }
        }

        /// <summary>Consulta las citas de la fecha (una vez) y refresca resumen y grilla.</summary>
        private void ConsultarYMostrar(int idCitaSeleccionar)
        {
            try
            {
                gestor.CargarCitasDelDia(dtpFecha.Value.Date);
            }
            catch (Exception ex)
            {
                Interfaz.MostrarErrorBaseDatos(ex);
                return;
            }
            ActualizarTarjetas();
            MostrarFiltradas(idCitaSeleccionar);
        }

        // ===================== Filtros EN MEMORIA =====================

        // Consultorio o estado: se filtra la lista ya cargada, sin ir a la base.
        private void FiltroMemoria_Changed(object sender, EventArgs e)
        {
            if (!cargandoFiltros)
            {
                MostrarFiltradas(IdSeleccionado());
            }
        }

        /// <summary>Llena la grilla con las citas filtradas en memoria por GestorPosta.</summary>
        private void MostrarFiltradas(int idCitaSeleccionar)
        {
            ConsultorioItem consultorio = cboConsultorio.SelectedItem as ConsultorioItem;
            string estado = cboEstado.SelectedItem is EstadoCita ? cboEstado.SelectedItem.ToString() : Todos;

            List<CitaResumen> citas = gestor.FiltrarCitas(consultorio != null ? consultorio.Nombre : Todos, estado);

            grdCitas.Rows.Clear();
            foreach (CitaResumen c in citas)
            {
                int i = grdCitas.Rows.Add(c.Turno, c.HoraTexto, c.NombrePaciente, Mascara.Dni(c.Dni),
                                          c.Consultorio, c.Profesional, TextoEstado(c.Estado));
                DataGridViewRow fila = grdCitas.Rows[i];
                fila.Tag = c;

                if (c.Estado == EstadoCita.Cancelada)
                {
                    // Las canceladas se muestran en gris.
                    fila.DefaultCellStyle.ForeColor = ColorCancelada;
                    fila.DefaultCellStyle.SelectionForeColor = ColorCancelada;
                }
                if (c.IdCita == idCitaSeleccionar)
                {
                    grdCitas.CurrentCell = fila.Cells[0];
                }
            }

            lblResumen.Text = "Mostrando " + citas.Count + " de " + gestor.CitasDelDia.Count + " citas";
            ActualizarBotones();
        }

        /// <summary>
        /// Tarjetas de resumen del día completo (sin aplicar los filtros), calculadas
        /// con GestorPosta.ResumenPorEstado() (GroupBy) y ProximaCita() (OrderBy).
        /// </summary>
        private void ActualizarTarjetas()
        {
            Dictionary<string, int> resumen = gestor.ResumenPorEstado();

            lblValorTotal.Text = gestor.CitasDelDia.Count.ToString();
            lblValorPendientes.Text = resumen[EstadoCita.Pendiente.ToString()].ToString();
            lblValorEnCola.Text = resumen[EstadoCita.EnCola.ToString()].ToString();
            lblValorAtendidas.Text = resumen[EstadoCita.Atendida.ToString()].ToString();

            CitaResumen proxima = gestor.ProximaCita();
            lblProxima.Text = proxima != null
                ? "Próxima cita:\n" + proxima.HoraTexto + "  " + proxima.NombrePaciente
                : "Próxima cita:\n—";
        }

        private static string TextoEstado(EstadoCita estado)
        {
            return estado == EstadoCita.EnCola ? "En cola" : estado.ToString();
        }

        private CitaResumen Seleccionada()
        {
            DataGridViewRow fila = grdCitas.CurrentRow;
            return fila != null ? fila.Tag as CitaResumen : null;
        }

        private int IdSeleccionado()
        {
            CitaResumen c = Seleccionada();
            return c != null ? c.IdCita : 0;
        }

        private void grdCitas_SelectionChanged(object sender, EventArgs e)
        {
            ActualizarBotones();
        }

        // Reglas de Negocio: Reprogramar solo Pendiente; Cancelar, Pendiente o En cola.
        // (En cola, Atendida y Cancelada → Reprogramar deshabilitado.)
        private void ActualizarBotones()
        {
            CitaResumen c = Seleccionada();
            btnReprogramar.Enabled = c != null && CitaNegocio.PuedeReprogramarse(c.Estado);
            btnCancelar.Enabled = c != null && CitaNegocio.PuedeCancelarse(c.Estado);
        }

        // ===================== Acciones =====================

        private void btnNueva_Click(object sender, EventArgs e)
        {
            using (FrmNuevaCita dialogo = new FrmNuevaCita())
            {
                MostrarDialogo(dialogo);
            }
        }

        private void btnReprogramar_Click(object sender, EventArgs e)
        {
            CitaResumen c = Seleccionada();
            if (c == null)
            {
                return;
            }
            using (FrmNuevaCita dialogo = new FrmNuevaCita(c))
            {
                MostrarDialogo(dialogo);
            }
        }

        // Muestra el diálogo modal; si se guardó, va a la fecha de la cita y la selecciona.
        private void MostrarDialogo(FrmNuevaCita dialogo)
        {
            // Este formulario está incrustado (TopLevel = false), así que el dueño
            // del diálogo es la ventana principal que lo contiene.
            if (dialogo.ShowDialog(TopLevelControl) != DialogResult.OK)
            {
                return;
            }

            cargandoFiltros = true;
            dtpFecha.Value = dialogo.FechaProgramada;
            cargandoFiltros = false;
            ConsultarYMostrar(dialogo.IdCitaGuardada);   // Hubo cambios: se vuelve a consultar
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            CitaResumen c = Seleccionada();
            if (c == null)
            {
                return;
            }

            DialogResult confirmar = MessageBox.Show(
                "¿Cancelar la cita del turno " + c.Turno + " de " + c.NombrePaciente +
                " (" + c.HoraTexto + ", " + c.Consultorio + ")?\n\n" +
                "La cita no se borra: quedará registrada como Cancelada y la franja se liberará.",
                "Cancelar cita", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (confirmar != DialogResult.Yes)
            {
                return;
            }

            ResultadoOperacion r = negocio.Cancelar(c.IdCita);
            if (!r.Exito)
            {
                MessageBox.Show(r.MensajesComoTexto(), "No se pudo cancelar la cita",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            ConsultarYMostrar(c.IdCita);   // Hubo cambios: se vuelve a consultar
        }
    }
}
