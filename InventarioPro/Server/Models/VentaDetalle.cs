namespace InventarioPro.Server.Models
{
    public class VentaDetalle
    {
        public int Id { get; set; }
        public int IdPresentacion { get; set; }
        public int Cantidad { get; set; } = 0;
        public decimal ?Precio { get; set; }
        public bool ?Eliminado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }


    }
}
