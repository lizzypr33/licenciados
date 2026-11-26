using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SigepostaAdmision.Entidades;
using SigepostaAdmision.Negocio;
using SigepostaAdmision.Seguridad;
using System.Threading.Tasks;

namespace SigepostaAdmision.Presentacion
{
    /// <summary>
    /// Módulo Historias clínicas: lista los pacientes con su historia (o "Sin
    /// historia"), muestra el detalle y las atenciones del seleccionado, y
    /// permite abrir una historia o activarla / inactivarla.
    ///
    /// Decisión de diseño: aquí no hay reglas ni SQL. Las acciones se piden a
    /// HistoriaNegocio y el formulario solo muestra el resultado. Habilitar o
    /// deshabilitar botones según si hay historia es solo presentación; la regla
    /// "una sola historia por paciente" la vuelve a comprobar Negocio.
    /// </summary>
    public partial class FrmHistorias : Form
    {
        private const string TextoSinHistoria = "Sin historia";

        private readonly HistoriaNegocio negocio = new HistoriaNegocio();
        private readonly AtencionNegocio atencionNegocio = new AtencionNegocio();
        private string ultimoFiltro = string.Empty;
        private readonly Font fuenteSinHistoria;   // Cursiva para "Sin historia" (se crea una sola vez)

        // Ver diagnósticos es solo del Administrador (la regla está en Negocio/Permisos).
        private const string AyudaSinPermiso = "Solo el Administrador puede ver diagnósticos";
        private readonly bool puedeVerDiagnostico;
        private readonly ToolTip ayuda = new ToolTip();
        private bool mostrandoAyuda;

        public FrmHistorias()
        {
            InitializeComponent();
            fuenteSinHistoria = new Font(Font, FontStyle.Italic);
            Disposed += (s, e) => { fuenteSinHistoria.Dispose(); ayuda.Dispose(); };

            puedeVerDiagnostico = Sesion.HaySesion && Permisos.PuedeVerDiagnostico(Sesion.UsuarioActual.Rol);
            if (!puedeVerDiagnostico)
            {
                ayuda.SetToolTip(btnVerDiagnostico, AyudaSinPermiso);
                // Un botón DESHABILITADO no recibe eventos del mouse, así que su ToolTip
                // no aparece solo: se muestra desde el panel que lo contiene.
                pnlDetalle.MouseMove += pnlDetalle_MouseMove;
                pnlDetalle.MouseLeave += (s, e) => OcultarAyuda();
            }
            Tema.Aplicar(this);   // Solo colores
        }

        private void pnlDetalle_MouseMove(object sender, MouseEventArgs e)
        {
            bool sobreBoton = btnVerDiagnostico.Bounds.Contains(e.Location);
            if (sobreBoton && !mostrandoAyuda)
            {
                ayuda.Show(AyudaSinPermiso, pnlDetalle, btnVerDiagnostico.Left, btnVerDiagnostico.Bottom + 2, 3000);
                mostrandoAyuda = true;
            }
            else if (!sobreBoton && mostrandoAyuda)
            {
                OcultarAyuda();
            }
        }

        private void OcultarAyuda()
        {
            ayuda.Hide(pnlDetalle);
            mostrandoAyuda = false;
        }

        // Habilita "Ver diagnóstico" solo si hay una atención elegida Y el rol lo permite.
        private void ActualizarBotonDiagnostico()
        {
            btnVerDiagnostico.Enabled = puedeVerDiagnostico &&
                grdAtenciones.CurrentRow != null && grdAtenciones.CurrentRow.Tag is int;
        }

        private void FrmHistorias_Load(object sender, EventArgs e)
        {
            Interfaz.PonerTextoGuia(txtBuscar, "Número de historia, DNI, apellido o nombre");
            CargarLista(string.Empty);
        }

        // ===================== Búsqueda =====================

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            CargarLista(txtBuscar.Text.Trim());
        }

