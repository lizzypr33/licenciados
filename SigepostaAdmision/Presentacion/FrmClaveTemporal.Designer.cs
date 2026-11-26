namespace SigepostaAdmision.Presentacion
{
    partial class FrmClaveTemporal
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
            this.lblUsuarioValor = new System.Windows.Forms.Label();
            this.txtClave = new System.Windows.Forms.TextBox();
            this.btnCopiar = new System.Windows.Forms.Button();
            this.lblCopiado = new System.Windows.Forms.Label();
            this.lblAviso = new System.Windows.Forms.Label();
            this.btnCerrar = new System.Windows.Forms.Button();
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
            this.lblTitulo.Text = "Contraseña temporal";
            //
            // lblUsuario / lblUsuarioValor
            //
            this.lblUsuario.AutoSize = true;
            this.lblUsuario.ForeColor = System.Drawing.Color.FromArgb(107, 114, 128);
            this.lblUsuario.Location = new System.Drawing.Point(16, 50);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.TabIndex = 1;
            this.lblUsuario.Text = "Usuario:";
            this.lblUsuarioValor.AutoEllipsis = true;
            this.lblUsuarioValor.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.lblUsuarioValor.Location = new System.Drawing.Point(80, 50);
            this.lblUsuarioValor.Name = "lblUsuarioValor";
            this.lblUsuarioValor.Size = new System.Drawing.Size(320, 18);
            this.lblUsuarioValor.TabIndex = 2;
            //
            // txtClave (solo lectura)
            //
            this.txtClave.BackColor = System.Drawing.Color.FromArgb(249, 250, 251);
            this.txtClave.Font = new System.Drawing.Font("Consolas", 16F, System.Drawing.FontStyle.Bold);
            this.txtClave.Location = new System.Drawing.Point(16, 78);
            this.txtClave.Name = "txtClave";
            this.txtClave.ReadOnly = true;
            this.txtClave.Size = new System.Drawing.Size(270, 33);
            this.txtClave.TabIndex = 3;
            this.txtClave.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            //
            // btnCopiar
            //
            this.btnCopiar.Location = new System.Drawing.Point(296, 79);
            this.btnCopiar.Name = "btnCopiar";
            this.btnCopiar.Size = new System.Drawing.Size(104, 31);
            this.btnCopiar.TabIndex = 4;
            this.btnCopiar.Text = "Copiar";
            this.btnCopiar.UseVisualStyleBackColor = true;
            this.btnCopiar.Click += new System.EventHandler(this.btnCopiar_Click);
            //
            // lblCopiado
            //
            this.lblCopiado.ForeColor = System.Drawing.Color.FromArgb(21, 128, 61);
            this.lblCopiado.Location = new System.Drawing.Point(16, 116);
            this.lblCopiado.Name = "lblCopiado";
            this.lblCopiado.Size = new System.Drawing.Size(384, 18);
            this.lblCopiado.TabIndex = 5;
            //
            // lblAviso
            //
            this.lblAviso.BackColor = System.Drawing.Color.FromArgb(255, 251, 235);
            this.lblAviso.ForeColor = System.Drawing.Color.FromArgb(146, 64, 14);
            this.lblAviso.Location = new System.Drawing.Point(16, 140);
            this.lblAviso.Name = "lblAviso";
            this.lblAviso.Padding = new System.Windows.Forms.Padding(6);
            this.lblAviso.Size = new System.Drawing.Size(384, 48);
            this.lblAviso.TabIndex = 6;
            this.lblAviso.Text = "Se muestra una sola vez. Entrégala al usuario y pídele que la cambie al ingresar.";
            //
            // btnCerrar
            //
            this.btnCerrar.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnCerrar.Location = new System.Drawing.Point(300, 200);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(100, 30);
            this.btnCerrar.TabIndex = 7;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = true;
            //
            // FrmClaveTemporal
            //
            this.AcceptButton = this.btnCerrar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.btnCerrar;
            this.ClientSize = new System.Drawing.Size(416, 244);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.lblUsuario);
            this.Controls.Add(this.lblUsuarioValor);
            this.Controls.Add(this.txtClave);
            this.Controls.Add(this.btnCopiar);
            this.Controls.Add(this.lblCopiado);
            this.Controls.Add(this.lblAviso);
            this.Controls.Add(this.btnCerrar);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmClaveTemporal";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Contraseña temporal";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblUsuario;
        private System.Windows.Forms.Label lblUsuarioValor;
        private System.Windows.Forms.TextBox txtClave;
        private System.Windows.Forms.Button btnCopiar;
        private System.Windows.Forms.Label lblCopiado;
        private System.Windows.Forms.Label lblAviso;
        private System.Windows.Forms.Button btnCerrar;
    }
}
