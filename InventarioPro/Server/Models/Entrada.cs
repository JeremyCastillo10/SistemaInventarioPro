namespace InventarioPro.Server.Models
{
    public class Entrada
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public decimal ?MontoTotal { get; set; }
        public bool? Eliminado { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}
