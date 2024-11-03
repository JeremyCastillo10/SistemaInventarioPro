using InventarioPro.Server.DAL;
using InventarioPro.Server.Models;
using InventarioPro.Shared.DTOS.Producto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        [HttpGet("GetProductos")]
        public async Task<ActionResult> getProductos()
        {
            var productos = await _db.Productos
                .Where(p => p.Eliminado != true)
                .Select(p => new Producto_DTO
                {
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    Precio = (Convert.ToDecimal(p.Precio)),
                    Costo = (Convert.ToDecimal(p.Precio)),
                    CategoriaId = p.CategoriaId ?? 0,
                    Existencia = p.Existencia ?? 0,
                    Codigo = p.Codigo,
                    ImagenProducto = p.ImagenProducto != null ? Convert.ToBase64String(p.ImagenProducto) : null,
                    FechaCreacion = p.FechaCreacion,
                    FechaActulizacion = p.FechaActualizacion,
                }).ToListAsync();
            if (productos != null)
                return Ok(productos);
            return BadRequest("Error al obtener");
        }

        [HttpGet("GetPorCategoria/{idCategoria}")]
        public async Task<ActionResult> GetPorCategoria(int idCategoria)
        {
            var pro = await (from listaproductos in _db.Productos
                             join cate in _db.Categorias on listaproductos.CategoriaId equals cate.Id
                             where listaproductos.CategoriaId == idCategoria && listaproductos.Eliminado != true
                             select new Producto_DTO
                             {
                                 Id = listaproductos.Id,
                                 Nombre = listaproductos.Nombre,
                                 Descripcion = listaproductos.Descripcion,
                                 Precio = Convert.ToDecimal(listaproductos.Precio),
                                 Costo = Convert.ToDecimal(listaproductos.Precio),
                                 CategoriaId = listaproductos.CategoriaId ?? 0,
                                 Existencia = listaproductos.Existencia ?? 0,
                                 Codigo = listaproductos.Codigo,
                                 ImagenProducto = listaproductos.ImagenProducto != null ?
                                     Convert.ToBase64String(listaproductos.ImagenProducto) : null,
                                 FechaCreacion = listaproductos.FechaCreacion,
                                 FechaActulizacion = listaproductos.FechaActualizacion,
                             }).ToListAsync();

            if (pro != null && pro.Count > 0)
                return Ok(pro);

            return NotFound("No se encontraron productos en esta categoría.");
        }
        [HttpGet("GetPorNombre/{nombre}")]
        public async Task<ActionResult> GetPorNombre(string nombre)
        {
            var pro = await (from listaproductos in _db.Productos
                             join cate in _db.Categorias on listaproductos.CategoriaId equals cate.Id
                             where listaproductos.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase) && listaproductos.Eliminado != true
                             select new Producto_DTO
                             {
                                 Id = listaproductos.Id,
                                 Nombre = listaproductos.Nombre,
                                 Descripcion = listaproductos.Descripcion,
                                 Precio = Convert.ToDecimal(listaproductos.Precio),
                                 Costo = Convert.ToDecimal(listaproductos.Precio),
                                 CategoriaId = listaproductos.CategoriaId ?? 0,
                                 Existencia = listaproductos.Existencia ?? 0,
                                 Codigo = listaproductos.Codigo,
                                 ImagenProducto = listaproductos.ImagenProducto != null ?
                                     Convert.ToBase64String(listaproductos.ImagenProducto) : null,
                                 FechaCreacion = listaproductos.FechaCreacion,
                                 FechaActulizacion = listaproductos.FechaActualizacion,
                             }).ToListAsync();

            if (pro != null && pro.Count > 0)
                return Ok(pro);

            return NotFound("No se encontraron productos en esta categoría.");
        }
        [HttpGet("FiltrarPorFechas/{fecha1}/{fecha2}")]
        public async Task<ActionResult<List<Producto_DTO>>> FiltrarPorFechas(DateTime fecha1, DateTime fecha2)
        {
            var productos = await _db.Productos
                .Where(p => p.FechaCreacion >= fecha1 && p.FechaCreacion <= fecha2 && p.Eliminado != true)
                .Select(p => new Producto_DTO
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    Precio = Convert.ToDecimal(p.Precio),
                    Costo = Convert.ToDecimal(p.Costo),
                    CategoriaId = p.CategoriaId ?? 0,
                    Existencia = p.Existencia ?? 0,
                    Codigo = p.Codigo,
                    ImagenProducto = p.ImagenProducto != null ? Convert.ToBase64String(p.ImagenProducto) : null,
                    FechaCreacion = p.FechaCreacion,
                    FechaActulizacion = p.FechaActualizacion,
                })
                .ToListAsync();

            if (productos != null && productos.Count > 0)
                return Ok(productos);

            return NotFound("No se encontraron productos en este rango de fechas.");
        }


    }
}
