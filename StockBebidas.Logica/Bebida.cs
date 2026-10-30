namespace StockBebidas.Logica
{
    public class Bebida
    {
        public string Nombre { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }

        public Bebida()
        {
        }

        public Bebida(string nombre, int cantidad, decimal precio)
        {
            Nombre = nombre;
            Cantidad = cantidad;
            Precio = precio;
        }
    }
}