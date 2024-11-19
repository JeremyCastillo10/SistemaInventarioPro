using InventarioPro.Server.DAL;
using InventarioPro.Server.Models;
using InventarioPro.Shared.DTOS.Categoria;
using InventarioPro.Shared.DTOS.Producto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventarioPro.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly Contexto _db;
        public CategoriaController(Contexto db)
        {
            _db = db;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categoria_DTO>>> GetAllArticulos()
        {
            var articulos = await _db.Categorias
                .Where(a => a.Eliminado == false)
                .Select(a => new Categoria_DTO
                {
                    Id = a.Id,
                    Nombre = a.Nombre,
                })
                .ToListAsync();

            return Ok(articulos);
        }
        [HttpPost("Savecategoria")]

        public async Task<ActionResult<int>> GuardarCategoria([FromBody] Categoria_DTO categoria_DTO)
        {
            
            


            if (categoria_DTO.Id > 0)
            {
                var categoria = await _db.Categorias.FindAsync(categoria_DTO.Id);
                categoria.Nombre = categoria_DTO.Nombre;
                categoria.FechaActualizacion = DateTime.Now;

                await _db.SaveChangesAsync();

            }
            else
            {
                var categoria = new Categoria();

                categoria.Nombre = categoria_DTO.Nombre;
                categoria.FechaCreacion = DateTime.Now;
                categoria.Eliminado = false;

                _db.Categorias.Add(categoria);
                await _db.SaveChangesAsync();
            }
            return 0;
        }
    }
}
