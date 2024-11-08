using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioPro.Shared.DTOS.Empresa
{
    public class Empresa_DTO
    {
        public int Id { get; set; }
        [Display(Name = "RNC")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string RNC { get; set; } = string.Empty;
        [Display(Name = "Empresa")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string Nombre { get; set; } = string.Empty;
        [Display(Name = "Dirección")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string Direccion { get; set; } = string.Empty;
        [Display(Name = "Teléfono")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string Telefono { get; set; } = string.Empty;
        public string Logo { get; set; } = string.Empty;
        public bool PermitirCargaData { get; set; }
    }
}
