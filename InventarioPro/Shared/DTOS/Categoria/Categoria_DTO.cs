using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioPro.Shared.DTOS.Categoria
{
    public class Categoria_DTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo es obligatorio")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "La longitud debe estar entre 5 y 50 caracteres")]
        public string? Nombre { get; set; }
        public bool Eliminado { get; set; } = false;
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }

    }
}
