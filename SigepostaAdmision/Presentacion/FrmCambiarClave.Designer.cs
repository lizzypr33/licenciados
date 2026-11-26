namespace SigepostaAdmision.Presentacion
{
    partial class FrmCambiarClave
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
            this.lblActual = new System.Windows.Forms.Label();
            this.txtActual = new System.Windows.Forms.TextBox();
            this.lblNueva = new System.Windows.Forms.Label();
            this.txtNueva = new System.Windows.Forms.TextBox();
            this.lblConfirmacion = new System.Windows.Forms.Label();
            this.txtConfirmacion = new System.Windows.Forms.TextBox();
            this.lblReglas = new System.Windows.Forms.Label();
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
            this.lblTitulo.Text = "Cambiar mi contraseña";
            //
            // lblActual / txtActual
            //
            this.lblActual.AutoSize = true;
            this.lblActual.Location = new System.Drawing.Point(16, 56);
            this.lblActual.Name = "lblActual";
            this.lblActual.TabIndex = 1;
            this.lblActual.Text = "Contraseña actual:";
            this.txtActual.Location = new System.Drawing.Point(170, 52);
            this.txtActual.MaxLength = 128;
            this.txtActual.Name = "txtActual";
            this.txtActual.Size = new System.Drawing.Size(230, 23);
            this.txtActual.TabIndex = 2;
            this.txtActual.UseSystemPasswordChar = true;
            //
            // lblNueva / txtNueva
            //
            this.lblNueva.AutoSize = true;
            this.lblNueva.Location = new System.Drawing.Point(16, 88);
            this.lblNueva.Name = "lblNueva";
            this.lblNueva.TabIndex = 3;
            this.lblNueva.Text = "Nueva contraseña:";
            this.txtNueva.Location = new System.Drawing.Point(170, 84);
            this.txtNueva.MaxLength = 128;
            this.txtNueva.Name = "txtNueva";
            this.txtNueva.Size = new System.Drawing.Size(230, 23);
            this.txtNueva.TabIndex = 4;
            this.txtNueva.UseSystemPasswordChar = true;
            //
            // lblConfirmacion / txtConfirmacion
            //
            this.lblConfirmacion.AutoSize = true;
            this.lblConfirmacion.Location = new System.Drawing.Point(16, 120);
            this.lblConfirmacion.Name = "lblConfirmacion";
            this.lblConfirmacion.TabIndex = 5;
            this.lblConfirmacion.Text = "Confirmar contraseña:";
            this.txtConfirmacion.Location = new System.Drawing.Point(170, 116);
            this.txtConfirmacion.MaxLength = 128;
            this.txtConfirmacion.Name = "txtConfirmacion";
            this.txtConfirmacion.Size = new System.Drawing.Size(230, 23);
            this.txtConfirmacion.TabIndex = 6;
            this.txtConfirmacion.UseSystemPasswordChar = true;
            //
            // lblReglas (texto de UsuarioNegocio.TextoReglasClave)
            //
            this.lblReglas.ForeColor = System.Drawing.Color.FromArgb(107, 114, 128);
            this.lblReglas.Location = new System.Drawing.Point(16, 150);
            this.lblReglas.Name = "lblReglas";
            this.lblReglas.Size = new System.Drawing.Size(384, 48);
            this.lblReglas.TabIndex = 7;
            //
            // btnGuardar
            //
            this.btnGuardar.Location = new System.Drawing.Point(192, 208);
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
            this.btnCancelar.Location = new System.Drawing.Point(300, 208);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(100, 30);
            this.btnCancelar.TabIndex = 9;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            //
            // FrmCambiarClave
            //
            this.AcceptButton = this.btnGuardar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.btnCancelar;
            this.ClientSize = new System.Drawing.Size(416, 252);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.lblActual);
            this.Controls.Add(this.txtActual);
            this.Controls.Add(this.lblNueva);
            this.Controls.Add(this.txtNueva);
            this.Controls.Add(this.lblConfirmacion);
            this.Controls.Add(this.txtConfirmacion);
            this.Controls.Add(this.lblReglas);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.btnCancelar);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmCambiarClave";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cambiar contraseña";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmCambiarClave_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblActual;
        private System.Windows.Forms.TextBox txtActual;
        private System.Windows.Forms.Label lblNueva;
        private System.Windows.Forms.TextBox txtNueva;
        private System.Windows.Forms.Label lblConfirmacion;
        private System.Windows.Forms.TextBox txtConfirmacion;
        private System.Windows.Forms.Label lblReglas;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCancelar;
    }
}
