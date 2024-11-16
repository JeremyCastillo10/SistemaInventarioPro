using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioPro.Shared.DTOs
{
    public class RespuestaAuthenticacion
    {
        public string Token { get; set; }
        public DateTime FechaExpiracion { get; set; }
    }
}
