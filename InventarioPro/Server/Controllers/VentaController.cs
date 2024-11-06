using InventarioPro.Server.DAL;
using InventarioPro.Server.Models;
using InventarioPro.Shared.DTOS.Venta;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
                    Fecha = ventaDto.Fecha,
                    MontoTotal = ventaDto.MontoTotal,
                    Nombre = ventaDto.Nombre,
                    Cedula = ventaDto.Cedula,
                };

                _db.Ventas.Add(venta);
                await _db.SaveChangesAsync();

                int idVenta = venta.Id;

                try
                {
                    // Agregar detalles de la venta
                    if (ventaDto.VentaDetalle_DTOs != null && ventaDto.VentaDetalle_DTOs.Any())
                    {
                        foreach (var detalle in ventaDto.VentaDetalle_DTOs)
                        {
                            // Crear el detalle de la venta
                            var ventaDetalle = new VentaDetalle
                            {
                                IdProducto = detalle.IdProducto,
                                Cantidad = detalle.Cantidad,
                                Precio = detalle.Precio,
                                IdVenta = idVenta
                            };

                            _db.VentaDetalles.Add(ventaDetalle);

                            // Restar existencia del producto
                            var producto = await _db.Productos.FindAsync(detalle.IdProducto);
                            if (producto != null)
                            {
                                producto.Existencia -= detalle.Cantidad; // Ajusta la propiedad de existencia
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
                // Actualizar venta existente
                var venta = await _db.Ventas.FindAsync(ventaDto.Id);
                if (venta == null)
                {
                    return NotFound("Venta no encontrada.");
                }

                venta.Fecha = ventaDto.Fecha;
                venta.MontoTotal = ventaDto.MontoTotal;
                venta.Nombre = ventaDto.Nombre;
                venta.Cedula = ventaDto.Cedula;

                // Actualizar o agregar detalles de la venta
                foreach (var detalle in ventaDto.VentaDetalle_DTOs)
                {
                    var ventaDetalle = await _db.VentaDetalles
                        .Where(d => d.IdVenta == venta.Id && d.IdProducto == detalle.IdProducto)
                        .FirstOrDefaultAsync();

                    if (ventaDetalle != null)
                    {
                        // Actualizar detalle existente
                        ventaDetalle.Cantidad = detalle.Cantidad;
                        ventaDetalle.Precio = detalle.Precio;

                        // Restar la nueva cantidad del producto
                        var producto = await _db.Productos.FindAsync(detalle.IdProducto);
                        if (producto != null)
                        {
                            producto.Existencia -= (detalle.Cantidad - ventaDetalle.Cantidad); // Ajusta la propiedad de existencia
                            _db.Productos.Update(producto);
                        }
                    }
                    else
                    {
                        // Agregar nuevo detalle
                        ventaDetalle = new VentaDetalle
                        {
                            IdProducto = detalle.IdProducto,
                            Cantidad = detalle.Cantidad,
                            Precio = detalle.Precio,
                            IdVenta = venta.Id
                        };

                        _db.VentaDetalles.Add(ventaDetalle);

                        // Restar existencia del producto
                        var producto = await _db.Productos.FindAsync(detalle.IdProducto);
                        if (producto != null)
                        {
                            producto.Existencia -= detalle.Cantidad; // Ajusta la propiedad de existencia
                            _db.Productos.Update(producto);
                        }
                    }
                }

                await _db.SaveChangesAsync();
                return Ok("Venta modificada con éxito.");
            }
        }

    }

}
