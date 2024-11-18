using InventarioPro.Server.DAL;
using InventarioPro.Server.Models;
using InventarioPro.Shared.DTOS.Empresa;
using InventarioPro.Shared.DTOS.Producto;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventarioPro.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpresaController : ControllerBase
    {
        private readonly Contexto _db;

        public EmpresaController(Contexto db)
        {
            _db = db;
        }
        [HttpGet]
        public async Task<ActionResult> getEmpresa()
        {
            var productos = await _db.Empresas
               .Where(p => p.PermitirCargarData != false)
               .Select(p => new Empresa_DTO
               {
                   Id = p.Id,
                   Nombre = p.Nombre,
                   Direccion = p.Direccion,
                   RNC = p.RNC,
                   Telefono = p.Telefono,
                Logo = p.Logo != null ? Convert.ToBase64String(p.Logo) : null,
               }).FirstOrDefaultAsync();
            if (productos != null)
                return Ok(productos);
            return BadRequest("Error al obtener");
        }

        [HttpPost("GuardarEmpresa")]
        public async Task<ActionResult> CrearEmpresa(Empresa_DTO empresa)
        {
            if (empresa.Id == 0)
            {
                var emp = new Empresa
                {
                    Nombre = empresa.Nombre,
                    Direccion = empresa.Direccion,
                    RNC = empresa.RNC,
                    Telefono = empresa.Telefono,
                    PermitirCargarData = true,
                    Logo = empresa.Logo != null ? Convert.FromBase64String(empresa.Logo) : null

                };
                try
                {
                    _db.Add(emp);
                    await _db.SaveChangesAsync();
                    return Ok(emp);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                var empresaExiste = await _db.Empresas.FirstOrDefaultAsync(p => p.Id == empresa.Id);
                if (empresaExiste == null)
                {
                    return NotFound("No encontrado.");
                }
               empresaExiste.Nombre = empresa.Nombre;
               empresaExiste.RNC = empresa.RNC;
               empresaExiste.Direccion = empresa.Direccion;
                empresaExiste.Telefono = empresa.Telefono;
                empresaExiste.PermitirCargarData = true;
               empresaExiste.Logo = empresa.Logo != null ? Convert.FromBase64String(empresa.Logo) : null;
                try
                {
                    await _db.SaveChangesAsync();
                    return Ok(empresaExiste);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }

            }

        }
        [HttpGet("{id}")]
        public async Task<ActionResult> GetEmpresaById(int id)
        {
            // Buscamos la empresa con el ID específico
            var empresa = await _db.Empresas
                .Where(p => p.Id == id && p.PermitirCargarData != false)
                .Select(p => new Empresa_DTO
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Direccion = p.Direccion,
                    RNC = p.RNC,
                    Telefono = p.Telefono,
                    Logo = p.Logo != null ? Convert.ToBase64String(p.Logo) : null
                })
                .FirstOrDefaultAsync();

            // Si la empresa no fue encontrada, retornamos un 404 NotFound
            if (empresa == null)
            {
                return NotFound("Empresa no encontrada.");
            }

            // Si la empresa fue encontrada, la retornamos con un 200 OK
            return Ok(empresa);
        }

    }

}
