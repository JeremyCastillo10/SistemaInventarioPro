using InventarioPro.Server.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InventarioPro.Server.DAL
{
    public class Contexto:IdentityDbContext
    {
        public Contexto(DbContextOptions <Contexto> opt) :base(opt)
        {
        }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<VentaDetalle> VentaDetalles { get; set; }
        public DbSet<Entrada> Entradas { get; set; }
        public DbSet<EntradaDetalle> EntradaDetalles { get; set; }

    }
}
