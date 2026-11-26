namespace SigepostaAdmision.Presentacion
{
    partial class FrmCola
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle estiloEncabezado1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle estiloEncabezado2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle estiloCentrado = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlSuperior = new System.Windows.Forms.Panel();
            this.lblConsultorio = new System.Windows.Forms.Label();
            this.cboConsultorio = new System.Windows.Forms.ComboBox();
            this.chkAuto = new System.Windows.Forms.CheckBox();
            this.chkCarga = new System.Windows.Forms.CheckBox();
            this.pbActualizando = new System.Windows.Forms.ProgressBar();
            this.lblActualizacion = new System.Windows.Forms.Label();
            this.pnlEstado = new System.Windows.Forms.Panel();
            this.lblEstado = new System.Windows.Forms.Label();
            this.tlpPrincipal = new System.Windows.Forms.TableLayoutPanel();
            this.pnlIzquierdo = new System.Windows.Forms.Panel();
            this.grdPendientes = new System.Windows.Forms.DataGridView();
            this.colHoraPend = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPacientePend = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colConsultorioPend = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProfesionalPend = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlIzqBotones = new System.Windows.Forms.Panel();
            this.btnRegistrarLlegada = new System.Windows.Forms.Button();
            this.lblPendientes = new System.Windows.Forms.Label();
            this.pnlDerecho = new System.Windows.Forms.Panel();
            this.grdCola = new System.Windows.Forms.DataGridView();
            this.colPos = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTurno = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPacienteCola = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colConsultorioCola = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProfesionalCola = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLlegada = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEspera = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEsperaEstimada = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEstado = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlColaBotones = new System.Windows.Forms.Panel();
            this.btnLlamarSiguiente = new System.Windows.Forms.Button();
            this.btnDeshacerLlegada = new System.Windows.Forms.Button();
            this.btnRegistrarAtencion = new System.Windows.Forms.Button();
            this.btnDemoRollback = new System.Windows.Forms.Button();
            this.pnlRecordatorios = new System.Windows.Forms.Panel();
            this.lstRecordatorios = new System.Windows.Forms.ListBox();
            this.lblRecordatorios = new System.Windows.Forms.Label();
            this.lblCola = new System.Windows.Forms.Label();
            this.worker = new System.ComponentModel.BackgroundWorker();
            this.timerReintento = new System.Windows.Forms.Timer(this.components);
            this.pnlSuperior.SuspendLayout();
            this.pnlEstado.SuspendLayout();
            this.tlpPrincipal.SuspendLayout();
            this.pnlIzquierdo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdPendientes)).BeginInit();
            this.pnlIzqBotones.SuspendLayout();
            this.pnlDerecho.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdCola)).BeginInit();
            this.pnlColaBotones.SuspendLayout();
            this.pnlRecordatorios.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlSuperior (filtro y controles de actualización)
            //
            this.pnlSuperior.Controls.Add(this.lblConsultorio);
            this.pnlSuperior.Controls.Add(this.cboConsultorio);
            this.pnlSuperior.Controls.Add(this.chkAuto);
            this.pnlSuperior.Controls.Add(this.chkCarga);
            this.pnlSuperior.Controls.Add(this.pbActualizando);
            this.pnlSuperior.Controls.Add(this.lblActualizacion);
            this.pnlSuperior.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSuperior.Name = "pnlSuperior";
            this.pnlSuperior.Size = new System.Drawing.Size(814, 50);
            this.pnlSuperior.TabIndex = 0;
            //
            // lblConsultorio
            //
            this.lblConsultorio.AutoSize = true;
            this.lblConsultorio.Location = new System.Drawing.Point(12, 17);
            this.lblConsultorio.Name = "lblConsultorio";
            this.lblConsultorio.TabIndex = 0;
            this.lblConsultorio.Text = "Consultorio:";
            //
            // cboConsultorio
            //
            this.cboConsultorio.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboConsultorio.Location = new System.Drawing.Point(88, 13);
            this.cboConsultorio.Name = "cboConsultorio";
            this.cboConsultorio.Size = new System.Drawing.Size(180, 23);
            this.cboConsultorio.TabIndex = 1;
            this.cboConsultorio.SelectedIndexChanged += new System.EventHandler(this.cboConsultorio_SelectedIndexChanged);
            //
            // chkAuto
            //
            this.chkAuto.AutoSize = true;
            this.chkAuto.Checked = true;
            this.chkAuto.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAuto.Location = new System.Drawing.Point(282, 15);
            this.chkAuto.Name = "chkAuto";
            this.chkAuto.TabIndex = 2;
            this.chkAuto.Text = "Actualización automática";
            this.chkAuto.UseVisualStyleBackColor = true;
            this.chkAuto.CheckedChanged += new System.EventHandler(this.chkAuto_CheckedChanged);
            //
            // chkCarga
            //
            this.chkCarga.AutoSize = true;
            this.chkCarga.Location = new System.Drawing.Point(452, 15);
            this.chkCarga.Name = "chkCarga";
            this.chkCarga.TabIndex = 3;
            this.chkCarga.Text = "Simular carga pesada";
            this.chkCarga.UseVisualStyleBackColor = true;
            this.chkCarga.CheckedChanged += new System.EventHandler(this.chkCarga_CheckedChanged);
            //
            // pbActualizando (visible solo mientras se actualiza)
            //
            this.pbActualizando.Location = new System.Drawing.Point(608, 17);
            this.pbActualizando.MarqueeAnimationSpeed = 30;
            this.pbActualizando.Name = "pbActualizando";
            this.pbActualizando.Size = new System.Drawing.Size(60, 14);
            this.pbActualizando.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.pbActualizando.TabIndex = 4;
            this.pbActualizando.Visible = false;
            //
            // lblActualizacion
            //
            this.lblActualizacion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblActualizacion.ForeColor = System.Drawing.Color.FromArgb(107, 114, 128);
            this.lblActualizacion.Location = new System.Drawing.Point(672, 8);
            this.lblActualizacion.Name = "lblActualizacion";
            this.lblActualizacion.Size = new System.Drawing.Size(134, 34);
            this.lblActualizacion.TabIndex = 5;
            this.lblActualizacion.Text = "Última actualización:\n—";
            this.lblActualizacion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // pnlEstado (barra de estado inferior)
            //
            this.pnlEstado.BackColor = System.Drawing.Color.FromArgb(243, 244, 246);
            this.pnlEstado.Controls.Add(this.lblEstado);
            this.pnlEstado.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlEstado.Name = "pnlEstado";
            this.pnlEstado.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.pnlEstado.Size = new System.Drawing.Size(814, 26);
            this.pnlEstado.TabIndex = 2;
            //
            // lblEstado
            //
            this.lblEstado.AutoEllipsis = true;
            this.lblEstado.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblEstado.ForeColor = System.Drawing.Color.FromArgb(55, 65, 81);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.TabIndex = 0;
            this.lblEstado.Text = "Iniciando…";
            this.lblEstado.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // tlpPrincipal (izquierda: pendientes 34 % | derecha: cola 66 %)
            //
            this.tlpPrincipal.ColumnCount = 2;
            this.tlpPrincipal.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34F));
            this.tlpPrincipal.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 66F));
            this.tlpPrincipal.Controls.Add(this.pnlIzquierdo, 0, 0);
            this.tlpPrincipal.Controls.Add(this.pnlDerecho, 1, 0);
            this.tlpPrincipal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpPrincipal.Name = "tlpPrincipal";
            this.tlpPrincipal.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.tlpPrincipal.RowCount = 1;
            this.tlpPrincipal.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpPrincipal.TabIndex = 1;
            //
            // pnlIzquierdo
            //
            this.pnlIzquierdo.Controls.Add(this.grdPendientes);
            this.pnlIzquierdo.Controls.Add(this.pnlIzqBotones);
            this.pnlIzquierdo.Controls.Add(this.lblPendientes);
            this.pnlIzquierdo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlIzquierdo.Name = "pnlIzquierdo";
            this.pnlIzquierdo.TabIndex = 0;
            //
            // grdPendientes
            //
            this.grdPendientes.AllowUserToAddRows = false;
            this.grdPendientes.AllowUserToDeleteRows = false;
            this.grdPendientes.AllowUserToResizeRows = false;
            this.grdPendientes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grdPendientes.BackgroundColor = System.Drawing.Color.White;
            this.grdPendientes.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.grdPendientes.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            estiloEncabezado1.BackColor = System.Drawing.Color.FromArgb(243, 244, 246);
            estiloEncabezado1.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            estiloEncabezado1.ForeColor = System.Drawing.Color.FromArgb(55, 65, 81);
            estiloEncabezado1.SelectionBackColor = System.Drawing.Color.FromArgb(243, 244, 246);
            estiloEncabezado1.SelectionForeColor = System.Drawing.Color.FromArgb(55, 65, 81);
            estiloEncabezado1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.grdPendientes.ColumnHeadersDefaultCellStyle = estiloEncabezado1;
            this.grdPendientes.ColumnHeadersHeight = 36;
            this.grdPendientes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.grdPendientes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colHoraPend,
            this.colPacientePend,
            this.colConsultorioPend,
            this.colProfesionalPend});
            this.grdPendientes.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(219, 234, 254);
            this.grdPendientes.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(30, 64, 175);
            this.grdPendientes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdPendientes.EnableHeadersVisualStyles = false;
            this.grdPendientes.GridColor = System.Drawing.Color.FromArgb(229, 231, 235);
            this.grdPendientes.MultiSelect = false;
            this.grdPendientes.Name = "grdPendientes";
            this.grdPendientes.ReadOnly = true;
            this.grdPendientes.RowHeadersVisible = false;
            this.grdPendientes.RowTemplate.Height = 24;
            this.grdPendientes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdPendientes.TabIndex = 1;
            this.grdPendientes.SelectionChanged += new System.EventHandler(this.Grilla_SelectionChanged);
            //
            // colHoraPend
            //
            this.colHoraPend.FillWeight = 40F;
            this.colHoraPend.MinimumWidth = 46;
            this.colHoraPend.HeaderText = "Hora";
            this.colHoraPend.Name = "colHoraPend";
            this.colHoraPend.ReadOnly = true;
            //
            // colPacientePend
            //
            this.colPacientePend.FillWeight = 130F;
            this.colPacientePend.HeaderText = "Paciente";
            this.colPacientePend.Name = "colPacientePend";
            this.colPacientePend.ReadOnly = true;
            //
            // colConsultorioPend
            //
            this.colConsultorioPend.FillWeight = 90F;
            this.colConsultorioPend.MinimumWidth = 76;
            this.colConsultorioPend.HeaderText = "Consultorio";
            this.colConsultorioPend.Name = "colConsultorioPend";
            this.colConsultorioPend.ReadOnly = true;
            //
            // colProfesionalPend
            //
            this.colProfesionalPend.FillWeight = 90F;
            this.colProfesionalPend.MinimumWidth = 76;
            this.colProfesionalPend.HeaderText = "Profesional";
            this.colProfesionalPend.Name = "colProfesionalPend";
            this.colProfesionalPend.ReadOnly = true;
            //
            // pnlIzqBotones
            //
            this.pnlIzqBotones.Controls.Add(this.btnRegistrarLlegada);
            this.pnlIzqBotones.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlIzqBotones.Name = "pnlIzqBotones";
            this.pnlIzqBotones.Size = new System.Drawing.Size(330, 44);
            this.pnlIzqBotones.TabIndex = 2;
            //
            // btnRegistrarLlegada
            //
            this.btnRegistrarLlegada.Location = new System.Drawing.Point(0, 8);
            this.btnRegistrarLlegada.Name = "btnRegistrarLlegada";
            this.btnRegistrarLlegada.Size = new System.Drawing.Size(150, 30);
            this.btnRegistrarLlegada.TabIndex = 0;
            this.btnRegistrarLlegada.Text = "Registrar llegada";
            this.btnRegistrarLlegada.UseVisualStyleBackColor = true;
            this.btnRegistrarLlegada.Click += new System.EventHandler(this.btnRegistrarLlegada_Click);
            //
            // lblPendientes
            //
            this.lblPendientes.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblPendientes.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.lblPendientes.ForeColor = System.Drawing.Color.FromArgb(30, 64, 175);
            this.lblPendientes.Name = "lblPendientes";
            this.lblPendientes.Size = new System.Drawing.Size(330, 28);
            this.lblPendientes.TabIndex = 0;
            this.lblPendientes.Text = "Citas pendientes de hoy";
            this.lblPendientes.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlDerecho
            //
            this.pnlDerecho.Controls.Add(this.grdCola);
            this.pnlDerecho.Controls.Add(this.pnlColaBotones);
            this.pnlDerecho.Controls.Add(this.pnlRecordatorios);
            this.pnlDerecho.Controls.Add(this.lblCola);
            this.pnlDerecho.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDerecho.Name = "pnlDerecho";
            this.pnlDerecho.TabIndex = 1;
            //
            // grdCola
            //
            this.grdCola.AllowUserToAddRows = false;
            this.grdCola.AllowUserToDeleteRows = false;
            this.grdCola.AllowUserToResizeRows = false;
            this.grdCola.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grdCola.BackgroundColor = System.Drawing.Color.White;
            this.grdCola.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.grdCola.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            estiloEncabezado2.BackColor = System.Drawing.Color.FromArgb(243, 244, 246);
            estiloEncabezado2.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            estiloEncabezado2.ForeColor = System.Drawing.Color.FromArgb(55, 65, 81);
            estiloEncabezado2.SelectionBackColor = System.Drawing.Color.FromArgb(243, 244, 246);
            estiloEncabezado2.SelectionForeColor = System.Drawing.Color.FromArgb(55, 65, 81);
            estiloEncabezado2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.grdCola.ColumnHeadersDefaultCellStyle = estiloEncabezado2;
            this.grdCola.ColumnHeadersHeight = 50;
            this.grdCola.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.grdCola.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colPos,
            this.colTurno,
            this.colPacienteCola,
            this.colConsultorioCola,
            this.colProfesionalCola,
            this.colLlegada,
            this.colEspera,
            this.colEsperaEstimada,
            this.colEstado});
            this.grdCola.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(219, 234, 254);
            this.grdCola.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(30, 64, 175);
            this.grdCola.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdCola.EnableHeadersVisualStyles = false;
            this.grdCola.GridColor = System.Drawing.Color.FromArgb(229, 231, 235);
            this.grdCola.MultiSelect = false;
            this.grdCola.Name = "grdCola";
            this.grdCola.ReadOnly = true;
            this.grdCola.RowHeadersVisible = false;
            this.grdCola.RowTemplate.Height = 24;
            this.grdCola.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdCola.TabIndex = 1;
            this.grdCola.SelectionChanged += new System.EventHandler(this.Grilla_SelectionChanged);
            //
            // columnas de la cola
            //
            estiloCentrado.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colPos.DefaultCellStyle = estiloCentrado;
            this.colPos.FillWeight = 30F;
            this.colPos.MinimumWidth = 34;
            this.colPos.HeaderText = "Pos.";
            this.colPos.Name = "colPos";
            this.colPos.ReadOnly = true;
            this.colTurno.DefaultCellStyle = estiloCentrado;
            this.colTurno.FillWeight = 42F;
            this.colTurno.MinimumWidth = 44;
            this.colTurno.HeaderText = "Turno";
            this.colTurno.Name = "colTurno";
            this.colTurno.ReadOnly = true;
            this.colPacienteCola.FillWeight = 110F;
            this.colPacienteCola.MinimumWidth = 80;
            this.colPacienteCola.HeaderText = "Paciente";
            this.colPacienteCola.Name = "colPacienteCola";
            this.colPacienteCola.ReadOnly = true;
            this.colConsultorioCola.FillWeight = 85F;
            this.colConsultorioCola.MinimumWidth = 70;
            this.colConsultorioCola.HeaderText = "Consultorio";
            this.colConsultorioCola.Name = "colConsultorioCola";
            this.colConsultorioCola.ReadOnly = true;
            this.colProfesionalCola.FillWeight = 85F;
            this.colProfesionalCola.MinimumWidth = 70;
            this.colProfesionalCola.HeaderText = "Profesional";
            this.colProfesionalCola.Name = "colProfesionalCola";
            this.colProfesionalCola.ReadOnly = true;
            this.colLlegada.DefaultCellStyle = estiloCentrado;
            this.colLlegada.FillWeight = 52F;
            this.colLlegada.MinimumWidth = 52;
            this.colLlegada.HeaderText = "Llegada";
            this.colLlegada.Name = "colLlegada";
            this.colLlegada.ReadOnly = true;
            this.colEspera.DefaultCellStyle = estiloCentrado;
            this.colEspera.FillWeight = 50F;
            this.colEspera.MinimumWidth = 50;
            this.colEspera.HeaderText = "Espera (min)";
            this.colEspera.Name = "colEspera";
            this.colEspera.ReadOnly = true;
            this.colEsperaEstimada.DefaultCellStyle = estiloCentrado;
            this.colEsperaEstimada.FillWeight = 60F;
            this.colEsperaEstimada.MinimumWidth = 60;
            this.colEsperaEstimada.HeaderText = "Espera estimada (min)";
            this.colEsperaEstimada.Name = "colEsperaEstimada";
            this.colEsperaEstimada.ReadOnly = true;
            this.colEstado.FillWeight = 62F;
            this.colEstado.MinimumWidth = 60;
            this.colEstado.HeaderText = "Estado";
            this.colEstado.Name = "colEstado";
            this.colEstado.ReadOnly = true;
            //
            // pnlColaBotones (debajo de la cola)
            //
            this.pnlColaBotones.Controls.Add(this.btnLlamarSiguiente);
            this.pnlColaBotones.Controls.Add(this.btnDeshacerLlegada);
            this.pnlColaBotones.Controls.Add(this.btnRegistrarAtencion);
            this.pnlColaBotones.Controls.Add(this.btnDemoRollback);
            this.pnlColaBotones.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlColaBotones.Name = "pnlColaBotones";
            this.pnlColaBotones.Size = new System.Drawing.Size(460, 44);
            this.pnlColaBotones.TabIndex = 2;
            //
            // btnLlamarSiguiente
            //
            this.btnLlamarSiguiente.Location = new System.Drawing.Point(0, 8);
            this.btnLlamarSiguiente.Name = "btnLlamarSiguiente";
            this.btnLlamarSiguiente.Size = new System.Drawing.Size(125, 30);
            this.btnLlamarSiguiente.TabIndex = 0;
            this.btnLlamarSiguiente.Text = "Llamar siguiente";
            this.btnLlamarSiguiente.UseVisualStyleBackColor = true;
            this.btnLlamarSiguiente.Click += new System.EventHandler(this.btnLlamarSiguiente_Click);
            //
            // btnRegistrarAtencion (solo para pacientes "Llamado")
            //
            this.btnRegistrarAtencion.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.btnRegistrarAtencion.Location = new System.Drawing.Point(262, 8);
            this.btnRegistrarAtencion.Name = "btnRegistrarAtencion";
            this.btnRegistrarAtencion.Size = new System.Drawing.Size(140, 30);
            this.btnRegistrarAtencion.TabIndex = 2;
            this.btnRegistrarAtencion.Text = "Registrar atención";
            this.btnRegistrarAtencion.UseVisualStyleBackColor = true;
            this.btnRegistrarAtencion.Click += new System.EventHandler(this.btnRegistrarAtencion_Click);
            //
            // btnDemoRollback (visible solo para el Administrador)
            //
            this.btnDemoRollback.ForeColor = System.Drawing.Color.FromArgb(146, 64, 14);
            this.btnDemoRollback.Location = new System.Drawing.Point(408, 8);
            this.btnDemoRollback.Name = "btnDemoRollback";
            this.btnDemoRollback.Size = new System.Drawing.Size(115, 30);
            this.btnDemoRollback.TabIndex = 3;
            this.btnDemoRollback.Text = "Demo rollback";
            this.btnDemoRollback.UseVisualStyleBackColor = true;
            this.btnDemoRollback.Visible = false;
            this.btnDemoRollback.Click += new System.EventHandler(this.btnDemoRollback_Click);
            //
            // btnDeshacerLlegada
            //
            this.btnDeshacerLlegada.Location = new System.Drawing.Point(131, 8);
            this.btnDeshacerLlegada.Name = "btnDeshacerLlegada";
            this.btnDeshacerLlegada.Size = new System.Drawing.Size(125, 30);
            this.btnDeshacerLlegada.TabIndex = 1;
            this.btnDeshacerLlegada.Text = "Deshacer llegada";
            this.btnDeshacerLlegada.UseVisualStyleBackColor = true;
            this.btnDeshacerLlegada.Click += new System.EventHandler(this.btnDeshacerLlegada_Click);
            //
            // pnlRecordatorios (abajo a la derecha)
            //
            this.pnlRecordatorios.Controls.Add(this.lstRecordatorios);
            this.pnlRecordatorios.Controls.Add(this.lblRecordatorios);
            this.pnlRecordatorios.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlRecordatorios.Name = "pnlRecordatorios";
            this.pnlRecordatorios.Padding = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.pnlRecordatorios.Size = new System.Drawing.Size(460, 108);
            this.pnlRecordatorios.TabIndex = 3;
            //
            // lstRecordatorios
            //
            this.lstRecordatorios.BackColor = System.Drawing.Color.FromArgb(255, 251, 235);
            this.lstRecordatorios.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstRecordatorios.IntegralHeight = false;
            this.lstRecordatorios.Name = "lstRecordatorios";
            this.lstRecordatorios.TabIndex = 1;
            //
            // lblRecordatorios
            //
            this.lblRecordatorios.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblRecordatorios.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.lblRecordatorios.ForeColor = System.Drawing.Color.FromArgb(146, 64, 14);
            this.lblRecordatorios.Name = "lblRecordatorios";
            this.lblRecordatorios.Size = new System.Drawing.Size(460, 22);
            this.lblRecordatorios.TabIndex = 0;
            this.lblRecordatorios.Text = "Recordatorios (próxima hora) — simulación, no se envían";
            this.lblRecordatorios.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblCola
            //
            this.lblCola.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCola.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.lblCola.ForeColor = System.Drawing.Color.FromArgb(30, 64, 175);
            this.lblCola.Name = "lblCola";
            this.lblCola.Size = new System.Drawing.Size(460, 28);
            this.lblCola.TabIndex = 0;
            this.lblCola.Text = "Cola de atención";
            this.lblCola.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // worker (BackgroundWorker: consulta la cola en otro hilo)
            //
            this.worker.WorkerReportsProgress = true;
            this.worker.WorkerSupportsCancellation = true;
            this.worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.worker_DoWork);
            this.worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.worker_ProgressChanged);
            this.worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.worker_RunWorkerCompleted);
            //
            // timerReintento (vuelve a intentar tras "Sin conexión")
            //
            this.timerReintento.Interval = 5000;
            this.timerReintento.Tick += new System.EventHandler(this.timerReintento_Tick);
            //
            // FrmCola
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(814, 520);
            // El panel central (Fill) se agrega primero; luego las barras superior e inferior.
            this.Controls.Add(this.tlpPrincipal);
            this.Controls.Add(this.pnlSuperior);
            this.Controls.Add(this.pnlEstado);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "FrmCola";
            this.Text = "Cola de atención";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmCola_FormClosing);
            this.Load += new System.EventHandler(this.FrmCola_Load);
            this.pnlSuperior.ResumeLayout(false);
            this.pnlSuperior.PerformLayout();
            this.pnlEstado.ResumeLayout(false);
            this.tlpPrincipal.ResumeLayout(false);
            this.pnlIzquierdo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdPendientes)).EndInit();
            this.pnlIzqBotones.ResumeLayout(false);
            this.pnlDerecho.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdCola)).EndInit();
            this.pnlColaBotones.ResumeLayout(false);
            this.pnlRecordatorios.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlSuperior;
        private System.Windows.Forms.Label lblConsultorio;
        private System.Windows.Forms.ComboBox cboConsultorio;
        private System.Windows.Forms.CheckBox chkAuto;
        private System.Windows.Forms.CheckBox chkCarga;
        private System.Windows.Forms.ProgressBar pbActualizando;
        private System.Windows.Forms.Label lblActualizacion;
        private System.Windows.Forms.Panel pnlEstado;
        private System.Windows.Forms.Label lblEstado;
        private System.Windows.Forms.TableLayoutPanel tlpPrincipal;
        private System.Windows.Forms.Panel pnlIzquierdo;
        private System.Windows.Forms.DataGridView grdPendientes;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHoraPend;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPacientePend;
        private System.Windows.Forms.DataGridViewTextBoxColumn colConsultorioPend;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProfesionalPend;
        private System.Windows.Forms.Panel pnlIzqBotones;
        private System.Windows.Forms.Button btnRegistrarLlegada;
        private System.Windows.Forms.Label lblPendientes;
        private System.Windows.Forms.Panel pnlDerecho;
        private System.Windows.Forms.DataGridView grdCola;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPos;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTurno;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPacienteCola;
        private System.Windows.Forms.DataGridViewTextBoxColumn colConsultorioCola;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProfesionalCola;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLlegada;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEspera;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEsperaEstimada;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEstado;
        private System.Windows.Forms.Panel pnlColaBotones;
        private System.Windows.Forms.Button btnLlamarSiguiente;
        private System.Windows.Forms.Button btnDeshacerLlegada;
        private System.Windows.Forms.Button btnRegistrarAtencion;
        private System.Windows.Forms.Button btnDemoRollback;
        private System.Windows.Forms.Panel pnlRecordatorios;
        private System.Windows.Forms.ListBox lstRecordatorios;
        private System.Windows.Forms.Label lblRecordatorios;
        private System.Windows.Forms.Label lblCola;
        private System.ComponentModel.BackgroundWorker worker;
        private System.Windows.Forms.Timer timerReintento;
    }
}
