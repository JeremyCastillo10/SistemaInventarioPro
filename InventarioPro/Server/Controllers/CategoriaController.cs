using InventarioPro.Server.DAL;
using InventarioPro.Shared.DTOS.Categoria;
using Microsoft.AspNetCore.Http;
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
        // GET: api/Articulos
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
    }
}
