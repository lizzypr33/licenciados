using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SigepostaAdmision.Entidades;
using SigepostaAdmision.Negocio;
using SigepostaAdmision.Seguridad;

namespace SigepostaAdmision.Presentacion
{
    /// <summary>
    /// Módulo Pacientes: búsqueda, listado, registro y edición.
    ///
    /// Decisión de diseño: este formulario NO valida reglas ni escribe SQL.
    /// Solo lee lo que escribió el usuario, se lo entrega a PacienteNegocio y
    /// muestra el resultado (los errores llegan en un ResultadoOperacion).
    ///
    /// El formulario trabaja en tres MODOS:
    ///   - Ver:    los campos son de solo lectura; se elige un paciente en la grilla.
    ///   - Nuevo:  todos los campos habilitados, incluido el DNI.
    ///   - Editar: campos habilitados excepto el DNI, que nunca se modifica.
    /// Mientras se registra o edita, la grilla y la búsqueda se bloquean para no
    /// perder por accidente los cambios al hacer clic en otra fila.
    /// </summary>
    public partial class FrmPacientes : Form
    {
        private enum Modo { Ver, Nuevo, Editar }

        private readonly PacienteNegocio negocio = new PacienteNegocio();
        private Modo modo = Modo.Ver;
        private Paciente pacienteSeleccionado;   // Paciente mostrado en el panel (modo Ver/Editar)
        private string ultimaBusqueda = string.Empty;

        // Última lista traída de la base. El filtro en vivo trabaja sobre ESTA
        // lista en memoria (LINQ), sin volver a consultar la base de datos.
        private List<Paciente> listaCargada = new List<Paciente>();

        public FrmPacientes()
        {
            InitializeComponent();
            cboSexo.Items.AddRange(new object[] { "Masculino", "Femenino" });
            Tema.Aplicar(this);   // Solo colores
        }

        private void FrmPacientes_Load(object sender, EventArgs e)
        {
            Interfaz.PonerTextoGuia(txtBuscar,"Buscar por DNI, apellido o nombre");
            Interfaz.PonerTextoGuia(txtFiltro, "Filtrar en la lista");
            CambiarModo(Modo.Ver);
            CargarLista(string.Empty);
        }

        // ===================== Búsqueda y listado =====================

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            Buscar();
        }

