﻿using System;
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
        // Estadísticas y Reportes
        public bool VerEstadistica { get; set; }
        public bool VerReportes { get; set; }
        public bool ExportalExcel { get; set; }
        public bool ExportalPdf { get; set; }

        // Entrada
        public bool CrearEntrada { get; set; }
        public bool EditarEntrada { get; set; }
        public bool VerEntrada { get; set; }
        public bool EliminarEntrada { get; set; }

        // Venta
        public bool CrearVenta { get; set; }
        public bool EditarVenta { get; set; }
        public bool VerVenta { get; set; }
        public bool EliminarVenta { get; set; }

        // Producto
        public bool CrearProducto { get; set; }
        public bool VerProducto { get; set; }
        public bool EditarProducto { get; set; }
        public bool EliminarProducto { get; set; }

        // Categoría
        public bool CrearCategoria { get; set; }
        public bool VerCategoria { get; set; }
        public bool EditarCategoria { get; set; }
        public bool EliminarCategoria { get; set; }

        // Usuario
        public bool CrearUsuario { get; set; }
        public bool VerUsuario { get; set; }
        public bool EditarUsuario { get; set; }
        public bool EliminarUsuario { get; set; }

        // Suplidor
        public bool CrearSuplidor { get; set; }
        public bool VerSuplidor { get; set; }
        public bool EditarSuplidor { get; set; }
        public bool EliminarSuplidor { get; set; }

        //Roles
        public bool VerRoles { get; set; }
        public bool CrearRoles { get; set; }
        public bool EditarRoles { get; set; }
        public bool EliminarRoles { get; set; }

        //Roles
        public bool VerEmpresa { get; set; }
        public bool CrearEmpresa { get; set; }
        public bool EditarEmpresa { get; set; }

        //Presentacion
        public bool VerPresentacion { get; set; }
        public bool CrearPresentacion { get; set; }
        public bool EditarPresentacion { get; set; }
        public bool EliminarPresentacion { get; set; }


    }
}
