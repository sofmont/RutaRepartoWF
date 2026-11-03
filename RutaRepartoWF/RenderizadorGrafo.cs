using System.Drawing.Drawing2D;

namespace RutaRepartoWF;

/// <summary>Panel con doble búfer para que el dibujo no parpadee.</summary>
public sealed class PanelDobleBuffer : Panel
{
    public PanelDobleBuffer()
    {
        DoubleBuffered = true;
        ResizeRedraw = true;
    }
}

/// <summary>
/// Dibuja el grafo completo (aristas tenues) y, encima, el recorrido elegido
/// con flechas numeradas en el orden de visita.
/// </summary>
public sealed class RenderizadorGrafo
{
    private const float Radio = 28f;
    private readonly GrafoCompleto _grafo;

    public RenderizadorGrafo(GrafoCompleto grafo)
    {
        _grafo = grafo;
    }

    public void Dibujar(Graphics g, Rectangle area, int[]? tour, Color colorTour, int inicio)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(Color.White);
        if (area.Width < 100 || area.Height < 100) return;

        PointF[] pos = CalcularPosiciones(area);
        using var fuentePeso = new Font("Segoe UI", 7.5f);
        using var fuenteTour = new Font("Segoe UI", 8.5f, FontStyle.Bold);
        using var fuenteNodo = new Font("Segoe UI", 7.5f, FontStyle.Bold);
        using var centrado = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

        // Grafo completo K_n: todas las aristas tenues (si no hay tour, con su peso).
        using (var tenue = new Pen(Color.Gainsboro, 1f))
        {
            for (int i = 0; i < _grafo.N; i++)
                for (int j = i + 1; j < _grafo.N; j++)
                {
                    g.DrawLine(tenue, pos[i], pos[j]);
                    if (tour == null)
                        Etiqueta(g, Medio(pos[i], pos[j]), _grafo.Dist(i, j).ToString(), fuentePeso,
                            Brushes.Gray, centrado);
                }
        }

        // Recorrido con flechas.
        if (tour != null)
        {
            using var pluma = new Pen(colorTour, 3f);
            using var flecha = new AdjustableArrowCap(4f, 5f);
            pluma.CustomEndCap = flecha;
            using var brochaTexto = new SolidBrush(colorTour);

            for (int k = 0; k < tour.Length; k++)
            {
                int a = tour[k], b = tour[(k + 1) % tour.Length];
                (PointF p1, PointF p2) = Recortar(pos[a], pos[b]);
                g.DrawLine(pluma, p1, p2);
                Etiqueta(g, Medio(p1, p2), $"{k + 1}: {_grafo.Dist(a, b)} km", fuenteTour, brochaTexto, centrado);
            }
        }

        for (int i = 0; i < _grafo.N; i++)
        {
            var rect = new RectangleF(pos[i].X - Radio, pos[i].Y - Radio, 2 * Radio, 2 * Radio);
            using (var brocha = new SolidBrush(i == inicio ? Color.Gold : Color.LightSteelBlue))
                g.FillEllipse(brocha, rect);
            using (var borde = new Pen(Color.DarkSlateGray, 2f))
                g.DrawEllipse(borde, rect);
            g.DrawString(_grafo.Nombre(i), fuenteNodo, Brushes.Black, rect, centrado);
        }
    }

    private static PointF Medio(PointF a, PointF b) => new((a.X + b.X) / 2, (a.Y + b.Y) / 2);

    private static (PointF, PointF) Recortar(PointF a, PointF b)
    {
        float dx = b.X - a.X, dy = b.Y - a.Y;
        float largo = MathF.Sqrt(dx * dx + dy * dy);
        if (largo < 1f) return (a, b);
        float ux = dx / largo, uy = dy / largo;
        return (new PointF(a.X + ux * Radio, a.Y + uy * Radio), new PointF(b.X - ux * Radio, b.Y - uy * Radio));
    }

    private static void Etiqueta(Graphics g, PointF centro, string texto, Font fuente, Brush brocha, StringFormat formato)
    {
        SizeF tam = g.MeasureString(texto, fuente);
        var caja = new RectangleF(centro.X - tam.Width / 2, centro.Y - tam.Height / 2, tam.Width, tam.Height);
        g.FillRectangle(Brushes.White, caja);
        g.DrawString(texto, fuente, brocha, caja, formato);
    }

    private PointF[] CalcularPosiciones(Rectangle area)
    {
        int n = _grafo.N;
        var pos = new PointF[n];
        float cx = area.Left + area.Width / 2f;
        float cy = area.Top + area.Height / 2f;
        float r = Math.Min(area.Width, area.Height) / 2f - Radio - 15f;
        for (int i = 0; i < n; i++)
        {
            double ang = -Math.PI / 2 + 2 * Math.PI * i / n;
            pos[i] = new PointF(cx + r * (float)Math.Cos(ang), cy + r * (float)Math.Sin(ang));
        }
        return pos;
    }
}