        // Enter dentro de la caja de búsqueda ejecuta la búsqueda.
        private void txtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;   // Evita el "beep" de Windows
                Buscar();
            }
        }

        private void Buscar()
        {
            string texto = txtBuscar.Text.Trim();
            if (!CargarLista(texto))
            {
                return;
            }

            // Si se buscó un DNI válido y no existe, se ofrece registrarlo.
            // (La regla "qué es un DNI válido" se consulta a Negocio, no se repite aquí.)
            if (grdPacientes.Rows.Count == 0 && PacienteNegocio.EsDniValido(texto))
            {
                DialogResult r = MessageBox.Show(
                    "No hay ningún paciente con DNI " + texto + ".\n¿Desea registrarlo ahora?",
                    "Paciente no encontrado", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (r == DialogResult.Yes)
                {
                    IniciarNuevo(texto);
                }
            }
        }

        /// <summary>
        /// Llena la grilla con el resultado de la búsqueda (texto vacío = recientes).
        /// Devuelve false si no se pudo consultar la base de datos.
        /// </summary>
        private bool CargarLista(string texto)
        {
            List<Paciente> lista;
            try
            {
                lista = negocio.Buscar(texto);
            }
            catch (Exception ex)
            {
                MostrarErrorBaseDatos(ex);
                return false;
            }

            ultimaBusqueda = texto;
            listaCargada = lista;
            MostrarLista();
            return true;
        }

        /// <summary>
        /// Muestra en la grilla la lista cargada, aplicando el filtro en vivo
        /// (GestorPosta.FiltrarPacientes usa LINQ sobre la lista en memoria).
        /// </summary>
        private void MostrarLista()
        {
            List<Paciente> visibles = GestorPosta.FiltrarPacientes(listaCargada, txtFiltro.Text);

            grdPacientes.Rows.Clear();
            foreach (Paciente p in visibles)
            {
                AgregarFila(p, grdPacientes.Rows.Count);
            }

            string origen = ultimaBusqueda.Length == 0 ? "Pacientes recientes" : "Resultados";
            lblResultados.Text = txtFiltro.Text.Trim().Length == 0
                ? origen + ": " + listaCargada.Count
                : origen + ": mostrando " + visibles.Count + " de " + listaCargada.Count;

            MostrarSeleccionActual();
        }

        // Filtro en vivo: se aplica mientras se escribe, sin consultar la base.
        private void txtFiltro_TextChanged(object sender, EventArgs e)
        {
            if (modo == Modo.Ver)
            {
                MostrarLista();
            }
        }

        // Agrega una fila a la grilla; el objeto Paciente se guarda en Tag.
        private void AgregarFila(Paciente p, int posicion)
        {
            grdPacientes.Rows.Insert(posicion,
                Mascara.Dni(p.Dni),   // En la lista, enmascarado; en el panel de detalle, completo
                p.Apellidos, p.Nombres, p.FechaNacimiento, p.ObtenerEdad(),
                p.Sexo.ToString(), p.Telefono);
            grdPacientes.Rows[posicion].Tag = p;
        }

        // Al cambiar de fila (solo en modo Ver) se muestran sus datos en el panel.
        private void grdPacientes_SelectionChanged(object sender, EventArgs e)
        {
            if (modo == Modo.Ver)
            {
                MostrarSeleccionActual();
            }
        }

        private void MostrarSeleccionActual()
        {
            DataGridViewRow fila = grdPacientes.CurrentRow;
            pacienteSeleccionado = fila != null ? fila.Tag as Paciente : null;

            if (pacienteSeleccionado != null)
            {
                MostrarPaciente(pacienteSeleccionado);
            }
            else
            {
                LimpiarCampos();
            }
            btnEditar.Enabled = modo == Modo.Ver && pacienteSeleccionado != null;
        }

        // ===================== Nuevo / Editar / Guardar / Cancelar =====================

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            IniciarNuevo(string.Empty);
        }

        private void IniciarNuevo(string dni)
        {
            LimpiarCampos();
            txtDni.Text = dni;
            CambiarModo(Modo.Nuevo);
            if (dni.Length == 0)
            {
                txtDni.Focus();
            }
            else
            {
                txtApellidoPaterno.Focus();
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (pacienteSeleccionado == null)
            {
                return;
            }
            CambiarModo(Modo.Editar);
            txtApellidoPaterno.Focus();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            DatosPaciente datos = LeerCampos();

            // Toda la validación ocurre en la capa de Negocio.
            ResultadoOperacion resultado = modo == Modo.Nuevo
                ? negocio.Registrar(datos)
                : negocio.Actualizar(pacienteSeleccionado.IdPaciente, datos);

            if (!resultado.Exito)
            {
                // Todos los errores juntos, en un solo mensaje.
                MessageBox.Show("No se pudo guardar el paciente:\n\n" + resultado.MensajesComoTexto(),
                    "Revise los datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool eraNuevo = modo == Modo.Nuevo;
            CambiarModo(Modo.Ver);
            RefrescarYSeleccionar(resultado.IdGenerado.Value);
            lblResultados.Text = eraNuevo ? "Paciente registrado correctamente." : "Cambios guardados correctamente.";

            // Al registrar, Negocio abre la historia clínica en la misma transacción
            // y devuelve su número para informarlo al usuario.
            if (eraNuevo)
            {
                MessageBox.Show("Paciente registrado. Historia clínica " + resultado.NumeroHistoria + " abierta.",
                    "Paciente registrado", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            CambiarModo(Modo.Ver);
            MostrarSeleccionActual();   // Vuelve a mostrar los datos originales
        }

        /// <summary>
        /// Recarga la lista actual y selecciona al paciente guardado. Si no aparece
        /// en la lista (por ejemplo, se le cambió el apellido y ya no coincide con
        /// la búsqueda), se agrega al inicio para que el usuario lo vea.
        /// </summary>
        private void RefrescarYSeleccionar(int idPaciente)
        {
            if (!CargarLista(ultimaBusqueda))
            {
                return;
            }

            DataGridViewRow fila = BuscarFila(idPaciente);
            if (fila == null)
            {
                try
                {
                    Paciente p = negocio.ObtenerPorId(idPaciente);
                    if (p == null)
                    {
                        return;
                    }
                    AgregarFila(p, 0);
                    fila = grdPacientes.Rows[0];
                }
                catch (Exception ex)
                {
                    MostrarErrorBaseDatos(ex);
                    return;
                }
            }

            grdPacientes.CurrentCell = fila.Cells[0];   // Dispara SelectionChanged
            fila.Selected = true;
            MostrarSeleccionActual();
        }

        private DataGridViewRow BuscarFila(int idPaciente)
        {
            foreach (DataGridViewRow fila in grdPacientes.Rows)
            {
                Paciente p = fila.Tag as Paciente;
                if (p != null && p.IdPaciente == idPaciente)
                {
                    return fila;
                }
            }
            return null;
        }

        // ===================== Modos y campos =====================

        /// <summary>Habilita o bloquea los controles según el modo.</summary>
        private void CambiarModo(Modo nuevo)
        {
            modo = nuevo;
            bool editable = nuevo != Modo.Ver;

            // El DNI solo se escribe al registrar; al editar queda deshabilitado.
            txtDni.ReadOnly = nuevo != Modo.Nuevo;
            txtDni.Enabled = nuevo != Modo.Editar;

            txtApellidoPaterno.ReadOnly = !editable;
            txtApellidoMaterno.ReadOnly = !editable;
            txtNombres.ReadOnly = !editable;
            txtDireccion.ReadOnly = !editable;
            txtTelefono.ReadOnly = !editable;
            dtpFechaNacimiento.Enabled = editable;
            cboSexo.Enabled = editable;

            btnGuardar.Enabled = editable;
            btnCancelar.Enabled = editable;
            btnEditar.Enabled = !editable && pacienteSeleccionado != null;

            // Mientras se edita no se puede cambiar de paciente ni buscar.
            grdPacientes.Enabled = !editable;
            txtBuscar.Enabled = !editable;
            txtFiltro.Enabled = !editable;
            btnBuscar.Enabled = !editable;
            btnNuevo.Enabled = !editable;

            switch (nuevo)
            {
                case Modo.Nuevo: lblTituloDatos.Text = "Nuevo paciente"; break;
                case Modo.Editar: lblTituloDatos.Text = "Editando paciente"; break;
                default: lblTituloDatos.Text = "Datos del paciente"; break;
            }
        }

        private void MostrarPaciente(Paciente p)
        {
            txtDni.Text = p.Dni;
            txtApellidoPaterno.Text = p.ApellidoPaterno;
            txtApellidoMaterno.Text = p.ApellidoMaterno;
            txtNombres.Text = p.Nombres;
            dtpFechaNacimiento.Value = p.FechaNacimiento;
            cboSexo.SelectedIndex = p.Sexo == 'M' ? 0 : 1;
            txtDireccion.Text = p.Direccion;
            txtTelefono.Text = p.Telefono;
            lblNumeroHistoria.Text = p.NumeroHistoriaClinica;   // Vacío si no tiene historia
        }

        private void LimpiarCampos()
        {
            txtDni.Clear();
            txtApellidoPaterno.Clear();
            txtApellidoMaterno.Clear();
            txtNombres.Clear();
            dtpFechaNacimiento.Value = DateTime.Today;
            cboSexo.SelectedIndex = -1;
            txtDireccion.Clear();
            txtTelefono.Clear();
            lblNumeroHistoria.Text = string.Empty;
        }

        // Copia los controles a un DatosPaciente "en bruto" (sin validar).
        // El ComboBox muestra Masculino/Femenino, pero se envía 'M' o 'F'.
        private DatosPaciente LeerCampos()
        {
            char sexo = '\0';
            if (cboSexo.SelectedIndex == 0) sexo = 'M';
            else if (cboSexo.SelectedIndex == 1) sexo = 'F';

            return new DatosPaciente
            {
                Dni = txtDni.Text,
                ApellidoPaterno = txtApellidoPaterno.Text,
                ApellidoMaterno = txtApellidoMaterno.Text,
                Nombres = txtNombres.Text,
                FechaNacimiento = dtpFechaNacimiento.Value.Date,
                Sexo = sexo,
                Direccion = txtDireccion.Text,
                Telefono = txtTelefono.Text
            };
        }

        private static void MostrarErrorBaseDatos(Exception ex)
        {
            Interfaz.MostrarErrorBaseDatos(ex);
        }
    }
}
