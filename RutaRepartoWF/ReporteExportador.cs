using System.Globalization;
using System.Text;

namespace RutaRepartoWF;

/// <summary>Genera el reporte en texto de las tres fases y el resultado.</summary>
public static class ReporteExportador
{
    public static string Generar(GrafoCompleto g, ResultadoTsp r)
    {
        var sb = new StringBuilder();
        sb.AppendLine("═══════════════════════════════════════════════════════");
        sb.AppendLine("  REPORTE — RUTA DE REPARTO (PROBLEMA DEL VIAJERO)");
        sb.AppendLine("═══════════════════════════════════════════════════════");
        sb.AppendLine();
        sb.AppendLine("── MATRIZ DE ADYACENCIA (km) ──");
        sb.AppendLine(g.FormatearMatriz());
        sb.AppendLine("── LISTA DE ADYACENCIA (ordenada por distancia) ──");
        sb.AppendLine(g.FormatearLista());
        sb.AppendLine(Traza(g, r));
        sb.Append(Resultado(g, r));
        return sb.ToString();
    }

    public static string Traza(GrafoCompleto g, ResultadoTsp r)
    {
        var sb = new StringBuilder();
        if (r.Violaciones > 0)
        {
            sb.AppendLine($"⚠ Aviso: {r.Violaciones} par(es) violan la desigualdad triangular; " +
                          "las garantías métricas no aplican (los algoritmos siguen funcionando).");
            sb.AppendLine();
        }

        sb.AppendLine($"── FASE 1: VECINO MÁS CERCANO desde {g.Nombre(r.Inicio)} ──");
        foreach (PasoVecino p in r.TrazaVmc)
        {
            sb.AppendLine($"Paso {p.Numero}: en {g.Nombre(p.Actual)} | candidatos: {p.Candidatos}");
            sb.AppendLine($"        → ir a {g.Nombre(p.Elegido)} (+{p.Distancia} km)   acumulado = {p.Acumulado} km");
        }
        sb.AppendLine($"Recorrido: {Recorridos.Texto(g, r.TourVmc)}   = {r.CostoVmc} km");
        sb.AppendLine();

        sb.AppendLine("── FASE 2: MEJORA LOCAL 2-OPT ──");
        if (r.TrazaDosOpt.Count == 0)
            sb.AppendLine("Ningún intercambio 2-opt mejora el recorrido (ya es óptimo local).");
        foreach (PasoDosOpt p in r.TrazaDosOpt)
        {
            sb.AppendLine($"Pasada {p.Pasada}:");
            sb.AppendLine($"    quitar  {p.Quita}");
            sb.AppendLine($"    agregar {p.Agrega}");
            sb.AppendLine($"    Δ = {p.Delta} km → invertir segmento");
            sb.AppendLine($"    {p.NuevoTour}   = {p.NuevoCosto} km");
        }
        if (r.TrazaDosOpt.Count > 0)
            sb.AppendLine($"Pasada {r.TrazaDosOpt.Count + 1}: ningún Δ < 0 → óptimo local.");
        sb.AppendLine();

        sb.AppendLine("── FASE 3: CERTIFICADO EXACTO (HELD-KARP) ──");
        if (r.TourOptimo is null)
        {
            sb.AppendLine($"Omitido: n = {g.N} > {HeldKarp.MaxN} (O(n²·2ⁿ) sería impráctico). " +
                          "Se reporta sólo la heurística.");
        }
        else
        {
            sb.AppendLine($"Subconjuntos: 2^{g.N - 1} = {1L << (g.N - 1)}   transiciones evaluadas: {r.TransicionesHk:N0}");
            sb.AppendLine($"Recorrido óptimo: {Recorridos.Texto(g, r.TourOptimo)}   = {r.CostoOptimo} km");
        }
        return sb.ToString();
    }

    public static string Resultado(GrafoCompleto g, ResultadoTsp r)
    {
        var sb = new StringBuilder();
        sb.AppendLine("── RESULTADO ──");
        sb.AppendLine($"Vecino más cercano : {r.CostoVmc,4} km{Razon(r.CostoVmc, r.CostoOptimo)}");
        sb.AppendLine($"VMC + 2-opt        : {r.CostoDosOpt,4} km{Razon(r.CostoDosOpt, r.CostoOptimo)}");
        if (r.CostoOptimo.HasValue)
            sb.AppendLine($"Óptimo (Held-Karp) : {r.CostoOptimo.Value,4} km");
        sb.AppendLine();
        sb.AppendLine("Ruta recomendada para el repartidor:");

        int[] tour = r.Recomendado;
        long acumulado = 0;
        for (int i = 0; i < tour.Length; i++)
        {
            int a = tour[i], b = tour[(i + 1) % tour.Length];
            acumulado += g.Dist(a, b);
            sb.AppendLine($"  {i + 1,2}. {g.Nombre(a),-11} → {g.Nombre(b),-11} {g.Dist(a, b),4} km   (acumulado {acumulado} km)");
        }
        return sb.ToString();
    }

    private static string Razon(long costo, long? optimo)
    {
        if (optimo is not long opt || opt <= 0) return "";
        double razon = (double)costo / opt;
        double exceso = 100.0 * (costo - opt) / opt;
        return string.Format(CultureInfo.InvariantCulture, "   ({0:F3} × óptimo, +{1:F1} %)", razon, exceso);
    }
}
