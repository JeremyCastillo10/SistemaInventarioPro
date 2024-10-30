namespace InventarioPro.Server.Models
{
    public class Venta
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public decimal MontoTotal { get; set; }
        public string Nombre { get; set; }
        public string Cedula { get; set; }
    }
}
