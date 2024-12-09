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
        public decimal? TotalProductosVendidos { get; set; }

        public decimal? UltimaVenta { get; set; }
        public decimal? VentasPromedioPorDia { get; set; } // Ventas promedio por día
        public decimal? VentasTotalesUltimoMes { get; set; } // Ventas totales en el último mes
        public decimal? VentasPromedioPorTransaccion { get; set; } // Promedio por transacción
        public decimal? VentasAcumuladasYTD { get; set; } // Ventas acumuladas desde el inicio del año
        public string ProductoMasVendido { get; set; } // Resumen de ventas por producto más vendido
        public List<VentaDetalle_DTO> Ventas { get; set; }

    }
}
