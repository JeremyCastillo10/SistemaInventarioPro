using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioPro.Shared.DTOS.Permiso
{
    public class Permiso_DTO
    {
        public int Id { get; set; }
        public string? IdRol { get; set; }
        public string? Nombre { get; set; }
        public bool VerEstadistica { get; set; }
        public bool VerReportes { get; set; }
        public bool ExportalExcel { get; set; }
        public bool ExportalPdf { get; set; }

        public bool CrearEntrada { get; set; }
        public bool EditarEntrada { get; set; }
        public bool VerEntrada { get; set; } 
        public bool EliminarEntrada { get; set; } 

        public bool CrearProducto { get; set; } 
        public bool VerProducto { get; set; } 
        public bool EditarProducto { get; set; } 
        public bool EliminarProducto { get; set; } 

        public bool CrearCategoria { get; set; } 
        public bool VerCategoria { get; set; } 
        public bool EditarCategoria { get; set; } 
        public bool EliminarCategoria { get; set; } 

        public bool CrearUsuario { get; set; } 
        public bool VerUsuario { get; set; }
        public bool EditarUsuario { get; set; }
        public bool EliminarUsuario { get; set; } 

    }
}
