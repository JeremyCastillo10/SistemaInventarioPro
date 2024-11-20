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
                [Range(typeof(DateTime), "1/2/0001", "12/31/9999", ErrorMessage = "El campo es obligatorio")]
                public DateTime Fecha { get; set; }
                public decimal MontoTotal { get; set; }

                public virtual ICollection<EntradaDetalle_DTO> entradaDetalle_DTO { get; set; } = new List<EntradaDetalle_DTO>();
        }
}
