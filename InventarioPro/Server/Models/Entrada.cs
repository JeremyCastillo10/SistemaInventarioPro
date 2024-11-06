using System;
using System.Collections.Generic;
namespace InventarioPro.Server.Models
{
    public class Entrada
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public decimal MontoTotal { get; set; }

        public virtual ICollection<EntradaDetalle> entradaDetalle {get; set; }= new List<EntradaDetalle>();
    }
}
