using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioPro.Shared.DTOS.Entrada
{
        public class EntradaDetalle_DTO
        {
                public int Id { get; set; }
                public int EntradaId { get; set; }
                public int IdProducto { get; set; }
                public int Cantidad { get; set; }
                public decimal Costo { get; set; }
                public decimal? SubMontoTotal { get; set; }
        }
}
