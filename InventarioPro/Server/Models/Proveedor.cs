namespace InventarioPro.Server.Models
{
    public class Proveedor
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public string? Celular { get; set; }
        public string? Email { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion{ get; set; }
        public bool? Eliminado { get; set; }
    }

}
