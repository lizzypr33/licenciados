namespace SigepostaAdmision.Presentacion
{
    partial class FrmRegistrarAtencion
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
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblPaciente = new System.Windows.Forms.Label();
            this.lblPacienteValor = new System.Windows.Forms.Label();
            this.lblDni = new System.Windows.Forms.Label();
            this.lblDniValor = new System.Windows.Forms.Label();
            this.lblConsultorio = new System.Windows.Forms.Label();
            this.lblConsultorioValor = new System.Windows.Forms.Label();
            this.lblProfesional = new System.Windows.Forms.Label();
            this.lblProfesionalValor = new System.Windows.Forms.Label();
            this.lblLlamado = new System.Windows.Forms.Label();
            this.lblLlamadoValor = new System.Windows.Forms.Label();
            this.lblObservaciones = new System.Windows.Forms.Label();
            this.txtObservaciones = new System.Windows.Forms.TextBox();
            this.lblContador = new System.Windows.Forms.Label();
            this.lblDiagnostico = new System.Windows.Forms.Label();
            this.txtDiagnostico = new System.Windows.Forms.TextBox();
            this.lblContadorDiagnostico = new System.Windows.Forms.Label();
            this.btnConfirmar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // lblTitulo
            //
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(30, 64, 175);
            this.lblTitulo.Location = new System.Drawing.Point(14, 12);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Registrar atención";
            //
            // lblPaciente / lblPacienteValor
            //
            this.lblPaciente.AutoSize = true;
            this.lblPaciente.ForeColor = System.Drawing.Color.FromArgb(107, 114, 128);
            this.lblPaciente.Location = new System.Drawing.Point(16, 52);
            this.lblPaciente.Name = "lblPaciente";
            this.lblPaciente.TabIndex = 1;
            this.lblPaciente.Text = "Paciente:";
            this.lblPacienteValor.AutoEllipsis = true;
            this.lblPacienteValor.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.lblPacienteValor.Location = new System.Drawing.Point(130, 52);
            this.lblPacienteValor.Name = "lblPacienteValor";
            this.lblPacienteValor.Size = new System.Drawing.Size(314, 18);
            this.lblPacienteValor.TabIndex = 2;
            //
            // lblDni / lblDniValor
            //
            this.lblDni.AutoSize = true;
            this.lblDni.ForeColor = System.Drawing.Color.FromArgb(107, 114, 128);
            this.lblDni.Location = new System.Drawing.Point(16, 76);
            this.lblDni.Name = "lblDni";
            this.lblDni.TabIndex = 3;
            this.lblDni.Text = "DNI:";
            this.lblDniValor.Location = new System.Drawing.Point(130, 76);
            this.lblDniValor.Name = "lblDniValor";
            this.lblDniValor.Size = new System.Drawing.Size(314, 18);
            this.lblDniValor.TabIndex = 4;
            //
            // lblConsultorio / lblConsultorioValor
            //
            this.lblConsultorio.AutoSize = true;
            this.lblConsultorio.ForeColor = System.Drawing.Color.FromArgb(107, 114, 128);
            this.lblConsultorio.Location = new System.Drawing.Point(16, 100);
            this.lblConsultorio.Name = "lblConsultorio";
            this.lblConsultorio.TabIndex = 5;
            this.lblConsultorio.Text = "Consultorio:";
            this.lblConsultorioValor.AutoEllipsis = true;
            this.lblConsultorioValor.Location = new System.Drawing.Point(130, 100);
            this.lblConsultorioValor.Name = "lblConsultorioValor";
            this.lblConsultorioValor.Size = new System.Drawing.Size(314, 18);
            this.lblConsultorioValor.TabIndex = 6;
            //
            // lblProfesional / lblProfesionalValor
            //
            this.lblProfesional.AutoSize = true;
            this.lblProfesional.ForeColor = System.Drawing.Color.FromArgb(107, 114, 128);
            this.lblProfesional.Location = new System.Drawing.Point(16, 124);
            this.lblProfesional.Name = "lblProfesional";
            this.lblProfesional.TabIndex = 7;
            this.lblProfesional.Text = "Profesional:";
            this.lblProfesionalValor.AutoEllipsis = true;
            this.lblProfesionalValor.Location = new System.Drawing.Point(130, 124);
            this.lblProfesionalValor.Name = "lblProfesionalValor";
            this.lblProfesionalValor.Size = new System.Drawing.Size(314, 18);
            this.lblProfesionalValor.TabIndex = 8;
            //
            // lblLlamado / lblLlamadoValor
            //
            this.lblLlamado.AutoSize = true;
            this.lblLlamado.ForeColor = System.Drawing.Color.FromArgb(107, 114, 128);
            this.lblLlamado.Location = new System.Drawing.Point(16, 148);
            this.lblLlamado.Name = "lblLlamado";
            this.lblLlamado.TabIndex = 9;
            this.lblLlamado.Text = "Llamado a las:";
            this.lblLlamadoValor.Location = new System.Drawing.Point(130, 148);
            this.lblLlamadoValor.Name = "lblLlamadoValor";
            this.lblLlamadoValor.Size = new System.Drawing.Size(314, 18);
            this.lblLlamadoValor.TabIndex = 10;
            //
            // lblObservaciones
            //
            this.lblObservaciones.AutoSize = true;
            this.lblObservaciones.Location = new System.Drawing.Point(16, 182);
            this.lblObservaciones.Name = "lblObservaciones";
            this.lblObservaciones.TabIndex = 11;
            this.lblObservaciones.Text = "Observaciones (opcional):";
            //
            // txtObservaciones
            //
            this.txtObservaciones.Location = new System.Drawing.Point(16, 202);
            this.txtObservaciones.MaxLength = 300;
            this.txtObservaciones.Multiline = true;
            this.txtObservaciones.Name = "txtObservaciones";
            this.txtObservaciones.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtObservaciones.Size = new System.Drawing.Size(428, 100);
            this.txtObservaciones.TabIndex = 12;
            this.txtObservaciones.TextChanged += new System.EventHandler(this.txtObservaciones_TextChanged);
            //
            // lblContador ("n / 300")
            //
            this.lblContador.Location = new System.Drawing.Point(324, 305);
            this.lblContador.Name = "lblContador";
            this.lblContador.Size = new System.Drawing.Size(120, 18);
            this.lblContador.TabIndex = 13;
            this.lblContador.Text = "0 / 300";
            this.lblContador.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblDiagnostico
            //
            this.lblDiagnostico.AutoSize = true;
            this.lblDiagnostico.Location = new System.Drawing.Point(16, 328);
            this.lblDiagnostico.Name = "lblDiagnostico";
            this.lblDiagnostico.TabIndex = 16;
            this.lblDiagnostico.Text = "Diagnóstico (lo indica el personal de salud, opcional; se guarda cifrado):";
            //
            // txtDiagnostico
            //
            this.txtDiagnostico.Location = new System.Drawing.Point(16, 348);
            this.txtDiagnostico.MaxLength = 500;
            this.txtDiagnostico.Multiline = true;
            this.txtDiagnostico.Name = "txtDiagnostico";
            this.txtDiagnostico.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDiagnostico.Size = new System.Drawing.Size(428, 90);
            this.txtDiagnostico.TabIndex = 17;
            this.txtDiagnostico.TextChanged += new System.EventHandler(this.txtDiagnostico_TextChanged);
            //
            // lblContadorDiagnostico ("n / 500")
            //
            this.lblContadorDiagnostico.Location = new System.Drawing.Point(324, 441);
            this.lblContadorDiagnostico.Name = "lblContadorDiagnostico";
            this.lblContadorDiagnostico.Size = new System.Drawing.Size(120, 18);
            this.lblContadorDiagnostico.TabIndex = 18;
            this.lblContadorDiagnostico.Text = "0 / 500";
            this.lblContadorDiagnostico.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // btnConfirmar
            //
            this.btnConfirmar.Location = new System.Drawing.Point(226, 472);
            this.btnConfirmar.Name = "btnConfirmar";
            this.btnConfirmar.Size = new System.Drawing.Size(105, 30);
            this.btnConfirmar.TabIndex = 14;
            this.btnConfirmar.Text = "Confirmar";
            this.btnConfirmar.UseVisualStyleBackColor = true;
            this.btnConfirmar.Click += new System.EventHandler(this.btnConfirmar_Click);
            //
            // btnCancelar
            //
            this.btnCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelar.Location = new System.Drawing.Point(339, 472);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(105, 30);
            this.btnCancelar.TabIndex = 15;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            //
            // FrmRegistrarAtencion
            //
            // Sin AcceptButton: en el cuadro multilínea, Enter debe crear una nueva línea.
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.btnCancelar;
            this.ClientSize = new System.Drawing.Size(460, 516);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.lblPaciente);
            this.Controls.Add(this.lblPacienteValor);
            this.Controls.Add(this.lblDni);
            this.Controls.Add(this.lblDniValor);
            this.Controls.Add(this.lblConsultorio);
            this.Controls.Add(this.lblConsultorioValor);
            this.Controls.Add(this.lblProfesional);
            this.Controls.Add(this.lblProfesionalValor);
            this.Controls.Add(this.lblLlamado);
            this.Controls.Add(this.lblLlamadoValor);
            this.Controls.Add(this.lblObservaciones);
            this.Controls.Add(this.txtObservaciones);
            this.Controls.Add(this.lblContador);
            this.Controls.Add(this.lblDiagnostico);
            this.Controls.Add(this.txtDiagnostico);
            this.Controls.Add(this.lblContadorDiagnostico);
            this.Controls.Add(this.btnConfirmar);
            this.Controls.Add(this.btnCancelar);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmRegistrarAtencion";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Registrar atención";
            this.Load += new System.EventHandler(this.FrmRegistrarAtencion_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblPaciente;
        private System.Windows.Forms.Label lblPacienteValor;
        private System.Windows.Forms.Label lblDni;
        private System.Windows.Forms.Label lblDniValor;
        private System.Windows.Forms.Label lblConsultorio;
        private System.Windows.Forms.Label lblConsultorioValor;
        private System.Windows.Forms.Label lblProfesional;
        private System.Windows.Forms.Label lblProfesionalValor;
        private System.Windows.Forms.Label lblLlamado;
        private System.Windows.Forms.Label lblLlamadoValor;
        private System.Windows.Forms.Label lblObservaciones;
        private System.Windows.Forms.TextBox txtObservaciones;
        private System.Windows.Forms.Label lblContador;
        private System.Windows.Forms.Label lblDiagnostico;
        private System.Windows.Forms.TextBox txtDiagnostico;
        private System.Windows.Forms.Label lblContadorDiagnostico;
        private System.Windows.Forms.Button btnConfirmar;
        private System.Windows.Forms.Button btnCancelar;
    }
}
