using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioPro.Shared.DTOS.Cuenta
{

        public class CrearRolDTO
        {
            public string NombreRol { get; set; }
            public string Permisos { get; set; } // Permisos separados por coma
        }
    
}
