namespace SigepostaAdmision.Presentacion
{
    partial class FrmRespaldo
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
            this.grpContrasena = new System.Windows.Forms.GroupBox();
            this.lblContrasena = new System.Windows.Forms.Label();
            this.txtContrasena = new System.Windows.Forms.TextBox();
            this.lblConfirmacion = new System.Windows.Forms.Label();
            this.txtConfirmacion = new System.Windows.Forms.TextBox();
            this.chkCifrar = new System.Windows.Forms.CheckBox();
            this.grpHistoria = new System.Windows.Forms.GroupBox();
            this.lblNumero = new System.Windows.Forms.Label();
            this.txtNumero = new System.Windows.Forms.TextBox();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.lblPacienteHistoria = new System.Windows.Forms.Label();
            this.btnExportarHistoria = new System.Windows.Forms.Button();
            this.grpReporte = new System.Windows.Forms.GroupBox();
            this.lblFecha = new System.Windows.Forms.Label();
            this.dtpFecha = new System.Windows.Forms.DateTimePicker();
            this.lblPosta = new System.Windows.Forms.Label();
            this.cboPosta = new System.Windows.Forms.ComboBox();
            this.btnExportarReporte = new System.Windows.Forms.Button();
            this.grpClaves = new System.Windows.Forms.GroupBox();
            this.lblClavesInfo = new System.Windows.Forms.Label();
            this.btnExportarClaves = new System.Windows.Forms.Button();
            this.btnImportarClaves = new System.Windows.Forms.Button();
            this.grpVerificar = new System.Windows.Forms.GroupBox();
            this.btnAbrirRespaldo = new System.Windows.Forms.Button();
            this.txtResumen = new System.Windows.Forms.TextBox();
            this.pbProgreso = new System.Windows.Forms.ProgressBar();
            this.lblEstado = new System.Windows.Forms.Label();
            this.worker = new System.ComponentModel.BackgroundWorker();
            this.grpContrasena.SuspendLayout();
            this.grpHistoria.SuspendLayout();
            this.grpReporte.SuspendLayout();
            this.grpClaves.SuspendLayout();
            this.grpVerificar.SuspendLayout();
            this.SuspendLayout();
            //
            // grpContrasena ("Contraseña del respaldo")
            //
            this.grpContrasena.Controls.Add(this.lblContrasena);
            this.grpContrasena.Controls.Add(this.txtContrasena);
            this.grpContrasena.Controls.Add(this.lblConfirmacion);
            this.grpContrasena.Controls.Add(this.txtConfirmacion);
            this.grpContrasena.Controls.Add(this.chkCifrar);
            this.grpContrasena.Location = new System.Drawing.Point(12, 8);
            this.grpContrasena.Name = "grpContrasena";
            this.grpContrasena.Size = new System.Drawing.Size(390, 112);
            this.grpContrasena.TabIndex = 0;
            this.grpContrasena.TabStop = false;
            this.grpContrasena.Text = "Contraseña del respaldo";
            //
            // lblContrasena
            //
            this.lblContrasena.AutoSize = true;
            this.lblContrasena.Location = new System.Drawing.Point(12, 27);
            this.lblContrasena.Name = "lblContrasena";
            this.lblContrasena.TabIndex = 0;
            this.lblContrasena.Text = "Contraseña:";
            //
            // txtContrasena
            //
            this.txtContrasena.Location = new System.Drawing.Point(120, 23);
            this.txtContrasena.MaxLength = 128;
            this.txtContrasena.Name = "txtContrasena";
            this.txtContrasena.Size = new System.Drawing.Size(255, 23);
            this.txtContrasena.TabIndex = 1;
            this.txtContrasena.UseSystemPasswordChar = true;
            //
            // lblConfirmacion
            //
            this.lblConfirmacion.AutoSize = true;
            this.lblConfirmacion.Location = new System.Drawing.Point(12, 56);
            this.lblConfirmacion.Name = "lblConfirmacion";
            this.lblConfirmacion.TabIndex = 2;
            this.lblConfirmacion.Text = "Confirmación:";
            //
            // txtConfirmacion
            //
            this.txtConfirmacion.Location = new System.Drawing.Point(120, 52);
            this.txtConfirmacion.MaxLength = 128;
            this.txtConfirmacion.Name = "txtConfirmacion";
            this.txtConfirmacion.Size = new System.Drawing.Size(255, 23);
            this.txtConfirmacion.TabIndex = 3;
            this.txtConfirmacion.UseSystemPasswordChar = true;
            //
            // chkCifrar
            //
            this.chkCifrar.AutoSize = true;
            this.chkCifrar.Checked = true;
            this.chkCifrar.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCifrar.Location = new System.Drawing.Point(120, 82);
            this.chkCifrar.Name = "chkCifrar";
            this.chkCifrar.TabIndex = 4;
            this.chkCifrar.Text = "Cifrar el respaldo (recomendado)";
            this.chkCifrar.UseVisualStyleBackColor = true;
            //
            // grpHistoria ("Historia clínica")
            //
            this.grpHistoria.Controls.Add(this.lblNumero);
            this.grpHistoria.Controls.Add(this.txtNumero);
            this.grpHistoria.Controls.Add(this.btnBuscar);
            this.grpHistoria.Controls.Add(this.lblPacienteHistoria);
            this.grpHistoria.Controls.Add(this.btnExportarHistoria);
            this.grpHistoria.Location = new System.Drawing.Point(12, 126);
            this.grpHistoria.Name = "grpHistoria";
            this.grpHistoria.Size = new System.Drawing.Size(390, 116);
            this.grpHistoria.TabIndex = 1;
            this.grpHistoria.TabStop = false;
            this.grpHistoria.Text = "Historia clínica";
            //
            // lblNumero
            //
            this.lblNumero.AutoSize = true;
            this.lblNumero.Location = new System.Drawing.Point(12, 28);
            this.lblNumero.Name = "lblNumero";
            this.lblNumero.TabIndex = 0;
            this.lblNumero.Text = "N° de historia:";
            //
            // txtNumero
            //
            this.txtNumero.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtNumero.Location = new System.Drawing.Point(120, 24);
            this.txtNumero.MaxLength = 12;
            this.txtNumero.Name = "txtNumero";
            this.txtNumero.Size = new System.Drawing.Size(130, 23);
            this.txtNumero.TabIndex = 1;
            this.txtNumero.Text = "HC-";
            //
            // btnBuscar
            //
            this.btnBuscar.Location = new System.Drawing.Point(258, 22);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(117, 27);
            this.btnBuscar.TabIndex = 2;
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.UseVisualStyleBackColor = true;
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            //
            // lblPacienteHistoria (nombre del paciente con DNI enmascarado)
            //
            this.lblPacienteHistoria.AutoEllipsis = true;
            this.lblPacienteHistoria.ForeColor = System.Drawing.Color.FromArgb(55, 65, 81);
            this.lblPacienteHistoria.Location = new System.Drawing.Point(12, 54);
            this.lblPacienteHistoria.Name = "lblPacienteHistoria";
            this.lblPacienteHistoria.Size = new System.Drawing.Size(363, 20);
            this.lblPacienteHistoria.TabIndex = 3;
            this.lblPacienteHistoria.Text = "Busque una historia para ver el paciente.";
            //
            // btnExportarHistoria
            //
            this.btnExportarHistoria.Location = new System.Drawing.Point(12, 78);
            this.btnExportarHistoria.Name = "btnExportarHistoria";
            this.btnExportarHistoria.Size = new System.Drawing.Size(170, 28);
            this.btnExportarHistoria.TabIndex = 4;
            this.btnExportarHistoria.Text = "Exportar historia...";
            this.btnExportarHistoria.UseVisualStyleBackColor = true;
            this.btnExportarHistoria.Click += new System.EventHandler(this.btnExportarHistoria_Click);
            //
            // grpReporte ("Reporte diario")
            //
            this.grpReporte.Controls.Add(this.lblFecha);
            this.grpReporte.Controls.Add(this.dtpFecha);
            this.grpReporte.Controls.Add(this.lblPosta);
            this.grpReporte.Controls.Add(this.cboPosta);
            this.grpReporte.Controls.Add(this.btnExportarReporte);
            this.grpReporte.Location = new System.Drawing.Point(12, 248);
            this.grpReporte.Name = "grpReporte";
            this.grpReporte.Size = new System.Drawing.Size(390, 116);
            this.grpReporte.TabIndex = 2;
            this.grpReporte.TabStop = false;
            this.grpReporte.Text = "Reporte diario";
            //
            // lblFecha
            //
            this.lblFecha.AutoSize = true;
            this.lblFecha.Location = new System.Drawing.Point(12, 28);
            this.lblFecha.Name = "lblFecha";
            this.lblFecha.TabIndex = 0;
            this.lblFecha.Text = "Fecha:";
            //
            // dtpFecha
            //
            this.dtpFecha.CustomFormat = "ddd dd/MM/yyyy";
            this.dtpFecha.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFecha.Location = new System.Drawing.Point(120, 24);
            this.dtpFecha.Name = "dtpFecha";
            this.dtpFecha.Size = new System.Drawing.Size(150, 23);
            this.dtpFecha.TabIndex = 1;
            //
            // lblPosta
            //
            this.lblPosta.AutoSize = true;
            this.lblPosta.Location = new System.Drawing.Point(12, 57);
            this.lblPosta.Name = "lblPosta";
            this.lblPosta.TabIndex = 2;
            this.lblPosta.Text = "Posta:";
            //
            // cboPosta
            //
            this.cboPosta.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPosta.Location = new System.Drawing.Point(120, 53);
            this.cboPosta.Name = "cboPosta";
            this.cboPosta.Size = new System.Drawing.Size(255, 23);
            this.cboPosta.TabIndex = 3;
            //
            // btnExportarReporte
            //
            this.btnExportarReporte.Location = new System.Drawing.Point(12, 80);
            this.btnExportarReporte.Name = "btnExportarReporte";
            this.btnExportarReporte.Size = new System.Drawing.Size(170, 28);
            this.btnExportarReporte.TabIndex = 4;
            this.btnExportarReporte.Text = "Exportar reporte...";
            this.btnExportarReporte.UseVisualStyleBackColor = true;
            this.btnExportarReporte.Click += new System.EventHandler(this.btnExportarReporte_Click);
            //
            // grpClaves ("Claves del sistema")
            //
            this.grpClaves.Controls.Add(this.lblClavesInfo);
            this.grpClaves.Controls.Add(this.btnExportarClaves);
            this.grpClaves.Controls.Add(this.btnImportarClaves);
            this.grpClaves.Location = new System.Drawing.Point(12, 370);
            this.grpClaves.Name = "grpClaves";
            this.grpClaves.Size = new System.Drawing.Size(390, 92);
            this.grpClaves.TabIndex = 3;
            this.grpClaves.TabStop = false;
            this.grpClaves.Text = "Claves del sistema";
            //
            // lblClavesInfo
            //
            this.lblClavesInfo.ForeColor = System.Drawing.Color.FromArgb(107, 114, 128);
            this.lblClavesInfo.Location = new System.Drawing.Point(12, 20);
            this.lblClavesInfo.Name = "lblClavesInfo";
            this.lblClavesInfo.Size = new System.Drawing.Size(363, 32);
            this.lblClavesInfo.TabIndex = 0;
            this.lblClavesInfo.Text = "Sin una copia de las claves, los DNI y diagnósticos cifrados no se podrán leer en otro equipo o usuario.";
            //
            // btnExportarClaves
            //
            this.btnExportarClaves.Location = new System.Drawing.Point(12, 55);
            this.btnExportarClaves.Name = "btnExportarClaves";
            this.btnExportarClaves.Size = new System.Drawing.Size(170, 28);
            this.btnExportarClaves.TabIndex = 1;
            this.btnExportarClaves.Text = "Exportar claves...";
            this.btnExportarClaves.UseVisualStyleBackColor = true;
            this.btnExportarClaves.Click += new System.EventHandler(this.btnExportarClaves_Click);
            //
            // btnImportarClaves
            //
            this.btnImportarClaves.ForeColor = System.Drawing.Color.FromArgb(146, 64, 14);
            this.btnImportarClaves.Location = new System.Drawing.Point(205, 55);
            this.btnImportarClaves.Name = "btnImportarClaves";
            this.btnImportarClaves.Size = new System.Drawing.Size(170, 28);
            this.btnImportarClaves.TabIndex = 2;
            this.btnImportarClaves.Text = "Importar claves...";
            this.btnImportarClaves.UseVisualStyleBackColor = true;
            this.btnImportarClaves.Click += new System.EventHandler(this.btnImportarClaves_Click);
            //
            // grpVerificar ("Verificar respaldo")
            //
            this.grpVerificar.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.grpVerificar.Controls.Add(this.btnAbrirRespaldo);
            this.grpVerificar.Controls.Add(this.txtResumen);
            this.grpVerificar.Location = new System.Drawing.Point(414, 8);
            this.grpVerificar.Name = "grpVerificar";
            this.grpVerificar.Size = new System.Drawing.Size(388, 454);
            this.grpVerificar.TabIndex = 4;
            this.grpVerificar.TabStop = false;
            this.grpVerificar.Text = "Verificar respaldo";
            //
            // btnAbrirRespaldo
            //
            this.btnAbrirRespaldo.Location = new System.Drawing.Point(12, 22);
            this.btnAbrirRespaldo.Name = "btnAbrirRespaldo";
            this.btnAbrirRespaldo.Size = new System.Drawing.Size(170, 28);
            this.btnAbrirRespaldo.TabIndex = 0;
            this.btnAbrirRespaldo.Text = "Abrir respaldo...";
            this.btnAbrirRespaldo.UseVisualStyleBackColor = true;
            this.btnAbrirRespaldo.Click += new System.EventHandler(this.btnAbrirRespaldo_Click);
            //
            // txtResumen (solo lectura)
            //
            this.txtResumen.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtResumen.BackColor = System.Drawing.Color.FromArgb(249, 250, 251);
            this.txtResumen.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtResumen.Location = new System.Drawing.Point(12, 58);
            this.txtResumen.Multiline = true;
            this.txtResumen.Name = "txtResumen";
            this.txtResumen.ReadOnly = true;
            this.txtResumen.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtResumen.Size = new System.Drawing.Size(364, 384);
            this.txtResumen.TabIndex = 1;
            this.txtResumen.WordWrap = false;
            //
            // pbProgreso
            //
            this.pbProgreso.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pbProgreso.Location = new System.Drawing.Point(12, 474);
            this.pbProgreso.Name = "pbProgreso";
            this.pbProgreso.Size = new System.Drawing.Size(390, 18);
            this.pbProgreso.TabIndex = 5;
            //
            // lblEstado
            //
            this.lblEstado.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lblEstado.AutoEllipsis = true;
            this.lblEstado.ForeColor = System.Drawing.Color.FromArgb(55, 65, 81);
            this.lblEstado.Location = new System.Drawing.Point(414, 472);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Size = new System.Drawing.Size(388, 22);
            this.lblEstado.TabIndex = 6;
            this.lblEstado.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // worker (BackgroundWorker: exporta y verifica sin congelar la ventana)
            //
            this.worker.WorkerReportsProgress = true;
            this.worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.worker_DoWork);
            this.worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.worker_ProgressChanged);
            this.worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.worker_RunWorkerCompleted);
            //
            // FrmRespaldo
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(814, 520);
            this.Controls.Add(this.grpContrasena);
            this.Controls.Add(this.grpHistoria);
            this.Controls.Add(this.grpReporte);
            this.Controls.Add(this.grpClaves);
            this.Controls.Add(this.grpVerificar);
            this.Controls.Add(this.pbProgreso);
            this.Controls.Add(this.lblEstado);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "FrmRespaldo";
            this.Text = "Respaldo";
            this.Load += new System.EventHandler(this.FrmRespaldo_Load);
            this.grpContrasena.ResumeLayout(false);
            this.grpContrasena.PerformLayout();
            this.grpHistoria.ResumeLayout(false);
            this.grpHistoria.PerformLayout();
            this.grpReporte.ResumeLayout(false);
            this.grpReporte.PerformLayout();
            this.grpClaves.ResumeLayout(false);
            this.grpVerificar.ResumeLayout(false);
            this.grpVerificar.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox grpContrasena;
        private System.Windows.Forms.Label lblContrasena;
        private System.Windows.Forms.TextBox txtContrasena;
        private System.Windows.Forms.Label lblConfirmacion;
        private System.Windows.Forms.TextBox txtConfirmacion;
        private System.Windows.Forms.CheckBox chkCifrar;
        private System.Windows.Forms.GroupBox grpHistoria;
        private System.Windows.Forms.Label lblNumero;
        private System.Windows.Forms.TextBox txtNumero;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.Label lblPacienteHistoria;
        private System.Windows.Forms.Button btnExportarHistoria;
        private System.Windows.Forms.GroupBox grpReporte;
        private System.Windows.Forms.Label lblFecha;
        private System.Windows.Forms.DateTimePicker dtpFecha;
        private System.Windows.Forms.Label lblPosta;
        private System.Windows.Forms.ComboBox cboPosta;
        private System.Windows.Forms.Button btnExportarReporte;
        private System.Windows.Forms.GroupBox grpClaves;
        private System.Windows.Forms.Label lblClavesInfo;
        private System.Windows.Forms.Button btnExportarClaves;
        private System.Windows.Forms.Button btnImportarClaves;
        private System.Windows.Forms.GroupBox grpVerificar;
        private System.Windows.Forms.Button btnAbrirRespaldo;
        private System.Windows.Forms.TextBox txtResumen;
        private System.Windows.Forms.ProgressBar pbProgreso;
        private System.Windows.Forms.Label lblEstado;
        private System.ComponentModel.BackgroundWorker worker;
    }
}
