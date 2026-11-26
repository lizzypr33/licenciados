namespace SigepostaAdmision.Presentacion
{
    partial class FrmReportes
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea areaGrafico = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series serieGrafico = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title tituloGrafico = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.pnlFiltros = new System.Windows.Forms.Panel();
            this.lblTipo = new System.Windows.Forms.Label();
            this.cboTipo = new System.Windows.Forms.ComboBox();
            this.lblDesde = new System.Windows.Forms.Label();
            this.dtpDesde = new System.Windows.Forms.DateTimePicker();
            this.lblHasta = new System.Windows.Forms.Label();
            this.dtpHasta = new System.Windows.Forms.DateTimePicker();
            this.btnGenerar = new System.Windows.Forms.Button();
            this.lblPosta = new System.Windows.Forms.Label();
            this.cboPosta = new System.Windows.Forms.ComboBox();
            this.lblConsultorio = new System.Windows.Forms.Label();
            this.cboConsultorio = new System.Windows.Forms.ComboBox();
            this.lblEstado = new System.Windows.Forms.Label();
            this.cboEstado = new System.Windows.Forms.ComboBox();
            this.lblUsuario = new System.Windows.Forms.Label();
            this.txtUsuario = new System.Windows.Forms.TextBox();
            this.lblAccion = new System.Windows.Forms.Label();
            this.txtAccion = new System.Windows.Forms.TextBox();
            this.pbGenerando = new System.Windows.Forms.ProgressBar();
            this.pnlResumen = new System.Windows.Forms.Panel();
            this.lblResumen = new System.Windows.Forms.Label();
            this.pnlCentro = new System.Windows.Forms.Panel();
            this.grdReporte = new System.Windows.Forms.DataGridView();
            this.lblSinDatos = new System.Windows.Forms.Label();
            this.chtReporte = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.pnlAcciones = new System.Windows.Forms.Panel();
            this.btnExportarCsv = new System.Windows.Forms.Button();
            this.btnImprimir = new System.Windows.Forms.Button();
            this.worker = new System.ComponentModel.BackgroundWorker();
            this.pnlFiltros.SuspendLayout();
            this.pnlResumen.SuspendLayout();
            this.pnlCentro.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdReporte)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chtReporte)).BeginInit();
            this.pnlAcciones.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlFiltros (filtros; solo se muestran los que aplican al reporte)
            //
            this.pnlFiltros.Controls.Add(this.lblTipo);
            this.pnlFiltros.Controls.Add(this.cboTipo);
            this.pnlFiltros.Controls.Add(this.lblDesde);
            this.pnlFiltros.Controls.Add(this.dtpDesde);
            this.pnlFiltros.Controls.Add(this.lblHasta);
            this.pnlFiltros.Controls.Add(this.dtpHasta);
            this.pnlFiltros.Controls.Add(this.btnGenerar);
            this.pnlFiltros.Controls.Add(this.lblPosta);
            this.pnlFiltros.Controls.Add(this.cboPosta);
            this.pnlFiltros.Controls.Add(this.lblConsultorio);
            this.pnlFiltros.Controls.Add(this.cboConsultorio);
            this.pnlFiltros.Controls.Add(this.lblEstado);
            this.pnlFiltros.Controls.Add(this.cboEstado);
            this.pnlFiltros.Controls.Add(this.lblUsuario);
            this.pnlFiltros.Controls.Add(this.txtUsuario);
            this.pnlFiltros.Controls.Add(this.lblAccion);
            this.pnlFiltros.Controls.Add(this.txtAccion);
            this.pnlFiltros.Controls.Add(this.pbGenerando);
            this.pnlFiltros.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFiltros.Name = "pnlFiltros";
            this.pnlFiltros.Size = new System.Drawing.Size(814, 80);
            this.pnlFiltros.TabIndex = 0;
            //
            // lblTipo / cboTipo
            //
            this.lblTipo.AutoSize = true;
            this.lblTipo.Location = new System.Drawing.Point(12, 15);
            this.lblTipo.Name = "lblTipo";
            this.lblTipo.TabIndex = 0;
            this.lblTipo.Text = "Reporte:";
            this.cboTipo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTipo.FormattingEnabled = true;
            this.cboTipo.Location = new System.Drawing.Point(72, 11);
            this.cboTipo.Name = "cboTipo";
            this.cboTipo.Size = new System.Drawing.Size(230, 23);
            this.cboTipo.TabIndex = 1;
            this.cboTipo.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.cboTipo_Format);
            this.cboTipo.SelectedIndexChanged += new System.EventHandler(this.cboTipo_SelectedIndexChanged);
            //
            // lblDesde / dtpDesde
            //
            this.lblDesde.AutoSize = true;
            this.lblDesde.Location = new System.Drawing.Point(316, 15);
            this.lblDesde.Name = "lblDesde";
            this.lblDesde.TabIndex = 2;
            this.lblDesde.Text = "Desde:";
            this.dtpDesde.CustomFormat = "dd/MM/yyyy";
            this.dtpDesde.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDesde.Location = new System.Drawing.Point(366, 11);
            this.dtpDesde.Name = "dtpDesde";
            this.dtpDesde.Size = new System.Drawing.Size(115, 23);
            this.dtpDesde.TabIndex = 3;
            //
            // lblHasta / dtpHasta
            //
            this.lblHasta.AutoSize = true;
            this.lblHasta.Location = new System.Drawing.Point(493, 15);
            this.lblHasta.Name = "lblHasta";
            this.lblHasta.TabIndex = 4;
            this.lblHasta.Text = "Hasta:";
            this.dtpHasta.CustomFormat = "dd/MM/yyyy";
            this.dtpHasta.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpHasta.Location = new System.Drawing.Point(539, 11);
            this.dtpHasta.Name = "dtpHasta";
            this.dtpHasta.Size = new System.Drawing.Size(115, 23);
            this.dtpHasta.TabIndex = 5;
            //
            // btnGenerar
            //
            this.btnGenerar.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.btnGenerar.Location = new System.Drawing.Point(672, 9);
            this.btnGenerar.Name = "btnGenerar";
            this.btnGenerar.Size = new System.Drawing.Size(130, 28);
            this.btnGenerar.TabIndex = 6;
            this.btnGenerar.Text = "Generar";
            this.btnGenerar.UseVisualStyleBackColor = true;
            this.btnGenerar.Click += new System.EventHandler(this.btnGenerar_Click);
            //
            // lblPosta / cboPosta
            //
            this.lblPosta.AutoSize = true;
            this.lblPosta.Location = new System.Drawing.Point(12, 49);
            this.lblPosta.Name = "lblPosta";
            this.lblPosta.TabIndex = 7;
            this.lblPosta.Text = "Posta:";
            this.cboPosta.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPosta.Location = new System.Drawing.Point(72, 45);
            this.cboPosta.Name = "cboPosta";
            this.cboPosta.Size = new System.Drawing.Size(230, 23);
            this.cboPosta.TabIndex = 8;
            this.cboPosta.SelectedIndexChanged += new System.EventHandler(this.cboPosta_SelectedIndexChanged);
            //
            // lblConsultorio / cboConsultorio (solo Citas del día)
            //
            this.lblConsultorio.AutoSize = true;
            this.lblConsultorio.Location = new System.Drawing.Point(316, 49);
            this.lblConsultorio.Name = "lblConsultorio";
            this.lblConsultorio.TabIndex = 9;
            this.lblConsultorio.Text = "Consultorio:";
            this.cboConsultorio.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboConsultorio.Location = new System.Drawing.Point(392, 45);
            this.cboConsultorio.Name = "cboConsultorio";
            this.cboConsultorio.Size = new System.Drawing.Size(165, 23);
            this.cboConsultorio.TabIndex = 10;
            //
            // lblEstado / cboEstado (solo Citas del día)
            //
            this.lblEstado.AutoSize = true;
            this.lblEstado.Location = new System.Drawing.Point(566, 49);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.TabIndex = 11;
            this.lblEstado.Text = "Estado:";
            this.cboEstado.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEstado.Location = new System.Drawing.Point(616, 45);
            this.cboEstado.Name = "cboEstado";
            this.cboEstado.Size = new System.Drawing.Size(105, 23);
            this.cboEstado.TabIndex = 12;
            //
            // lblUsuario / txtUsuario (solo Auditoría)
            //
            this.lblUsuario.AutoSize = true;
            this.lblUsuario.Location = new System.Drawing.Point(12, 49);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.TabIndex = 13;
            this.lblUsuario.Text = "Usuario:";
            this.txtUsuario.Location = new System.Drawing.Point(72, 45);
            this.txtUsuario.MaxLength = 30;
            this.txtUsuario.Name = "txtUsuario";
            this.txtUsuario.Size = new System.Drawing.Size(230, 23);
            this.txtUsuario.TabIndex = 14;
            //
            // lblAccion / txtAccion (solo Auditoría)
            //
            this.lblAccion.AutoSize = true;
            this.lblAccion.Location = new System.Drawing.Point(316, 49);
            this.lblAccion.Name = "lblAccion";
            this.lblAccion.TabIndex = 15;
            this.lblAccion.Text = "Acción:";
            this.txtAccion.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtAccion.Location = new System.Drawing.Point(366, 45);
            this.txtAccion.MaxLength = 40;
            this.txtAccion.Name = "txtAccion";
            this.txtAccion.Size = new System.Drawing.Size(191, 23);
            this.txtAccion.TabIndex = 16;
            //
            // pbGenerando (Marquee, visible solo mientras se genera)
            //
            this.pbGenerando.Location = new System.Drawing.Point(732, 50);
            this.pbGenerando.MarqueeAnimationSpeed = 30;
            this.pbGenerando.Name = "pbGenerando";
            this.pbGenerando.Size = new System.Drawing.Size(70, 14);
            this.pbGenerando.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.pbGenerando.TabIndex = 17;
            this.pbGenerando.Visible = false;
            //
            // pnlResumen (totales)
            //
            this.pnlResumen.BackColor = System.Drawing.Color.FromArgb(243, 244, 246);
            this.pnlResumen.Controls.Add(this.lblResumen);
            this.pnlResumen.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlResumen.Name = "pnlResumen";
            this.pnlResumen.Padding = new System.Windows.Forms.Padding(12, 0, 12, 0);
            this.pnlResumen.Size = new System.Drawing.Size(814, 30);
            this.pnlResumen.TabIndex = 1;
            //
            // lblResumen
            //
            this.lblResumen.AutoEllipsis = true;
            this.lblResumen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblResumen.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.lblResumen.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.lblResumen.Name = "lblResumen";
            this.lblResumen.TabIndex = 0;
            this.lblResumen.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlCentro (grilla o mensaje "sin datos", y gráfico a la derecha)
            //
            this.pnlCentro.Controls.Add(this.grdReporte);
            this.pnlCentro.Controls.Add(this.lblSinDatos);
            this.pnlCentro.Controls.Add(this.chtReporte);
            this.pnlCentro.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCentro.Name = "pnlCentro";
            this.pnlCentro.TabIndex = 2;
            //
            // grdReporte (solo lectura; se enlaza al DataTable)
            //
            this.grdReporte.AllowUserToAddRows = false;
            this.grdReporte.AllowUserToDeleteRows = false;
            this.grdReporte.AllowUserToResizeRows = false;
            this.grdReporte.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.grdReporte.BackgroundColor = System.Drawing.Color.White;
            this.grdReporte.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.grdReporte.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.grdReporte.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            estiloEncabezado.BackColor = System.Drawing.Color.FromArgb(243, 244, 246);
            estiloEncabezado.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            estiloEncabezado.ForeColor = System.Drawing.Color.FromArgb(55, 65, 81);
            estiloEncabezado.SelectionBackColor = System.Drawing.Color.FromArgb(243, 244, 246);
            estiloEncabezado.SelectionForeColor = System.Drawing.Color.FromArgb(55, 65, 81);
            this.grdReporte.ColumnHeadersDefaultCellStyle = estiloEncabezado;
            this.grdReporte.ColumnHeadersHeight = 30;
            this.grdReporte.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.grdReporte.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(219, 234, 254);
            this.grdReporte.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(30, 64, 175);
            this.grdReporte.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdReporte.EnableHeadersVisualStyles = false;
            this.grdReporte.GridColor = System.Drawing.Color.FromArgb(229, 231, 235);
            this.grdReporte.MultiSelect = false;
            this.grdReporte.Name = "grdReporte";
            this.grdReporte.ReadOnly = true;
            this.grdReporte.RowHeadersVisible = false;
            this.grdReporte.RowTemplate.Height = 24;
            this.grdReporte.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdReporte.TabIndex = 0;
            //
            // lblSinDatos ("No hay datos para esos filtros")
            //
            this.lblSinDatos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSinDatos.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblSinDatos.ForeColor = System.Drawing.Color.FromArgb(107, 114, 128);
            this.lblSinDatos.Name = "lblSinDatos";
            this.lblSinDatos.TabIndex = 1;
            this.lblSinDatos.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // chtReporte (gráfico de columnas, azul grisáceo)
            //
            areaGrafico.Name = "Area";
            areaGrafico.BackColor = System.Drawing.Color.White;
            areaGrafico.AxisX.Interval = 1D;
            areaGrafico.AxisX.MajorGrid.Enabled = false;
            areaGrafico.AxisX.LineColor = System.Drawing.Color.FromArgb(148, 163, 184);
            areaGrafico.AxisX.LabelStyle.Font = new System.Drawing.Font("Segoe UI", 8F);
            areaGrafico.AxisX.LabelStyle.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105);
            areaGrafico.AxisY.LineColor = System.Drawing.Color.FromArgb(148, 163, 184);
            areaGrafico.AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(226, 232, 240);
            areaGrafico.AxisY.LabelStyle.Font = new System.Drawing.Font("Segoe UI", 8F);
            areaGrafico.AxisY.LabelStyle.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105);
            this.chtReporte.ChartAreas.Add(areaGrafico);
            serieGrafico.ChartArea = "Area";
            serieGrafico.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
            serieGrafico.Color = System.Drawing.Color.FromArgb(100, 116, 139);
            serieGrafico.Font = new System.Drawing.Font("Segoe UI", 8F);
            serieGrafico.IsValueShownAsLabel = true;
            serieGrafico.LabelForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            serieGrafico.Name = "Serie";
            serieGrafico["PointWidth"] = "0.6";
            this.chtReporte.Series.Add(serieGrafico);
            tituloGrafico.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            tituloGrafico.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            tituloGrafico.Name = "Titulo";
            tituloGrafico.Text = "Gráfico";
            this.chtReporte.Titles.Add(tituloGrafico);
            this.chtReporte.BackColor = System.Drawing.Color.White;
            this.chtReporte.Dock = System.Windows.Forms.DockStyle.Right;
            this.chtReporte.Name = "chtReporte";
            this.chtReporte.Size = new System.Drawing.Size(300, 300);
            this.chtReporte.TabIndex = 2;
            this.chtReporte.Visible = false;
            //
            // pnlAcciones (exportar e imprimir)
            //
            this.pnlAcciones.BackColor = System.Drawing.Color.FromArgb(249, 250, 251);
            this.pnlAcciones.Controls.Add(this.btnExportarCsv);
            this.pnlAcciones.Controls.Add(this.btnImprimir);
            this.pnlAcciones.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlAcciones.Name = "pnlAcciones";
            this.pnlAcciones.Size = new System.Drawing.Size(814, 46);
            this.pnlAcciones.TabIndex = 3;
            //
            // btnExportarCsv
            //
            this.btnExportarCsv.Enabled = false;
            this.btnExportarCsv.Location = new System.Drawing.Point(12, 8);
            this.btnExportarCsv.Name = "btnExportarCsv";
            this.btnExportarCsv.Size = new System.Drawing.Size(140, 30);
            this.btnExportarCsv.TabIndex = 0;
            this.btnExportarCsv.Text = "Exportar CSV...";
            this.btnExportarCsv.UseVisualStyleBackColor = true;
            this.btnExportarCsv.Click += new System.EventHandler(this.btnExportarCsv_Click);
            //
            // btnImprimir
            //
            this.btnImprimir.Enabled = false;
            this.btnImprimir.Location = new System.Drawing.Point(160, 8);
            this.btnImprimir.Name = "btnImprimir";
            this.btnImprimir.Size = new System.Drawing.Size(120, 30);
            this.btnImprimir.TabIndex = 1;
            this.btnImprimir.Text = "Imprimir...";
            this.btnImprimir.UseVisualStyleBackColor = true;
            this.btnImprimir.Click += new System.EventHandler(this.btnImprimir_Click);
            //
            // worker (BackgroundWorker: ejecuta la consulta sin congelar la ventana)
            //
            this.worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.worker_DoWork);
            this.worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.worker_RunWorkerCompleted);
            //
            // FrmReportes
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(814, 520);
            // El panel central (Fill) va primero; de los de arriba, el último agregado queda más arriba.
            this.Controls.Add(this.pnlCentro);
            this.Controls.Add(this.pnlResumen);
            this.Controls.Add(this.pnlFiltros);
            this.Controls.Add(this.pnlAcciones);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "FrmReportes";
            this.Text = "Reportes";
            this.Load += new System.EventHandler(this.FrmReportes_Load);
            this.pnlFiltros.ResumeLayout(false);
            this.pnlFiltros.PerformLayout();
            this.pnlResumen.ResumeLayout(false);
            this.pnlCentro.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdReporte)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chtReporte)).EndInit();
            this.pnlAcciones.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlFiltros;
        private System.Windows.Forms.Label lblTipo;
        private System.Windows.Forms.ComboBox cboTipo;
        private System.Windows.Forms.Label lblDesde;
        private System.Windows.Forms.DateTimePicker dtpDesde;
        private System.Windows.Forms.Label lblHasta;
        private System.Windows.Forms.DateTimePicker dtpHasta;
        private System.Windows.Forms.Button btnGenerar;
        private System.Windows.Forms.Label lblPosta;
        private System.Windows.Forms.ComboBox cboPosta;
        private System.Windows.Forms.Label lblConsultorio;
        private System.Windows.Forms.ComboBox cboConsultorio;
        private System.Windows.Forms.Label lblEstado;
        private System.Windows.Forms.ComboBox cboEstado;
        private System.Windows.Forms.Label lblUsuario;
        private System.Windows.Forms.TextBox txtUsuario;
        private System.Windows.Forms.Label lblAccion;
        private System.Windows.Forms.TextBox txtAccion;
        private System.Windows.Forms.ProgressBar pbGenerando;
        private System.Windows.Forms.Panel pnlResumen;
        private System.Windows.Forms.Label lblResumen;
        private System.Windows.Forms.Panel pnlCentro;
        private System.Windows.Forms.DataGridView grdReporte;
        private System.Windows.Forms.Label lblSinDatos;
        private System.Windows.Forms.DataVisualization.Charting.Chart chtReporte;
        private System.Windows.Forms.Panel pnlAcciones;
        private System.Windows.Forms.Button btnExportarCsv;
        private System.Windows.Forms.Button btnImprimir;
        private System.ComponentModel.BackgroundWorker worker;
    }
}
