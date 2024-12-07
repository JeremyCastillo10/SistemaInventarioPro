using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioPro.Shared.DTOS.Entrada
{
        public class Entrada_DTO
        {

                public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
                public decimal MontoTotal { get; set; }

                public virtual ICollection<EntradaDetalle_DTO> entradaDetalle_DTO { get; set; } = new List<EntradaDetalle_DTO>();
        }
}
