using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioPro.Shared.DTOS.Producto
{
        using System.ComponentModel.DataAnnotations;

        public class Producto_DTO
        {
                public int Id { get; set; }

                [Display(Name = "Nombre")]
                [Required(ErrorMessage = "El campo {0} es requerido.")]
                [StringLength(100, ErrorMessage = "El campo {0} no puede exceder {1} caracteres.")]
                public string Nombre { get; set; } = string.Empty;

                [Display(Name = "Descripción")]
                [Required(ErrorMessage = "El campo {0} es requerido.")]
                [StringLength(500, ErrorMessage = "El campo {0} no puede exceder {1} caracteres.")]
                public string Descripcion { get; set; } = string.Empty;

                public string? ImagenProducto { get; set; } = string.Empty;

                [Display(Name = "Categoría")]
                [Required(ErrorMessage = "El campo {0} es requerido.")]
                public int CategoriaId { get; set; } = 0;


                [Display(Name = "Precio")]
                [Required(ErrorMessage = "El campo {0} es requerido.")]
                [Range(0.01, double.MaxValue, ErrorMessage = "El {0} debe ser mayor que cero.")]
                public decimal Precio { get; set; }


                public decimal Costo { get; set; } = 0;

                 public int Existencia { get; set; } = 0;

                [Display(Name = "Código")]
                [Required(ErrorMessage = "El campo {0} es requerido.")]
                [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1} caracteres.")]
                public string Codigo { get; set; }
                public DateTime FechaCreacion { get; set; } = DateTime.Now;
                public DateTime FechaActulizacion { get; set; }

                public bool Eliminado { get; set; } = false;

               public virtual ICollection<Presentacion_DTO> Presentaciones { get; set; } = new List<Presentacion_DTO>();



    }

}
