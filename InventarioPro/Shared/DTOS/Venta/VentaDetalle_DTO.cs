using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioPro.Shared.DTOS.Venta
{
    public class VentaDetalle_DTO
    {
        public int Id { get; set; }
        public int IdPresentacion { get; set; }

        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public bool Eliminado { get; set; } = false;
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public decimal? SubMontoTotal { get; set; }
        public decimal? MontoTotal { get; set; }
    }
}
