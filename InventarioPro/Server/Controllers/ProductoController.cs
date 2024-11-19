using InventarioPro.Server.DAL;
using InventarioPro.Server.Models;
using InventarioPro.Shared.DTOS.Categoria;
using InventarioPro.Shared.DTOS.Categoria;
using InventarioPro.Shared.DTOS.Producto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using static System.Runtime.InteropServices.JavaScript.JSType;
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

            if (productoDto.Id == 0)
                if (productoDto.Id == 0)
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
                    var productoExistente = await _db.Productos.FirstOrDefaultAsync(p => p.Id == productoDto.Id);
                    if (productoExistente == null)
                    {
                        return NotFound("Producto no encontrado.");
                    }

                    productoExistente.FechaActualizacion = DateTime.Now;
                    productoExistente.Nombre = productoDto.Nombre;
                    productoExistente.Existencia = productoDto.Existencia;
                    productoExistente.Descripcion = productoDto.Descripcion;
                    productoExistente.CategoriaId = productoDto.CategoriaId;
                    productoExistente.Precio = productoDto.Precio;
                    productoExistente.Costo = productoDto.Costo;
                    productoExistente.ImagenProducto = productoDto.ImagenProducto != null ? Convert.FromBase64String(productoDto.ImagenProducto) : null;
                    var productoExistente = await _db.Productos.FirstOrDefaultAsync(p => p.Id == productoDto.Id);
                    if (productoExistente == null)
                    {
                        return NotFound("Producto no encontrado.");
                    }

                    productoExistente.FechaActualizacion = DateTime.Now;
                    productoExistente.Nombre = productoDto.Nombre;
                    productoExistente.Existencia = productoDto.Existencia;
                    productoExistente.Descripcion = productoDto.Descripcion;
                    productoExistente.CategoriaId = productoDto.CategoriaId;
                    productoExistente.Precio = productoDto.Precio;
                    productoExistente.Costo = productoDto.Costo;
                    productoExistente.ImagenProducto = productoDto.ImagenProducto != null ? Convert.FromBase64String(productoDto.ImagenProducto) : null;

                    try
                    {
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
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    Precio = (Convert.ToDecimal(p.Precio)),
                    Costo = (Convert.ToDecimal(p.Costo)),
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
        [HttpGet("FiltrarPorFechas/{fecha1?}/{fecha2?}")]
        public async Task<ActionResult<List<Producto_DTO>>> FiltrarPorFechas(string fecha1, string fecha2)
        {
            var productos = await _db.Productos
                .Where(p => p.FechaCreacion >= Convert.ToDateTime(fecha1) && p.FechaCreacion <= Convert.ToDateTime(fecha2) && p.Eliminado != true)
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
        [HttpGet("GetProductoId/{id}")]
        public async Task<ActionResult> GetPorId(int id)
        {
            var pro = await (from listaproductos in _db.Productos
                             join cate in _db.Categorias on listaproductos.CategoriaId equals cate.Id
                             where listaproductos.Id == id && listaproductos.Eliminado != true
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
                             }).FirstOrDefaultAsync();

            if (pro != null)
                return Ok(pro);

            return NotFound("No se encontraron productos en esta categoría.");
        }
        [HttpGet("GetResumenInventario")]
        public async Task<ActionResult<ResumenInventarioDTO>> GetResumenInventario()
        {
            var totalProducts = await _db.Productos.CountAsync(p => p.Eliminado != true);
            var inStock = await _db.Productos.CountAsync(p => p.Eliminado != true && p.Existencia > 0);
            var outOfStock = totalProducts - inStock;

            var totalValue = await _db.Productos
                .Where(p => p.Eliminado != true)
                .SumAsync(p => p.Costo * p.Existencia);

            var averagePrice = totalProducts > 0
                ? await _db.Productos
                    .Where(p => p.Eliminado != true)
                    .AverageAsync(p => p.Costo)
                : 0;
            var ultimoProducto = await _db.Productos
                .OrderByDescending(p => p.FechaCreacion)
                .Select(p => p.Nombre)
                .FirstOrDefaultAsync();

            var totalCategories = await _db.Categorias.CountAsync();

            var resumen = new ResumenInventarioDTO
            {
                TotalProducts = totalProducts,
                InStock = inStock,
                OutOfStock = outOfStock,
                TotalInventoryValue = totalValue,
                AveragePrice = averagePrice,
                TotalCategories = totalCategories,
                UltimoProducto = ultimoProducto
            };

            return Ok(resumen);
        }

        [HttpGet("GetProductosPorCategoria")]
        public async Task<ActionResult<IEnumerable<CuentaCategoriaDTO>>> GetProductosPorCategoria()
        {
            var productosPorCategoria = await _db.Productos
                .Where(p => p.Eliminado != true)
                .GroupBy(p => p.CategoriaId)
                .Select(g => new CuentaCategoriaDTO
                {
                    Category = g.Key ?? 0,
                    Count = g.Count()
                }).ToListAsync();

            return Ok(productosPorCategoria);
        }

    }
}
