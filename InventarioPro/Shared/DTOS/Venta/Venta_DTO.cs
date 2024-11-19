using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioPro.Shared.DTOS.Venta
{
        public class Venta_DTO
        {
                public int Id { get; set; }
                public DateTime Fecha { get; set; }
                public decimal MontoTotal { get; set; }
                public string Nombre { get; set; }
                public string Cedula { get; set; }
                public DateTime FechaCreacion { get; set; } = DateTime.Now;
                public DateTime FechaActualizacion { get; set; }
                public List<VentaDetalle_DTO> VentaDetalle_DTOs { get; set; } = new List<VentaDetalle_DTO>();
                public bool Eliminado { get; set; } = false;




        }
}
