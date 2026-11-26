namespace SigepostaAdmision.Presentacion
{
    partial class FrmPrincipal
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
            this.pnlContenido = new System.Windows.Forms.Panel();
            this.pnlMenu = new System.Windows.Forms.Panel();
            this.flpMenu = new System.Windows.Forms.FlowLayoutPanel();
            this.btnPacientes = new System.Windows.Forms.Button();
            this.btnHistorias = new System.Windows.Forms.Button();
            this.btnCitas = new System.Windows.Forms.Button();
            this.btnCola = new System.Windows.Forms.Button();
            this.btnReportes = new System.Windows.Forms.Button();
            this.btnRespaldo = new System.Windows.Forms.Button();
            this.btnUsuarios = new System.Windows.Forms.Button();
            this.btnCerrarSesion = new System.Windows.Forms.Button();
            this.pnlSuperior = new System.Windows.Forms.Panel();
            this.lblUsuario = new System.Windows.Forms.Label();
            this.lnkCambiarClave = new System.Windows.Forms.LinkLabel();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.pnlLineaSuperior = new System.Windows.Forms.Panel();
            this.ssEstado = new System.Windows.Forms.StatusStrip();
            this.lblEstado = new System.Windows.Forms.ToolStripStatusLabel();
            this.pnlMenu.SuspendLayout();
            this.flpMenu.SuspendLayout();
            this.pnlSuperior.SuspendLayout();
            this.ssEstado.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlContenido (panel central donde se incrusta el módulo)
            //
            this.pnlContenido.BackColor = System.Drawing.Color.White;
            this.pnlContenido.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContenido.Name = "pnlContenido";
            this.pnlContenido.TabIndex = 0;
            //
            // pnlMenu (menú lateral de 170 px)
            //
            this.pnlMenu.BackColor = System.Drawing.Color.FromArgb(243, 244, 246);
            this.pnlMenu.Controls.Add(this.flpMenu);
            this.pnlMenu.Controls.Add(this.btnCerrarSesion);
            this.pnlMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlMenu.Name = "pnlMenu";
            this.pnlMenu.Size = new System.Drawing.Size(170, 500);
            this.pnlMenu.TabIndex = 1;
            //
            // flpMenu (botones de módulo, uno debajo de otro)
            //
            this.flpMenu.Controls.Add(this.btnPacientes);
            this.flpMenu.Controls.Add(this.btnHistorias);
            this.flpMenu.Controls.Add(this.btnCitas);
            this.flpMenu.Controls.Add(this.btnCola);
            this.flpMenu.Controls.Add(this.btnReportes);
            this.flpMenu.Controls.Add(this.btnRespaldo);
            this.flpMenu.Controls.Add(this.btnUsuarios);
            this.flpMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpMenu.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpMenu.Name = "flpMenu";
            this.flpMenu.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.flpMenu.TabIndex = 0;
            this.flpMenu.WrapContents = false;
            //
            // btnPacientes
            //
            this.btnPacientes.FlatAppearance.BorderSize = 0;
            this.btnPacientes.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(229, 231, 235);
            this.btnPacientes.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPacientes.ForeColor = System.Drawing.Color.FromArgb(55, 65, 81);
            this.btnPacientes.Margin = new System.Windows.Forms.Padding(0);
            this.btnPacientes.Name = "btnPacientes";
            this.btnPacientes.Padding = new System.Windows.Forms.Padding(14, 0, 0, 0);
            this.btnPacientes.Size = new System.Drawing.Size(170, 40);
            this.btnPacientes.TabIndex = 0;
            this.btnPacientes.Text = "Pacientes";
            this.btnPacientes.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPacientes.UseVisualStyleBackColor = false;
            this.btnPacientes.Click += new System.EventHandler(this.btnModulo_Click);
            //
            // btnHistorias
            //
            this.btnHistorias.FlatAppearance.BorderSize = 0;
            this.btnHistorias.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(229, 231, 235);
            this.btnHistorias.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHistorias.ForeColor = System.Drawing.Color.FromArgb(55, 65, 81);
            this.btnHistorias.Margin = new System.Windows.Forms.Padding(0);
            this.btnHistorias.Name = "btnHistorias";
            this.btnHistorias.Padding = new System.Windows.Forms.Padding(14, 0, 0, 0);
            this.btnHistorias.Size = new System.Drawing.Size(170, 40);
            this.btnHistorias.TabIndex = 1;
            this.btnHistorias.Text = "Historias";
            this.btnHistorias.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnHistorias.UseVisualStyleBackColor = false;
            this.btnHistorias.Click += new System.EventHandler(this.btnModulo_Click);
            //
            // btnCitas
            //
            this.btnCitas.FlatAppearance.BorderSize = 0;
            this.btnCitas.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(229, 231, 235);
            this.btnCitas.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCitas.ForeColor = System.Drawing.Color.FromArgb(55, 65, 81);
            this.btnCitas.Margin = new System.Windows.Forms.Padding(0);
            this.btnCitas.Name = "btnCitas";
            this.btnCitas.Padding = new System.Windows.Forms.Padding(14, 0, 0, 0);
            this.btnCitas.Size = new System.Drawing.Size(170, 40);
            this.btnCitas.TabIndex = 2;
            this.btnCitas.Text = "Citas";
            this.btnCitas.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCitas.UseVisualStyleBackColor = false;
            this.btnCitas.Click += new System.EventHandler(this.btnModulo_Click);
            //
            // btnCola
            //
            this.btnCola.FlatAppearance.BorderSize = 0;
            this.btnCola.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(229, 231, 235);
            this.btnCola.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCola.ForeColor = System.Drawing.Color.FromArgb(55, 65, 81);
            this.btnCola.Margin = new System.Windows.Forms.Padding(0);
            this.btnCola.Name = "btnCola";
            this.btnCola.Padding = new System.Windows.Forms.Padding(14, 0, 0, 0);
            this.btnCola.Size = new System.Drawing.Size(170, 40);
            this.btnCola.TabIndex = 3;
            this.btnCola.Text = "Cola de atención";
            this.btnCola.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCola.UseVisualStyleBackColor = false;
            this.btnCola.Click += new System.EventHandler(this.btnModulo_Click);
            //
            // btnReportes
            //
            this.btnReportes.FlatAppearance.BorderSize = 0;
            this.btnReportes.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(229, 231, 235);
            this.btnReportes.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReportes.ForeColor = System.Drawing.Color.FromArgb(55, 65, 81);
            this.btnReportes.Margin = new System.Windows.Forms.Padding(0);
            this.btnReportes.Name = "btnReportes";
            this.btnReportes.Padding = new System.Windows.Forms.Padding(14, 0, 0, 0);
            this.btnReportes.Size = new System.Drawing.Size(170, 40);
            this.btnReportes.TabIndex = 4;
            this.btnReportes.Text = "Reportes";
            this.btnReportes.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnReportes.UseVisualStyleBackColor = false;
            this.btnReportes.Click += new System.EventHandler(this.btnModulo_Click);
            //
            // btnRespaldo
            //
            this.btnRespaldo.FlatAppearance.BorderSize = 0;
            this.btnRespaldo.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(229, 231, 235);
            this.btnRespaldo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRespaldo.ForeColor = System.Drawing.Color.FromArgb(55, 65, 81);
            this.btnRespaldo.Margin = new System.Windows.Forms.Padding(0);
            this.btnRespaldo.Name = "btnRespaldo";
            this.btnRespaldo.Padding = new System.Windows.Forms.Padding(14, 0, 0, 0);
            this.btnRespaldo.Size = new System.Drawing.Size(170, 40);
            this.btnRespaldo.TabIndex = 5;
            this.btnRespaldo.Text = "Respaldo";
            this.btnRespaldo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRespaldo.UseVisualStyleBackColor = false;
            this.btnRespaldo.Click += new System.EventHandler(this.btnModulo_Click);
            //
            // btnUsuarios
            //
            this.btnUsuarios.FlatAppearance.BorderSize = 0;
            this.btnUsuarios.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(229, 231, 235);
            this.btnUsuarios.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUsuarios.ForeColor = System.Drawing.Color.FromArgb(55, 65, 81);
            this.btnUsuarios.Margin = new System.Windows.Forms.Padding(0);
            this.btnUsuarios.Name = "btnUsuarios";
            this.btnUsuarios.Padding = new System.Windows.Forms.Padding(14, 0, 0, 0);
            this.btnUsuarios.Size = new System.Drawing.Size(170, 40);
            this.btnUsuarios.TabIndex = 6;
            this.btnUsuarios.Text = "Usuarios";
            this.btnUsuarios.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUsuarios.UseVisualStyleBackColor = false;
            this.btnUsuarios.Click += new System.EventHandler(this.btnModulo_Click);
            //
            // btnCerrarSesion (anclado abajo del menú)
            //
            this.btnCerrarSesion.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnCerrarSesion.FlatAppearance.BorderSize = 0;
            this.btnCerrarSesion.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(254, 226, 226);
            this.btnCerrarSesion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCerrarSesion.ForeColor = System.Drawing.Color.FromArgb(153, 27, 27);
            this.btnCerrarSesion.Name = "btnCerrarSesion";
            this.btnCerrarSesion.Padding = new System.Windows.Forms.Padding(14, 0, 0, 0);
            this.btnCerrarSesion.Size = new System.Drawing.Size(170, 44);
            this.btnCerrarSesion.TabIndex = 1;
            this.btnCerrarSesion.Text = "Cerrar sesión";
            this.btnCerrarSesion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCerrarSesion.UseVisualStyleBackColor = false;
            this.btnCerrarSesion.Click += new System.EventHandler(this.btnCerrarSesion_Click);
            //
            // pnlSuperior (barra superior)
            //
            this.pnlSuperior.BackColor = System.Drawing.Color.White;
            this.pnlSuperior.Controls.Add(this.lblUsuario);
            this.pnlSuperior.Controls.Add(this.lblTitulo);
            this.pnlSuperior.Controls.Add(this.pnlLineaSuperior);
            // Se agrega al final para que quede en el extremo derecho, junto al usuario.
            this.pnlSuperior.Controls.Add(this.lnkCambiarClave);
            this.pnlSuperior.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSuperior.Name = "pnlSuperior";
            this.pnlSuperior.Padding = new System.Windows.Forms.Padding(16, 0, 16, 0);
            this.pnlSuperior.Size = new System.Drawing.Size(984, 50);
            this.pnlSuperior.TabIndex = 2;
            //
            // lblUsuario (usuario y rol, a la derecha)
            //
            this.lblUsuario.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblUsuario.ForeColor = System.Drawing.Color.FromArgb(75, 85, 99);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new System.Drawing.Size(350, 49);
            this.lblUsuario.TabIndex = 1;
            this.lblUsuario.Text = "usuario  |  rol";
            this.lblUsuario.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lnkCambiarClave ("Cambiar contraseña", visible para todos los roles)
            //
            this.lnkCambiarClave.ActiveLinkColor = System.Drawing.Color.FromArgb(30, 64, 175);
            this.lnkCambiarClave.Dock = System.Windows.Forms.DockStyle.Right;
            this.lnkCambiarClave.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.lnkCambiarClave.LinkColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.lnkCambiarClave.Name = "lnkCambiarClave";
            this.lnkCambiarClave.Size = new System.Drawing.Size(140, 49);
            this.lnkCambiarClave.TabIndex = 3;
            this.lnkCambiarClave.TabStop = true;
            this.lnkCambiarClave.Text = "Cambiar contraseña";
            this.lnkCambiarClave.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lnkCambiarClave.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkCambiarClave_LinkClicked);
            //
            // lblTitulo (nombre del sistema, a la izquierda)
            //
            this.lblTitulo.AutoSize = false;
            this.lblTitulo.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(30, 64, 175);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(300, 49);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "SIGEPOSTA · Admisión";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlLineaSuperior (línea divisoria gris de 1 px)
            //
            this.pnlLineaSuperior.BackColor = System.Drawing.Color.FromArgb(229, 231, 235);
            this.pnlLineaSuperior.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlLineaSuperior.Name = "pnlLineaSuperior";
            this.pnlLineaSuperior.Size = new System.Drawing.Size(952, 1);
            this.pnlLineaSuperior.TabIndex = 2;
            //
            // ssEstado (barra de estado inferior)
            //
            this.ssEstado.BackColor = System.Drawing.Color.FromArgb(243, 244, 246);
            this.ssEstado.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblEstado});
            this.ssEstado.Name = "ssEstado";
            this.ssEstado.SizingGrip = true;
            this.ssEstado.TabIndex = 3;
            //
            // lblEstado
            //
            this.lblEstado.ForeColor = System.Drawing.Color.FromArgb(75, 85, 99);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Text = "Conectado a SigepostaAdmision";
            //
            // FrmPrincipal
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            // Orden de Controls.Add: el panel Fill va primero para que se acomode
            // en el espacio que dejan la barra superior, el menú y la barra de estado.
            this.Controls.Add(this.pnlContenido);
            this.Controls.Add(this.pnlMenu);
            this.Controls.Add(this.pnlSuperior);
            this.Controls.Add(this.ssEstado);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MinimumSize = new System.Drawing.Size(900, 560);
            this.Name = "FrmPrincipal";
            this.Size = new System.Drawing.Size(1000, 640);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SIGEPOSTA · Admisión";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmPrincipal_FormClosed);
            this.Load += new System.EventHandler(this.FrmPrincipal_Load);
            this.pnlMenu.ResumeLayout(false);
            this.flpMenu.ResumeLayout(false);
            this.pnlSuperior.ResumeLayout(false);
            this.ssEstado.ResumeLayout(false);
            this.ssEstado.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Panel pnlContenido;
        private System.Windows.Forms.Panel pnlMenu;
        private System.Windows.Forms.FlowLayoutPanel flpMenu;
        private System.Windows.Forms.Button btnPacientes;
        private System.Windows.Forms.Button btnHistorias;
        private System.Windows.Forms.Button btnCitas;
        private System.Windows.Forms.Button btnCola;
        private System.Windows.Forms.Button btnReportes;
        private System.Windows.Forms.Button btnRespaldo;
        private System.Windows.Forms.Button btnUsuarios;
        private System.Windows.Forms.Button btnCerrarSesion;
        private System.Windows.Forms.Panel pnlSuperior;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblUsuario;
        private System.Windows.Forms.LinkLabel lnkCambiarClave;
        private System.Windows.Forms.Panel pnlLineaSuperior;
        private System.Windows.Forms.StatusStrip ssEstado;
        private System.Windows.Forms.ToolStripStatusLabel lblEstado;
    }
}
