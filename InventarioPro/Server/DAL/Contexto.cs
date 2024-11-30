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
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Empresa> Empresas { get; set; }

        public DbSet<Permiso> Permisos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // Seeding de datos iniciales (opcional)
            modelBuilder.Entity<Categoria>().HasData(
                 new Categoria { Id = 1, Nombre = "Bebidas", Eliminado= false },
                 new Categoria { Id = 2, Nombre = "Ropa", Eliminado = false },
                 new Categoria { Id = 3, Nombre = "Lacteos", Eliminado = false }
                );
        }

    }
}
