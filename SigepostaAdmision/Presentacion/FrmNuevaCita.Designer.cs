namespace SigepostaAdmision.Presentacion
{
    partial class FrmNuevaCita
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
            this.lblSeccionPaciente = new System.Windows.Forms.Label();
            this.lblDni = new System.Windows.Forms.Label();
            this.txtDni = new System.Windows.Forms.TextBox();
            this.btnBuscarPaciente = new System.Windows.Forms.Button();
            this.lblNombreTitulo = new System.Windows.Forms.Label();
            this.lblNombre = new System.Windows.Forms.Label();
            this.lblEdadTitulo = new System.Windows.Forms.Label();
            this.lblEdad = new System.Windows.Forms.Label();
            this.lblHistoriaTitulo = new System.Windows.Forms.Label();
            this.lblHistoria = new System.Windows.Forms.Label();
            this.pnlSeparador = new System.Windows.Forms.Panel();
            this.lblSeccionProgramacion = new System.Windows.Forms.Label();
            this.lblServicio = new System.Windows.Forms.Label();
            this.cboServicio = new System.Windows.Forms.ComboBox();
            this.lblConsultorio = new System.Windows.Forms.Label();
            this.cboConsultorio = new System.Windows.Forms.ComboBox();
            this.lblProfesional = new System.Windows.Forms.Label();
            this.cboProfesional = new System.Windows.Forms.ComboBox();
            this.lblFecha = new System.Windows.Forms.Label();
            this.dtpFecha = new System.Windows.Forms.DateTimePicker();
            this.lblHora = new System.Windows.Forms.Label();
            this.cboHora = new System.Windows.Forms.ComboBox();
            this.lblFranjas = new System.Windows.Forms.Label();
            this.btnGuardar = new System.Windows.Forms.Button();
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
            this.lblTitulo.Text = "Programar nueva cita";
            //
            // lblSeccionPaciente
            //
            this.lblSeccionPaciente.AutoSize = true;
            this.lblSeccionPaciente.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.lblSeccionPaciente.Location = new System.Drawing.Point(16, 48);
            this.lblSeccionPaciente.Name = "lblSeccionPaciente";
            this.lblSeccionPaciente.TabIndex = 1;
            this.lblSeccionPaciente.Text = "Paciente";
            //
            // lblDni
            //
            this.lblDni.AutoSize = true;
            this.lblDni.Location = new System.Drawing.Point(16, 78);
            this.lblDni.Name = "lblDni";
            this.lblDni.TabIndex = 2;
            this.lblDni.Text = "DNI:";
            //
            // txtDni
            //
            this.txtDni.Location = new System.Drawing.Point(110, 74);
            this.txtDni.MaxLength = 8;
            this.txtDni.Name = "txtDni";
            this.txtDni.Size = new System.Drawing.Size(120, 23);
            this.txtDni.TabIndex = 3;
            this.txtDni.TextChanged += new System.EventHandler(this.txtDni_TextChanged);
            this.txtDni.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtDni_KeyDown);
            //
            // btnBuscarPaciente
            //
            this.btnBuscarPaciente.Location = new System.Drawing.Point(238, 72);
            this.btnBuscarPaciente.Name = "btnBuscarPaciente";
            this.btnBuscarPaciente.Size = new System.Drawing.Size(130, 27);
            this.btnBuscarPaciente.TabIndex = 4;
            this.btnBuscarPaciente.Text = "Buscar paciente";
            this.btnBuscarPaciente.UseVisualStyleBackColor = true;
            this.btnBuscarPaciente.Click += new System.EventHandler(this.btnBuscarPaciente_Click);
            //
            // lblNombreTitulo
            //
            this.lblNombreTitulo.AutoSize = true;
            this.lblNombreTitulo.ForeColor = System.Drawing.Color.FromArgb(107, 114, 128);
            this.lblNombreTitulo.Location = new System.Drawing.Point(16, 108);
            this.lblNombreTitulo.Name = "lblNombreTitulo";
            this.lblNombreTitulo.TabIndex = 5;
            this.lblNombreTitulo.Text = "Nombre:";
            //
            // lblNombre
            //
            this.lblNombre.AutoEllipsis = true;
            this.lblNombre.Location = new System.Drawing.Point(110, 108);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Size = new System.Drawing.Size(374, 18);
            this.lblNombre.TabIndex = 6;
            this.lblNombre.Text = "—";
            //
            // lblEdadTitulo
            //
            this.lblEdadTitulo.AutoSize = true;
            this.lblEdadTitulo.ForeColor = System.Drawing.Color.FromArgb(107, 114, 128);
            this.lblEdadTitulo.Location = new System.Drawing.Point(16, 130);
            this.lblEdadTitulo.Name = "lblEdadTitulo";
            this.lblEdadTitulo.TabIndex = 7;
            this.lblEdadTitulo.Text = "Edad:";
            //
            // lblEdad
            //
            this.lblEdad.Location = new System.Drawing.Point(110, 130);
            this.lblEdad.Name = "lblEdad";
            this.lblEdad.Size = new System.Drawing.Size(120, 18);
            this.lblEdad.TabIndex = 8;
            this.lblEdad.Text = "—";
            //
            // lblHistoriaTitulo
            //
            this.lblHistoriaTitulo.AutoSize = true;
            this.lblHistoriaTitulo.ForeColor = System.Drawing.Color.FromArgb(107, 114, 128);
            this.lblHistoriaTitulo.Location = new System.Drawing.Point(238, 130);
            this.lblHistoriaTitulo.Name = "lblHistoriaTitulo";
            this.lblHistoriaTitulo.TabIndex = 9;
            this.lblHistoriaTitulo.Text = "Historia:";
            //
            // lblHistoria
            //
            this.lblHistoria.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.lblHistoria.Location = new System.Drawing.Point(296, 130);
            this.lblHistoria.Name = "lblHistoria";
            this.lblHistoria.Size = new System.Drawing.Size(188, 18);
            this.lblHistoria.TabIndex = 10;
            this.lblHistoria.Text = "—";
            //
            // pnlSeparador (línea divisoria)
            //
            this.pnlSeparador.BackColor = System.Drawing.Color.FromArgb(229, 231, 235);
            this.pnlSeparador.Location = new System.Drawing.Point(16, 160);
            this.pnlSeparador.Name = "pnlSeparador";
            this.pnlSeparador.Size = new System.Drawing.Size(468, 1);
            this.pnlSeparador.TabIndex = 11;
            //
            // lblSeccionProgramacion
            //
            this.lblSeccionProgramacion.AutoSize = true;
            this.lblSeccionProgramacion.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.lblSeccionProgramacion.Location = new System.Drawing.Point(16, 172);
            this.lblSeccionProgramacion.Name = "lblSeccionProgramacion";
            this.lblSeccionProgramacion.TabIndex = 12;
            this.lblSeccionProgramacion.Text = "Programación";
            //
            // lblServicio
            //
            this.lblServicio.AutoSize = true;
            this.lblServicio.Location = new System.Drawing.Point(16, 203);
            this.lblServicio.Name = "lblServicio";
            this.lblServicio.TabIndex = 13;
            this.lblServicio.Text = "Servicio:";
            //
            // cboServicio
            //
            this.cboServicio.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboServicio.Location = new System.Drawing.Point(110, 199);
            this.cboServicio.Name = "cboServicio";
            this.cboServicio.Size = new System.Drawing.Size(374, 23);
            this.cboServicio.TabIndex = 14;
            this.cboServicio.SelectedIndexChanged += new System.EventHandler(this.cboServicio_SelectedIndexChanged);
            //
            // lblConsultorio
            //
            this.lblConsultorio.AutoSize = true;
            this.lblConsultorio.Location = new System.Drawing.Point(16, 235);
            this.lblConsultorio.Name = "lblConsultorio";
            this.lblConsultorio.TabIndex = 15;
            this.lblConsultorio.Text = "Consultorio:";
            //
            // cboConsultorio
            //
            this.cboConsultorio.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboConsultorio.Location = new System.Drawing.Point(110, 231);
            this.cboConsultorio.Name = "cboConsultorio";
            this.cboConsultorio.Size = new System.Drawing.Size(374, 23);
            this.cboConsultorio.TabIndex = 16;
            this.cboConsultorio.SelectedIndexChanged += new System.EventHandler(this.cboConsultorio_SelectedIndexChanged);
            //
            // lblProfesional
            //
            this.lblProfesional.AutoSize = true;
            this.lblProfesional.Location = new System.Drawing.Point(16, 267);
            this.lblProfesional.Name = "lblProfesional";
            this.lblProfesional.TabIndex = 17;
            this.lblProfesional.Text = "Profesional:";
            //
            // cboProfesional
            //
            this.cboProfesional.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProfesional.Location = new System.Drawing.Point(110, 263);
            this.cboProfesional.Name = "cboProfesional";
            this.cboProfesional.Size = new System.Drawing.Size(374, 23);
            this.cboProfesional.TabIndex = 18;
            this.cboProfesional.SelectedIndexChanged += new System.EventHandler(this.cboProfesional_SelectedIndexChanged);
            //
            // lblFecha
            //
            this.lblFecha.AutoSize = true;
            this.lblFecha.Location = new System.Drawing.Point(16, 299);
            this.lblFecha.Name = "lblFecha";
            this.lblFecha.TabIndex = 19;
            this.lblFecha.Text = "Fecha:";
            //
            // dtpFecha
            //
            this.dtpFecha.CustomFormat = "ddd dd/MM/yyyy";
            this.dtpFecha.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFecha.Location = new System.Drawing.Point(110, 295);
            this.dtpFecha.Name = "dtpFecha";
            this.dtpFecha.Size = new System.Drawing.Size(150, 23);
            this.dtpFecha.TabIndex = 20;
            this.dtpFecha.ValueChanged += new System.EventHandler(this.dtpFecha_ValueChanged);
            //
            // lblHora
            //
            this.lblHora.AutoSize = true;
            this.lblHora.Location = new System.Drawing.Point(280, 299);
            this.lblHora.Name = "lblHora";
            this.lblHora.TabIndex = 21;
            this.lblHora.Text = "Hora:";
            //
            // cboHora
            //
            this.cboHora.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboHora.FormattingEnabled = true;
            this.cboHora.Location = new System.Drawing.Point(324, 295);
            this.cboHora.Name = "cboHora";
            this.cboHora.Size = new System.Drawing.Size(160, 23);
            this.cboHora.TabIndex = 22;
            this.cboHora.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.cboHora_Format);
            //
            // lblFranjas (información de horarios libres)
            //
            this.lblFranjas.ForeColor = System.Drawing.Color.FromArgb(107, 114, 128);
            this.lblFranjas.Location = new System.Drawing.Point(110, 324);
            this.lblFranjas.Name = "lblFranjas";
            this.lblFranjas.Size = new System.Drawing.Size(374, 18);
            this.lblFranjas.TabIndex = 23;
            //
            // btnGuardar
            //
            this.btnGuardar.Location = new System.Drawing.Point(266, 360);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(105, 30);
            this.btnGuardar.TabIndex = 24;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            //
            // btnCancelar
            //
            this.btnCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelar.Location = new System.Drawing.Point(379, 360);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(105, 30);
            this.btnCancelar.TabIndex = 25;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            //
            // FrmNuevaCita
            //
            // No se define AcceptButton a propósito: Enter en la caja DNI busca al
            // paciente en lugar de guardar la cita.
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.btnCancelar;
            this.ClientSize = new System.Drawing.Size(500, 406);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.lblSeccionPaciente);
            this.Controls.Add(this.lblDni);
            this.Controls.Add(this.txtDni);
            this.Controls.Add(this.btnBuscarPaciente);
            this.Controls.Add(this.lblNombreTitulo);
            this.Controls.Add(this.lblNombre);
            this.Controls.Add(this.lblEdadTitulo);
            this.Controls.Add(this.lblEdad);
            this.Controls.Add(this.lblHistoriaTitulo);
            this.Controls.Add(this.lblHistoria);
            this.Controls.Add(this.pnlSeparador);
            this.Controls.Add(this.lblSeccionProgramacion);
            this.Controls.Add(this.lblServicio);
            this.Controls.Add(this.cboServicio);
            this.Controls.Add(this.lblConsultorio);
            this.Controls.Add(this.cboConsultorio);
            this.Controls.Add(this.lblProfesional);
            this.Controls.Add(this.cboProfesional);
            this.Controls.Add(this.lblFecha);
            this.Controls.Add(this.dtpFecha);
            this.Controls.Add(this.lblHora);
            this.Controls.Add(this.cboHora);
            this.Controls.Add(this.lblFranjas);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.btnCancelar);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmNuevaCita";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Nueva cita";
            this.Load += new System.EventHandler(this.FrmNuevaCita_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblSeccionPaciente;
        private System.Windows.Forms.Label lblDni;
        private System.Windows.Forms.TextBox txtDni;
        private System.Windows.Forms.Button btnBuscarPaciente;
        private System.Windows.Forms.Label lblNombreTitulo;
        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.Label lblEdadTitulo;
        private System.Windows.Forms.Label lblEdad;
        private System.Windows.Forms.Label lblHistoriaTitulo;
        private System.Windows.Forms.Label lblHistoria;
        private System.Windows.Forms.Panel pnlSeparador;
        private System.Windows.Forms.Label lblSeccionProgramacion;
        private System.Windows.Forms.Label lblServicio;
        private System.Windows.Forms.ComboBox cboServicio;
        private System.Windows.Forms.Label lblConsultorio;
        private System.Windows.Forms.ComboBox cboConsultorio;
        private System.Windows.Forms.Label lblProfesional;
        private System.Windows.Forms.ComboBox cboProfesional;
        private System.Windows.Forms.Label lblFecha;
        private System.Windows.Forms.DateTimePicker dtpFecha;
        private System.Windows.Forms.Label lblHora;
        private System.Windows.Forms.ComboBox cboHora;
        private System.Windows.Forms.Label lblFranjas;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCancelar;
    }
}
