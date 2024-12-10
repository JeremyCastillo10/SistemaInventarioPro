using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioPro.Shared.DTOS.Producto
{
    public class Presentacion_DTO
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal? Precio { get; set; }
        public int? Cantidad { get; set; }
        public decimal? Volumen { get; set; }
        public string? UnidadMedida { get; set; } = string.Empty;
        public string? Descripcion { get; set; } = string.Empty;
        public bool Eliminado { get; set; } = false;
        public Producto_DTO ?Producto { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaActulizacion { get; set; }

    }
}
