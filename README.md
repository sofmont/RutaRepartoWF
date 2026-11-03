# RutaRepartoWF — Ruta de reparto (Problema del Viajero)

Grafo **completo** no dirigido (km). Tres fases:
1. Vecino más cercano — O(n²)
2. Mejora local 2-opt (mejor mejora) — O(n²) por pasada
3. Held-Karp (programación dinámica exacta, n ≤ 18) — O(n²·2ⁿ) tiempo, O(n·2ⁿ) espacio

Abrir `RutaRepartoWF.slnx` en Visual Studio 2022 17.13+ (o `dotnet run --project RutaRepartoWF`), .NET 10, Windows.

Pestañas: matriz · lista (ordenada por distancia) · grafo (elige qué recorrido ver) · traza de las 3 fases · resultado.
Botones: planear ruta, cargar zona `.txt` (ver `zona_ejemplo.txt`), zona de ejemplo, exportar reporte `.txt`.
