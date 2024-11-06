namespace InventarioPro.Server.Models
{
        public class Categoria
        {
                public int Id { get; set; }
                public string? Nombre { get; set; }
                public bool? Eliminado { get; set; }
                public DateTime FechaCreacion { get; set; }
                public DateTime FechaActualizacion { get; set; }
        }
}
