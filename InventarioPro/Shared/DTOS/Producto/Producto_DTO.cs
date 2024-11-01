using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioPro.Shared.DTOS.Producto
{
    public class Producto_DTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string? ImagenProducto { get; set; }
        public string Categoria { get; set; }
        public decimal Precio { get; set; }
        public decimal Costo { get; set; }
        public int Existencia { get; set; }
        public string Codigo { get; set; }
        public bool Eliminado { get; set; }
    }
}
