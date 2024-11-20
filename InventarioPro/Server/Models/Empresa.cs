namespace InventarioPro.Server.Models
{
    public class Empresa
    {
        public int Id { get; set; }
        public string ?RNC { get; set; }
        public string ?Nombre { get; set; }
        public string ?Direccion { get; set; }
        public string ?Telefono { get; set; }
        public byte[]? Logo { get; set; }
        public bool ?PermitirCargarData { get; set; }
    }
}
