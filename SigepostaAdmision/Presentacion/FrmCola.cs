using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SigepostaAdmision.Entidades;
using SigepostaAdmision.Negocio;

namespace SigepostaAdmision.Presentacion
{
    /// <summary>
    /// Cola de atención ASÍNCRONA (Unidad 5): se actualiza sola cada 5 segundos
    /// sin congelar la ventana.
    ///
    /// ¿Por qué un hilo en segundo plano? Consultar la base de datos tarda. Si se
    /// hiciera en el HILO DE LA INTERFAZ (el que dibuja la ventana y atiende clics),
    /// la ventana quedaría "congelada" mientras tanto ("No responde"). Por eso la
    /// consulta corre en un BackgroundWorker (otro hilo).
    ///
    /// REGLA DE ORO de Windows Forms: los controles SOLO se pueden tocar desde el
    /// hilo que los creó (el de la interfaz). Si DoWork (otro hilo) cambiara una
    /// grilla, habría errores ("operación no válida a través de subprocesos") o
    /// fallos impredecibles. Por eso:
    ///   - DoWork SOLO obtiene datos (un ColaSnapshot) y NUNCA toca controles.
    ///   - Entrega los datos con ReportProgress(0, snapshot).
    ///   - ProgressChanged y RunWorkerCompleted se ejecutan automáticamente en el
    ///     hilo de la interfaz: ahí, y solo ahí, se actualizan las grillas.
    ///
    /// Las acciones de los botones usan async/await con Task.Run: el trabajo va a
    /// otro hilo y, al terminar, "await" vuelve solo al hilo de la interfaz.
    /// </summary>
    public partial class FrmCola : Form
    {
        private const int SegundosEntreActualizaciones = 5;
        private const int PasoEsperaMs = 250;          // La espera se hace en pasos cortos para poder cancelar rápido
        private const int SimulacionCargaMs = 3000;    // "Simular carga pesada"

        // Marca que DoWork envía al empezar cada ciclo (para mostrar la barra de progreso).
        private static readonly object InicioCiclo = new object();

        private static readonly Color ColorLlamado = Tema.TextoExito;   // Verde #15803D (mismo valor que antes)

        private readonly ColaNegocio colaNegocio = new ColaNegocio();
        private readonly CitaNegocio citaNegocio = new CitaNegocio();
        private readonly AtencionNegocio atencionNegocio = new AtencionNegocio();

        // --- Datos que lee el hilo en segundo plano ---
        // DoWork no puede leer controles (CheckBox, ComboBox), así que el hilo de la
        // interfaz copia sus valores en estos campos. "volatile" asegura que el otro
        // hilo vea siempre el valor más reciente.
        private volatile bool simularCarga;
        private volatile bool refrescoSolicitado;   // Corta la espera de 5 s para refrescar ya
        private volatile int idConsultorioFiltro;   // 0 = Todos
        private volatile bool cerrando;

        // --- Datos que solo usa el hilo de la interfaz ---
        private bool reiniciarAlTerminar;   // Refresco pedido mientras el worker terminaba
        private bool operacionEnCurso;      // Una acción de botón está corriendo
        private bool ignorarEventos;        // Mientras se llenan los controles por código

        public FrmCola()
        {
            InitializeComponent();
            Tema.Aplicar(this);   // Solo colores
        }

