using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioPro.Shared.DTOS.Cuenta
{
    public class AsignarPermisosDTO
    {
        public string Role { get; set; } // Nombre del rol al que se le asignarán los permisos
        public string Permisos { get; set; } // Permisos separados por coma
    }
}
