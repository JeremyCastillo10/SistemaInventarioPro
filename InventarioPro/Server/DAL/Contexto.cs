using InventarioPro.Server.Models;
using Microsoft.AspNetCore.Identity;
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
        public DbSet<ApplicationUser> Usuarios { get; set; }
        public DbSet<Presentacion> Presentaciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Categoria>().HasData(
                 new Categoria { Id = 1, Nombre = "Bebidas", Eliminado= false },
                 new Categoria { Id = 2, Nombre = "Ropa", Eliminado = false },
                 new Categoria { Id = 3, Nombre = "Lacteos", Eliminado = false }
                );
            var adminRoleId = Guid.NewGuid().ToString();
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = adminRoleId,
                    Name = "Administrador",
                    NormalizedName = "ADMINISTRADOR"
                });

            var adminUserId = Guid.NewGuid().ToString();
            var adminEmail = "admin@inventario.com";
            var adminPassword = "Admin123!"; 
            var passwordHasher = new PasswordHasher<IdentityUser>();
            var hashedPassword = passwordHasher.HashPassword(null, adminPassword);

            modelBuilder.Entity<IdentityUser>().HasData(
                new IdentityUser
                {
                    Id = adminUserId,
                    UserName = adminEmail,
                    NormalizedUserName = adminEmail.ToUpper(),
                    Email = adminEmail,
                    NormalizedEmail = adminEmail.ToUpper(),
                    EmailConfirmed = true,
                    PasswordHash = hashedPassword,
                    SecurityStamp = Guid.NewGuid().ToString()
                });

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    UserId = adminUserId,
                    RoleId = adminRoleId
                });

            modelBuilder.Entity<Permiso>().HasData(
                new Permiso
                {
                    Id = 1,
                    IdRol = adminRoleId,
                    VerEstadistica = true,
                    VerReportes = true,
                    ExportalExcel = true,
                    ExportalPdf = true,
                    CrearEntrada = true,
                    EditarEntrada = true,
                    VerEntrada = true,
                    EliminarEntrada = true,
                    CrearProducto = true,
                    VerProducto = true,
                    EditarProducto = true,
                    EliminarProducto = true,
                    CrearCategoria = true,
                    VerCategoria = true,
                    EditarCategoria = true,
                    EliminarCategoria = true,
                    CrearUsuario = true,
                    VerUsuario = true,
                    EditarUsuario = true,
                    EliminarUsuario = true,
                    CrearVenta = true,
                    EditarVenta = true,
                    VerVenta = true,
                    EliminarVenta = true,
                    CrearSuplidor = true,
                    VerSuplidor = true,
                    EditarSuplidor = true,
                    EliminarSuplidor = true,
                    VerRoles = true,
                    CrearRoles = true,
                    EditarRoles = true,
                    EliminarRoles = true,
                    VerEmpresa = true,
                    CrearEmpresa = true,
                    EditarEmpresa = true
                });
        }

    }
}
