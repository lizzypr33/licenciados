namespace SigepostaAdmision.Presentacion
{
    partial class FrmCitas
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle estiloEncabezado = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle estiloTurno = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlFiltros = new System.Windows.Forms.Panel();
            this.lblFecha = new System.Windows.Forms.Label();
            this.dtpFecha = new System.Windows.Forms.DateTimePicker();
            this.lblConsultorio = new System.Windows.Forms.Label();
            this.cboConsultorio = new System.Windows.Forms.ComboBox();
            this.lblEstado = new System.Windows.Forms.Label();
            this.cboEstado = new System.Windows.Forms.ComboBox();
            this.btnActualizar = new System.Windows.Forms.Button();
            this.grdCitas = new System.Windows.Forms.DataGridView();
            this.colTurno = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHora = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPaciente = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDni = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colConsultorio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProfesional = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEstado = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlAcciones = new System.Windows.Forms.Panel();
            this.btnNueva = new System.Windows.Forms.Button();
            this.btnReprogramar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.lblResumen = new System.Windows.Forms.Label();
            this.pnlResumen = new System.Windows.Forms.Panel();
            this.pnlCardTotal = new System.Windows.Forms.Panel();
            this.lblValorTotal = new System.Windows.Forms.Label();
            this.lblTextoTotal = new System.Windows.Forms.Label();
            this.pnlCardPendientes = new System.Windows.Forms.Panel();
            this.lblValorPendientes = new System.Windows.Forms.Label();
            this.lblTextoPendientes = new System.Windows.Forms.Label();
            this.pnlCardEnCola = new System.Windows.Forms.Panel();
            this.lblValorEnCola = new System.Windows.Forms.Label();
            this.lblTextoEnCola = new System.Windows.Forms.Label();
            this.pnlCardAtendidas = new System.Windows.Forms.Panel();
            this.lblValorAtendidas = new System.Windows.Forms.Label();
            this.lblTextoAtendidas = new System.Windows.Forms.Label();
            this.lblProxima = new System.Windows.Forms.Label();
            this.pnlResumen.SuspendLayout();
            this.pnlCardTotal.SuspendLayout();
            this.pnlCardPendientes.SuspendLayout();
            this.pnlCardEnCola.SuspendLayout();
            this.pnlCardAtendidas.SuspendLayout();
            this.pnlFiltros.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdCitas)).BeginInit();
            this.pnlAcciones.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlFiltros (barra superior de filtros)
            //
            this.pnlFiltros.Controls.Add(this.lblFecha);
            this.pnlFiltros.Controls.Add(this.dtpFecha);
            this.pnlFiltros.Controls.Add(this.lblConsultorio);
            this.pnlFiltros.Controls.Add(this.cboConsultorio);
            this.pnlFiltros.Controls.Add(this.lblEstado);
            this.pnlFiltros.Controls.Add(this.cboEstado);
            this.pnlFiltros.Controls.Add(this.btnActualizar);
            this.pnlFiltros.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFiltros.Name = "pnlFiltros";
            this.pnlFiltros.Size = new System.Drawing.Size(814, 52);
            this.pnlFiltros.TabIndex = 0;
            //
            // lblFecha
            //
            this.lblFecha.AutoSize = true;
            this.lblFecha.Location = new System.Drawing.Point(12, 18);
            this.lblFecha.Name = "lblFecha";
            this.lblFecha.TabIndex = 0;
            this.lblFecha.Text = "Fecha:";
            //
            // dtpFecha
            //
            this.dtpFecha.CustomFormat = "ddd dd/MM/yyyy";
            this.dtpFecha.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFecha.Location = new System.Drawing.Point(58, 14);
            this.dtpFecha.Name = "dtpFecha";
            this.dtpFecha.Size = new System.Drawing.Size(130, 23);
            this.dtpFecha.TabIndex = 1;
            this.dtpFecha.ValueChanged += new System.EventHandler(this.dtpFecha_ValueChanged);
            //
            // lblConsultorio
            //
            this.lblConsultorio.AutoSize = true;
            this.lblConsultorio.Location = new System.Drawing.Point(200, 18);
            this.lblConsultorio.Name = "lblConsultorio";
            this.lblConsultorio.TabIndex = 2;
            this.lblConsultorio.Text = "Consultorio:";
            //
            // cboConsultorio
            //
            this.cboConsultorio.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboConsultorio.Location = new System.Drawing.Point(276, 14);
            this.cboConsultorio.Name = "cboConsultorio";
            this.cboConsultorio.Size = new System.Drawing.Size(200, 23);
            this.cboConsultorio.TabIndex = 3;
            this.cboConsultorio.SelectedIndexChanged += new System.EventHandler(this.FiltroMemoria_Changed);
            //
            // lblEstado
            //
            this.lblEstado.AutoSize = true;
            this.lblEstado.Location = new System.Drawing.Point(488, 18);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.TabIndex = 4;
            this.lblEstado.Text = "Estado:";
            //
            // cboEstado
            //
            this.cboEstado.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEstado.Location = new System.Drawing.Point(538, 14);
            this.cboEstado.Name = "cboEstado";
            this.cboEstado.Size = new System.Drawing.Size(120, 23);
            this.cboEstado.TabIndex = 5;
            this.cboEstado.SelectedIndexChanged += new System.EventHandler(this.FiltroMemoria_Changed);
            //
            // btnActualizar
            //
            this.btnActualizar.Location = new System.Drawing.Point(670, 12);
            this.btnActualizar.Name = "btnActualizar";
            this.btnActualizar.Size = new System.Drawing.Size(100, 28);
            this.btnActualizar.TabIndex = 6;
            this.btnActualizar.Text = "Actualizar";
            this.btnActualizar.UseVisualStyleBackColor = true;
            this.btnActualizar.Click += new System.EventHandler(this.btnActualizar_Click);
            //
            // grdCitas
            //
            this.grdCitas.AllowUserToAddRows = false;
            this.grdCitas.AllowUserToDeleteRows = false;
            this.grdCitas.AllowUserToResizeRows = false;
            this.grdCitas.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grdCitas.BackgroundColor = System.Drawing.Color.White;
            this.grdCitas.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.grdCitas.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.grdCitas.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            estiloEncabezado.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            estiloEncabezado.BackColor = System.Drawing.Color.FromArgb(243, 244, 246);
            estiloEncabezado.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            estiloEncabezado.ForeColor = System.Drawing.Color.FromArgb(55, 65, 81);
            estiloEncabezado.SelectionBackColor = System.Drawing.Color.FromArgb(243, 244, 246);
            estiloEncabezado.SelectionForeColor = System.Drawing.Color.FromArgb(55, 65, 81);
            this.grdCitas.ColumnHeadersDefaultCellStyle = estiloEncabezado;
            this.grdCitas.ColumnHeadersHeight = 32;
            this.grdCitas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.grdCitas.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTurno,
            this.colHora,
            this.colPaciente,
            this.colDni,
            this.colConsultorio,
            this.colProfesional,
            this.colEstado});
            this.grdCitas.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(219, 234, 254);
            this.grdCitas.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(30, 64, 175);
            this.grdCitas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdCitas.EnableHeadersVisualStyles = false;
            this.grdCitas.GridColor = System.Drawing.Color.FromArgb(229, 231, 235);
            this.grdCitas.MultiSelect = false;
            this.grdCitas.Name = "grdCitas";
            this.grdCitas.ReadOnly = true;
            this.grdCitas.RowHeadersVisible = false;
            this.grdCitas.RowTemplate.Height = 26;
            this.grdCitas.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdCitas.TabIndex = 1;
            this.grdCitas.SelectionChanged += new System.EventHandler(this.grdCitas_SelectionChanged);
            //
            // colTurno
            //
            estiloTurno.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colTurno.DefaultCellStyle = estiloTurno;
            this.colTurno.FillWeight = 40F;
            this.colTurno.HeaderText = "Turno";
            this.colTurno.Name = "colTurno";
            this.colTurno.ReadOnly = true;
            //
            // colHora
            //
            this.colHora.FillWeight = 45F;
            this.colHora.HeaderText = "Hora";
            this.colHora.Name = "colHora";
            this.colHora.ReadOnly = true;
            //
            // colPaciente
            //
            this.colPaciente.FillWeight = 160F;
            this.colPaciente.HeaderText = "Paciente";
            this.colPaciente.Name = "colPaciente";
            this.colPaciente.ReadOnly = true;
            //
            // colDni
            //
            this.colDni.FillWeight = 60F;
            this.colDni.HeaderText = "DNI";
            this.colDni.Name = "colDni";
            this.colDni.ReadOnly = true;
            //
            // colConsultorio
            //
            this.colConsultorio.FillWeight = 120F;
            this.colConsultorio.HeaderText = "Consultorio";
            this.colConsultorio.Name = "colConsultorio";
            this.colConsultorio.ReadOnly = true;
            //
            // colProfesional
            //
            this.colProfesional.FillWeight = 120F;
            this.colProfesional.HeaderText = "Profesional";
            this.colProfesional.Name = "colProfesional";
            this.colProfesional.ReadOnly = true;
            //
            // colEstado
            //
            this.colEstado.FillWeight = 60F;
            this.colEstado.HeaderText = "Estado";
            this.colEstado.Name = "colEstado";
            this.colEstado.ReadOnly = true;
            //
            // pnlAcciones (botones inferiores)
            //
            this.pnlAcciones.BackColor = System.Drawing.Color.FromArgb(249, 250, 251);
            this.pnlAcciones.Controls.Add(this.btnNueva);
            this.pnlAcciones.Controls.Add(this.btnReprogramar);
            this.pnlAcciones.Controls.Add(this.btnCancelar);
            this.pnlAcciones.Controls.Add(this.lblResumen);
            this.pnlAcciones.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlAcciones.Name = "pnlAcciones";
            this.pnlAcciones.Size = new System.Drawing.Size(814, 56);
            this.pnlAcciones.TabIndex = 2;
            //
            // btnNueva
            //
            this.btnNueva.Location = new System.Drawing.Point(12, 13);
            this.btnNueva.Name = "btnNueva";
            this.btnNueva.Size = new System.Drawing.Size(120, 30);
            this.btnNueva.TabIndex = 0;
            this.btnNueva.Text = "Nueva cita";
            this.btnNueva.UseVisualStyleBackColor = true;
            this.btnNueva.Click += new System.EventHandler(this.btnNueva_Click);
            //
            // btnReprogramar
            //
            this.btnReprogramar.Enabled = false;
            this.btnReprogramar.Location = new System.Drawing.Point(140, 13);
            this.btnReprogramar.Name = "btnReprogramar";
            this.btnReprogramar.Size = new System.Drawing.Size(120, 30);
            this.btnReprogramar.TabIndex = 1;
            this.btnReprogramar.Text = "Reprogramar";
            this.btnReprogramar.UseVisualStyleBackColor = true;
            this.btnReprogramar.Click += new System.EventHandler(this.btnReprogramar_Click);
            //
            // btnCancelar
            //
            this.btnCancelar.Enabled = false;
            this.btnCancelar.Location = new System.Drawing.Point(268, 13);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(120, 30);
            this.btnCancelar.TabIndex = 2;
            this.btnCancelar.Text = "Cancelar cita";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            //
            // lblResumen
            //
            this.lblResumen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblResumen.ForeColor = System.Drawing.Color.FromArgb(107, 114, 128);
            this.lblResumen.Location = new System.Drawing.Point(502, 13);
            this.lblResumen.Name = "lblResumen";
            this.lblResumen.Size = new System.Drawing.Size(300, 30);
            this.lblResumen.TabIndex = 3;
            this.lblResumen.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // pnlResumen (tarjetas de resumen del día)
            //
            this.pnlResumen.Controls.Add(this.pnlCardTotal);
            this.pnlResumen.Controls.Add(this.pnlCardPendientes);
            this.pnlResumen.Controls.Add(this.pnlCardEnCola);
            this.pnlResumen.Controls.Add(this.pnlCardAtendidas);
            this.pnlResumen.Controls.Add(this.lblProxima);
            this.pnlResumen.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlResumen.Name = "pnlResumen";
            this.pnlResumen.Size = new System.Drawing.Size(814, 76);
            this.pnlResumen.TabIndex = 3;
            //
            // pnlCardTotal
            //
            this.pnlCardTotal.BackColor = System.Drawing.Color.FromArgb(243, 244, 246);
            this.pnlCardTotal.Controls.Add(this.lblValorTotal);
            this.pnlCardTotal.Controls.Add(this.lblTextoTotal);
            this.pnlCardTotal.Location = new System.Drawing.Point(12, 6);
            this.pnlCardTotal.Name = "pnlCardTotal";
            this.pnlCardTotal.Size = new System.Drawing.Size(150, 62);
            this.pnlCardTotal.TabIndex = 0;
            //
            // lblValorTotal
            //
            this.lblValorTotal.AutoSize = true;
            this.lblValorTotal.Font = new System.Drawing.Font("Segoe UI Semibold", 16F);
            this.lblValorTotal.ForeColor = System.Drawing.Color.FromArgb(17, 24, 39);
            this.lblValorTotal.Location = new System.Drawing.Point(10, 3);
            this.lblValorTotal.Name = "lblValorTotal";
            this.lblValorTotal.TabIndex = 0;
            this.lblValorTotal.Text = "0";
            //
            // lblTextoTotal
            //
            this.lblTextoTotal.AutoSize = true;
            this.lblTextoTotal.ForeColor = System.Drawing.Color.FromArgb(107, 114, 128);
            this.lblTextoTotal.Location = new System.Drawing.Point(12, 38);
            this.lblTextoTotal.Name = "lblTextoTotal";
            this.lblTextoTotal.TabIndex = 1;
            this.lblTextoTotal.Text = "Citas del día";
            //
            // pnlCardPendientes
            //
            this.pnlCardPendientes.BackColor = System.Drawing.Color.FromArgb(243, 244, 246);
            this.pnlCardPendientes.Controls.Add(this.lblValorPendientes);
            this.pnlCardPendientes.Controls.Add(this.lblTextoPendientes);
            this.pnlCardPendientes.Location = new System.Drawing.Point(170, 6);
            this.pnlCardPendientes.Name = "pnlCardPendientes";
            this.pnlCardPendientes.Size = new System.Drawing.Size(150, 62);
            this.pnlCardPendientes.TabIndex = 1;
            //
            // lblValorPendientes
            //
            this.lblValorPendientes.AutoSize = true;
            this.lblValorPendientes.Font = new System.Drawing.Font("Segoe UI Semibold", 16F);
            this.lblValorPendientes.ForeColor = System.Drawing.Color.FromArgb(17, 24, 39);
            this.lblValorPendientes.Location = new System.Drawing.Point(10, 3);
            this.lblValorPendientes.Name = "lblValorPendientes";
            this.lblValorPendientes.TabIndex = 0;
            this.lblValorPendientes.Text = "0";
            //
            // lblTextoPendientes
            //
            this.lblTextoPendientes.AutoSize = true;
            this.lblTextoPendientes.ForeColor = System.Drawing.Color.FromArgb(107, 114, 128);
            this.lblTextoPendientes.Location = new System.Drawing.Point(12, 38);
            this.lblTextoPendientes.Name = "lblTextoPendientes";
            this.lblTextoPendientes.TabIndex = 1;
            this.lblTextoPendientes.Text = "Pendientes";
            //
            // pnlCardEnCola
            //
            this.pnlCardEnCola.BackColor = System.Drawing.Color.FromArgb(243, 244, 246);
            this.pnlCardEnCola.Controls.Add(this.lblValorEnCola);
            this.pnlCardEnCola.Controls.Add(this.lblTextoEnCola);
            this.pnlCardEnCola.Location = new System.Drawing.Point(328, 6);
            this.pnlCardEnCola.Name = "pnlCardEnCola";
            this.pnlCardEnCola.Size = new System.Drawing.Size(150, 62);
            this.pnlCardEnCola.TabIndex = 2;
            //
            // lblValorEnCola
            //
            this.lblValorEnCola.AutoSize = true;
            this.lblValorEnCola.Font = new System.Drawing.Font("Segoe UI Semibold", 16F);
            this.lblValorEnCola.ForeColor = System.Drawing.Color.FromArgb(17, 24, 39);
            this.lblValorEnCola.Location = new System.Drawing.Point(10, 3);
            this.lblValorEnCola.Name = "lblValorEnCola";
            this.lblValorEnCola.TabIndex = 0;
            this.lblValorEnCola.Text = "0";
            //
            // lblTextoEnCola
            //
            this.lblTextoEnCola.AutoSize = true;
            this.lblTextoEnCola.ForeColor = System.Drawing.Color.FromArgb(107, 114, 128);
            this.lblTextoEnCola.Location = new System.Drawing.Point(12, 38);
            this.lblTextoEnCola.Name = "lblTextoEnCola";
            this.lblTextoEnCola.TabIndex = 1;
            this.lblTextoEnCola.Text = "En cola";
            //
            // pnlCardAtendidas
            //
            this.pnlCardAtendidas.BackColor = System.Drawing.Color.FromArgb(243, 244, 246);
            this.pnlCardAtendidas.Controls.Add(this.lblValorAtendidas);
            this.pnlCardAtendidas.Controls.Add(this.lblTextoAtendidas);
            this.pnlCardAtendidas.Location = new System.Drawing.Point(486, 6);
            this.pnlCardAtendidas.Name = "pnlCardAtendidas";
            this.pnlCardAtendidas.Size = new System.Drawing.Size(150, 62);
            this.pnlCardAtendidas.TabIndex = 3;
            //
            // lblValorAtendidas
            //
            this.lblValorAtendidas.AutoSize = true;
            this.lblValorAtendidas.Font = new System.Drawing.Font("Segoe UI Semibold", 16F);
            this.lblValorAtendidas.ForeColor = System.Drawing.Color.FromArgb(17, 24, 39);
            this.lblValorAtendidas.Location = new System.Drawing.Point(10, 3);
            this.lblValorAtendidas.Name = "lblValorAtendidas";
            this.lblValorAtendidas.TabIndex = 0;
            this.lblValorAtendidas.Text = "0";
            //
            // lblTextoAtendidas
            //
            this.lblTextoAtendidas.AutoSize = true;
            this.lblTextoAtendidas.ForeColor = System.Drawing.Color.FromArgb(107, 114, 128);
            this.lblTextoAtendidas.Location = new System.Drawing.Point(12, 38);
            this.lblTextoAtendidas.Name = "lblTextoAtendidas";
            this.lblTextoAtendidas.TabIndex = 1;
            this.lblTextoAtendidas.Text = "Atendidas";
            //
            // lblProxima (próxima cita pendiente)
            //
            this.lblProxima.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lblProxima.AutoEllipsis = true;
            this.lblProxima.BackColor = System.Drawing.Color.FromArgb(239, 246, 255);
            this.lblProxima.ForeColor = System.Drawing.Color.FromArgb(30, 64, 175);
            this.lblProxima.Location = new System.Drawing.Point(644, 6);
            this.lblProxima.Name = "lblProxima";
            this.lblProxima.Padding = new System.Windows.Forms.Padding(8, 0, 4, 0);
            this.lblProxima.Size = new System.Drawing.Size(158, 62);
            this.lblProxima.TabIndex = 4;
            this.lblProxima.Text = "Próxima cita:\n—";
            this.lblProxima.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // FrmCitas
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(814, 520);
            // La grilla (Dock = Fill) se agrega primero para ocupar el espacio entre las barras.
            // De los paneles de arriba, el último agregado queda más arriba: filtros y luego resumen.
            this.Controls.Add(this.grdCitas);
            this.Controls.Add(this.pnlResumen);
            this.Controls.Add(this.pnlFiltros);
            this.Controls.Add(this.pnlAcciones);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "FrmCitas";
            this.Text = "Citas";
            this.Load += new System.EventHandler(this.FrmCitas_Load);
            this.pnlFiltros.ResumeLayout(false);
            this.pnlFiltros.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdCitas)).EndInit();
            this.pnlAcciones.ResumeLayout(false);
            this.pnlCardTotal.ResumeLayout(false);
            this.pnlCardTotal.PerformLayout();
            this.pnlCardPendientes.ResumeLayout(false);
            this.pnlCardPendientes.PerformLayout();
            this.pnlCardEnCola.ResumeLayout(false);
            this.pnlCardEnCola.PerformLayout();
            this.pnlCardAtendidas.ResumeLayout(false);
            this.pnlCardAtendidas.PerformLayout();
            this.pnlResumen.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlFiltros;
        private System.Windows.Forms.Label lblFecha;
        private System.Windows.Forms.DateTimePicker dtpFecha;
        private System.Windows.Forms.Label lblConsultorio;
        private System.Windows.Forms.ComboBox cboConsultorio;
        private System.Windows.Forms.Label lblEstado;
        private System.Windows.Forms.ComboBox cboEstado;
        private System.Windows.Forms.Button btnActualizar;
        private System.Windows.Forms.DataGridView grdCitas;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTurno;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHora;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPaciente;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDni;
        private System.Windows.Forms.DataGridViewTextBoxColumn colConsultorio;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProfesional;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEstado;
        private System.Windows.Forms.Panel pnlAcciones;
        private System.Windows.Forms.Button btnNueva;
        private System.Windows.Forms.Button btnReprogramar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Label lblResumen;
        private System.Windows.Forms.Panel pnlResumen;
        private System.Windows.Forms.Panel pnlCardTotal;
        private System.Windows.Forms.Label lblValorTotal;
        private System.Windows.Forms.Label lblTextoTotal;
        private System.Windows.Forms.Panel pnlCardPendientes;
        private System.Windows.Forms.Label lblValorPendientes;
        private System.Windows.Forms.Label lblTextoPendientes;
        private System.Windows.Forms.Panel pnlCardEnCola;
        private System.Windows.Forms.Label lblValorEnCola;
        private System.Windows.Forms.Label lblTextoEnCola;
        private System.Windows.Forms.Panel pnlCardAtendidas;
        private System.Windows.Forms.Label lblValorAtendidas;
        private System.Windows.Forms.Label lblTextoAtendidas;
        private System.Windows.Forms.Label lblProxima;
    }
}
