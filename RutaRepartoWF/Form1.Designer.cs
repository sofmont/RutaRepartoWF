namespace RutaRepartoWF
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer? components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            tabControl = new TabControl();
            tabMatriz = new TabPage();
            dgvMatriz = new DataGridView();
            tabLista = new TabPage();
            txtLista = new TextBox();
            tabGrafo = new TabPage();
            panelGrafo = new PanelDobleBuffer();
            tabTraza = new TabPage();
            dgvTraza = new DataGridView();
            tabResultado = new TabPage();
            txtResultado = new TextBox();
            panelControles = new FlowLayoutPanel();
            lblInicio = new Label();
            cboInicio = new ComboBox();
            lblVer = new Label();
            cboVer = new ComboBox();
            btnCalcular = new Button();
            btnCargar = new Button();
            btnEjemplo = new Button();
            btnExportar = new Button();
            lblEstado = new Label();
            tabControl.SuspendLayout();
            tabMatriz.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvMatriz).BeginInit();
            tabLista.SuspendLayout();
            tabGrafo.SuspendLayout();
            tabTraza.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvTraza).BeginInit();
            tabResultado.SuspendLayout();
            panelControles.SuspendLayout();
            SuspendLayout();
            //
            // tabControl
            //
            tabControl.Controls.Add(tabMatriz);
            tabControl.Controls.Add(tabLista);
            tabControl.Controls.Add(tabGrafo);
            tabControl.Controls.Add(tabTraza);
            tabControl.Controls.Add(tabResultado);
            tabControl.Dock = DockStyle.Fill;
            tabControl.Location = new Point(0, 50);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(1200, 650);
            tabControl.TabIndex = 0;
            //
            // tabMatriz
            //
            tabMatriz.Controls.Add(dgvMatriz);
            tabMatriz.Name = "tabMatriz";
            tabMatriz.Padding = new Padding(3);
            tabMatriz.Text = "Matriz de adyacencia";
            tabMatriz.UseVisualStyleBackColor = true;
            //
            // dgvMatriz
            //
            dgvMatriz.AllowUserToAddRows = false;
            dgvMatriz.AllowUserToDeleteRows = false;
            dgvMatriz.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvMatriz.BackgroundColor = Color.White;
            dgvMatriz.Dock = DockStyle.Fill;
            dgvMatriz.Name = "dgvMatriz";
            dgvMatriz.ReadOnly = true;
            dgvMatriz.RowHeadersWidth = 110;
            dgvMatriz.TabIndex = 0;
            //
            // tabLista
            //
            tabLista.Controls.Add(txtLista);
            tabLista.Name = "tabLista";
            tabLista.Padding = new Padding(3);
            tabLista.Text = "Lista de adyacencia";
            tabLista.UseVisualStyleBackColor = true;
            //
            // txtLista
            //
            txtLista.Dock = DockStyle.Fill;
            txtLista.Font = new Font("Consolas", 10F);
            txtLista.Multiline = true;
            txtLista.Name = "txtLista";
            txtLista.ReadOnly = true;
            txtLista.ScrollBars = ScrollBars.Both;
            txtLista.WordWrap = false;
            txtLista.TabIndex = 0;
            //
            // tabGrafo
            //
            tabGrafo.Controls.Add(panelGrafo);
            tabGrafo.Name = "tabGrafo";
            tabGrafo.Padding = new Padding(3);
            tabGrafo.Text = "Grafo";
            tabGrafo.UseVisualStyleBackColor = true;
            //
            // panelGrafo
            //
            panelGrafo.BackColor = Color.White;
            panelGrafo.Dock = DockStyle.Fill;
            panelGrafo.Name = "panelGrafo";
            panelGrafo.TabIndex = 0;
            //
            // tabTraza
            //
            tabTraza.Controls.Add(dgvTraza);
            tabTraza.Name = "tabTraza";
            tabTraza.Padding = new Padding(3);
            tabTraza.Text = "Traza (VMC, 2-opt, Held-Karp)";
            tabTraza.UseVisualStyleBackColor = true;
            //
            // dgvTraza
            //
            dgvTraza.AllowUserToAddRows = false;
            dgvTraza.AllowUserToDeleteRows = false;
            dgvTraza.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvTraza.BackgroundColor = Color.White;
            dgvTraza.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvTraza.DefaultCellStyle.Font = new Font("Consolas", 9F);
            dgvTraza.Dock = DockStyle.Fill;
            dgvTraza.Name = "dgvTraza";
            dgvTraza.ReadOnly = true;
            dgvTraza.RowHeadersVisible = false;
            dgvTraza.TabIndex = 0;
            //
            // tabResultado
            //
            tabResultado.Controls.Add(txtResultado);
            tabResultado.Name = "tabResultado";
            tabResultado.Padding = new Padding(3);
            tabResultado.Text = "Resultado";
            tabResultado.UseVisualStyleBackColor = true;
            //
            // txtResultado
            //
            txtResultado.Dock = DockStyle.Fill;
            txtResultado.Font = new Font("Consolas", 10F);
            txtResultado.Multiline = true;
            txtResultado.Name = "txtResultado";
            txtResultado.ReadOnly = true;
            txtResultado.ScrollBars = ScrollBars.Both;
            txtResultado.WordWrap = false;
            txtResultado.TabIndex = 0;
            //
            // panelControles
            //
            panelControles.Controls.Add(lblInicio);
            panelControles.Controls.Add(cboInicio);
            panelControles.Controls.Add(lblVer);
            panelControles.Controls.Add(cboVer);
            panelControles.Controls.Add(btnCalcular);
            panelControles.Controls.Add(btnCargar);
            panelControles.Controls.Add(btnEjemplo);
            panelControles.Controls.Add(btnExportar);
            panelControles.Controls.Add(lblEstado);
            panelControles.Dock = DockStyle.Top;
            panelControles.Location = new Point(0, 0);
            panelControles.Name = "panelControles";
            panelControles.Padding = new Padding(8, 8, 8, 4);
            panelControles.Size = new Size(1200, 50);
            panelControles.TabIndex = 1;
            panelControles.WrapContents = false;
            //
            // lblInicio
            //
            lblInicio.AutoSize = true;
            lblInicio.Margin = new Padding(3, 8, 3, 0);
            lblInicio.Name = "lblInicio";
            lblInicio.Text = "Salida/regreso:";
            //
            // cboInicio
            //
            cboInicio.DropDownStyle = ComboBoxStyle.DropDownList;
            cboInicio.Name = "cboInicio";
            cboInicio.Size = new Size(130, 28);
            cboInicio.TabIndex = 0;
            //
            // lblVer
            //
            lblVer.AutoSize = true;
            lblVer.Margin = new Padding(12, 8, 3, 0);
            lblVer.Name = "lblVer";
            lblVer.Text = "Mostrar recorrido:";
            //
            // cboVer
            //
            cboVer.DropDownStyle = ComboBoxStyle.DropDownList;
            cboVer.Name = "cboVer";
            cboVer.Size = new Size(190, 28);
            cboVer.TabIndex = 1;
            //
            // btnCalcular
            //
            btnCalcular.BackColor = Color.SteelBlue;
            btnCalcular.Cursor = Cursors.Hand;
            btnCalcular.FlatStyle = FlatStyle.Flat;
            btnCalcular.ForeColor = Color.White;
            btnCalcular.Margin = new Padding(12, 0, 3, 0);
            btnCalcular.Name = "btnCalcular";
            btnCalcular.Size = new Size(160, 36);
            btnCalcular.TabIndex = 2;
            btnCalcular.Text = "Planear ruta";
            btnCalcular.UseVisualStyleBackColor = false;
            //
            // btnCargar
            //
            btnCargar.Cursor = Cursors.Hand;
            btnCargar.Margin = new Padding(12, 0, 3, 0);
            btnCargar.Name = "btnCargar";
            btnCargar.Size = new Size(140, 36);
            btnCargar.TabIndex = 3;
            btnCargar.Text = "Cargar zona .txt…";
            btnCargar.UseVisualStyleBackColor = true;
            //
            // btnEjemplo
            //
            btnEjemplo.Cursor = Cursors.Hand;
            btnEjemplo.Name = "btnEjemplo";
            btnEjemplo.Size = new Size(130, 36);
            btnEjemplo.TabIndex = 4;
            btnEjemplo.Text = "Zona de ejemplo";
            btnEjemplo.UseVisualStyleBackColor = true;
            //
            // btnExportar
            //
            btnExportar.BackColor = Color.DarkSlateGray;
            btnExportar.Cursor = Cursors.Hand;
            btnExportar.FlatStyle = FlatStyle.Flat;
            btnExportar.ForeColor = Color.White;
            btnExportar.Margin = new Padding(12, 0, 3, 0);
            btnExportar.Name = "btnExportar";
            btnExportar.Size = new Size(140, 36);
            btnExportar.TabIndex = 5;
            btnExportar.Text = "Exportar a .txt";
            btnExportar.UseVisualStyleBackColor = false;
            //
            // lblEstado
            //
            lblEstado.AutoSize = true;
            lblEstado.ForeColor = Color.DimGray;
            lblEstado.Margin = new Padding(12, 8, 3, 0);
            lblEstado.Name = "lblEstado";
            lblEstado.Text = "";
            //
            // Form1
            //
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1200, 700);
            Controls.Add(tabControl);
            Controls.Add(panelControles);
            MinimumSize = new Size(950, 550);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Ruta de reparto — Problema del Viajero (VMC + 2-opt + Held-Karp)";
            tabControl.ResumeLayout(false);
            tabMatriz.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvMatriz).EndInit();
            tabLista.ResumeLayout(false);
            tabLista.PerformLayout();
            tabGrafo.ResumeLayout(false);
            tabTraza.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvTraza).EndInit();
            tabResultado.ResumeLayout(false);
            tabResultado.PerformLayout();
            panelControles.ResumeLayout(false);
            panelControles.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl;
        private TabPage tabMatriz;
        private DataGridView dgvMatriz;
        private TabPage tabLista;
        private TextBox txtLista;
        private TabPage tabGrafo;
        private PanelDobleBuffer panelGrafo;
        private TabPage tabTraza;
        private DataGridView dgvTraza;
        private TabPage tabResultado;
        private TextBox txtResultado;
        private FlowLayoutPanel panelControles;
        private Label lblInicio;
        private ComboBox cboInicio;
        private Label lblVer;
        private ComboBox cboVer;
        private Button btnCalcular;
        private Button btnCargar;
        private Button btnEjemplo;
        private Button btnExportar;
        private Label lblEstado;
    }
}
