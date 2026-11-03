namespace RutaRepartoWF;

/// <summary>Utilidades para recorridos: un tour es una permutación cíclica que regresa al inicio.</summary>
public static class Recorridos
{
    public static long Costo(GrafoCompleto g, int[] tour)
    {
        long c = 0;
        for (int i = 0; i < tour.Length; i++) c += g.Dist(tour[i], tour[(i + 1) % tour.Length]);
        return c;
    }

    public static string Texto(GrafoCompleto g, int[] tour) =>
        string.Join(" → ", tour.Select(g.Nombre)) + " → " + g.Nombre(tour[0]);
}

// ─────────────────────────────────────────────────────────────────────────────
//  1) Vecino más cercano — Tiempo O(n²), Espacio O(n)
// ─────────────────────────────────────────────────────────────────────────────

public sealed record PasoVecino(int Numero, int Actual, string Candidatos, int Elegido, int Distancia, long Acumulado);

public static class VecinoMasCercano
{
    public static int[] Resolver(GrafoCompleto g, int inicio, List<PasoVecino> traza)
    {
        int n = g.N;
        if (inicio < 0 || inicio >= n) throw new ArgumentOutOfRangeException(nameof(inicio));

        var visitado = new bool[n];
        var tour = new int[n];
        tour[0] = inicio;
        visitado[inicio] = true;
        long acumulado = 0;

        for (int paso = 1; paso < n; paso++)
        {
            int u = tour[paso - 1];
            int mejor = -1;
            var candidatos = new List<string>();
            for (int v = 0; v < n; v++)
            {
                if (visitado[v]) continue;
                candidatos.Add($"{g.Nombre(v)}={g.Dist(u, v)}");
                if (mejor == -1 || g.Dist(u, v) < g.Dist(u, mejor)) mejor = v;   // empate → menor índice
            }
            visitado[mejor] = true;
            tour[paso] = mejor;
            acumulado += g.Dist(u, mejor);
            traza.Add(new PasoVecino(paso, u, string.Join(", ", candidatos), mejor, g.Dist(u, mejor), acumulado));
        }

        int ultimo = tour[n - 1];
        acumulado += g.Dist(ultimo, inicio);
        traza.Add(new PasoVecino(n, ultimo, "(todos visitados) regreso obligatorio", inicio,
            g.Dist(ultimo, inicio), acumulado));
        return tour;
    }
}

// ─────────────────────────────────────────────────────────────────────────────
//  2) 2-opt con estrategia de mejor mejora — O(n²) por pasada
// ─────────────────────────────────────────────────────────────────────────────

public sealed record PasoDosOpt(int Pasada, string Quita, string Agrega, long Delta, string NuevoTour, long NuevoCosto);

public static class DosOpt
{
    /// <summary>
    /// Para aristas no adyacentes (a,b) y (c,d) del tour evalúa
    /// Δ = d(a,c) + d(b,d) − d(a,b) − d(c,d). Si el mejor Δ &lt; 0 invierte el segmento b…c.
    /// Repite hasta que ningún intercambio mejore (óptimo local). El inicio queda fijo.
    /// </summary>
    public static int[] Mejorar(GrafoCompleto g, int[] inicial, List<PasoDosOpt> traza)
    {
        int[] t = (int[])inicial.Clone();
        int n = t.Length;
        long costo = Recorridos.Costo(g, t);

        for (int pasada = 1; ; pasada++)
        {
            long mejorDelta = 0;
            int mi = -1, mk = -1;
            for (int i = 1; i < n - 1; i++)
            {
                for (int k = i + 1; k < n; k++)
                {
                    int a = t[i - 1], b = t[i], c = t[k], d = t[(k + 1) % n];
                    if (a == c || b == d) continue;            // aristas adyacentes: sin efecto
                    long delta = (long)g.Dist(a, c) + g.Dist(b, d) - g.Dist(a, b) - g.Dist(c, d);
                    if (delta < mejorDelta)
                    {
                        mejorDelta = delta;
                        mi = i;
                        mk = k;
                    }
                }
            }
            if (mi == -1) break;                               // óptimo local 2-opt

            int pa = t[mi - 1], pb = t[mi], pc = t[mk], pd = t[(mk + 1) % n];
            Array.Reverse(t, mi, mk - mi + 1);
            costo += mejorDelta;
            traza.Add(new PasoDosOpt(pasada,
                $"({g.Nombre(pa)},{g.Nombre(pb)})={g.Dist(pa, pb)} + ({g.Nombre(pc)},{g.Nombre(pd)})={g.Dist(pc, pd)}",
                $"({g.Nombre(pa)},{g.Nombre(pc)})={g.Dist(pa, pc)} + ({g.Nombre(pb)},{g.Nombre(pd)})={g.Dist(pb, pd)}",
                mejorDelta, Recorridos.Texto(g, t), costo));
        }
        return t;
    }
}

