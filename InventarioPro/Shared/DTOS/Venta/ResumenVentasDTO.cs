using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioPro.Shared.DTOS.Venta
{
    public class ResumenVentasDTO
    {
        public int TotalVentas { get; set; }
        public int NumeroDeTransacciones { get; set; }
        public decimal? PromedioPorVenta { get; set; }
        public int VentasCanceladas { get; set; }
        public int Devoluciones { get; set; }
        public decimal? UltimaVenta { get; set; }
    }
}
