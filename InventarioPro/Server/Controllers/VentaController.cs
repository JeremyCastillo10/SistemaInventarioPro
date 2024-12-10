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
        public async Task<ActionResult<int>> GuardarVenta([FromBody] Venta_DTO venta_DTO)
        {
            try
            {
                // Validar si es una venta existente o nueva
                if (venta_DTO.Id != 0)
                {
                    // Modificar venta existente
                    var venta = await _db.Ventas
                        .Include(v => v.ventaDetalle)
                        .FirstOrDefaultAsync(v => v.Id == venta_DTO.Id);

                    if (venta == null)
                    {
                        return NotFound("Venta no encontrada.");
                    }

                    // Actualizar campos de la venta
                    venta.Fecha = venta_DTO.Fecha;
                    venta.MontoTotal = venta_DTO.MontoTotal;
                    venta.Nombre = venta_DTO.Nombre;
                    venta.Cedula = venta_DTO.Cedula;
                    venta.FechaActualizacion = DateTime.Now;

                    // Actualizar detalles de la venta
                    List<VentaDetalle> listVentaDetalles = new List<VentaDetalle>();
                    foreach (var item in venta_DTO.VentaDetalle_DTOs)
                    {
                        var presentacion = await _db.Presentaciones
                            .Include(p => p.Producto)
                            .FirstOrDefaultAsync(p => p.Id == item.IdPresentacion);

                        if (presentacion == null || presentacion.Producto == null)
                        {
                            return BadRequest(new { error = $"Presentación con ID {item.IdPresentacion} no encontrada." });
                        }

                        // Cálculo correcto: Cantidad total en términos del producto base
                        int cantidadTotal = (int)(item.Cantidad * presentacion.Cantidad);

                        if (cantidadTotal > presentacion.Producto.Existencia)
                        {
                            return BadRequest(new { error = $"La cantidad total excede las existencias del producto." });
                        }

                        // Actualizar existencias
                        presentacion.Producto.Existencia -= cantidadTotal;

                        listVentaDetalles.Add(new VentaDetalle
                        {
                            Id = item.Id,
                            IdPresentacion = item.IdPresentacion,
                            Cantidad = item.Cantidad,
                            Precio = item.Precio,
                        });
                    }

                    venta.ventaDetalle = listVentaDetalles;
                    await _db.SaveChangesAsync();
                }
                else
                {
                    // Crear nueva venta
                    var venta = new Venta
                    {
                        Fecha = venta_DTO.Fecha,
                        MontoTotal = venta_DTO.MontoTotal,
                        Nombre = venta_DTO.Nombre,
                        Cedula = venta_DTO.Cedula,
                        FechaActualizacion = DateTime.Now
                    };

                    List<VentaDetalle> listVentaDetalles = new List<VentaDetalle>();
                    foreach (var item in venta_DTO.VentaDetalle_DTOs)
                    {
                        var presentacion = await _db.Presentaciones
                            .Include(p => p.Producto)
                            .FirstOrDefaultAsync(p => p.Id == item.IdPresentacion);

                        if (presentacion == null || presentacion.Producto == null)
                        {
                            return BadRequest(new { error = $"Presentación con ID {item.IdPresentacion} no encontrada." });
                        }

                        // Cálculo correcto: Cantidad total en términos del producto base
                        int cantidadTotal = (int)(item.Cantidad * presentacion.Cantidad);

                        if (cantidadTotal > presentacion.Producto.Existencia)
                        {
                            return BadRequest(new { error = $"La cantidad total excede las existencias del producto." });
                        }

                        presentacion.Producto.Existencia -= cantidadTotal;

                        listVentaDetalles.Add(new VentaDetalle
                        {
                            IdPresentacion = item.IdPresentacion,
                            Cantidad = item.Cantidad,
                            Precio = item.Precio,
                            Eliminado = false,
                        });
                    }

                    venta.ventaDetalle = listVentaDetalles;
                    _db.Ventas.Add(venta);
                    await _db.SaveChangesAsync();
                }

                return Ok(new { message = "Venta guardada exitosamente." });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, new { error = "Ocurrió un error inesperado." });
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

            var ultimaVenta = await _db.Ventas
                .Where(v => v.Eliminado != true && v.MontoTotal.HasValue)
                .OrderBy(v => v.Fecha)
                .Select(v => v.MontoTotal)
                .FirstOrDefaultAsync();

            // Ventas promedio por día en el mes actual
            var ventasPromedioPorDia = await _db.Ventas
                .Where(v => v.Fecha.Month == DateTime.Now.Month && v.Eliminado != true)
                .SumAsync(v => v.MontoTotal ?? 0) / DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

            // Ventas totales en el último mes
            var ventasTotalesUltimoMes = await _db.Ventas
                .Where(v => v.Fecha.Month == DateTime.Now.AddMonths(-1).Month && v.Eliminado != true)
                .SumAsync(v => v.MontoTotal ?? 0);

            // Promedio de ventas por transacción
            var ventasPromedioPorTransaccion = totalVentas > 0
                ? await _db.Ventas
                    .Where(v => v.Eliminado != true && v.MontoTotal.HasValue)
                    .AverageAsync(v => v.MontoTotal.Value)
                : 0;

            // Ventas acumuladas desde el inicio del año
            var ventasAcumuladasYTD = await _db.Ventas
                .Where(v => v.Fecha.Year == DateTime.Now.Year && v.Eliminado != true)
                .SumAsync(v => v.MontoTotal ?? 0);

            // Producto más vendido (ID del producto más vendido)
            var productoMasVendidoId = await _db.VentaDetalles
                .Where(vd => vd.Eliminado != true)
                .GroupBy(vd => vd.IdPresentacion)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefaultAsync();

            // Obtener el nombre del producto más vendido usando su ID
            var productoMasVendido = await _db.Productos
                .Where(p => p.Id == productoMasVendidoId && p.Eliminado != true)
                .Select(p => p.Nombre)
                .FirstOrDefaultAsync();
            // Total de productos vendidos
            var totalProductosVendidos = await _db.VentaDetalles
                .Where(vd => vd.Eliminado != true)
                .SumAsync(vd => vd.Cantidad);

            var resumen = new ResumenVentasDTO
            {
                TotalVentas = totalVentas,
                NumeroDeTransacciones = numeroDeTransacciones,
                PromedioPorVenta = promedioPorVenta,
                UltimaVenta = ultimaVenta,
                VentasPromedioPorDia = ventasPromedioPorDia,
                VentasTotalesUltimoMes = ventasTotalesUltimoMes,
                VentasPromedioPorTransaccion = ventasPromedioPorTransaccion,
                VentasAcumuladasYTD = ventasAcumuladasYTD,
                TotalProductosVendidos = totalProductosVendidos,
                ProductoMasVendido = productoMasVendido // Aquí se incluye el nombre del producto más vendido
            };

            return Ok(resumen);
        }

        [HttpGet("GetVentas")]
        public async Task<ActionResult<List<Venta_DTO>>> GetVentas()
        {
            var ventas = await _db.Ventas
                .Where(v => v.Eliminado != true)
                .Include(v => v.ventaDetalle.Where(d => d.Eliminado != true)) // Incluir los detalles de la venta
                .ToListAsync();

            var ventasDto = ventas.Select(v => new Venta_DTO
            {
                Id = v.Id,
                Fecha = v.Fecha,
                MontoTotal = Convert.ToDecimal(v.MontoTotal),
                Nombre = v.Nombre,
                Cedula = v.Cedula,
                VentaDetalle_DTOs = v.ventaDetalle.Select(d => new VentaDetalle_DTO
                {
                    IdPresentacion = d.IdPresentacion,
                    Cantidad = d.Cantidad,
                    Precio = Convert.ToDecimal(d.Precio)
                }).ToList()
            }).ToList();

            if (ventasDto != null && ventasDto.Count > 0)
            {
                return Ok(ventasDto);
            }
            return NotFound("No se encontraron ventas.");
        }


        [HttpGet("GetVentaById/{id}")]
        public async Task<ActionResult<Venta_DTO>> GetVentaById(int id)
        {
            var venta = await _db.Ventas
                .Where(v => v.Id == id && v.Eliminado != true)
                .Include(v => v.ventaDetalle.Where(d => d.Eliminado != true)) // Incluir VentaDetalles
                .FirstOrDefaultAsync();

            if (venta == null)
            {
                return NotFound(new { error = "Venta no encontrada." });
            }

            // Proyección de los datos a Venta_DTO
            var ventaDto = new Venta_DTO
            {
                Id = venta.Id,
                Fecha = venta.Fecha,
                MontoTotal = Convert.ToDecimal(venta.MontoTotal),
                Nombre = venta.Nombre,
                Cedula = venta.Cedula,
                FechaCreacion = venta.FechaCreacion,
                FechaActualizacion = venta.FechaActualizacion,
                VentaDetalle_DTOs = venta.ventaDetalle.Select(d => new VentaDetalle_DTO
                {
                    Id = d.Id,
                    IdPresentacion = d.IdPresentacion,
                    Cantidad = d.Cantidad,
                    Precio = Convert.ToDecimal(d.Precio),
                    FechaCreacion = d.FechaCreacion,
                    FechaActualizacion = d.FechaActualizacion
                }).ToList()
            };

            return Ok(ventaDto);
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