        // Enter dentro de la caja de búsqueda ejecuta la búsqueda.
        private void txtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                CargarLista(txtBuscar.Text.Trim());
            }
        }

        /// <summary>Llena la grilla principal. Devuelve false si falló la consulta.</summary>
        private bool CargarLista(string filtro)
        {
            List<HistoriaResumen> lista;
            try
            {
                lista = negocio.Buscar(filtro);
            }
            catch (Exception ex)
            {
                Interfaz.MostrarErrorBaseDatos(ex);
                return false;
            }

            ultimoFiltro = filtro;
            grdHistorias.Rows.Clear();
            foreach (HistoriaResumen h in lista)
            {
                int i = grdHistorias.Rows.Add(
                    h.TieneHistoria ? h.NumeroHistoria : TextoSinHistoria,
                    Mascara.Dni(h.Dni),   // DNI enmascarado en la lista
                    h.NombreCompleto,
                    h.FechaApertura,                       // null se muestra vacío
                    h.Estado.HasValue ? h.Estado.Value.ToString() : string.Empty,
                    h.TotalAtenciones);

                DataGridViewRow fila = grdHistorias.Rows[i];
                fila.Tag = h;
                if (!h.TieneHistoria)
                {
                    // "Sin historia" en gris y cursiva para distinguirlo a simple vista.
                    fila.Cells[0].Style.ForeColor = Tema.TextoInactivo;   // Gris #9CA3AF (mismo valor que antes)
                    fila.Cells[0].Style.Font = fuenteSinHistoria;
                }
                else if (h.Estado == EstadoHistoria.Inactiva)
                {
                    fila.Cells[4].Style.ForeColor = Tema.TextoError;      // Rojo #B91C1C (mismo valor que antes)
                }
            }

            lblResultados.Text = "Pacientes: " + lista.Count;
            MostrarDetalle();
            return true;
        }

        private void grdHistorias_SelectionChanged(object sender, EventArgs e)
        {
            MostrarDetalle();
        }

        // ===================== Detalle =====================

        private HistoriaResumen Seleccionado()
        {
            DataGridViewRow fila = grdHistorias.CurrentRow;
            return fila != null ? fila.Tag as HistoriaResumen : null;
        }

        /// <summary>Muestra el detalle y las atenciones del paciente seleccionado.</summary>
        private void MostrarDetalle()
        {
            HistoriaResumen h = Seleccionado();
            grdAtenciones.Rows.Clear();

            btnVerDiagnostico.Enabled = false;
            if (h == null)
            {
                lblPacienteValor.Text = string.Empty;
                lblDniValor.Text = string.Empty;
                lblNumeroValor.Text = string.Empty;
                lblAperturaValor.Text = string.Empty;
                lblEstadoValor.Text = string.Empty;
                btnAbrirHistoria.Enabled = false;
                btnCambiarEstado.Enabled = false;
                btnCambiarEstado.Text = "Activar / Inactivar historia";
                return;
            }

            lblPacienteValor.Text = h.NombreCompleto;
            lblDniValor.Text = Mascara.Dni(h.Dni);
            lblNumeroValor.Text = h.TieneHistoria ? h.NumeroHistoria : TextoSinHistoria;
            lblAperturaValor.Text = h.FechaApertura.HasValue ? h.FechaApertura.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty;
            lblEstadoValor.Text = h.Estado.HasValue ? h.Estado.Value.ToString() : string.Empty;

            // "Abrir historia" solo si no tiene; "Activar / Inactivar" solo si tiene.
            btnAbrirHistoria.Enabled = !h.TieneHistoria;
            btnCambiarEstado.Enabled = h.TieneHistoria;
            btnCambiarEstado.Text = HistoriaNegocio.EstadoDestino(h.Estado.GetValueOrDefault(EstadoHistoria.Activa)) == EstadoHistoria.Activa
                ? "Activar historia" : "Inactivar historia";

            // Solo se consulta la base de datos si el paciente tiene atenciones.
            if (h.TotalAtenciones > 0)
            {
                try
                {
                    foreach (AtencionResumen a in negocio.ObtenerAtenciones(h.IdPaciente))
                    {
                        int i = grdAtenciones.Rows.Add(a.Fecha, a.Hora.ToString(@"hh\:mm"), a.Consultorio,
                                                       a.Profesional, a.Observaciones, a.RegistradaPor);
                        // Solo se guarda el Id: el diagnóstico NO se trae ni se descifra
                        // hasta que alguien pulse "Ver diagnóstico" (y queda auditado).
                        grdAtenciones.Rows[i].Tag = a.IdAtencion;
                    }
                }
                catch (Exception ex)
                {
                    Interfaz.MostrarErrorBaseDatos(ex);
                }
            }
            lblAtenciones.Text = "Atenciones registradas: " + h.TotalAtenciones;
            ActualizarBotonDiagnostico();
        }

        private void grdAtenciones_SelectionChanged(object sender, EventArgs e)
        {
            ActualizarBotonDiagnostico();
        }

        /// <summary>
        /// Muestra el diagnóstico de la atención seleccionada. Descifrar y auditar
        /// es responsabilidad de AtencionNegocio; el formulario solo muestra el texto.
        /// </summary>
        private async void btnVerDiagnostico_Click(object sender, EventArgs e)
        {
            DataGridViewRow fila = grdAtenciones.CurrentRow;
            if (fila == null || !(fila.Tag is int))
            {
                return;
            }
            int idAtencion = (int)fila.Tag;

            btnVerDiagnostico.Enabled = false;
            bool registrado = false;
            string texto;
            try
            {
                texto = await Task.Run(() => atencionNegocio.VerDiagnostico(idAtencion, out registrado));
            }
            catch (Exception ex)
            {
                texto = RegistroTecnico.ErrorParaUsuario("Historias - ver diagnóstico", ex);
            }
            if (IsDisposed)
            {
                return;
            }
            ActualizarBotonDiagnostico();

            MessageBox.Show(
                texto + (registrado ? "\n\nEste acceso quedó registrado en la auditoría." : ""),
                "Diagnóstico — atención N° " + idAtencion,
                MessageBoxButtons.OK, registrado ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
        }

        // ===================== Acciones =====================

        private void btnAbrirHistoria_Click(object sender, EventArgs e)
        {
            HistoriaResumen h = Seleccionado();
            if (h == null)
            {
                return;
            }

            ResultadoOperacion r = negocio.AbrirParaPaciente(h.IdPaciente);
            if (!r.Exito)
            {
                MostrarErrores(r);
                return;
            }

            RecargarYSeleccionar(h.IdPaciente);
            MessageBox.Show("Historia clínica " + r.NumeroHistoria + " abierta para " + h.NombreCompleto + ".",
                "Historia abierta", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnCambiarEstado_Click(object sender, EventArgs e)
        {
            HistoriaResumen h = Seleccionado();
            if (h == null || !h.TieneHistoria)
            {
                return;
            }

            // La decisión Activa ↔ Inactiva es una regla de Negocio; el formulario solo la pide.
            EstadoHistoria nuevo = HistoriaNegocio.EstadoDestino(h.Estado.GetValueOrDefault(EstadoHistoria.Activa));

            // Inactivar tiene consecuencias, así que se pide confirmación.
            if (nuevo == EstadoHistoria.Inactiva)
            {
                DialogResult confirmar = MessageBox.Show(
                    "¿Inactivar la historia " + h.NumeroHistoria + " de " + h.NombreCompleto + "?\n\n" +
                    "Una historia inactiva no podrá recibir citas nuevas. Podrá activarla de nuevo cuando lo necesite.",
                    "Inactivar historia", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (confirmar != DialogResult.Yes)
                {
                    return;
                }
            }

            ResultadoOperacion r = negocio.CambiarEstado(h.IdHistoria.Value, nuevo);
            if (!r.Exito)
            {
                MostrarErrores(r);
                return;
            }
            RecargarYSeleccionar(h.IdPaciente);
        }

        // Recarga la lista con el último filtro y vuelve a seleccionar al paciente.
        private void RecargarYSeleccionar(int idPaciente)
        {
            if (!CargarLista(ultimoFiltro))
            {
                return;
            }
            foreach (DataGridViewRow fila in grdHistorias.Rows)
            {
                HistoriaResumen h = fila.Tag as HistoriaResumen;
                if (h != null && h.IdPaciente == idPaciente)
                {
                    grdHistorias.CurrentCell = fila.Cells[0];
                    fila.Selected = true;
                    break;
                }
            }
            MostrarDetalle();
        }

        private static void MostrarErrores(ResultadoOperacion r)
        {
            MessageBox.Show(r.MensajesComoTexto(), "No se pudo completar la operación",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
