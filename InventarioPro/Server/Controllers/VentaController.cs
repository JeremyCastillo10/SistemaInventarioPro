using InventarioPro.Server.DAL;
using InventarioPro.Server.Models;
using InventarioPro.Shared.DTOS.Venta;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace InventarioPro.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentaController : ControllerBase
    {
        private readonly Contexto _db;

        public VentaController(Contexto db)
        {
            _db = db;
        }

        [HttpPost("Guardar")]
        public async Task<ActionResult<int>> Guardar(Venta_DTO ventaDto)
        {
            if (ventaDto.Id == 0)
            {
                // Crear nueva venta
                var venta = new Venta
                {
                    FechaActualizacion = DateTime.Now,
                    Fecha = DateTime.Now,
                    MontoTotal = ventaDto.MontoTotal,
                    Nombre = ventaDto.Nombre,
                    Cedula = ventaDto.Cedula,
                    Eliminado = false
                };

                _db.Ventas.Add(venta);
                await _db.SaveChangesAsync();

                int idVenta = venta.Id;

                try
                {
                    if (ventaDto.VentaDetalle_DTOs != null && ventaDto.VentaDetalle_DTOs.Any())
                    {
                        foreach (var detalle in ventaDto.VentaDetalle_DTOs)
                        {
                            var ventaDetalle = new VentaDetalle
                            {
                                FechaCreacion = DateTime.Now,
                                FechaActualizacion = DateTime.Now,
                                IdProducto = detalle.IdProducto,
                                Cantidad = detalle.Cantidad,
                                Precio = detalle.Precio,
                                IdVenta = idVenta,
                                Eliminado = false

                            };

                            _db.VentaDetalles.Add(ventaDetalle);

                          
                            var producto = await _db.Productos.FindAsync(detalle.IdProducto);
                            if (producto != null)
                            {
                                producto.Existencia -= detalle.Cantidad; 
                                _db.Productos.Update(producto);
                            }
                        }
                        await _db.SaveChangesAsync();
                    }
                }
                catch (DbUpdateException ex)
                {
                    var errorMessage = ex.InnerException?.Message ?? ex.Message;
                    return BadRequest(new { error = errorMessage });
                }

                return CreatedAtAction(nameof(Guardar), new { id = idVenta }, new { id = idVenta, message = "Venta creada exitosamente." });
            }
            else
            {
                var venta = await _db.Ventas.FindAsync(ventaDto.Id);
                if (venta == null)
                {
                    return NotFound("Venta no encontrada.");
                }

                venta.Fecha = ventaDto.Fecha;
                venta.MontoTotal = ventaDto.MontoTotal;
                venta.Nombre = ventaDto.Nombre;
                venta.Cedula = ventaDto.Cedula;
                venta.FechaActualizacion = DateTime.Now;

                foreach (var detalle in ventaDto.VentaDetalle_DTOs)
                {
                    var ventaDetalle = await _db.VentaDetalles
                        .Where(d => d.IdVenta == venta.Id && d.IdProducto == detalle.IdProducto)
                        .FirstOrDefaultAsync();

                    if (ventaDetalle != null)
                    {
                        ventaDetalle.Cantidad = detalle.Cantidad;
                        ventaDetalle.Precio = detalle.Precio;
                        ventaDetalle.FechaActualizacion = DateTime.Now;
                        ventaDetalle.Eliminado = venta.Eliminado;

                        var producto = await _db.Productos.FindAsync(detalle.IdProducto);
                        if (producto != null)
                        {
                            producto.Existencia -= (detalle.Cantidad - ventaDetalle.Cantidad); 
                            _db.Productos.Update(producto);
                        }
                    }
                    else
                    {
                
                        ventaDetalle = new VentaDetalle
                        {
                            IdProducto = detalle.IdProducto,
                            Cantidad = detalle.Cantidad,
                            Precio = detalle.Precio,
                            IdVenta = venta.Id,
                            FechaActualizacion = DateTime.Now,
                            Eliminado = venta.Eliminado

                        };

                        _db.VentaDetalles.Add(ventaDetalle);

         
                        var producto = await _db.Productos.FindAsync(detalle.IdProducto);
                        if (producto != null)
                        {
                            producto.Existencia -= detalle.Cantidad;
                            _db.Productos.Update(producto);
                        }
                    }
                }

                await _db.SaveChangesAsync();
                return Ok("Venta modificada con éxito.");
            }
        }
        [HttpGet("GetResumenVentas")]
        public async Task<ActionResult<ResumenVentasDTO>> GetResumenVentas()
        {
            var totalVentas = await _db.Ventas.CountAsync(v => v.Eliminado != true);

            var numeroDeTransacciones = await _db.VentaDetalles
                .Where(vd => vd.Eliminado != true)
                .CountAsync();

            var promedioPorVenta = totalVentas > 0
                ? await _db.Ventas
                    .Where(v => v.Eliminado != true && v.MontoTotal.HasValue)
                    .AverageAsync(v => v.MontoTotal.Value)
                : 0;

            var ventasCanceladas = await _db.Ventas.CountAsync(v => v.Eliminado != true && v.MontoTotal == null); // Ejemplo de criterio

            var devoluciones = await _db.VentaDetalles
                .Where(vd => vd.Eliminado != true && vd.Cantidad < 0) // Si la cantidad es negativa, se trata como devolución
                .CountAsync();

            var ultimaVenta = await _db.Ventas
                .Where(v => v.Eliminado != true && v.MontoTotal.HasValue)
                .OrderBy(v => v.Fecha)
                .Select(v => v.MontoTotal)
                .FirstOrDefaultAsync();

            var resumen = new ResumenVentasDTO
            {
                TotalVentas = totalVentas,
                NumeroDeTransacciones = numeroDeTransacciones,
                PromedioPorVenta = promedioPorVenta,
                VentasCanceladas = ventasCanceladas,
                Devoluciones = devoluciones,
                UltimaVenta = ultimaVenta
            };

            return Ok(resumen);
        }

        [HttpGet("GetVentas")]
        public async Task<ActionResult<List<Venta_DTO>>> GetVentas()
        {
            var ventas = await _db.Ventas
                .Where(v => v.Eliminado != true) 
                .ToListAsync();

            var ventasDto = new List<Venta_DTO>();

            foreach (var v in ventas)
            {
                var ventaDto = new Venta_DTO
                {
                    Id = v.Id,
                    Fecha = v.Fecha,
                    MontoTotal = Convert.ToDecimal(v.MontoTotal),
                    Nombre = v.Nombre,
                    Cedula = v.Cedula
                };

                var detalles = await _db.VentaDetalles
                    .Where(d => d.IdVenta == v.Id && d.Eliminado != true)
                    .ToListAsync();

                foreach (var detalle in detalles)
                {
                    ventaDto.VentaDetalle_DTOs.Add(new VentaDetalle_DTO
                    {
                        IdProducto = detalle.IdProducto,
                        Cantidad = detalle.Cantidad,
                        Precio = Convert.ToDecimal(detalle.Precio)
                    });
                }

                ventasDto.Add(ventaDto);
            }

            if (ventasDto != null && ventasDto.Count > 0)
            {
                return Ok(ventasDto);
            }
            return NotFound("No se encontraron ventas.");
        }

        [HttpGet("GetVentaById/{id}")]
        public async Task<ActionResult<Venta_DTO>> GetVentaById(int id)
        {
            var venta = await (from v in _db.Ventas
                               where v.Id == id && v.Eliminado != true  
                               select new Venta_DTO
                               {
                                   Id = v.Id,
                                   Fecha = v.Fecha,
                                   MontoTotal = Convert.ToDecimal(v.MontoTotal),
                                   Nombre = v.Nombre,
                                   Cedula = v.Cedula,
                                   FechaCreacion = v.FechaCreacion,
                                   FechaActualizacion = v.FechaActualizacion,

                                   VentaDetalle_DTOs = (from d in _db.VentaDetalles
                                                        where d.IdVenta == v.Id && d.Eliminado != true
                                                        select new VentaDetalle_DTO
                                                        {
                                                            Id = d.Id,  
                                                            IdProducto = d.IdProducto,  
                                                            Cantidad = d.Cantidad,  
                                                            Precio = Convert.ToDecimal(d.Precio),  
                                                            IdVenta = d.IdVenta,  
                                                            FechaCreacion = d.FechaCreacion,
                                                            FechaActualizacion = d.FechaActualizacion

                                                        }).ToList()
                               }).FirstOrDefaultAsync();

            if (venta == null)
            {
                return NotFound(new { error = "Venta no encontrada." });
            }

            return Ok(venta);
        }
        [HttpPut("EliminarDetalle/{id}")]
        public async Task<IActionResult> EliminarDetalle(int id, [FromBody] bool eliminado)
        {
            try
            {
                var detalle = await _db.VentaDetalles.FirstOrDefaultAsync(d => d.Id == id);

                if (detalle == null)
                {
                    return NotFound(new { error = "Detalle no encontrado." });
                }

                detalle.Eliminado = true;

                _db.VentaDetalles.Update(detalle);
                await _db.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Hubo un problema al eliminar el detalle: " + ex.Message });
            }
        }

    }

}