        // "async": incluso la carga inicial de consultorios se hace fuera del hilo de
        // la interfaz, para que la ventana no se congele si la base tarda en responder.
        private async void FrmCola_Load(object sender, EventArgs e)
        {
            ignorarEventos = true;
            cboConsultorio.Items.Add("Todos");
            cboConsultorio.SelectedIndex = 0;
            ignorarEventos = false;

            simularCarga = chkCarga.Checked;
            // La demostración de Rollback solo la ve el Administrador.
            btnDemoRollback.Visible = Sesion.HaySesion && Sesion.UsuarioActual.Rol == RolUsuario.Administrador;
            ActualizarBotones();
            SolicitarRefresco();   // Arranca el ciclo automático (la casilla viene activada)

            try
            {
                ConsultorioItem[] consultorios = (await Task.Run(() => citaNegocio.Consultorios(null))).ToArray();
                if (!FormularioCerrado())
                {
                    cboConsultorio.Items.AddRange(consultorios);   // Ya estamos de vuelta en el hilo de la interfaz
                }
            }
            catch (Exception)
            {
                if (!FormularioCerrado())
                {
                    lblEstado.Text = "Sin conexión: no se pudieron cargar los consultorios.";
                }
            }
        }

        // ===================== Control del BackgroundWorker (hilo de la interfaz) =====================

        /// <summary>
        /// Pide datos nuevos. Si el worker está libre, lo arranca (en ciclo continuo
        /// si "Actualización automática" está activa, o una sola vez si no). Si ya
        /// está corriendo, activa refrescoSolicitado para que corte su espera.
        /// </summary>
        private void SolicitarRefresco()
        {
            if (FormularioCerrado())
            {
                return;
            }

            if (worker.IsBusy)
            {
                refrescoSolicitado = true;
                // Si el worker va a terminar (modo "una vez" o cancelándose), se relanza al acabar.
                if (!chkAuto.Checked || worker.CancellationPending)
                {
                    reiniciarAlTerminar = true;
                }
                return;
            }

            refrescoSolicitado = false;
            worker.RunWorkerAsync(chkAuto.Checked);   // Argumento: true = ciclo continuo
        }

        /// <summary>
        /// SE EJECUTA EN OTRO HILO. Aquí está PROHIBIDO tocar controles.
        /// Ciclo: obtener snapshot → (simular carga) → ReportProgress → esperar 5 s.
        /// Una SqlException (sin conexión) sale de aquí y llega a RunWorkerCompleted.
        /// </summary>
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker w = (BackgroundWorker)sender;
            bool continuo = (bool)e.Argument;

            while (!w.CancellationPending)
            {
                w.ReportProgress(0, InicioCiclo);   // "Empieza la actualización" (muestra la barra)

                int filtro = idConsultorioFiltro;
                ColaSnapshot foto = colaNegocio.ObtenerSnapshot(DateTime.Today, null,
                    filtro == 0 ? (int?)null : filtro);

                if (simularCarga)
                {
                    // Demostración: aunque este hilo "duerma" 3 s, la ventana sigue
                    // respondiendo, porque el hilo de la interfaz está libre.
                    Thread.Sleep(SimulacionCargaMs);
                }

                if (w.CancellationPending)
                {
                    break;
                }

                // Se ENTREGAN los datos al hilo de la interfaz; él los dibujará.
                w.ReportProgress(0, foto);

                if (!continuo)
                {
                    return;
                }

                // Espera de 5 s en pasos de 250 ms: así una cancelación o un refresco
                // pedido se atienden en menos de un cuarto de segundo.
                int pasos = SegundosEntreActualizaciones * 1000 / PasoEsperaMs;
                for (int i = 0; i < pasos && !w.CancellationPending && !refrescoSolicitado; i++)
                {
                    Thread.Sleep(PasoEsperaMs);
                }
                refrescoSolicitado = false;
            }
            e.Cancel = true;
        }

        /// <summary>
        /// Se ejecuta en el HILO DE LA INTERFAZ (BackgroundWorker se encarga de
        /// pasarlo a este hilo). Por eso aquí sí se pueden actualizar controles.
        /// </summary>
        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (FormularioCerrado())
            {
                return;   // El formulario ya se cerró: no se tocan controles liberados
            }

            if (e.UserState == InicioCiclo)
            {
                pbActualizando.Visible = true;
                return;
            }

