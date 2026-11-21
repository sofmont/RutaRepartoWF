using System.Text;

namespace RutaRepartoWF;

/// <summary>Error de formato o de contenido en el archivo de la zona.</summary>
public sealed class MatrizInvalidaException : Exception
{
    public MatrizInvalidaException(string mensaje) : base(mensaje) { }
}

/// <summary>
/// Lee la matriz de distancias desde un archivo. Una FILA por nodo:
///     NODOS Bodega Farmacia Papeleria
///     FILA Bodega    0 15 13
///     FILA Farmacia 15  0  9
///     FILA Papeleria 13 9  0
/// </summary>
public static class LectorMatriz
{
    public static GrafoCompleto Leer(string ruta)
    {
        if (string.IsNullOrWhiteSpace(ruta))
            throw new MatrizInvalidaException("No se indicó un archivo.");
        if (!File.Exists(ruta))
            throw new MatrizInvalidaException($"No existe el archivo: {ruta}");

        string[] lineas;
        try
        {
            if (new FileInfo(ruta).Length > 1_000_000)
                throw new MatrizInvalidaException("El archivo es demasiado grande.");
            lineas = File.ReadAllLines(ruta, Encoding.UTF8);
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
            throw new MatrizInvalidaException($"No se pudo leer el archivo: {e.Message}");
        }

        string[]? nombres = null;
        int[]?[]? filas = null;

        for (int i = 0; i < lineas.Length; i++)
        {
            string linea = lineas[i];
            int hash = linea.IndexOf('#');
            if (hash >= 0) linea = linea[..hash];
            linea = linea.Trim();
            if (linea.Length == 0) continue;

            string[] t = linea.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
            string pre = $"Línea {i + 1}: ";
            switch (t[0].ToUpperInvariant())
            {
                case "NODOS":
                    if (nombres != null) throw new MatrizInvalidaException(pre + "NODOS aparece dos veces.");
                    nombres = t.Skip(1).ToArray();
                    if (nombres.Length < GrafoCompleto.MinNodos || nombres.Length > GrafoCompleto.MaxNodos)
                        throw new MatrizInvalidaException(
                            pre + $"Se requieren entre {GrafoCompleto.MinNodos} y {GrafoCompleto.MaxNodos} puntos.");
                    filas = new int[nombres.Length][];
                    break;

                case "FILA":
                    if (nombres == null || filas == null)
                        throw new MatrizInvalidaException(pre + "Debe declarar NODOS primero.");
                    if (t.Length != nombres.Length + 2)
                        throw new MatrizInvalidaException(pre + $"FILA debe tener el nombre y {nombres.Length} valores.");
                    int idx = Array.FindIndex(nombres, s => string.Equals(s, t[1], StringComparison.OrdinalIgnoreCase));
                    if (idx < 0) throw new MatrizInvalidaException(pre + $"Punto desconocido: {t[1]}");
                    if (filas[idx] != null) throw new MatrizInvalidaException(pre + $"FILA repetida para {t[1]}");
                    var valores = new int[nombres.Length];
                    for (int k = 0; k < nombres.Length; k++)
                        if (!int.TryParse(t[k + 2], out valores[k]))
                            throw new MatrizInvalidaException(pre + $"Valor no entero: {t[k + 2]}");
                    filas[idx] = valores;
                    break;

                default:
                    throw new MatrizInvalidaException(pre + $"Instrucción desconocida: {t[0]}");
            }
        }

        if (nombres == null || filas == null)
            throw new MatrizInvalidaException("El archivo no declara NODOS.");

        int n = nombres.Length;
        var matriz = new int[n, n];
        for (int i = 0; i < n; i++)
        {
            int[]? fila = filas[i];
            if (fila == null) throw new MatrizInvalidaException($"Falta la FILA de {nombres[i]}.");
            for (int j = 0; j < n; j++) matriz[i, j] = fila[j];
        }

        try
        {
            return new GrafoCompleto(nombres, matriz);
        }
        catch (ArgumentException e)
        {
            throw new MatrizInvalidaException(e.Message);
        }
    }
}
