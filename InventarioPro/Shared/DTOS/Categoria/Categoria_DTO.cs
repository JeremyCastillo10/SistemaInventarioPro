using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioPro.Shared.DTOS.Categoria
{
    public class Categoria_DTO
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public bool Eliminado { get; set; } = false;
 
    }
}
