using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioPro.Shared.DTOS.Producto
{
    public class ResumenInventarioDTO
    {
        public int TotalProducts { get; set; }
        public int InStock { get; set; }
        public int OutOfStock { get; set; }
        public decimal ?TotalInventoryValue { get; set; } // Valor total del inventario
        public decimal ?AveragePrice { get; set; } // Precio promedio
        public int TotalCategories { get; set; } // Total de categorías
        public string ?UltimoProducto { get; set; } 
    }
}
