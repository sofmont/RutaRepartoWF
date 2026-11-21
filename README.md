# RutaRepartoWF — Ruta de reparto (Problema del Viajero)

**Unidad 4 · Tarea 4.3.**

## Datos de la entrega

| | |
|---|---|
| Alumna | Barbara Sofia Montalvo Ontiveros |
| Matrícula | I25050373 |
| Carrera | Ingeniería en Informática |
| Asignatura | Estructuras de Datos |
| Docente | Ruben Riojas |
| Institución | Instituto Tecnológico Superior de Monclova |
| Video explicativo | _(pegar aquí el enlace público de YouTube)_ |


Grafo **completo** no dirigido (km). Tres fases:
1. Vecino más cercano — O(n²)
2. Mejora local 2-opt (mejor mejora) — O(n²) por pasada
3. Held-Karp (programación dinámica exacta, n ≤ 18) — O(n²·2ⁿ) tiempo, O(n·2ⁿ) espacio

Abrir `RutaRepartoWF.slnx` en Visual Studio 2022 17.13+ (o `dotnet run --project RutaRepartoWF`), .NET 10, Windows.

Pestañas: matriz · lista (ordenada por distancia) · grafo (elige qué recorrido ver) · traza de las 3 fases · resultado.
Botones: planear ruta, cargar zona `.txt` (ver `zona_ejemplo.txt`), zona de ejemplo, exportar reporte `.txt`.

## Resultados con la zona de ejemplo (7 puntos, inicio en Bodega)

| Fase | Ruta | Km |
|---|---|---|
| Vecino más cercano | Bodega → Florería → Taller → Papelería → Panadería → Farmacia → Oficina → Bodega | 85 |
| + 2-opt (2 pasadas) | Bodega → Panadería → Farmacia → Papelería → Taller → Florería → Oficina → Bodega | 67 |
| Held-Karp (exacto, 480 transiciones) | la misma ruta | 67 |

Ahorro: 18 km (21.2 %). Verificado contra fuerza bruta (720 rutas).
