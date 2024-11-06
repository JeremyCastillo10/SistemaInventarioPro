namespace InventarioPro.Server.Models
{
    public class EntradaDetalle
    {
        public int Id { get; set; }
        public int EntradaId { get; set; }
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public bool? Eliminado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}
