using InventarioPro.Server.DAL;
using InventarioPro.Server.Models;
using InventarioPro.Shared.DTOS.Producto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventarioPro.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly Contexto _db;

        public ProductoController(Contexto db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<ActionResult<int>> GuardarProducto([FromBody] Producto_DTO productoDto)
        {
            if (productoDto == null)
            {
                return BadRequest("El objeto Producto es nulo.");
            }

            // Log para verificar el contenido del DTO
            Console.WriteLine($"Producto recibido: {productoDto.Nombre}");

            var existingProducto = await _db.Productos
                .FirstOrDefaultAsync(p => p.Codigo == productoDto.Codigo);

            if (existingProducto == null)
            {
                var producto = new Producto
                {
                    FechaCreacion = DateTime.Now,
                    FechaActualizacion = DateTime.Now,
                    Nombre = productoDto.Nombre,
                    Descripcion = productoDto.Descripcion,
                    CategoriaId = productoDto.CategoriaId,
                    Existencia = 0,
                    Eliminado = false,
                    Precio = productoDto.Precio,
                    Costo = productoDto.Costo,
                    Codigo = productoDto.Codigo,
                    ImagenProducto = productoDto.ImagenProducto != null ? Convert.FromBase64String(productoDto.ImagenProducto) : null
                };

                try
                {
                    _db.Productos.Add(producto);
                    await _db.SaveChangesAsync();
                    return CreatedAtAction(nameof(GuardarProducto), new { id = producto.Id }, new { id = producto.Id, message = "Producto creado exitosamente." });
                }
                catch (DbUpdateException ex)
                {
                    var errorMessage = ex.InnerException?.Message ?? ex.Message;
                    Console.WriteLine($"Error al guardar producto: {errorMessage}");
                    return BadRequest(new { error = errorMessage });
                }
            }
            else
            {
                // Actualización de producto existente
                existingProducto.Nombre = productoDto.Nombre;
                existingProducto.Descripcion = productoDto.Descripcion;
                existingProducto.CategoriaId = productoDto.CategoriaId;
                existingProducto.Precio = productoDto.Precio;
                existingProducto.Costo = productoDto.Costo;
                existingProducto.ImagenProducto = productoDto.ImagenProducto != null ? Convert.FromBase64String(productoDto.ImagenProducto) : null;
                existingProducto.FechaActualizacion = DateTime.Now;

                try
                {
                    _db.Productos.Update(existingProducto);
                    await _db.SaveChangesAsync();
                    return Ok("Producto modificado con éxito.");
                }
                catch (DbUpdateException ex)
                {
                    var errorMessage = ex.InnerException?.Message ?? ex.Message;
                    Console.WriteLine($"Error al actualizar producto: {errorMessage}");
                    return BadRequest(new { error = errorMessage });
                }
            }
        }
    }
}
