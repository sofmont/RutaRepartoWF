using System.Text;

namespace RutaRepartoWF;

public partial class Form1 : Form
{
    private const int VerVmc = 0, VerDosOpt = 1, VerOptimo = 2;

    private GrafoCompleto _zona;
    private RenderizadorGrafo _renderizador;
    private ResultadoTsp? _ultimoResultado;

    public Form1()
    {
        InitializeComponent();
        _zona = GrafoCompleto.CrearZonaReparto();
        _renderizador = new RenderizadorGrafo(_zona);

        cboVer.Items.AddRange(new object[] { "Vecino más cercano", "VMC + 2-opt", "Óptimo (Held-Karp)" });
        cboVer.SelectedIndex = VerDosOpt;

        ConectarEventos();
        ConfigurarColumnasTraza();
        MostrarZona();
    }

    // ────────────────────────────
    //  Inicialización
    // ────────────────────────────

    private void ConectarEventos()
    {
        btnCalcular.Click += AlCalcular;
        btnExportar.Click += AlExportar;
        btnCargar.Click += AlCargar;
        btnEjemplo.Click += (_, _) => UsarZona(GrafoCompleto.CrearZonaReparto(), "Zona de ejemplo cargada.");
        panelGrafo.Paint += AlDibujarGrafo;
        cboVer.SelectedIndexChanged += (_, _) => panelGrafo.Invalidate();
        cboInicio.SelectedIndexChanged += (_, _) => panelGrafo.Invalidate();
    }

    private void ConfigurarColumnasTraza()
    {
        dgvTraza.Columns.Clear();
        dgvTraza.Columns.Add("fase", "Fase");
        dgvTraza.Columns.Add("paso", "Paso");
        dgvTraza.Columns.Add("estado", "Estado / candidatos");
        dgvTraza.Columns.Add("decision", "Decisión");
        dgvTraza.Columns.Add("recorrido", "Recorrido / costo acumulado");
        dgvTraza.Columns["fase"]!.Width = 110;
        dgvTraza.Columns["paso"]!.Width = 60;
        dgvTraza.Columns["estado"]!.Width = 360;
        dgvTraza.Columns["decision"]!.Width = 300;
        dgvTraza.Columns["recorrido"]!.Width = 420;
        foreach (DataGridViewColumn c in dgvTraza.Columns) c.SortMode = DataGridViewColumnSortMode.NotSortable;
    }

    private void UsarZona(GrafoCompleto zona, string mensaje)
    {
        _zona = zona;
        _renderizador = new RenderizadorGrafo(zona);
        _ultimoResultado = null;
        dgvTraza.Rows.Clear();
        txtResultado.Clear();
        MostrarZona();
        lblEstado.Text = mensaje;
    }

    private void MostrarZona()
    {
        cboInicio.Items.Clear();
        foreach (string nombre in _zona.Nombres) cboInicio.Items.Add(nombre);
        cboInicio.SelectedIndex = 0;

        dgvMatriz.Columns.Clear();
        dgvMatriz.Rows.Clear();
        for (int j = 0; j < _zona.N; j++)
        {
            int col = dgvMatriz.Columns.Add($"c{j}", _zona.Nombre(j));
            dgvMatriz.Columns[col].SortMode = DataGridViewColumnSortMode.NotSortable;
        }
        for (int i = 0; i < _zona.N; i++)
        {
            var fila = new string[_zona.N];
            for (int j = 0; j < _zona.N; j++) fila[j] = _zona.Dist(i, j).ToString();
            int idx = dgvMatriz.Rows.Add(fila);
            dgvMatriz.Rows[idx].HeaderCell.Value = _zona.Nombre(i);
        }

        txtLista.Text = _zona.FormatearLista();
        panelGrafo.Invalidate();
        lblEstado.Text = $"{_zona.N} puntos, {_zona.N * (_zona.N - 1) / 2} aristas en el grafo completo";
    }

    // ────────────────────────────
    //  Planeación (3 fases)
    // ────────────────────────────

