namespace SigepostaAdmision.Presentacion
{
    partial class FrmPacientes
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
            System.Windows.Forms.DataGridViewCellStyle estiloFecha = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle estiloEdad = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle estiloSexo = new System.Windows.Forms.DataGridViewCellStyle();
            this.grdPacientes = new System.Windows.Forms.DataGridView();
            this.colDni = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colApellidos = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNombres = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFechaNacimiento = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEdad = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSexo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTelefono = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlBusqueda = new System.Windows.Forms.Panel();
            this.txtBuscar = new System.Windows.Forms.TextBox();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.btnNuevo = new System.Windows.Forms.Button();
            this.lblResultados = new System.Windows.Forms.Label();
            this.txtFiltro = new System.Windows.Forms.TextBox();
            this.pnlDatos = new System.Windows.Forms.Panel();
            this.lblTituloDatos = new System.Windows.Forms.Label();
            this.lblHistoriaTitulo = new System.Windows.Forms.Label();
            this.lblNumeroHistoria = new System.Windows.Forms.Label();
            this.lblDni = new System.Windows.Forms.Label();
            this.txtDni = new System.Windows.Forms.TextBox();
            this.lblApellidoPaterno = new System.Windows.Forms.Label();
            this.txtApellidoPaterno = new System.Windows.Forms.TextBox();
            this.lblApellidoMaterno = new System.Windows.Forms.Label();
            this.txtApellidoMaterno = new System.Windows.Forms.TextBox();
            this.lblNombres = new System.Windows.Forms.Label();
            this.txtNombres = new System.Windows.Forms.TextBox();
            this.lblFechaNacimiento = new System.Windows.Forms.Label();
            this.dtpFechaNacimiento = new System.Windows.Forms.DateTimePicker();
            this.lblSexo = new System.Windows.Forms.Label();
            this.cboSexo = new System.Windows.Forms.ComboBox();
            this.lblTelefono = new System.Windows.Forms.Label();
            this.txtTelefono = new System.Windows.Forms.TextBox();
            this.lblDireccion = new System.Windows.Forms.Label();
            this.txtDireccion = new System.Windows.Forms.TextBox();
            this.btnEditar = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.grdPacientes)).BeginInit();
            this.pnlBusqueda.SuspendLayout();
            this.pnlDatos.SuspendLayout();
            this.SuspendLayout();
            //
            // grdPacientes
            //
            this.grdPacientes.AllowUserToAddRows = false;
            this.grdPacientes.AllowUserToDeleteRows = false;
            this.grdPacientes.AllowUserToResizeRows = false;
            this.grdPacientes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grdPacientes.BackgroundColor = System.Drawing.Color.White;
            this.grdPacientes.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.grdPacientes.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.grdPacientes.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            estiloEncabezado.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            estiloEncabezado.BackColor = System.Drawing.Color.FromArgb(243, 244, 246);
            estiloEncabezado.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            estiloEncabezado.ForeColor = System.Drawing.Color.FromArgb(55, 65, 81);
            estiloEncabezado.SelectionBackColor = System.Drawing.Color.FromArgb(243, 244, 246);
            estiloEncabezado.SelectionForeColor = System.Drawing.Color.FromArgb(55, 65, 81);
            this.grdPacientes.ColumnHeadersDefaultCellStyle = estiloEncabezado;
            this.grdPacientes.ColumnHeadersHeight = 32;
            this.grdPacientes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.grdPacientes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colDni,
            this.colApellidos,
            this.colNombres,
            this.colFechaNacimiento,
            this.colEdad,
            this.colSexo,
            this.colTelefono});
            this.grdPacientes.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(219, 234, 254);
            this.grdPacientes.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(30, 64, 175);
            this.grdPacientes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdPacientes.EnableHeadersVisualStyles = false;
            this.grdPacientes.GridColor = System.Drawing.Color.FromArgb(229, 231, 235);
            this.grdPacientes.MultiSelect = false;
            this.grdPacientes.Name = "grdPacientes";
            this.grdPacientes.ReadOnly = true;
            this.grdPacientes.RowHeadersVisible = false;
            this.grdPacientes.RowTemplate.Height = 26;
            this.grdPacientes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdPacientes.TabIndex = 1;
            this.grdPacientes.SelectionChanged += new System.EventHandler(this.grdPacientes_SelectionChanged);
            //
            // colDni
            //
            this.colDni.FillWeight = 70F;
            this.colDni.HeaderText = "DNI";
            this.colDni.Name = "colDni";
            this.colDni.ReadOnly = true;
            //
            // colApellidos
            //
            this.colApellidos.FillWeight = 140F;
            this.colApellidos.HeaderText = "Apellidos";
            this.colApellidos.Name = "colApellidos";
            this.colApellidos.ReadOnly = true;
            //
            // colNombres
            //
            this.colNombres.FillWeight = 120F;
            this.colNombres.HeaderText = "Nombres";
            this.colNombres.Name = "colNombres";
            this.colNombres.ReadOnly = true;
            //
            // colFechaNacimiento
            //
            estiloFecha.Format = "dd/MM/yyyy";
            this.colFechaNacimiento.DefaultCellStyle = estiloFecha;
            this.colFechaNacimiento.FillWeight = 90F;
            this.colFechaNacimiento.HeaderText = "Fecha de nacimiento";
            this.colFechaNacimiento.Name = "colFechaNacimiento";
            this.colFechaNacimiento.ReadOnly = true;
            //
            // colEdad
            //
            estiloEdad.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.colEdad.DefaultCellStyle = estiloEdad;
            this.colEdad.FillWeight = 45F;
            this.colEdad.HeaderText = "Edad";
            this.colEdad.Name = "colEdad";
            this.colEdad.ReadOnly = true;
            //
            // colSexo
            //
            estiloSexo.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colSexo.DefaultCellStyle = estiloSexo;
            this.colSexo.FillWeight = 45F;
            this.colSexo.HeaderText = "Sexo";
            this.colSexo.Name = "colSexo";
            this.colSexo.ReadOnly = true;
            //
            // colTelefono
            //
            this.colTelefono.FillWeight = 80F;
            this.colTelefono.HeaderText = "Teléfono";
            this.colTelefono.Name = "colTelefono";
            this.colTelefono.ReadOnly = true;
            //
            // pnlBusqueda (barra superior de búsqueda)
            //
            this.pnlBusqueda.Controls.Add(this.txtBuscar);
            this.pnlBusqueda.Controls.Add(this.btnBuscar);
            this.pnlBusqueda.Controls.Add(this.btnNuevo);
            this.pnlBusqueda.Controls.Add(this.lblResultados);
            this.pnlBusqueda.Controls.Add(this.txtFiltro);
            this.pnlBusqueda.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlBusqueda.Name = "pnlBusqueda";
            this.pnlBusqueda.Size = new System.Drawing.Size(814, 84);
            this.pnlBusqueda.TabIndex = 0;
            //
            // txtBuscar
            //
            this.txtBuscar.Location = new System.Drawing.Point(12, 14);
            this.txtBuscar.MaxLength = 100;
            this.txtBuscar.Name = "txtBuscar";
            this.txtBuscar.Size = new System.Drawing.Size(300, 23);
            this.txtBuscar.TabIndex = 0;
            this.txtBuscar.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBuscar_KeyDown);
            //
            // btnBuscar
            //
            this.btnBuscar.Location = new System.Drawing.Point(320, 12);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(90, 28);
            this.btnBuscar.TabIndex = 1;
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.UseVisualStyleBackColor = true;
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            //
            // btnNuevo
            //
            this.btnNuevo.Location = new System.Drawing.Point(418, 12);
            this.btnNuevo.Name = "btnNuevo";
            this.btnNuevo.Size = new System.Drawing.Size(120, 28);
            this.btnNuevo.TabIndex = 2;
            this.btnNuevo.Text = "Nuevo paciente";
            this.btnNuevo.UseVisualStyleBackColor = true;
            this.btnNuevo.Click += new System.EventHandler(this.btnNuevo_Click);
            //
            // lblResultados
            //
            this.lblResultados.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblResultados.ForeColor = System.Drawing.Color.FromArgb(107, 114, 128);
            this.lblResultados.Location = new System.Drawing.Point(546, 46);
            this.lblResultados.Name = "lblResultados";
            this.lblResultados.Size = new System.Drawing.Size(256, 28);
            this.lblResultados.TabIndex = 3;
            this.lblResultados.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // txtFiltro (filtro en vivo sobre la lista ya cargada, sin consultar la base)
            //
            this.txtFiltro.Location = new System.Drawing.Point(12, 49);
            this.txtFiltro.MaxLength = 100;
            this.txtFiltro.Name = "txtFiltro";
            this.txtFiltro.Size = new System.Drawing.Size(300, 23);
            this.txtFiltro.TabIndex = 4;
            this.txtFiltro.TextChanged += new System.EventHandler(this.txtFiltro_TextChanged);
            //
            // pnlDatos (panel inferior con los datos del paciente)
            //
            this.pnlDatos.BackColor = System.Drawing.Color.FromArgb(249, 250, 251);
            this.pnlDatos.Controls.Add(this.lblTituloDatos);
            this.pnlDatos.Controls.Add(this.lblHistoriaTitulo);
            this.pnlDatos.Controls.Add(this.lblNumeroHistoria);
            this.pnlDatos.Controls.Add(this.lblDni);
            this.pnlDatos.Controls.Add(this.txtDni);
            this.pnlDatos.Controls.Add(this.lblApellidoPaterno);
            this.pnlDatos.Controls.Add(this.txtApellidoPaterno);
            this.pnlDatos.Controls.Add(this.lblApellidoMaterno);
            this.pnlDatos.Controls.Add(this.txtApellidoMaterno);
            this.pnlDatos.Controls.Add(this.lblNombres);
            this.pnlDatos.Controls.Add(this.txtNombres);
            this.pnlDatos.Controls.Add(this.lblFechaNacimiento);
            this.pnlDatos.Controls.Add(this.dtpFechaNacimiento);
            this.pnlDatos.Controls.Add(this.lblSexo);
            this.pnlDatos.Controls.Add(this.cboSexo);
            this.pnlDatos.Controls.Add(this.lblTelefono);
            this.pnlDatos.Controls.Add(this.txtTelefono);
            this.pnlDatos.Controls.Add(this.lblDireccion);
            this.pnlDatos.Controls.Add(this.txtDireccion);
            this.pnlDatos.Controls.Add(this.btnEditar);
            this.pnlDatos.Controls.Add(this.btnGuardar);
            this.pnlDatos.Controls.Add(this.btnCancelar);
            this.pnlDatos.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlDatos.Name = "pnlDatos";
            this.pnlDatos.Size = new System.Drawing.Size(814, 192);
            this.pnlDatos.TabIndex = 2;
            //
            // lblTituloDatos
            //
            this.lblTituloDatos.AutoSize = true;
            this.lblTituloDatos.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.lblTituloDatos.ForeColor = System.Drawing.Color.FromArgb(30, 64, 175);
            this.lblTituloDatos.Location = new System.Drawing.Point(10, 8);
            this.lblTituloDatos.Name = "lblTituloDatos";
            this.lblTituloDatos.TabIndex = 0;
            this.lblTituloDatos.Text = "Datos del paciente";
            //
            // lblHistoriaTitulo
            //
            this.lblHistoriaTitulo.AutoSize = true;
            this.lblHistoriaTitulo.ForeColor = System.Drawing.Color.FromArgb(107, 114, 128);
            this.lblHistoriaTitulo.Location = new System.Drawing.Point(518, 10);
            this.lblHistoriaTitulo.Name = "lblHistoriaTitulo";
            this.lblHistoriaTitulo.TabIndex = 20;
            this.lblHistoriaTitulo.Text = "Historia clínica:";
            // lblNumeroHistoria (solo lectura: lo asigna el sistema)
            this.lblNumeroHistoria.AutoSize = true;
            this.lblNumeroHistoria.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.lblNumeroHistoria.ForeColor = System.Drawing.Color.FromArgb(30, 64, 175);
            this.lblNumeroHistoria.Location = new System.Drawing.Point(615, 10);
            this.lblNumeroHistoria.Name = "lblNumeroHistoria";
            this.lblNumeroHistoria.TabIndex = 21;
            //
            // lblDni
            //
            this.lblDni.AutoSize = true;
            this.lblDni.Location = new System.Drawing.Point(12, 36);
            this.lblDni.Name = "lblDni";
            this.lblDni.TabIndex = 1;
            this.lblDni.Text = "DNI";
            //
            // txtDni
            //
            this.txtDni.Location = new System.Drawing.Point(12, 54);
            this.txtDni.MaxLength = 8;
            this.txtDni.Name = "txtDni";
            this.txtDni.Size = new System.Drawing.Size(110, 23);
            this.txtDni.TabIndex = 2;
            //
            // lblApellidoPaterno
            //
            this.lblApellidoPaterno.AutoSize = true;
            this.lblApellidoPaterno.Location = new System.Drawing.Point(134, 36);
            this.lblApellidoPaterno.Name = "lblApellidoPaterno";
            this.lblApellidoPaterno.TabIndex = 3;
            this.lblApellidoPaterno.Text = "Apellido paterno";
            //
            // txtApellidoPaterno
            //
            this.txtApellidoPaterno.Location = new System.Drawing.Point(134, 54);
            this.txtApellidoPaterno.MaxLength = 40;
            this.txtApellidoPaterno.Name = "txtApellidoPaterno";
            this.txtApellidoPaterno.Size = new System.Drawing.Size(180, 23);
            this.txtApellidoPaterno.TabIndex = 4;
            //
            // lblApellidoMaterno
            //
            this.lblApellidoMaterno.AutoSize = true;
            this.lblApellidoMaterno.Location = new System.Drawing.Point(326, 36);
            this.lblApellidoMaterno.Name = "lblApellidoMaterno";
            this.lblApellidoMaterno.TabIndex = 5;
            this.lblApellidoMaterno.Text = "Apellido materno";
            //
            // txtApellidoMaterno
            //
            this.txtApellidoMaterno.Location = new System.Drawing.Point(326, 54);
            this.txtApellidoMaterno.MaxLength = 40;
            this.txtApellidoMaterno.Name = "txtApellidoMaterno";
            this.txtApellidoMaterno.Size = new System.Drawing.Size(180, 23);
            this.txtApellidoMaterno.TabIndex = 6;
            //
            // lblNombres
            //
            this.lblNombres.AutoSize = true;
            this.lblNombres.Location = new System.Drawing.Point(518, 36);
            this.lblNombres.Name = "lblNombres";
            this.lblNombres.TabIndex = 7;
            this.lblNombres.Text = "Nombres";
            //
            // txtNombres
            //
            this.txtNombres.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNombres.Location = new System.Drawing.Point(518, 54);
            this.txtNombres.MaxLength = 60;
            this.txtNombres.Name = "txtNombres";
            this.txtNombres.Size = new System.Drawing.Size(284, 23);
            this.txtNombres.TabIndex = 8;
            //
            // lblFechaNacimiento
            //
            this.lblFechaNacimiento.AutoSize = true;
            this.lblFechaNacimiento.Location = new System.Drawing.Point(12, 86);
            this.lblFechaNacimiento.Name = "lblFechaNacimiento";
            this.lblFechaNacimiento.TabIndex = 9;
            this.lblFechaNacimiento.Text = "Fecha de nacimiento";
            //
            // dtpFechaNacimiento
            //
            this.dtpFechaNacimiento.CustomFormat = "dd/MM/yyyy";
            this.dtpFechaNacimiento.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFechaNacimiento.Location = new System.Drawing.Point(12, 104);
            this.dtpFechaNacimiento.Name = "dtpFechaNacimiento";
            this.dtpFechaNacimiento.Size = new System.Drawing.Size(110, 23);
            this.dtpFechaNacimiento.TabIndex = 10;
            //
            // lblSexo
            //
            this.lblSexo.AutoSize = true;
            this.lblSexo.Location = new System.Drawing.Point(134, 86);
            this.lblSexo.Name = "lblSexo";
            this.lblSexo.TabIndex = 11;
            this.lblSexo.Text = "Sexo";
            //
            // cboSexo
            //
            this.cboSexo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSexo.Location = new System.Drawing.Point(134, 104);
            this.cboSexo.Name = "cboSexo";
            this.cboSexo.Size = new System.Drawing.Size(180, 23);
            this.cboSexo.TabIndex = 12;
            //
            // lblTelefono
            //
            this.lblTelefono.AutoSize = true;
            this.lblTelefono.Location = new System.Drawing.Point(326, 86);
            this.lblTelefono.Name = "lblTelefono";
            this.lblTelefono.TabIndex = 13;
            this.lblTelefono.Text = "Teléfono (opcional)";
            //
            // txtTelefono
            //
            this.txtTelefono.Location = new System.Drawing.Point(326, 104);
            this.txtTelefono.MaxLength = 15;
            this.txtTelefono.Name = "txtTelefono";
            this.txtTelefono.Size = new System.Drawing.Size(180, 23);
            this.txtTelefono.TabIndex = 14;
            //
            // lblDireccion
            //
            this.lblDireccion.AutoSize = true;
            this.lblDireccion.Location = new System.Drawing.Point(518, 86);
            this.lblDireccion.Name = "lblDireccion";
            this.lblDireccion.TabIndex = 15;
            this.lblDireccion.Text = "Dirección (opcional)";
            //
            // txtDireccion
            //
            this.txtDireccion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDireccion.Location = new System.Drawing.Point(518, 104);
            this.txtDireccion.MaxLength = 120;
            this.txtDireccion.Name = "txtDireccion";
            this.txtDireccion.Size = new System.Drawing.Size(284, 23);
            this.txtDireccion.TabIndex = 16;
            //
            // btnEditar
            //
            this.btnEditar.Location = new System.Drawing.Point(12, 146);
            this.btnEditar.Name = "btnEditar";
            this.btnEditar.Size = new System.Drawing.Size(110, 30);
            this.btnEditar.TabIndex = 17;
            this.btnEditar.Text = "Editar";
            this.btnEditar.UseVisualStyleBackColor = true;
            this.btnEditar.Click += new System.EventHandler(this.btnEditar_Click);
            //
            // btnGuardar
            //
            this.btnGuardar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGuardar.Location = new System.Drawing.Point(574, 146);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(110, 30);
            this.btnGuardar.TabIndex = 18;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            //
            // btnCancelar
            //
            this.btnCancelar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelar.Location = new System.Drawing.Point(692, 146);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(110, 30);
            this.btnCancelar.TabIndex = 19;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            //
            // FrmPacientes
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(814, 520);
            // La grilla (Dock = Fill) se agrega primero para que ocupe el espacio
            // que dejan la barra de búsqueda (arriba) y el panel de datos (abajo).
            this.Controls.Add(this.grdPacientes);
            this.Controls.Add(this.pnlBusqueda);
            this.Controls.Add(this.pnlDatos);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "FrmPacientes";
            this.Text = "Pacientes";
            this.Load += new System.EventHandler(this.FrmPacientes_Load);
            ((System.ComponentModel.ISupportInitialize)(this.grdPacientes)).EndInit();
            this.pnlBusqueda.ResumeLayout(false);
            this.pnlBusqueda.PerformLayout();
            this.pnlDatos.ResumeLayout(false);
            this.pnlDatos.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.DataGridView grdPacientes;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDni;
        private System.Windows.Forms.DataGridViewTextBoxColumn colApellidos;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNombres;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFechaNacimiento;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEdad;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSexo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTelefono;
        private System.Windows.Forms.Panel pnlBusqueda;
        private System.Windows.Forms.TextBox txtBuscar;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.Button btnNuevo;
        private System.Windows.Forms.Label lblResultados;
        private System.Windows.Forms.TextBox txtFiltro;
        private System.Windows.Forms.Panel pnlDatos;
        private System.Windows.Forms.Label lblTituloDatos;
        private System.Windows.Forms.Label lblHistoriaTitulo;
        private System.Windows.Forms.Label lblNumeroHistoria;
        private System.Windows.Forms.Label lblDni;
        private System.Windows.Forms.TextBox txtDni;
        private System.Windows.Forms.Label lblApellidoPaterno;
        private System.Windows.Forms.TextBox txtApellidoPaterno;
        private System.Windows.Forms.Label lblApellidoMaterno;
        private System.Windows.Forms.TextBox txtApellidoMaterno;
        private System.Windows.Forms.Label lblNombres;
        private System.Windows.Forms.TextBox txtNombres;
        private System.Windows.Forms.Label lblFechaNacimiento;
        private System.Windows.Forms.DateTimePicker dtpFechaNacimiento;
        private System.Windows.Forms.Label lblSexo;
        private System.Windows.Forms.ComboBox cboSexo;
        private System.Windows.Forms.Label lblTelefono;
        private System.Windows.Forms.TextBox txtTelefono;
        private System.Windows.Forms.Label lblDireccion;
        private System.Windows.Forms.TextBox txtDireccion;
        private System.Windows.Forms.Button btnEditar;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCancelar;
    }
}
