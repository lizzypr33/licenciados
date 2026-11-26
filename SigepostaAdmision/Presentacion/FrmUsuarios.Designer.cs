namespace SigepostaAdmision.Presentacion
{
    partial class FrmUsuarios
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
            this.pnlSuperior = new System.Windows.Forms.Panel();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblResumen = new System.Windows.Forms.Label();
            this.grdUsuarios = new System.Windows.Forms.DataGridView();
            this.colUsuario = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPosta = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEstado = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlAcciones = new System.Windows.Forms.Panel();
            this.btnNuevo = new System.Windows.Forms.Button();
            this.btnEditar = new System.Windows.Forms.Button();
            this.btnEstado = new System.Windows.Forms.Button();
            this.btnRestablecer = new System.Windows.Forms.Button();
            this.pnlSuperior.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdUsuarios)).BeginInit();
            this.pnlAcciones.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlSuperior
            //
            this.pnlSuperior.Controls.Add(this.lblTitulo);
            this.pnlSuperior.Controls.Add(this.lblResumen);
            this.pnlSuperior.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSuperior.Name = "pnlSuperior";
            this.pnlSuperior.Size = new System.Drawing.Size(814, 48);
            this.pnlSuperior.TabIndex = 0;
            //
            // lblTitulo
            //
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI Semibold", 11F);
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(30, 64, 175);
            this.lblTitulo.Location = new System.Drawing.Point(12, 13);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Usuarios del sistema";
            //
            // lblResumen
            //
            this.lblResumen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblResumen.ForeColor = System.Drawing.Color.FromArgb(107, 114, 128);
            this.lblResumen.Location = new System.Drawing.Point(402, 12);
            this.lblResumen.Name = "lblResumen";
            this.lblResumen.Size = new System.Drawing.Size(400, 26);
            this.lblResumen.TabIndex = 1;
            this.lblResumen.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // grdUsuarios (solo lectura)
            //
            this.grdUsuarios.AllowUserToAddRows = false;
            this.grdUsuarios.AllowUserToDeleteRows = false;
            this.grdUsuarios.AllowUserToResizeRows = false;
            this.grdUsuarios.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grdUsuarios.BackgroundColor = System.Drawing.Color.White;
            this.grdUsuarios.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.grdUsuarios.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.grdUsuarios.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            estiloEncabezado.BackColor = System.Drawing.Color.FromArgb(243, 244, 246);
            estiloEncabezado.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            estiloEncabezado.ForeColor = System.Drawing.Color.FromArgb(55, 65, 81);
            estiloEncabezado.SelectionBackColor = System.Drawing.Color.FromArgb(243, 244, 246);
            estiloEncabezado.SelectionForeColor = System.Drawing.Color.FromArgb(55, 65, 81);
            this.grdUsuarios.ColumnHeadersDefaultCellStyle = estiloEncabezado;
            this.grdUsuarios.ColumnHeadersHeight = 32;
            this.grdUsuarios.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.grdUsuarios.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colUsuario,
            this.colRol,
            this.colPosta,
            this.colEstado});
            this.grdUsuarios.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(219, 234, 254);
            this.grdUsuarios.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(30, 64, 175);
            this.grdUsuarios.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdUsuarios.EnableHeadersVisualStyles = false;
            this.grdUsuarios.GridColor = System.Drawing.Color.FromArgb(229, 231, 235);
            this.grdUsuarios.MultiSelect = false;
            this.grdUsuarios.Name = "grdUsuarios";
            this.grdUsuarios.ReadOnly = true;
            this.grdUsuarios.RowHeadersVisible = false;
            this.grdUsuarios.RowTemplate.Height = 26;
            this.grdUsuarios.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdUsuarios.TabIndex = 1;
            this.grdUsuarios.SelectionChanged += new System.EventHandler(this.grdUsuarios_SelectionChanged);
            //
            // columnas
            //
            this.colUsuario.FillWeight = 120F;
            this.colUsuario.HeaderText = "Usuario";
            this.colUsuario.Name = "colUsuario";
            this.colUsuario.ReadOnly = true;
            this.colRol.FillWeight = 90F;
            this.colRol.HeaderText = "Rol";
            this.colRol.Name = "colRol";
            this.colRol.ReadOnly = true;
            this.colPosta.FillWeight = 150F;
            this.colPosta.HeaderText = "Posta";
            this.colPosta.Name = "colPosta";
            this.colPosta.ReadOnly = true;
            this.colEstado.FillWeight = 60F;
            this.colEstado.HeaderText = "Estado";
            this.colEstado.Name = "colEstado";
            this.colEstado.ReadOnly = true;
            //
            // pnlAcciones
            //
            this.pnlAcciones.BackColor = System.Drawing.Color.FromArgb(249, 250, 251);
            this.pnlAcciones.Controls.Add(this.btnNuevo);
            this.pnlAcciones.Controls.Add(this.btnEditar);
            this.pnlAcciones.Controls.Add(this.btnEstado);
            this.pnlAcciones.Controls.Add(this.btnRestablecer);
            this.pnlAcciones.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlAcciones.Name = "pnlAcciones";
            this.pnlAcciones.Size = new System.Drawing.Size(814, 56);
            this.pnlAcciones.TabIndex = 2;
            //
            // btnNuevo
            //
            this.btnNuevo.Location = new System.Drawing.Point(12, 13);
            this.btnNuevo.Name = "btnNuevo";
            this.btnNuevo.Size = new System.Drawing.Size(130, 30);
            this.btnNuevo.TabIndex = 0;
            this.btnNuevo.Text = "Nuevo usuario";
            this.btnNuevo.UseVisualStyleBackColor = true;
            this.btnNuevo.Click += new System.EventHandler(this.btnNuevo_Click);
            //
            // btnEditar
            //
            this.btnEditar.Enabled = false;
            this.btnEditar.Location = new System.Drawing.Point(150, 13);
            this.btnEditar.Name = "btnEditar";
            this.btnEditar.Size = new System.Drawing.Size(110, 30);
            this.btnEditar.TabIndex = 1;
            this.btnEditar.Text = "Editar";
            this.btnEditar.UseVisualStyleBackColor = true;
            this.btnEditar.Click += new System.EventHandler(this.btnEditar_Click);
            //
            // btnEstado (texto dinámico: "Activar" o "Desactivar", lo decide Negocio)
            //
            this.btnEstado.Enabled = false;
            this.btnEstado.Location = new System.Drawing.Point(268, 13);
            this.btnEstado.Name = "btnEstado";
            this.btnEstado.Size = new System.Drawing.Size(150, 30);
            this.btnEstado.TabIndex = 2;
            this.btnEstado.Text = "Activar / Desactivar";
            this.btnEstado.UseVisualStyleBackColor = true;
            this.btnEstado.Click += new System.EventHandler(this.btnEstado_Click);
            //
            // btnRestablecer
            //
            this.btnRestablecer.Enabled = false;
            this.btnRestablecer.Location = new System.Drawing.Point(426, 13);
            this.btnRestablecer.Name = "btnRestablecer";
            this.btnRestablecer.Size = new System.Drawing.Size(180, 30);
            this.btnRestablecer.TabIndex = 3;
            this.btnRestablecer.Text = "Restablecer contraseña";
            this.btnRestablecer.UseVisualStyleBackColor = true;
            this.btnRestablecer.Click += new System.EventHandler(this.btnRestablecer_Click);
            //
            // FrmUsuarios
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(814, 520);
            this.Controls.Add(this.grdUsuarios);
            this.Controls.Add(this.pnlSuperior);
            this.Controls.Add(this.pnlAcciones);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "FrmUsuarios";
            this.Text = "Usuarios";
            this.Load += new System.EventHandler(this.FrmUsuarios_Load);
            this.pnlSuperior.ResumeLayout(false);
            this.pnlSuperior.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdUsuarios)).EndInit();
            this.pnlAcciones.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlSuperior;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblResumen;
        private System.Windows.Forms.DataGridView grdUsuarios;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUsuario;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRol;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPosta;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEstado;
        private System.Windows.Forms.Panel pnlAcciones;
        private System.Windows.Forms.Button btnNuevo;
        private System.Windows.Forms.Button btnEditar;
        private System.Windows.Forms.Button btnEstado;
        private System.Windows.Forms.Button btnRestablecer;
    }
}
