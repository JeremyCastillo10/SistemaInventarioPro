namespace InventarioPro.Server.Models
{
    public class Presentacion
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal? Precio { get; set; }
        public int? Cantidad { get; set; }
        public decimal? Volumen { get; set; }
        public string? UnidadMedida { get; set; }
        public string? Descripcion { get; set; }
        public virtual Producto Producto { get; set; } 
        public bool Eliminado { get; set; }
        public DateTime FechaCreacion { get; set; } 
        public DateTime FechaActulizacion { get; set; }
    }
}
