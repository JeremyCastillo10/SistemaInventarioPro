using InventarioPro.Server.DAL;
using InventarioPro.Server.Models;
using InventarioPro.Shared.DTOS.Categoria;
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

            try
            {
                if (productoDto.Id == 0)
                {
                    // Crear nuevo producto
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
                        ImagenProducto = productoDto.ImagenProducto != null
                            ? Convert.FromBase64String(productoDto.ImagenProducto)
                            : null,
                        // Inicializar lista de presentaciones
                        Presentaciones = productoDto.Presentaciones?.Select(p => new Presentacion
                        {
                            Nombre = p.Nombre,
                            Precio = p.Precio,
                            Cantidad = p.Cantidad ?? 0,
                            Volumen = p.Volumen,
                            UnidadMedida = p.UnidadMedida,
                            Descripcion = p.Descripcion,
                            Eliminado = false,
                            ProductoId = productoDto.Id,  // Asignar el ProductoId
                            Producto = new Producto // Asignar un objeto Producto con solo el Id
                            {
                                Id = productoDto.Id
                            }
                        }).ToList() ?? new List<Presentacion>()
                    };

                    _db.Productos.Add(producto);
                    await _db.SaveChangesAsync();

                    return CreatedAtAction(
                        nameof(GuardarProducto),
                        new { id = producto.Id },
                        new
                        {
                            id = producto.Id,
                            message = "Producto creado exitosamente con sus presentaciones."
                        }
                    );
                }
                else
                {
                    // Actualizar producto existente
                    var productoExistente = await _db.Productos
                        .Include(p => p.Presentaciones)
                        .FirstOrDefaultAsync(p => p.Id == productoDto.Id);

                    if (productoExistente == null)
                    {
                        return NotFound("Producto no encontrado.");
                    }

                    // Actualizar propiedades del producto
                    productoExistente.FechaActualizacion = DateTime.Now;
                    productoExistente.Nombre = productoDto.Nombre;
                    productoExistente.Descripcion = productoDto.Descripcion;
                    productoExistente.CategoriaId = productoDto.CategoriaId;
                    productoExistente.Precio = productoDto.Precio;
                    productoExistente.Costo = productoDto.Costo;
                    productoExistente.Codigo = productoDto.Codigo;
                    productoExistente.ImagenProducto = productoDto.ImagenProducto != null
                        ? Convert.FromBase64String(productoDto.ImagenProducto)
                        : null;

                    if (productoDto.Presentaciones != null)
                    {
                        _db.Presentaciones.RemoveRange(productoExistente.Presentaciones);

                        productoExistente.Presentaciones = productoDto.Presentaciones.Select(p => new Presentacion
                        {
                            Nombre = p.Nombre,
                            Precio = p.Precio,
                            Cantidad = p.Cantidad ?? 0,
                            Volumen = p.Volumen,
                            UnidadMedida = p.UnidadMedida,
                            Descripcion = p.Descripcion,
                            FechaActulizacion = DateTime.Now,
                            ProductoId = productoDto.Id, // Asignar el ProductoId
                            Producto = new Producto // Asignar un objeto Producto con solo el Id
                            {
                                Id = productoDto.Id
                            }
                        }).ToList();
                    }

                    await _db.SaveChangesAsync();
                    return Ok("Producto y presentaciones modificados con éxito.");
                }
            }
            catch (DbUpdateException ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                Console.WriteLine($"Error al procesar producto: {errorMessage}");
                return BadRequest(new { error = errorMessage });
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


            [HttpGet("GetPresentacionById/{id}")]
            public async Task<ActionResult<Presentacion_DTO>> GetPresentacionById(int id)
            {
                try
                {
                    // Buscar la presentación por su ID
                    var presentacion = await _db.Presentaciones
                        .Include(p => p.Producto) // Incluir el producto asociado
                        .FirstOrDefaultAsync(p => p.Id == id);

                    if (presentacion == null)
                    {
                        return NotFound(new { message = "Presentación no encontrada." });
                    }

                    // Devolver el DTO de la presentación con el producto
                    var presentacionDTO = new Presentacion_DTO
                    {
                        Id = presentacion.Id,
                        ProductoId = presentacion.ProductoId,
                        Nombre = presentacion.Nombre,
                        Precio = presentacion.Precio,
                        Cantidad = presentacion.Cantidad,
                        Volumen = presentacion.Volumen,
                        UnidadMedida = presentacion.UnidadMedida,
                        Descripcion = presentacion.Descripcion,
                        Eliminado = presentacion.Eliminado,
                        Producto = new Producto_DTO
                        {
                            Id = presentacion.Producto.Id,
                            Nombre = presentacion.Producto.Nombre,
                            Descripcion = presentacion.Producto.Descripcion,
                          
                            Codigo = presentacion.Producto.Codigo,
                            FechaCreacion = presentacion.Producto.FechaCreacion,

                        }
                    };

                    return Ok(presentacionDTO);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { error = "Ocurrió un error inesperado.", details = ex.Message });
                }
            
        }
   

        [HttpGet("GetProductoId/{id}")]
        public async Task<ActionResult> GetPorId(int id)
        {
            var producto = await (from p in _db.Productos
                                  join c in _db.Categorias on p.CategoriaId equals c.Id
                                  where p.Id == id && p.Eliminado != true
                                  select new Producto_DTO
                                  {
                                      Id = p.Id,
                                      Nombre = p.Nombre,
                                      Descripcion = p.Descripcion,
                                      Precio = Convert.ToDecimal(p.Precio),
                                      Costo = Convert.ToDecimal(p.Costo),
                                      CategoriaId = p.CategoriaId ?? 0,
                                      Existencia = p.Existencia ?? 0,
                                      Codigo = p.Codigo,
                                      ImagenProducto = p.ImagenProducto != null ?
                                          Convert.ToBase64String(p.ImagenProducto) : null,
                                      FechaCreacion = p.FechaCreacion,
                                      FechaActulizacion = p.FechaActualizacion,

                                      // Agregar las presentaciones del producto
                                      Presentaciones = p.Presentaciones
                                          .Where(pr => pr.Eliminado != true) // Filtrar las presentaciones eliminadas si existe esa propiedad
                                          .Select(pr => new Presentacion_DTO
                                          {
                                              Id = pr.Id,
                                              Nombre = pr.Nombre,
                                              Precio = Convert.ToDecimal(pr.Precio),
                                              Volumen = pr.Volumen,
                                              UnidadMedida = pr.UnidadMedida,
                                              Descripcion = pr.Descripcion,
                                              Cantidad = pr.Cantidad
                                          }).ToList()
                                  }).FirstOrDefaultAsync();

            if (producto != null)
                return Ok(producto);

            return NotFound("No se encontró el producto.");
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
            var totalCategories = await _db.Categorias.CountAsync();
            var ultimoProducto = await _db.Productos
                .OrderByDescending(p => p.FechaCreacion)
                .Select(p => p.Nombre)
                .FirstOrDefaultAsync();

            // Nuevas propiedades:
            var productoMayorExistencia = await _db.Productos
                .Where(p => p.Eliminado != true)
                .OrderByDescending(p => p.Existencia)
                .Select(p => p.Nombre)
                .FirstOrDefaultAsync();

            var productosBajoCostoCount = await _db.Productos
                .Where(p => p.Eliminado != true && p.Costo < 10) // Costo menor a 10
                .CountAsync();

            var valorPromedioStock = inStock > 0
                ? await _db.Productos
                    .Where(p => p.Eliminado != true && p.Existencia > 0)
                    .AverageAsync(p => p.Costo * p.Existencia)
                : 0;

            var productosNuevosCount = await _db.Productos
                .Where(p => p.Eliminado != true && p.FechaCreacion > DateTime.Now.AddMonths(-1))
                .CountAsync();

            var productoMasCaro = await _db.Productos
                .Where(p => p.Eliminado != true)
                .OrderByDescending(p => p.Costo)
                .Select(p => p.Nombre)
                .FirstOrDefaultAsync();

            // Métricas Predictivas:
            var prediccionInventarioRiesgo = await _db.Productos
                .Where(p => p.Eliminado != true && p.Existencia <= 5) // Predicción de productos con bajo stock
                .CountAsync();

            var prediccionDemandaProductos = await _db.Productos
                .Where(p => p.Eliminado != true)
                .OrderByDescending(p => p.FechaCreacion)
                .Take(30) // Tomar los últimos 30 productos para estimar demanda
                .AverageAsync(p => p.Existencia);

            var tiempoReposicion = await _db.Productos
                .Where(p => p.Eliminado != true)
                .OrderByDescending(p => p.FechaCreacion)
                .Select(p => p.FechaCreacion)
                .Take(1)
                .FirstOrDefaultAsync();

            var valorTotalInventarioFuturo = await _db.Productos
                .Where(p => p.Eliminado != true)
                .SumAsync(p => p.Costo * p.Existencia);

            var prediccionProductosBajoCosto = await _db.Productos
                .Where(p => p.Eliminado != true && p.Costo < 10)
                .OrderByDescending(p => p.Existencia)
                .Take(5)
                .CountAsync();

            var resumen = new ResumenInventarioDTO
            {
                TotalProducts = totalProducts,
                InStock = inStock,
                OutOfStock = outOfStock,
                TotalInventoryValue = totalValue,
                AveragePrice = averagePrice,
                TotalCategories = totalCategories,
                UltimoProducto = ultimoProducto,
                ProductoMayorExistencia = productoMayorExistencia,
                ProductosBajoCostoCount = productosBajoCostoCount,
                ValorPromedioStock = valorPromedioStock,
                ProductosNuevosCount = productosNuevosCount,
                ProductoMasCaro = productoMasCaro,
                // Nuevas métricas
                PrediccionInventarioRiesgo = prediccionInventarioRiesgo,
                PrediccionDemandaProductos = (decimal?)prediccionDemandaProductos,
                TiempoReposicion = (DateTime.Now - tiempoReposicion).Days, // Ejemplo de cálculo de tiempo de reposición
                ValorTotalInventarioFuturo = valorTotalInventarioFuturo,
                PrediccionProductosBajoCosto = prediccionProductosBajoCosto
            };

            return Ok(resumen);
        }


        [HttpGet("GetPresentaciones")]
        public async Task<ActionResult<IEnumerable<Presentacion_DTO>>> GetPresentaciones()
        {
            var presentaciones = await _db.Presentaciones
                .Where(p => p.Eliminado != true) // Filtrar presentaciones no eliminadas
                .Select(p => new Presentacion_DTO
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Precio = p.Precio.HasValue ? Convert.ToDecimal(p.Precio) : 0,  // Validar null antes de convertir
                    Cantidad = p.Cantidad ?? 0,  // Usar 0 si es null
                    Volumen = p.Volumen ?? 0,  // Usar 0 si es null
                    UnidadMedida = p.UnidadMedida ?? "N/A",  // Usar valor por defecto si es null
                    Descripcion = p.Descripcion,
                    Eliminado = p.Eliminado,

                    // Incluir la información del Producto
                    Producto = new Producto_DTO
                    {
                        Id = p.ProductoId, // Asumir que Presentacion tiene una referencia a Producto
                        Nombre = p.Producto.Nombre,
                        Descripcion = p.Producto.Descripcion,
                        Precio = (decimal)p.Producto.Precio,
                        Costo = (decimal)p.Producto.Costo,
                        Existencia = (int)p.Producto.Existencia,
                        Codigo = p.Producto.Codigo,
                        FechaCreacion = p.Producto.FechaCreacion,
                    }
                })
                .ToListAsync();

            if (presentaciones != null && presentaciones.Count > 0)
                return Ok(presentaciones);

            return NotFound("No se encontraron presentaciones activas.");
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