// ─────────────────────────────────────────────────────────────────────────────
//  3) Held-Karp: programación dinámica exacta — O(n²·2ⁿ) tiempo, O(n·2ⁿ) espacio
// ─────────────────────────────────────────────────────────────────────────────

public sealed class HeldKarp
{
    public const int MaxN = 18;            // 2^17 · 17 estados ≈ 2.2 M
    private const long Infinito = long.MaxValue / 4;

    public long Transiciones { get; private set; }

    /// <summary>
    /// C(S, j) = costo mínimo de salir del inicio, visitar exactamente S (sin el inicio) y terminar en j ∈ S.
    ///   C({j}, j) = d(inicio, j)
    ///   C(S, j)   = min_{i ∈ S∖{j}} C(S∖{j}, i) + d(i, j)
    ///   OPT       = min_j C(V∖{inicio}, j) + d(j, inicio)
    /// Devuelve null si n &gt; MaxN.
    /// </summary>
    public int[]? Resolver(GrafoCompleto g, int inicio)
    {
        int n = g.N;
        if (n > MaxN) return null;
        int m = n - 1;
        var mapa = new int[m];                    // bit k de la máscara → nodo real
        for (int v = 0, k = 0; v < n; v++) if (v != inicio) mapa[k++] = v;

        int total = 1 << m;
        var c = new long[total, m];
        var padre = new int[total, m];
        for (int s = 0; s < total; s++)
            for (int j = 0; j < m; j++) c[s, j] = Infinito;
        for (int j = 0; j < m; j++) c[1 << j, j] = g.Dist(inicio, mapa[j]);

        Transiciones = 0;
        for (int s = 1; s < total; s++)
        {
            for (int j = 0; j < m; j++)
            {
                if ((s & (1 << j)) == 0 || c[s, j] == Infinito) continue;
                for (int k = 0; k < m; k++)
                {
                    if ((s & (1 << k)) != 0) continue;
                    int s2 = s | (1 << k);
                    long candidato = c[s, j] + g.Dist(mapa[j], mapa[k]);
                    Transiciones++;
                    if (candidato < c[s2, k])
                    {
                        c[s2, k] = candidato;
                        padre[s2, k] = j;
                    }
                }
            }
        }

        int lleno = total - 1, ultimo = 0;
        long mejor = Infinito;
        for (int j = 0; j < m; j++)
        {
            long candidato = c[lleno, j] + g.Dist(mapa[j], inicio);
            if (candidato < mejor) { mejor = candidato; ultimo = j; }
        }

        // Reconstrucción hacia atrás con la tabla de padres.
        var tour = new int[n];
        tour[0] = inicio;
        int mascara = lleno, actual = ultimo;
        for (int pos = n - 1; pos >= 1; pos--)
        {
            tour[pos] = mapa[actual];
            int anterior = padre[mascara, actual];
            mascara ^= 1 << actual;
            actual = anterior;
        }
        return tour;
    }
}

// ─────────────────────────────────────────────────────────────────────────────
//  Orquestación de las tres fases
// ─────────────────────────────────────────────────────────────────────────────

public sealed class ResultadoTsp
{
    public int Inicio { get; init; }
    public int Violaciones { get; init; }
    public int[] TourVmc { get; init; } = Array.Empty<int>();
    public long CostoVmc { get; init; }
    public List<PasoVecino> TrazaVmc { get; init; } = new();
    public int[] TourDosOpt { get; init; } = Array.Empty<int>();
    public long CostoDosOpt { get; init; }
    public List<PasoDosOpt> TrazaDosOpt { get; init; } = new();
    public int[]? TourOptimo { get; init; }
    public long? CostoOptimo { get; init; }
    public long TransicionesHk { get; init; }

    /// <summary>El mejor recorrido disponible para el repartidor.</summary>
    public int[] Recomendado =>
        TourOptimo != null && CostoOptimo < CostoDosOpt ? TourOptimo : TourDosOpt;
}

public static class PlaneadorTsp
{
    public static ResultadoTsp Planear(GrafoCompleto g, int inicio)
    {
        var trazaVmc = new List<PasoVecino>();
        int[] vmc = VecinoMasCercano.Resolver(g, inicio, trazaVmc);

        var trazaDos = new List<PasoDosOpt>();
        int[] dosOpt = DosOpt.Mejorar(g, vmc, trazaDos);

        var hk = new HeldKarp();
        int[]? opt = hk.Resolver(g, inicio);

        return new ResultadoTsp
        {
            Inicio = inicio,
            Violaciones = g.ViolacionesTriangulares(),
            TourVmc = vmc,
            CostoVmc = Recorridos.Costo(g, vmc),
            TrazaVmc = trazaVmc,
            TourDosOpt = dosOpt,
            CostoDosOpt = Recorridos.Costo(g, dosOpt),
            TrazaDosOpt = trazaDos,
            TourOptimo = opt,
            CostoOptimo = opt is null ? null : Recorridos.Costo(g, opt),
            TransicionesHk = hk.Transiciones
        };
    }
}
