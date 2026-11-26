namespace SigepostaAdmision.Presentacion
{
    partial class FrmHistorias
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
            System.Windows.Forms.DataGridViewCellStyle estiloFechaApertura = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle estiloAtencionesNum = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle estiloEncabezado2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle estiloFechaAtencion = new System.Windows.Forms.DataGridViewCellStyle();
            this.grdHistorias = new System.Windows.Forms.DataGridView();
            this.colNumero = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDni = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPaciente = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFechaApertura = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEstado = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAtenciones = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlBusqueda = new System.Windows.Forms.Panel();
            this.txtBuscar = new System.Windows.Forms.TextBox();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.lblResultados = new System.Windows.Forms.Label();
            this.pnlDetalle = new System.Windows.Forms.Panel();
            this.lblTituloDetalle = new System.Windows.Forms.Label();
            this.lblPaciente = new System.Windows.Forms.Label();
            this.lblPacienteValor = new System.Windows.Forms.Label();
            this.lblDni = new System.Windows.Forms.Label();
            this.lblDniValor = new System.Windows.Forms.Label();
            this.lblNumero = new System.Windows.Forms.Label();
            this.lblNumeroValor = new System.Windows.Forms.Label();
            this.lblApertura = new System.Windows.Forms.Label();
            this.lblAperturaValor = new System.Windows.Forms.Label();
            this.lblEstado = new System.Windows.Forms.Label();
            this.lblEstadoValor = new System.Windows.Forms.Label();
            this.btnAbrirHistoria = new System.Windows.Forms.Button();
            this.btnCambiarEstado = new System.Windows.Forms.Button();
            this.lblAtenciones = new System.Windows.Forms.Label();
            this.grdAtenciones = new System.Windows.Forms.DataGridView();
            this.btnVerDiagnostico = new System.Windows.Forms.Button();
            this.colFechaAtencion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHoraAtencion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colConsultorio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProfesionalAtencion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colObservaciones = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRegistradaPor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.grdHistorias)).BeginInit();
            this.pnlBusqueda.SuspendLayout();
            this.pnlDetalle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdAtenciones)).BeginInit();
            this.SuspendLayout();
            //
            // grdHistorias
            //
            this.grdHistorias.AllowUserToAddRows = false;
            this.grdHistorias.AllowUserToDeleteRows = false;
            this.grdHistorias.AllowUserToResizeRows = false;
            this.grdHistorias.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grdHistorias.BackgroundColor = System.Drawing.Color.White;
            this.grdHistorias.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.grdHistorias.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.grdHistorias.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            estiloEncabezado.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            estiloEncabezado.BackColor = System.Drawing.Color.FromArgb(243, 244, 246);
            estiloEncabezado.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            estiloEncabezado.ForeColor = System.Drawing.Color.FromArgb(55, 65, 81);
            estiloEncabezado.SelectionBackColor = System.Drawing.Color.FromArgb(243, 244, 246);
            estiloEncabezado.SelectionForeColor = System.Drawing.Color.FromArgb(55, 65, 81);
            this.grdHistorias.ColumnHeadersDefaultCellStyle = estiloEncabezado;
            this.grdHistorias.ColumnHeadersHeight = 32;
            this.grdHistorias.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.grdHistorias.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNumero,
            this.colDni,
            this.colPaciente,
            this.colFechaApertura,
            this.colEstado,
            this.colAtenciones});
            this.grdHistorias.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(219, 234, 254);
            this.grdHistorias.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(30, 64, 175);
            this.grdHistorias.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdHistorias.EnableHeadersVisualStyles = false;
            this.grdHistorias.GridColor = System.Drawing.Color.FromArgb(229, 231, 235);
            this.grdHistorias.MultiSelect = false;
            this.grdHistorias.Name = "grdHistorias";
            this.grdHistorias.ReadOnly = true;
            this.grdHistorias.RowHeadersVisible = false;
            this.grdHistorias.RowTemplate.Height = 26;
            this.grdHistorias.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdHistorias.TabIndex = 1;
            this.grdHistorias.SelectionChanged += new System.EventHandler(this.grdHistorias_SelectionChanged);
            //
            // colNumero
            //
            this.colNumero.FillWeight = 70F;
            this.colNumero.HeaderText = "N° historia";
            this.colNumero.Name = "colNumero";
            this.colNumero.ReadOnly = true;
            //
            // colDni
            //
            this.colDni.FillWeight = 60F;
            this.colDni.HeaderText = "DNI";
            this.colDni.Name = "colDni";
            this.colDni.ReadOnly = true;
            //
            // colPaciente
            //
            this.colPaciente.FillWeight = 180F;
            this.colPaciente.HeaderText = "Paciente";
            this.colPaciente.Name = "colPaciente";
            this.colPaciente.ReadOnly = true;
            //
            // colFechaApertura
            //
            estiloFechaApertura.Format = "dd/MM/yyyy";
            this.colFechaApertura.DefaultCellStyle = estiloFechaApertura;
            this.colFechaApertura.FillWeight = 75F;
            this.colFechaApertura.HeaderText = "Fecha de apertura";
            this.colFechaApertura.Name = "colFechaApertura";
            this.colFechaApertura.ReadOnly = true;
            //
            // colEstado
            //
            this.colEstado.FillWeight = 55F;
            this.colEstado.HeaderText = "Estado";
            this.colEstado.Name = "colEstado";
            this.colEstado.ReadOnly = true;
            //
            // colAtenciones
            //
            estiloAtencionesNum.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.colAtenciones.DefaultCellStyle = estiloAtencionesNum;
            this.colAtenciones.FillWeight = 55F;
            this.colAtenciones.HeaderText = "Atenciones";
            this.colAtenciones.Name = "colAtenciones";
            this.colAtenciones.ReadOnly = true;
            //
            // pnlBusqueda
            //
            this.pnlBusqueda.Controls.Add(this.txtBuscar);
            this.pnlBusqueda.Controls.Add(this.btnBuscar);
            this.pnlBusqueda.Controls.Add(this.lblResultados);
            this.pnlBusqueda.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlBusqueda.Name = "pnlBusqueda";
            this.pnlBusqueda.Size = new System.Drawing.Size(814, 52);
            this.pnlBusqueda.TabIndex = 0;
            //
            // txtBuscar
            //
            this.txtBuscar.Location = new System.Drawing.Point(12, 14);
            this.txtBuscar.MaxLength = 100;
            this.txtBuscar.Name = "txtBuscar";
            this.txtBuscar.Size = new System.Drawing.Size(330, 23);
            this.txtBuscar.TabIndex = 0;
            this.txtBuscar.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBuscar_KeyDown);
            //
            // btnBuscar
            //
            this.btnBuscar.Location = new System.Drawing.Point(350, 12);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(90, 28);
            this.btnBuscar.TabIndex = 1;
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.UseVisualStyleBackColor = true;
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            //
            // lblResultados
            //
            this.lblResultados.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblResultados.ForeColor = System.Drawing.Color.FromArgb(107, 114, 128);
            this.lblResultados.Location = new System.Drawing.Point(546, 12);
            this.lblResultados.Name = "lblResultados";
            this.lblResultados.Size = new System.Drawing.Size(256, 28);
            this.lblResultados.TabIndex = 2;
            this.lblResultados.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // pnlDetalle (panel inferior: datos de la historia + atenciones)
            //
            this.pnlDetalle.BackColor = System.Drawing.Color.FromArgb(249, 250, 251);
            this.pnlDetalle.Controls.Add(this.lblTituloDetalle);
            this.pnlDetalle.Controls.Add(this.lblPaciente);
            this.pnlDetalle.Controls.Add(this.lblPacienteValor);
            this.pnlDetalle.Controls.Add(this.lblDni);
            this.pnlDetalle.Controls.Add(this.lblDniValor);
            this.pnlDetalle.Controls.Add(this.lblNumero);
            this.pnlDetalle.Controls.Add(this.lblNumeroValor);
            this.pnlDetalle.Controls.Add(this.lblApertura);
            this.pnlDetalle.Controls.Add(this.lblAperturaValor);
            this.pnlDetalle.Controls.Add(this.lblEstado);
            this.pnlDetalle.Controls.Add(this.lblEstadoValor);
            this.pnlDetalle.Controls.Add(this.btnAbrirHistoria);
            this.pnlDetalle.Controls.Add(this.btnCambiarEstado);
            this.pnlDetalle.Controls.Add(this.lblAtenciones);
            this.pnlDetalle.Controls.Add(this.grdAtenciones);
            this.pnlDetalle.Controls.Add(this.btnVerDiagnostico);
            this.pnlDetalle.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlDetalle.Name = "pnlDetalle";
            this.pnlDetalle.Size = new System.Drawing.Size(814, 212);
            this.pnlDetalle.TabIndex = 2;
            //
            // lblTituloDetalle
            //
            this.lblTituloDetalle.AutoSize = true;
            this.lblTituloDetalle.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.lblTituloDetalle.ForeColor = System.Drawing.Color.FromArgb(30, 64, 175);
            this.lblTituloDetalle.Location = new System.Drawing.Point(10, 8);
            this.lblTituloDetalle.Name = "lblTituloDetalle";
            this.lblTituloDetalle.TabIndex = 0;
            this.lblTituloDetalle.Text = "Historia clínica";
            //
            // lblPaciente
            //
            this.lblPaciente.AutoSize = true;
            this.lblPaciente.ForeColor = System.Drawing.Color.FromArgb(107, 114, 128);
            this.lblPaciente.Location = new System.Drawing.Point(12, 38);
            this.lblPaciente.Name = "lblPaciente";
            this.lblPaciente.TabIndex = 1;
            this.lblPaciente.Text = "Paciente:";
            //
            // lblPacienteValor
            //
            this.lblPacienteValor.AutoEllipsis = true;
            this.lblPacienteValor.Location = new System.Drawing.Point(100, 38);
            this.lblPacienteValor.Name = "lblPacienteValor";
            this.lblPacienteValor.Size = new System.Drawing.Size(240, 18);
            this.lblPacienteValor.TabIndex = 2;
            //
            // lblDni
            //
            this.lblDni.AutoSize = true;
            this.lblDni.ForeColor = System.Drawing.Color.FromArgb(107, 114, 128);
            this.lblDni.Location = new System.Drawing.Point(12, 62);
            this.lblDni.Name = "lblDni";
            this.lblDni.TabIndex = 3;
            this.lblDni.Text = "DNI:";
            //
            // lblDniValor
            //
            this.lblDniValor.Location = new System.Drawing.Point(100, 62);
            this.lblDniValor.Name = "lblDniValor";
            this.lblDniValor.Size = new System.Drawing.Size(240, 18);
            this.lblDniValor.TabIndex = 4;
            //
            // lblNumero
            //
            this.lblNumero.AutoSize = true;
            this.lblNumero.ForeColor = System.Drawing.Color.FromArgb(107, 114, 128);
            this.lblNumero.Location = new System.Drawing.Point(12, 86);
            this.lblNumero.Name = "lblNumero";
            this.lblNumero.TabIndex = 5;
            this.lblNumero.Text = "N° historia:";
            //
            // lblNumeroValor
            //
            this.lblNumeroValor.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.lblNumeroValor.Location = new System.Drawing.Point(100, 86);
            this.lblNumeroValor.Name = "lblNumeroValor";
            this.lblNumeroValor.Size = new System.Drawing.Size(240, 18);
            this.lblNumeroValor.TabIndex = 6;
            //
            // lblApertura
            //
            this.lblApertura.AutoSize = true;
            this.lblApertura.ForeColor = System.Drawing.Color.FromArgb(107, 114, 128);
            this.lblApertura.Location = new System.Drawing.Point(12, 110);
            this.lblApertura.Name = "lblApertura";
            this.lblApertura.TabIndex = 7;
            this.lblApertura.Text = "Apertura:";
            //
            // lblAperturaValor
            //
            this.lblAperturaValor.Location = new System.Drawing.Point(100, 110);
            this.lblAperturaValor.Name = "lblAperturaValor";
            this.lblAperturaValor.Size = new System.Drawing.Size(240, 18);
            this.lblAperturaValor.TabIndex = 8;
            //
            // lblEstado
            //
            this.lblEstado.AutoSize = true;
            this.lblEstado.ForeColor = System.Drawing.Color.FromArgb(107, 114, 128);
            this.lblEstado.Location = new System.Drawing.Point(12, 134);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.TabIndex = 9;
            this.lblEstado.Text = "Estado:";
            //
            // lblEstadoValor
            //
            this.lblEstadoValor.Location = new System.Drawing.Point(100, 134);
            this.lblEstadoValor.Name = "lblEstadoValor";
            this.lblEstadoValor.Size = new System.Drawing.Size(240, 18);
            this.lblEstadoValor.TabIndex = 10;
            //
            // btnAbrirHistoria
            //
            this.btnAbrirHistoria.Location = new System.Drawing.Point(12, 166);
            this.btnAbrirHistoria.Name = "btnAbrirHistoria";
            this.btnAbrirHistoria.Size = new System.Drawing.Size(150, 30);
            this.btnAbrirHistoria.TabIndex = 11;
            this.btnAbrirHistoria.Text = "Abrir historia";
            this.btnAbrirHistoria.UseVisualStyleBackColor = true;
            this.btnAbrirHistoria.Click += new System.EventHandler(this.btnAbrirHistoria_Click);
            //
            // btnCambiarEstado
            //
            this.btnCambiarEstado.Location = new System.Drawing.Point(170, 166);
            this.btnCambiarEstado.Name = "btnCambiarEstado";
            this.btnCambiarEstado.Size = new System.Drawing.Size(170, 30);
            this.btnCambiarEstado.TabIndex = 12;
            this.btnCambiarEstado.Text = "Activar / Inactivar historia";
            this.btnCambiarEstado.UseVisualStyleBackColor = true;
            this.btnCambiarEstado.Click += new System.EventHandler(this.btnCambiarEstado_Click);
            //
            // lblAtenciones
            //
            this.lblAtenciones.AutoSize = true;
            this.lblAtenciones.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.lblAtenciones.ForeColor = System.Drawing.Color.FromArgb(55, 65, 81);
            this.lblAtenciones.Location = new System.Drawing.Point(360, 12);
            this.lblAtenciones.Name = "lblAtenciones";
            this.lblAtenciones.TabIndex = 13;
            this.lblAtenciones.Text = "Atenciones registradas";
            //
            // grdAtenciones (segunda grilla, solo lectura)
            //
            this.grdAtenciones.AllowUserToAddRows = false;
            this.grdAtenciones.AllowUserToDeleteRows = false;
            this.grdAtenciones.AllowUserToResizeRows = false;
            this.grdAtenciones.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.grdAtenciones.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grdAtenciones.BackgroundColor = System.Drawing.Color.White;
            this.grdAtenciones.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.grdAtenciones.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            estiloEncabezado2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            estiloEncabezado2.BackColor = System.Drawing.Color.FromArgb(243, 244, 246);
            estiloEncabezado2.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            estiloEncabezado2.ForeColor = System.Drawing.Color.FromArgb(55, 65, 81);
            estiloEncabezado2.SelectionBackColor = System.Drawing.Color.FromArgb(243, 244, 246);
            estiloEncabezado2.SelectionForeColor = System.Drawing.Color.FromArgb(55, 65, 81);
            estiloEncabezado2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.grdAtenciones.ColumnHeadersDefaultCellStyle = estiloEncabezado2;
            this.grdAtenciones.ColumnHeadersHeight = 38;
            this.grdAtenciones.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.grdAtenciones.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colFechaAtencion,
            this.colHoraAtencion,
            this.colConsultorio,
            this.colProfesionalAtencion,
            this.colObservaciones,
            this.colRegistradaPor});
            this.grdAtenciones.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(219, 234, 254);
            this.grdAtenciones.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(30, 64, 175);
            this.grdAtenciones.EnableHeadersVisualStyles = false;
            this.grdAtenciones.GridColor = System.Drawing.Color.FromArgb(229, 231, 235);
            this.grdAtenciones.Location = new System.Drawing.Point(360, 34);
            this.grdAtenciones.MultiSelect = false;
            this.grdAtenciones.Name = "grdAtenciones";
            this.grdAtenciones.ReadOnly = true;
            this.grdAtenciones.RowHeadersVisible = false;
            this.grdAtenciones.RowTemplate.Height = 24;
            this.grdAtenciones.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdAtenciones.Size = new System.Drawing.Size(442, 164);
            this.grdAtenciones.TabIndex = 14;
            this.grdAtenciones.SelectionChanged += new System.EventHandler(this.grdAtenciones_SelectionChanged);
            //
            // btnVerDiagnostico (descifra el diagnóstico de la fila seleccionada; queda auditado)
            //
            this.btnVerDiagnostico.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnVerDiagnostico.Enabled = false;
            this.btnVerDiagnostico.Location = new System.Drawing.Point(682, 5);
            this.btnVerDiagnostico.Name = "btnVerDiagnostico";
            this.btnVerDiagnostico.Size = new System.Drawing.Size(120, 26);
            this.btnVerDiagnostico.TabIndex = 15;
            this.btnVerDiagnostico.Text = "Ver diagnóstico";
            this.btnVerDiagnostico.UseVisualStyleBackColor = true;
            this.btnVerDiagnostico.Click += new System.EventHandler(this.btnVerDiagnostico_Click);
            //
            // colFechaAtencion
            //
            estiloFechaAtencion.Format = "dd/MM/yyyy";
            this.colFechaAtencion.DefaultCellStyle = estiloFechaAtencion;
            this.colFechaAtencion.FillWeight = 70F;
            this.colFechaAtencion.MinimumWidth = 72;
            this.colFechaAtencion.HeaderText = "Fecha";
            this.colFechaAtencion.Name = "colFechaAtencion";
            this.colFechaAtencion.ReadOnly = true;
            //
            // colHoraAtencion
            //
            this.colHoraAtencion.FillWeight = 45F;
            this.colHoraAtencion.MinimumWidth = 44;
            this.colHoraAtencion.HeaderText = "Hora";
            this.colHoraAtencion.Name = "colHoraAtencion";
            this.colHoraAtencion.ReadOnly = true;
            //
            // colConsultorio
            //
            this.colConsultorio.FillWeight = 95F;
            this.colConsultorio.MinimumWidth = 76;
            this.colConsultorio.HeaderText = "Consultorio";
            this.colConsultorio.Name = "colConsultorio";
            this.colConsultorio.ReadOnly = true;
            //
            // colProfesionalAtencion
            //
            this.colProfesionalAtencion.FillWeight = 95F;
            this.colProfesionalAtencion.HeaderText = "Profesional";
            this.colProfesionalAtencion.Name = "colProfesionalAtencion";
            this.colProfesionalAtencion.ReadOnly = true;
            //
            // colObservaciones (el texto completo se ve al pasar el mouse)
            //
            this.colObservaciones.FillWeight = 130F;
            this.colObservaciones.HeaderText = "Observaciones";
            this.colObservaciones.Name = "colObservaciones";
            this.colObservaciones.ReadOnly = true;
            //
            // colRegistradaPor
            //
            this.colRegistradaPor.FillWeight = 75F;
            this.colRegistradaPor.MinimumWidth = 86;
            this.colRegistradaPor.HeaderText = "Registrada por";
            this.colRegistradaPor.Name = "colRegistradaPor";
            this.colRegistradaPor.ReadOnly = true;
            //
            // FrmHistorias
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(814, 520);
            // La grilla principal (Dock = Fill) se agrega primero para que ocupe el
            // espacio entre la búsqueda (arriba) y el detalle (abajo).
            this.Controls.Add(this.grdHistorias);
            this.Controls.Add(this.pnlBusqueda);
            this.Controls.Add(this.pnlDetalle);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "FrmHistorias";
            this.Text = "Historias clínicas";
            this.Load += new System.EventHandler(this.FrmHistorias_Load);
            ((System.ComponentModel.ISupportInitialize)(this.grdHistorias)).EndInit();
            this.pnlBusqueda.ResumeLayout(false);
            this.pnlBusqueda.PerformLayout();
            this.pnlDetalle.ResumeLayout(false);
            this.pnlDetalle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdAtenciones)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.DataGridView grdHistorias;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNumero;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDni;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPaciente;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFechaApertura;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEstado;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAtenciones;
        private System.Windows.Forms.Panel pnlBusqueda;
        private System.Windows.Forms.TextBox txtBuscar;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.Label lblResultados;
        private System.Windows.Forms.Panel pnlDetalle;
        private System.Windows.Forms.Label lblTituloDetalle;
        private System.Windows.Forms.Label lblPaciente;
        private System.Windows.Forms.Label lblPacienteValor;
        private System.Windows.Forms.Label lblDni;
        private System.Windows.Forms.Label lblDniValor;
        private System.Windows.Forms.Label lblNumero;
        private System.Windows.Forms.Label lblNumeroValor;
        private System.Windows.Forms.Label lblApertura;
        private System.Windows.Forms.Label lblAperturaValor;
        private System.Windows.Forms.Label lblEstado;
        private System.Windows.Forms.Label lblEstadoValor;
        private System.Windows.Forms.Button btnAbrirHistoria;
        private System.Windows.Forms.Button btnCambiarEstado;
        private System.Windows.Forms.Label lblAtenciones;
        private System.Windows.Forms.DataGridView grdAtenciones;
        private System.Windows.Forms.Button btnVerDiagnostico;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFechaAtencion;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHoraAtencion;
        private System.Windows.Forms.DataGridViewTextBoxColumn colConsultorio;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProfesionalAtencion;
        private System.Windows.Forms.DataGridViewTextBoxColumn colObservaciones;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRegistradaPor;
    }
}
