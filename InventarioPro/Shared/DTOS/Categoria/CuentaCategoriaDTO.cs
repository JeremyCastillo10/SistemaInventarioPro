using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioPro.Shared.DTOS.Categoria
{
    public class CuentaCategoriaDTO
    {
        public int Category { get; set; } // ID de la categoría
        public string CategoryName { get; set; } // Nombre de la categoría
        public int Count { get; set; }
    }
}
