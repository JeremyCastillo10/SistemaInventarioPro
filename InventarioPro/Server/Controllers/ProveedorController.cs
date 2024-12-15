using InventarioPro.Server.DAL;
using InventarioPro.Server.Models;
using InventarioPro.Shared.DTOS.Categoria;
using InventarioPro.Shared.DTOS.Proveedor;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventarioPro.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProveedorController : ControllerBase
    {
        private readonly Contexto _db;
        public ProveedorController(Contexto db)
        {
            _db = db;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Proveedor_DTO>>> GetAllProveedores()
        {
            var articulos = await _db.Proveedores
                .Where(a => a.Eliminado == false)
                .Select(a => new Proveedor_DTO
                {
                    Id = a.Id,
                    Nombre = a.Nombre,
                    Apellido = a.Apellido,
                    Direccion = a.Direccion,
                    Email = a.Email,
                    Telefono = a.Telefono,
                    Celular = a.Celular,
                    Eliminado = a.Eliminado,
                    FechaActualizacion = a.FechaActualizacion,
                    FechaCreacion = a.FechaCreacion,
                })
                .ToListAsync();

            return Ok(articulos);
        }
        [HttpPost("Guardar")]
        public async Task<ActionResult> CrearProveedor(Proveedor_DTO proveedor)
        {
            if (proveedor.Id == 0)
            {
                var prov = new Proveedor
                {
                    Nombre = proveedor.Nombre,
                    Apellido = proveedor.Apellido,
                    Direccion = proveedor.Direccion,
                    Telefono = proveedor.Telefono,
                    Celular = proveedor.Celular,
                    Email = proveedor.Email,
                    FechaCreacion = DateTime.Now,
                    FechaActualizacion = DateTime.Now,
                    Eliminado = false
                };

                try
                {
                    _db.Add(prov);  
                    await _db.SaveChangesAsync();  
                    return Ok(prov);  
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);  
                }
            }
            else
            {
                var proveedorExiste = await _db.Proveedores.FirstOrDefaultAsync(p => p.Id == proveedor.Id);
                if (proveedorExiste == null)
                {
                    return NotFound("Proveedor no encontrado.");
                }

                proveedorExiste.Nombre = proveedor.Nombre;
                proveedorExiste.Apellido = proveedor.Apellido;
                proveedorExiste.Direccion = proveedor.Direccion;
                proveedorExiste.Telefono = proveedor.Telefono;
                proveedorExiste.Celular = proveedor.Celular;
                proveedorExiste.Email = proveedor.Email;
                proveedorExiste.FechaActualizacion = DateTime.Now;  
                proveedorExiste.Eliminado = proveedor.Eliminado ?? false;  

                try
                {
                    await _db.SaveChangesAsync();  
                    return Ok(proveedorExiste); 
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);  
                }
            }
        }
        // Eliminar proveedor (soft delete)
        [HttpDelete("Eliminar/{id}")]
        public async Task<ActionResult> EliminarProveedor(int id)
        {
            var proveedor = await _db.Proveedores.FirstOrDefaultAsync(p => p.Id == id);
            if (proveedor == null)
            {
                return NotFound("Proveedor no encontrado.");
            }

            proveedor.Eliminado = true;
            proveedor.FechaActualizacion = DateTime.Now;

            try
            {
                await _db.SaveChangesAsync();
                return Ok("Proveedor eliminado correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al eliminar el proveedor: {ex.Message}");
            }
        }

        // Obtener proveedor por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Proveedor_DTO>> GetProveedorById(int id)
        {
            var proveedor = await _db.Proveedores
                .Where(p => p.Id == id && p.Eliminado == false)
                .Select(p => new Proveedor_DTO
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Apellido = p.Apellido,
                    Direccion = p.Direccion,
                    Email = p.Email,
                    Telefono = p.Telefono,
                    Celular = p.Celular,
                    Eliminado = p.Eliminado,
                    FechaCreacion = p.FechaCreacion,
                    FechaActualizacion = p.FechaActualizacion,
                })
                .FirstOrDefaultAsync();

            if (proveedor == null)
            {
                return NotFound("Proveedor no encontrado.");
            }

            return Ok(proveedor);
        }

    }
}
