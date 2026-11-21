namespace RutaRepartoWF
{
    internal static class Program
    {
        /// <summary>
        ///  Punto de entrada: ruta de reparto (TSP) con vecino más cercano, 2-opt y Held-Karp.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}
