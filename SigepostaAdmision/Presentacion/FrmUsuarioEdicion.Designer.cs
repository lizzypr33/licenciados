namespace SigepostaAdmision.Presentacion
{
    partial class FrmUsuarioEdicion
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
            this.lblUsuario = new System.Windows.Forms.Label();
            this.txtUsuario = new System.Windows.Forms.TextBox();
            this.lblRol = new System.Windows.Forms.Label();
            this.cboRol = new System.Windows.Forms.ComboBox();
            this.lblPosta = new System.Windows.Forms.Label();
            this.cboPosta = new System.Windows.Forms.ComboBox();
            this.lblAyuda = new System.Windows.Forms.Label();
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
            this.lblTitulo.Text = "Usuario";
            //
            // lblUsuario / txtUsuario
            //
            this.lblUsuario.AutoSize = true;
            this.lblUsuario.Location = new System.Drawing.Point(16, 56);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.TabIndex = 1;
            this.lblUsuario.Text = "Usuario:";
            this.txtUsuario.Location = new System.Drawing.Point(110, 52);
            this.txtUsuario.MaxLength = 30;
            this.txtUsuario.Name = "txtUsuario";
            this.txtUsuario.Size = new System.Drawing.Size(270, 23);
            this.txtUsuario.TabIndex = 2;
            //
            // lblRol / cboRol
            //
            this.lblRol.AutoSize = true;
            this.lblRol.Location = new System.Drawing.Point(16, 90);
            this.lblRol.Name = "lblRol";
            this.lblRol.TabIndex = 3;
            this.lblRol.Text = "Rol:";
            this.cboRol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRol.FormattingEnabled = true;
            this.cboRol.Location = new System.Drawing.Point(110, 86);
            this.cboRol.Name = "cboRol";
            this.cboRol.Size = new System.Drawing.Size(270, 23);
            this.cboRol.TabIndex = 4;
            this.cboRol.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.cboRol_Format);
            this.cboRol.SelectedIndexChanged += new System.EventHandler(this.cboRol_SelectedIndexChanged);
            //
            // lblPosta / cboPosta (obligatoria solo para Admisión)
            //
            this.lblPosta.AutoSize = true;
            this.lblPosta.Location = new System.Drawing.Point(16, 124);
            this.lblPosta.Name = "lblPosta";
            this.lblPosta.TabIndex = 5;
            this.lblPosta.Text = "Posta:";
            this.cboPosta.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPosta.Location = new System.Drawing.Point(110, 120);
            this.cboPosta.Name = "cboPosta";
            this.cboPosta.Size = new System.Drawing.Size(270, 23);
            this.cboPosta.TabIndex = 6;
            //
            // lblAyuda
            //
            this.lblAyuda.ForeColor = System.Drawing.Color.FromArgb(107, 114, 128);
            this.lblAyuda.Location = new System.Drawing.Point(16, 152);
            this.lblAyuda.Name = "lblAyuda";
            this.lblAyuda.Size = new System.Drawing.Size(364, 34);
            this.lblAyuda.TabIndex = 7;
            this.lblAyuda.Text = "Usuario: 4 a 30 caracteres (letras, números, punto o guion bajo). " +
                "Al crear, se genera una contraseña temporal.";
            //
            // btnGuardar
            //
            this.btnGuardar.Location = new System.Drawing.Point(172, 196);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(100, 30);
            this.btnGuardar.TabIndex = 8;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            //
            // btnCancelar
            //
            this.btnCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelar.Location = new System.Drawing.Point(280, 196);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(100, 30);
            this.btnCancelar.TabIndex = 9;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            //
            // FrmUsuarioEdicion
            //
            this.AcceptButton = this.btnGuardar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.btnCancelar;
            this.ClientSize = new System.Drawing.Size(396, 240);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.lblUsuario);
            this.Controls.Add(this.txtUsuario);
            this.Controls.Add(this.lblRol);
            this.Controls.Add(this.cboRol);
            this.Controls.Add(this.lblPosta);
            this.Controls.Add(this.cboPosta);
            this.Controls.Add(this.lblAyuda);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.btnCancelar);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmUsuarioEdicion";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Usuario";
            this.Load += new System.EventHandler(this.FrmUsuarioEdicion_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblUsuario;
        private System.Windows.Forms.TextBox txtUsuario;
        private System.Windows.Forms.Label lblRol;
        private System.Windows.Forms.ComboBox cboRol;
        private System.Windows.Forms.Label lblPosta;
        private System.Windows.Forms.ComboBox cboPosta;
        private System.Windows.Forms.Label lblAyuda;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCancelar;
    }
}
