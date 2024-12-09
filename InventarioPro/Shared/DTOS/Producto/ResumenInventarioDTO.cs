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
        public decimal ?TotalInventoryValue { get; set; } 
        public decimal ?AveragePrice { get; set; } 
        public int TotalCategories { get; set; } 
        public string ?UltimoProducto { get; set; }

        // Nuevas propiedades:
        public string? ProductoMayorExistencia { get; set; } // Producto con mayor existencia
        public int ProductosBajoCostoCount { get; set; } // Productos con costo bajo
        public decimal? ValorPromedioStock { get; set; } // Valor promedio de los productos en stock
        public int ProductosNuevosCount { get; set; } // Productos nuevos en el último mes
        public string? ProductoMasCaro { get; set; } // Producto más caro

        // Métricas Predictivas
        public decimal? PrediccionInventarioRiesgo { get; set; } // Predicción de inventario en riesgo de agotarse
        public decimal? PrediccionDemandaProductos { get; set; } // Predicción de demanda de productos
        public decimal? TiempoReposicion { get; set; } // Tiempo estimado para reposición
        public decimal? ValorTotalInventarioFuturo { get; set; } // Valor total del inventario futuro
        public decimal? PrediccionProductosBajoCosto { get; set; } // Predicción de productos de bajo costo en alza
    }
}
