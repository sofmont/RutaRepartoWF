using System.Text;
using System.Text.RegularExpressions;

namespace RutaRepartoWF;

/// <summary>
/// Zona de reparto modelada como grafo COMPLETO, no dirigido y ponderado (km por calle).
/// La matriz de adyacencia es simétrica con diagonal 0.
/// </summary>
public sealed class GrafoCompleto
{
    public const int MinNodos = 3;
    public const int MaxNodos = 30;
    public const int MaxKm = 100_000;

    private static readonly Regex NombreValido = new(@"^[\p{L}\p{N}_-]{1,20}$");

    private readonly string[] _nombres;
    private readonly int[,] _d;

    public int N => _nombres.Length;
    public IReadOnlyList<string> Nombres => Array.AsReadOnly(_nombres);

    /// <summary>Valida y copia la matriz: cuadrada, diagonal 0, pesos positivos y simétrica.</summary>
    public GrafoCompleto(IReadOnlyList<string> nombres, int[,] matriz)
    {
        if (nombres is null || nombres.Count < MinNodos || nombres.Count > MaxNodos)
            throw new ArgumentException($"Se requieren entre {MinNodos} y {MaxNodos} puntos.");

        var vistos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (string s in nombres)
        {
            if (string.IsNullOrEmpty(s) || !NombreValido.IsMatch(s))
                throw new ArgumentException($"Nombre inválido: '{s}' (letras, dígitos, '_' o '-', máx. 20).");
            if (!vistos.Add(s))
                throw new ArgumentException($"Nombre repetido: {s}");
        }

        int n = nombres.Count;
        if (matriz is null || matriz.GetLength(0) != n || matriz.GetLength(1) != n)
            throw new ArgumentException($"La matriz debe ser de {n}×{n}.");

        for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
            {
                if (i == j && matriz[i, j] != 0)
                    throw new ArgumentException($"La diagonal debe ser 0 ({nombres[i]}).");
                if (i != j && (matriz[i, j] < 1 || matriz[i, j] > MaxKm))
                    throw new ArgumentException(
                        $"Distancia fuera de rango [1, {MaxKm}]: {nombres[i]}→{nombres[j]} = {matriz[i, j]}");
                if (matriz[i, j] != matriz[j, i])
                    throw new ArgumentException(
                        $"La matriz no es simétrica: {nombres[i]}→{nombres[j]} = {matriz[i, j]} " +
                        $"pero {nombres[j]}→{nombres[i]} = {matriz[j, i]}");
            }

        _nombres = nombres.ToArray();
        _d = (int[,])matriz.Clone();
    }

    public string Nombre(int i) => _nombres[i];

    public int Dist(int i, int j) => _d[i, j];

    public int Indice(string nombre) =>
        Array.FindIndex(_nombres, n => string.Equals(n, nombre, StringComparison.OrdinalIgnoreCase));

    /// <summary>Pares (i,k) que violan la desigualdad triangular d(i,k) ≤ d(i,j) + d(j,k) para algún j.</summary>
    public int ViolacionesTriangulares()
    {
        int c = 0;
        for (int i = 0; i < N; i++)
            for (int k = i + 1; k < N; k++)
                for (int j = 0; j < N; j++)
                    if (j != i && j != k && _d[i, k] > _d[i, j] + _d[j, k]) { c++; break; }
        return c;
    }

    public string FormatearMatriz()
    {
        int ancho = Math.Max(5, _nombres.Max(s => s.Length) + 2);
        var sb = new StringBuilder();
        sb.Append("".PadRight(ancho));
        foreach (string s in _nombres) sb.Append(s.PadRight(ancho));
        sb.AppendLine();
        for (int i = 0; i < N; i++)
        {
            sb.Append(_nombres[i].PadRight(ancho));
            for (int j = 0; j < N; j++) sb.Append(_d[i, j].ToString().PadRight(ancho));
            sb.AppendLine();
        }
        return sb.ToString();
    }

    /// <summary>Lista de adyacencia ordenada por distancia: lo que consulta el vecino más cercano.</summary>
    public string FormatearLista()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < N; i++)
        {
            int u = i;
            var vecinos = Enumerable.Range(0, N).Where(j => j != u)
                .OrderBy(j => _d[u, j]).ThenBy(j => j)
                .Select(j => $"[{_nombres[j]}, {_d[u, j]}]");
            sb.AppendLine($"{_nombres[i],-11}→ {string.Join(" → ", vecinos)}");
        }
        return sb.ToString();
    }

    /// <summary>Zona de ejemplo: una bodega y 6 clientes; km por calle entre cada par.</summary>
    public static GrafoCompleto CrearZonaReparto()
    {
        string[] puntos = { "Bodega", "Farmacia", "Papeleria", "Taller", "Floreria", "Oficina", "Panaderia" };
        int[,] km =
        {
            //Bod Far Pap Tal Flo Ofi Pan
            {  0, 15, 13, 14, 12, 19, 12 },   // Bodega
            { 15,  0,  9, 20, 20, 30,  3 },   // Farmacia
            { 13,  9,  0, 11, 11, 22,  8 },   // Papeleria
            { 14, 20, 11,  0,  2, 12, 17 },   // Taller
            { 12, 20, 11,  2,  0, 11, 17 },   // Floreria
            { 19, 30, 22, 12, 11,  0, 28 },   // Oficina
            { 12,  3,  8, 17, 17, 28,  0 },   // Panaderia
        };
        return new GrafoCompleto(puntos, km);
    }
}
