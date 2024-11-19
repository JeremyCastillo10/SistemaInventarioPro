using InventarioPro.Server.DAL;
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
            try
            {
                if (entrada_DTO.Id != 0)
                {
                    var entrada = await _db.Entradas
    .Include(e => e.entradaDetalle)
    .FirstOrDefaultAsync(e => e.Id == entrada_DTO.Id);

                    entrada.Id = entrada_DTO.Id;
                    entrada.Fecha = entrada_DTO.Fecha;
                    entrada.MontoTotal = entrada_DTO.MontoTotal;

                    List<EntradaDetalle> listEntradadestalle = new List<EntradaDetalle>();
                    foreach (var item in entrada_DTO.entradaDetalle_DTO)
                    {
                        listEntradadestalle.Add(new EntradaDetalle
                        {
                            Id = item.Id,
                            EntradaId = item.EntradaId,
                            IdProducto = item.IdProducto,
                            Cantidad = item.Cantidad,
                            Precio = item.Precio,

                        });

                    }

                    entrada.entradaDetalle = listEntradadestalle;

                    await _db.SaveChangesAsync();


                }
                else
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
            }
            catch (Exception ex)
            {
                // Aquí puedes loggear el error o revisarlo en tiempo de ejecución
                Console.WriteLine(ex.Message);
                return -1; // O cualquier código de error que prefieras
            }
            return 0;

        }
        [HttpGet("GetEntrada")]
        public async Task<ActionResult> GetEntrada()
        {
            var entradas = await _db.Entradas.Include(e => e.entradaDetalle) // Asegúrate de incluir los detalles
            .ToListAsync();

            List<Entrada_DTO> listentrada_DTOs = new List<Entrada_DTO>();

            foreach (var entrada in entradas)
            {
                // Crear una nueva instancia de Entrada_DTO para cada entrada
                var entradas_dto = new Entrada_DTO
                {
                    Id = entrada.Id,
                    Fecha = entrada.Fecha,
                    MontoTotal = entrada.MontoTotal ?? 0

                };

                // Crear una nueva lista de EntradaDetalle_DTO para cada entrada
                List<EntradaDetalle_DTO> listEntradadetaller_Dto = new List<EntradaDetalle_DTO>();

                foreach (var entradadetalle in entrada.entradaDetalle)
                {


                    // Crear una nueva instancia de EntradaDetalle_DTO para cada detalle
                    var entradaDetalle_dto = new EntradaDetalle_DTO
                    {
                        Id = entradadetalle.Id,
                        EntradaId = entradadetalle.EntradaId,
                        IdProducto = entradadetalle.IdProducto,
                        Cantidad = entradadetalle.Cantidad,
                        Precio = entradadetalle.Precio,
                        SubMontoTotal = entradadetalle.Cantidad * entradadetalle.Precio,

                    };
                    listEntradadetaller_Dto.Add(entradaDetalle_dto);
                }

                // Asignar la lista de detalles a la propiedad entradaDetalle_DTO del DTO de entrada
                entradas_dto.entradaDetalle_DTO = listEntradadetaller_Dto;

                // Agregar el DTO de entrada a la lista de entradas
                listentrada_DTOs.Add(entradas_dto);
            }

            return Ok(listentrada_DTOs);

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
