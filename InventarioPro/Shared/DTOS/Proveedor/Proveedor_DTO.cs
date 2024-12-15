using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioPro.Shared.DTOS.Proveedor
{
    public class Proveedor_DTO
    {
        public int Id { get; set; }
        public string? Nombre { get; set; } = string.Empty;
        public string? Apellido { get; set; } = string.Empty;
        public string? Direccion { get; set; } = string.Empty;
        public string? Telefono { get; set; } = string.Empty;
        public string? Celular { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaActualizacion { get; set; }
        public bool? Eliminado { get; set; } = false;
    }
}
