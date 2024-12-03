using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Required(ErrorMessage = "El nombre del cliente es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;
        [Required(ErrorMessage = "La cédula del cliente es obligatoria.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "La cédula debe tener exactamente 11 caracteres.")]
        public string Cedula { get; set; } = string.Empty;
                public DateTime FechaCreacion { get; set; } = DateTime.Now;
                public DateTime FechaActualizacion { get; set; }
                public List<VentaDetalle_DTO> VentaDetalle_DTOs { get; set; } = new List<VentaDetalle_DTO>();
                public bool Eliminado { get; set; } = false;




        }
}