    private void AlCalcular(object? sender, EventArgs e)
    {
        int inicio = cboInicio.SelectedIndex;
        if (inicio < 0)
        {
            MessageBox.Show("Seleccione el punto de salida y regreso.", "Datos incompletos",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (_zona.N > HeldKarp.MaxN)
            lblEstado.Text = $"n = {_zona.N} > {HeldKarp.MaxN}: se omite Held-Karp, sólo heurística.";

        Cursor = Cursors.WaitCursor;
        try
        {
            _ultimoResultado = PlaneadorTsp.Planear(_zona, inicio);
        }
        finally
        {
            Cursor = Cursors.Default;
        }

        MostrarTraza(_ultimoResultado);
        txtResultado.Text = ReporteExportador.Resultado(_zona, _ultimoResultado);
        if (_ultimoResultado.TourOptimo is null && cboVer.SelectedIndex == VerOptimo)
            cboVer.SelectedIndex = VerDosOpt;
        panelGrafo.Invalidate();

        string optimo = _ultimoResultado.CostoOptimo.HasValue ? $", óptimo {_ultimoResultado.CostoOptimo} km" : "";
        lblEstado.Text = $"VMC {_ultimoResultado.CostoVmc} km → 2-opt {_ultimoResultado.CostoDosOpt} km{optimo}";
        tabControl.SelectedTab = tabTraza;

        if (_ultimoResultado.Violaciones > 0)
            MessageBox.Show($"{_ultimoResultado.Violaciones} par(es) de puntos violan la desigualdad triangular." +
                            Environment.NewLine + "Los algoritmos funcionan, pero las garantías métricas no aplican.",
                "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void MostrarTraza(ResultadoTsp r)
    {
        dgvTraza.Rows.Clear();

        foreach (PasoVecino p in r.TrazaVmc)
        {
            int idx = dgvTraza.Rows.Add("1 · VMC", p.Numero,
                $"En {_zona.Nombre(p.Actual)}{Environment.NewLine}{p.Candidatos.Replace(", ", "  ")}",
                $"→ {_zona.Nombre(p.Elegido)} (+{p.Distancia} km)",
                $"acumulado = {p.Acumulado} km");
            dgvTraza.Rows[idx].DefaultCellStyle.BackColor = Color.AliceBlue;
        }
        AgregarFilaResumen("1 · VMC", $"{Recorridos.Texto(_zona, r.TourVmc)}  = {r.CostoVmc} km", Color.LightSteelBlue);

        foreach (PasoDosOpt p in r.TrazaDosOpt)
        {
            int idx = dgvTraza.Rows.Add("2 · 2-opt", p.Pasada,
                $"quitar  {p.Quita}{Environment.NewLine}agregar {p.Agrega}",
                $"Δ = {p.Delta} km → invertir segmento",
                $"{p.NuevoTour}{Environment.NewLine}= {p.NuevoCosto} km");
            dgvTraza.Rows[idx].DefaultCellStyle.BackColor = Color.Honeydew;
        }
        AgregarFilaResumen("2 · 2-opt",
            r.TrazaDosOpt.Count == 0
                ? "Ningún Δ < 0: el recorrido del VMC ya es óptimo local."
                : $"Pasada {r.TrazaDosOpt.Count + 1}: ningún Δ < 0 → óptimo local ({r.CostoDosOpt} km)",
            Color.PaleGreen);

        AgregarFilaResumen("3 · Held-Karp",
            r.TourOptimo is null
                ? $"Omitido: n = {_zona.N} > {HeldKarp.MaxN}"
                : $"2^{_zona.N - 1} subconjuntos, {r.TransicionesHk:N0} transiciones → " +
                  $"{Recorridos.Texto(_zona, r.TourOptimo)}  = {r.CostoOptimo} km",
            Color.Khaki);
    }

    private void AgregarFilaResumen(string fase, string texto, Color color)
    {
        int idx = dgvTraza.Rows.Add(fase, "—", "", "Resultado de la fase", texto);
        dgvTraza.Rows[idx].DefaultCellStyle.BackColor = color;
        dgvTraza.Rows[idx].DefaultCellStyle.Font = new Font(dgvTraza.DefaultCellStyle.Font ?? Font, FontStyle.Bold);
    }

    // ────────────────────────────
    //  Dibujo
    // ────────────────────────────

    private void AlDibujarGrafo(object? sender, PaintEventArgs e)
    {
        int[]? tour = null;
        Color color = Color.SteelBlue;
        if (_ultimoResultado != null)
        {
            switch (cboVer.SelectedIndex)
            {
                case VerVmc:
                    tour = _ultimoResultado.TourVmc;
                    color = Color.DarkOrange;
                    break;
                case VerOptimo when _ultimoResultado.TourOptimo != null:
                    tour = _ultimoResultado.TourOptimo;
                    color = Color.Crimson;
                    break;
                default:
                    tour = _ultimoResultado.TourDosOpt;
                    color = Color.SeaGreen;
                    break;
            }
        }
        int inicio = _ultimoResultado?.Inicio ?? cboInicio.SelectedIndex;
        _renderizador.Dibujar(e.Graphics, panelGrafo.ClientRectangle, tour, color, inicio);
    }

    // ────────────────────────────
    //  Archivos
    // ────────────────────────────

    private void AlCargar(object? sender, EventArgs e)
    {
        using var dialogo = new OpenFileDialog
        {
            Title = "Cargar zona de reparto",
            Filter = "Archivos de texto (*.txt)|*.txt|Todos los archivos (*.*)|*.*"
        };
        if (dialogo.ShowDialog(this) != DialogResult.OK) return;

        try
        {
            UsarZona(LectorMatriz.Leer(dialogo.FileName), $"Zona cargada: {Path.GetFileName(dialogo.FileName)}");
        }
        catch (MatrizInvalidaException ex)
        {
            MessageBox.Show(ex.Message + Environment.NewLine + Environment.NewLine + "Se conserva la zona anterior.",
                "Archivo inválido", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void AlExportar(object? sender, EventArgs e)
    {
        if (_ultimoResultado is null)
        {
            MessageBox.Show("Primero planee una ruta.", "Sin resultados",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var dialogo = new SaveFileDialog
        {
            Title = "Exportar reporte",
            Filter = "Archivo de texto (*.txt)|*.txt",
            FileName = "reporte_tsp.txt"
        };
        if (dialogo.ShowDialog(this) != DialogResult.OK) return;

        try
        {
            File.WriteAllText(dialogo.FileName, ReporteExportador.Generar(_zona, _ultimoResultado),
                new UTF8Encoding(true));
            lblEstado.Text = $"Reporte guardado: {Path.GetFileName(dialogo.FileName)}";
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            MessageBox.Show($"No se pudo guardar el archivo:{Environment.NewLine}{ex.Message}",
                "Error al exportar", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
