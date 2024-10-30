namespace InventarioPro.Server.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Categoria { get; set; }
        public decimal Precio { get; set; }
        public decimal Costo { get; set; }
        public int Existencia { get; set; }
        public string Codigo { get; set; }
        public bool Eliminado { get; set; }
    }
}
