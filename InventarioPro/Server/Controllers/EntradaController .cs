﻿using InventarioPro.Server.DAL;
using InventarioPro.Server.Models;
using InventarioPro.Shared.DTOS.Entrada;
using InventarioPro.Shared.DTOS.Producto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace InventarioPro.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntradaController : ControllerBase
    {
        private readonly Contexto _db;
        public EntradaController(Contexto db)
        {
            _db = db;

        }

        [HttpPost]
        public async Task<ActionResult<int>> GuardarEntrada([FromBody] Entrada_DTO entrada_DTO)
        {
            // var entradareal = new Entrada();
            // entradareal.Fecha = entrada.Fecha;
            // entradareal.MontoTotal = entrada.MontoTotal;
            // _db.Entradas.Add(entradareal);
            // _db.SaveChanges();
            // return 0;
            try
            {


                var entrada = new Entrada();
                entrada.Fecha = entrada_DTO.Fecha;
                entrada.MontoTotal = entrada_DTO.MontoTotal;

                List<EntradaDetalle> listEntradadestalle = new List<EntradaDetalle>();
                foreach (var item in entrada_DTO.entradaDetalle_DTO)
                {
                    listEntradadestalle.Add(new EntradaDetalle
                    {
                        EntradaId = item.EntradaId,
                        IdProducto = item.IdProducto,
                        Cantidad = item.Cantidad,
                        Precio = item.Precio,

                    });

                }

                entrada.entradaDetalle = listEntradadestalle;
                _db.Entradas.Add(entrada);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Aquí puedes loggear el error o revisarlo en tiempo de ejecución
                Console.WriteLine(ex.Message);
                return -1; // O cualquier código de error que prefieras
            }
            return 0;
        }

        [HttpPut("Modificarproducto")]
        public async Task<ActionResult<int>> Modificarproducto([FromBody] List<EntradaDetalle_DTO> entradaDetalle_DTO)
        {
            try
            {

                    foreach (var item in entradaDetalle_DTO)
                    {
                        var producto = await _db.Productos.FindAsync(item.IdProducto);
                        producto.Existencia += item.Cantidad;
                        producto.FechaActualizacion = DateTime.Now;
                    }

    
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Aquí puedes loggear el error o revisarlo en tiempo de ejecución
                Console.WriteLine(ex.Message);
                return -1; // O cualquier código de error que prefieras
            }
            return 0;
        }

    }
}
