namespace InventarioPro.Server.Models
{
    public class EntradaDetalle
    {
        public int Id { get; set; }
        public int EntradaId { get; set; }
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
    }
}