            ColaSnapshot foto = e.UserState as ColaSnapshot;
            if (foto != null)
            {
                MostrarSnapshot(foto);
                pbActualizando.Visible = operacionEnCurso;
                lblActualizacion.Text = "Última actualización:\n" + foto.Generado.ToString("HH:mm:ss");
                lblEstado.Text = "Conectado · En cola: " + foto.Cola.Count +
                                 " · Pendientes: " + foto.Pendientes.Count;
            }
        }

        /// <summary>
        /// Se ejecuta en el HILO DE LA INTERFAZ cuando DoWork termina: por
        /// cancelación, por una excepción o porque fue una actualización única.
        /// Los errores se muestran en la etiqueta de estado (sin MessageBox, para
        /// no interrumpir al usuario).
        /// </summary>
        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (FormularioCerrado())
            {
                return;
            }
            pbActualizando.Visible = operacionEnCurso;

            if (e.Error != null)
            {
                reiniciarAlTerminar = false;
                if (e.Error is SqlException)
                {
                    // Sin conexión: el worker queda detenido y un temporizador lo
                    // vuelve a lanzar en el siguiente ciclo (5 s) si sigue activo.
                    lblEstado.Text = "Sin conexión con la base de datos (" + DateTime.Now.ToString("HH:mm:ss") + "). " +
                        (chkAuto.Checked ? "Reintentando en " + SegundosEntreActualizaciones + " s…"
                                         : "Active la actualización automática para reintentar.");
                    if (chkAuto.Checked)
                    {
                        timerReintento.Start();
                    }
                }
                else
                {
                    // Error inesperado (no de conexión): se detiene y se informa con un
                    // mensaje genérico; el detalle va al registro técnico.
                    lblEstado.Text = RegistroTecnico.ErrorParaUsuario("Cola de atención", e.Error) +
                                     " La actualización automática se detuvo.";
                    ignorarEventos = true;
                    chkAuto.Checked = false;
                    ignorarEventos = false;
                }
                return;
            }

            if (e.Cancelled)
            {
                lblEstado.Text = "Actualización automática detenida.";
            }

            if (reiniciarAlTerminar)
            {
                reiniciarAlTerminar = false;
                SolicitarRefresco();
            }
        }

        // Reintento tras "Sin conexión".
        private void timerReintento_Tick(object sender, EventArgs e)
        {
            timerReintento.Stop();
            if (!FormularioCerrado() && chkAuto.Checked)
            {
                SolicitarRefresco();
            }
        }

        // ===================== Controles superiores =====================

        private void chkAuto_CheckedChanged(object sender, EventArgs e)
        {
            if (ignorarEventos)
            {
                return;
            }
            if (chkAuto.Checked)
            {
                SolicitarRefresco();
            }
            else
            {
                timerReintento.Stop();
                if (worker.IsBusy)
                {
                    worker.CancelAsync();   // DoWork lo detecta en menos de 250 ms
                }
            }
        }

        private void chkCarga_CheckedChanged(object sender, EventArgs e)
        {
            simularCarga = chkCarga.Checked;   // Copia para que la lea el otro hilo
        }

        private void cboConsultorio_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ignorarEventos)
            {
                return;
            }
            ConsultorioItem c = cboConsultorio.SelectedItem as ConsultorioItem;
            idConsultorioFiltro = c != null ? c.IdConsultorio : 0;
            SolicitarRefresco();
        }

        // ===================== Mostrar datos (solo hilo de la interfaz) =====================

        /// <summary>
        /// Vuelca el snapshot en las grillas y la lista de recordatorios, conservando
        /// la fila seleccionada (por IdCita) y la posición del scroll.
        /// </summary>
        private void MostrarSnapshot(ColaSnapshot foto)
        {
            // --- Citas pendientes ---
            int idPendiente = IdSeleccionado(grdPendientes);
            int scrollPendientes = grdPendientes.FirstDisplayedScrollingRowIndex;
            grdPendientes.Rows.Clear();
            foreach (CitaResumen c in foto.Pendientes)
            {
                int i = grdPendientes.Rows.Add(c.HoraTexto, c.NombrePaciente, c.Consultorio, c.Profesional);
                grdPendientes.Rows[i].Tag = c.IdCita;
            }
            Reseleccionar(grdPendientes, idPendiente, scrollPendientes);
            lblPendientes.Text = "Citas pendientes de hoy (" + foto.Pendientes.Count + ")";

            // --- Cola de atención ---
            int idCola = IdSeleccionado(grdCola);
            int scrollCola = grdCola.FirstDisplayedScrollingRowIndex;
            grdCola.Rows.Clear();
            foreach (ItemCola item in foto.Cola)
            {
                int i = grdCola.Rows.Add(
                    item.Posicion,
                    item.Turno,
                    item.Paciente,
                    item.Consultorio,
                    item.Profesional,
                    item.HoraLlegada.HasValue ? item.HoraLlegada.Value.ToString("HH:mm") : "",
                    item.EsperaMinutos,
                    item.Llamado ? "—" : item.EsperaEstimadaMinutos.ToString(),
                    item.Llamado ? "Llamado" : "Esperando");

                DataGridViewRow fila = grdCola.Rows[i];
                // Se guarda el ItemCola completo: las decisiones (¿está llamado?) usan
                // su campo booleano Llamado, no el texto que muestra la grilla.
                fila.Tag = item;
                if (item.Llamado)
                {
                    fila.DefaultCellStyle.ForeColor = ColorLlamado;
                }
            }
            Reseleccionar(grdCola, idCola, scrollCola);
            lblCola.Text = "Cola de atención (" + foto.Cola.Count + ")";

            // --- Recordatorios (simulados) ---
            lstRecordatorios.BeginUpdate();
            lstRecordatorios.Items.Clear();
            if (foto.Recordatorios.Count == 0)
            {
                lstRecordatorios.Items.Add("(No hay citas pendientes en la próxima hora)");
            }
            else
            {
                lstRecordatorios.Items.AddRange(foto.Recordatorios.ToArray());
            }
            lstRecordatorios.EndUpdate();

            ActualizarBotones();
        }

        private static int IdSeleccionado(DataGridView grilla)
        {
            return IdDeFila(grilla.CurrentRow);
        }

        // IdCita de una fila: en "pendientes" el Tag es el IdCita; en la cola, el ItemCola.
        private static int IdDeFila(DataGridViewRow fila)
        {
            if (fila == null) return 0;
            if (fila.Tag is ItemCola) return ((ItemCola)fila.Tag).IdCita;
            return fila.Tag is int ? (int)fila.Tag : 0;
        }

        // ItemCola de una fila de la cola (null si no hay).
        private static ItemCola ItemDeFila(DataGridViewRow fila)
        {
            return fila != null ? fila.Tag as ItemCola : null;
        }

        // Vuelve a seleccionar la fila con ese IdCita (si sigue en la grilla) y
        // restaura el scroll, para que la selección no "salte" en cada refresco.
        private static void Reseleccionar(DataGridView grilla, int idCita, int scroll)
        {
            if (idCita > 0)
            {
                foreach (DataGridViewRow fila in grilla.Rows)
                {
                    if (IdDeFila(fila) == idCita)
                    {
                        grilla.CurrentCell = fila.Cells[0];
                        break;
                    }
                }
            }
            if (scroll >= 0 && scroll < grilla.Rows.Count)
            {
                grilla.FirstDisplayedScrollingRowIndex = scroll;
            }
        }

        private void Grilla_SelectionChanged(object sender, EventArgs e)
        {
            ActualizarBotones();
        }

        private void ActualizarBotones()
        {
            bool libre = !operacionEnCurso;
            btnRegistrarLlegada.Enabled = libre && IdSeleccionado(grdPendientes) > 0;
            btnDeshacerLlegada.Enabled = libre && IdSeleccionado(grdCola) > 0;
            // "Llamar siguiente" solo si alguien de la cola aún no fue llamado
            // (se usa el booleano ItemCola.Llamado, no el texto de la columna Estado).
            btnLlamarSiguiente.Enabled = libre && grdCola.Rows.Cast<DataGridViewRow>()
                .Select(ItemDeFila).Any(item => item != null && !item.Llamado);
            // "Registrar atención" (y la demo) solo si el seleccionado ya fue llamado.
            ItemCola seleccionado = ItemDeFila(grdCola.CurrentRow);
            bool llamadoSeleccionado = libre && seleccionado != null && seleccionado.Llamado;
            btnRegistrarAtencion.Enabled = llamadoSeleccionado;
            btnDemoRollback.Enabled = llamadoSeleccionado;
        }

        // ===================== Acciones (en segundo plano con async/await) =====================

        private async void btnRegistrarLlegada_Click(object sender, EventArgs e)
        {
            int idCita = IdSeleccionado(grdPendientes);
            if (idCita == 0)
            {
                return;
            }
            string paciente = Convert.ToString(grdPendientes.CurrentRow.Cells[colPacientePend.Index].Value);

            ResultadoOperacion r = await EjecutarEnSegundoPlano(() => colaNegocio.RegistrarLlegada(idCita));
            if (r == null || FormularioCerrado())
            {
                return;
            }

            if (r.Exito)
            {
                lblEstado.Text = "Llegada registrada: " + paciente + " (" + DateTime.Now.ToString("HH:mm") + ").";
            }
            else
            {
                MessageBox.Show(r.MensajesComoTexto(), "No se pudo registrar la llegada",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            SolicitarRefresco();
        }

        private async void btnDeshacerLlegada_Click(object sender, EventArgs e)
        {
            int idCita = IdSeleccionado(grdCola);
            if (idCita == 0)
            {
                return;
            }
            string paciente = Convert.ToString(grdCola.CurrentRow.Cells[colPacienteCola.Index].Value);

            if (MessageBox.Show("¿Deshacer la llegada de " + paciente + "?\nLa cita volverá a Pendiente.",
                    "Deshacer llegada", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                return;
            }

            ResultadoOperacion r = await EjecutarEnSegundoPlano(() => colaNegocio.DeshacerLlegada(idCita));
            if (r == null || FormularioCerrado())
            {
                return;
            }

            if (r.Exito)
            {
                lblEstado.Text = "Llegada deshecha: " + paciente + " volvió a Pendiente.";
            }
            else
            {
                MessageBox.Show(r.MensajesComoTexto(), "No se pudo deshacer la llegada",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            SolicitarRefresco();
        }

        private async void btnLlamarSiguiente_Click(object sender, EventArgs e)
        {
            ConsultorioItem consultorio = cboConsultorio.SelectedItem as ConsultorioItem;
            int? idConsultorio = consultorio != null ? consultorio.IdConsultorio : (int?)null;

            // Se envuelve en Tuple para distinguir "error" (null) de "nadie en cola" (Item1 null).
            Tuple<CitaResumen> resultado = await EjecutarEnSegundoPlano(
                () => Tuple.Create(colaNegocio.LlamarSiguiente(idConsultorio)));
            if (resultado == null || FormularioCerrado())
            {
                return;
            }

            CitaResumen llamado = resultado.Item1;
            if (llamado == null)
            {
                MessageBox.Show("No hay pacientes esperando" +
                    (consultorio != null ? " en " + consultorio.Nombre : "") + ".",
                    "Llamar siguiente", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                lblEstado.Text = "Llamado: " + llamado.NombrePaciente + " → " + llamado.Consultorio + ".";
                MessageBox.Show(llamado.NombrePaciente + "\n" + llamado.Consultorio + " · turno " + llamado.Turno,
                    "Paciente llamado", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            SolicitarRefresco();
        }

        // Abre el diálogo de atención; al registrarla con éxito, refresca la cola.
        private void btnRegistrarAtencion_Click(object sender, EventArgs e)
        {
            int idCita = IdSeleccionado(grdCola);
            if (idCita == 0)
            {
                return;
            }
            using (FrmRegistrarAtencion dialogo = new FrmRegistrarAtencion(idCita))
            {
                // Este formulario está incrustado: el dueño del diálogo es la ventana principal.
                if (dialogo.ShowDialog(TopLevelControl) == DialogResult.OK && !FormularioCerrado())
                {
                    lblEstado.Text = "Atención N° " + dialogo.IdAtencionRegistrada + " registrada.";
                    SolicitarRefresco();
                }
            }
        }

        /// <summary>
        /// SOLO Administrador: intenta registrar la atención con un fallo provocado
        /// a mitad de la transacción y muestra el estado antes y después, para
        /// evidenciar que el Rollback no dejó nada guardado.
        /// </summary>
        private async void btnDemoRollback_Click(object sender, EventArgs e)
        {
            int idCita = IdSeleccionado(grdCola);
            if (idCita == 0)
            {
                return;
            }
            string paciente = Convert.ToString(grdCola.CurrentRow.Cells[colPacienteCola.Index].Value);

            if (MessageBox.Show(
                    "Demostración de Rollback con la cita de " + paciente + ".\n\n" +
                    "Se intentará registrar su atención y se provocará un error a mitad de la transacción. " +
                    "No debería quedar nada guardado.\n\n¿Continuar?",
                    "Demo rollback", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                return;
            }

            string informe = await EjecutarEnSegundoPlano(() => atencionNegocio.DemoRollback(idCita));
            if (informe == null || FormularioCerrado())
            {
                return;
            }
            MessageBox.Show(informe, "Demo rollback", MessageBoxButtons.OK, MessageBoxIcon.Information);
            SolicitarRefresco();
        }

        /// <summary>
        /// Ejecuta una acción de Negocio en un hilo del ThreadPool (Task.Run) y
        /// espera su resultado SIN bloquear la ventana. Tras "await", el código
        /// continúa automáticamente en el hilo de la interfaz, por eso aquí sí se
        /// pueden tocar controles. Devuelve null si hubo una excepción.
        /// </summary>
        private async Task<T> EjecutarEnSegundoPlano<T>(Func<T> trabajo) where T : class
        {
            operacionEnCurso = true;
            pbActualizando.Visible = true;
            ActualizarBotones();
            try
            {
                return await Task.Run(trabajo);
            }
            catch (Exception ex)
            {
                if (!FormularioCerrado())
                {
                    lblEstado.Text = ex is SqlException
                        ? "Sin conexión: no se pudo completar la acción."
                        : RegistroTecnico.ErrorParaUsuario("Cola de atención - acción", ex);
                }
                return null;
            }
            finally
            {
                if (!FormularioCerrado())
                {
                    operacionEnCurso = false;
                    pbActualizando.Visible = false;
                    ActualizarBotones();
                }
            }
        }

        // ===================== Cierre seguro =====================

        /// <summary>true si el formulario se está cerrando o ya fue liberado.</summary>
        private bool FormularioCerrado()
        {
            return cerrando || IsDisposed || Disposing;
        }

        // Al cerrar (o al cambiar de módulo en FrmPrincipal) se detiene todo.
        private void FrmCola_FormClosing(object sender, FormClosingEventArgs e)
        {
            Detener();
        }

        // Respaldo: si el formulario se libera sin pasar por FormClosing.
        protected override void OnHandleDestroyed(EventArgs e)
        {
            Detener();
            base.OnHandleDestroyed(e);
        }

        private void Detener()
        {
            cerrando = true;   // A partir de aquí ningún evento toca controles
            timerReintento.Stop();
            if (worker.IsBusy)
            {
                worker.CancelAsync();
            }
        }
    }
}
