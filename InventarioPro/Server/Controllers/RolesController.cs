using InventarioPro.Server.DAL;
using InventarioPro.Shared.DTOS.Permiso;
using InventarioPro.Shared.DTOS.Roles;
using InventarioPro.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace InventarioPro.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly Contexto _db;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public RolesController(Contexto db, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpPost("createRole")]
        public async Task<IActionResult> CreateRole([FromBody] Permiso_DTO permiso_DTO)
        {
            if (string.IsNullOrWhiteSpace(permiso_DTO.Nombre))
                return BadRequest("El nombre del rol no puede estar vacío.");

            if (!await _roleManager.RoleExistsAsync(permiso_DTO.Nombre))
            {
                var resultado = await _roleManager.CreateAsync(new IdentityRole(permiso_DTO.Nombre));
                if (resultado.Succeeded)
                {
                    var rol = await _roleManager.FindByNameAsync(permiso_DTO.Nombre);
                    Permiso permiso = new Permiso
                    {
                        Id = permiso_DTO.Id,
                        IdRol = rol.Id,
                        VerEstadistica = permiso_DTO.VerEstadistica,
                        VerReportes = permiso_DTO.VerReportes,
                        ExportalExcel = permiso_DTO.ExportalExcel,
                        ExportalPdf = permiso_DTO.ExportalPdf,
                        CrearEntrada = permiso_DTO.CrearEntrada,
                        EditarEntrada = permiso_DTO.EditarEntrada,
                        VerEntrada = permiso_DTO.VerEntrada,
                        EliminarEntrada = permiso_DTO.EliminarEntrada,
                        CrearProducto = permiso_DTO.CrearProducto,
                        VerProducto = permiso_DTO.VerProducto,
                        EditarProducto = permiso_DTO.EditarProducto,
                        EliminarProducto = permiso_DTO.EliminarProducto,
                        CrearCategoria = permiso_DTO.CrearCategoria,
                        VerCategoria = permiso_DTO.VerCategoria,
                        EditarCategoria = permiso_DTO.EditarCategoria,
                        EliminarCategoria = permiso_DTO.EliminarCategoria,
                        CrearUsuario = permiso_DTO.CrearUsuario,
                        VerUsuario = permiso_DTO.VerUsuario,
                        EditarUsuario = permiso_DTO.EditarUsuario,
                        EliminarUsuario = permiso_DTO.EliminarUsuario,
                        CrearVenta = permiso_DTO.CrearVenta,
                        EditarVenta = permiso_DTO.EditarVenta,
                        VerVenta = permiso_DTO.VerVenta,
                        EliminarVenta = permiso_DTO.EliminarVenta,
                        CrearSuplidor = permiso_DTO.CrearSuplidor,
                        VerSuplidor = permiso_DTO.VerSuplidor,
                        EditarSuplidor = permiso_DTO.EditarSuplidor,
                        EliminarSuplidor = permiso_DTO.EliminarSuplidor,
                        VerRoles = permiso_DTO.VerRoles,
                        CrearRoles = permiso_DTO.CrearRoles,
                        EditarRoles = permiso_DTO.EditarRoles,
                        EliminarRoles = permiso_DTO.EliminarRoles,
                        VerEmpresa = permiso_DTO.VerEmpresa,
                        CrearEmpresa = permiso_DTO.CrearEmpresa,
                        EditarEmpresa = permiso_DTO.EditarEmpresa
                    };

                    _db.Permisos.Add(permiso);
                    await _db.SaveChangesAsync();
                }
            }

            return Ok("Rol creado exitosamente.");
        }
      
        // Agregar un usuario a un rol específico
        [HttpPost("AddUserToRole")]
        public async Task<IActionResult> AddUserToRole(string userId, string roleName)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(roleName))
                return BadRequest("El ID del usuario y el nombre del rol no pueden estar vacíos.");

            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario == null)
                return NotFound("Usuario no encontrado.");

            var resultadoAgregarRol = await _userManager.AddToRoleAsync(usuario, roleName);
            if (!resultadoAgregarRol.Succeeded)
            {
                // Manejo de errores si agregar el usuario al rol falla
                return BadRequest("Error al agregar el usuario al rol.");
            }

            return Ok("Usuario agregado al rol exitosamente.");
        }

        // Obtener todos los roles de un usuario
        [HttpGet("GetUserRoles/{userId}")]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest("El ID del usuario no puede estar vacío.");

            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario == null)
                return NotFound("Usuario no encontrado.");

            var roles = await _userManager.GetRolesAsync(usuario);
            return Ok(roles);
        }

        // Obtener todos los roles existentes en el sistema
        [HttpGet("GetAllRoles")]
        public IActionResult GetAllRoles()
        {
            List<Roles_DTO> listroles_DTO = new List<Roles_DTO>();

            var roles = _roleManager.Roles.ToList();

            foreach (var role in roles)
            {
                listroles_DTO.Add(new Roles_DTO()
                {
                    Id=role.Id,
                    Nombre = role.Name,
                });
            }

            return Ok(listroles_DTO);
        }
        [HttpGet("getPermisos/{userId}")]
        public async Task<ActionResult<Permiso_DTO>> GetPermisosPorUsuario(string userId)
        {
            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            var roles = await _userManager.GetRolesAsync(usuario);
            if (roles.Count == 0)
            {
                return BadRequest("El usuario no tiene roles asignados.");
            }

            var permisos = await (from rol in _db.Roles
                                  join permiso in _db.Permisos on rol.Id equals permiso.IdRol
                                  where roles.Contains(rol.Name) 
                                  select new
                                  {
                                      rol.Id,
                                      rol.Name,
                                      permiso.VerEstadistica,
                                      permiso.VerReportes,
                                      permiso.ExportalExcel,
                                      permiso.ExportalPdf,
                                      permiso.CrearEntrada,
                                      permiso.EditarEntrada,
                                      permiso.VerEntrada,
                                      permiso.EliminarEntrada,
                                      permiso.CrearVenta,
                                      permiso.EditarVenta,
                                      permiso.VerVenta,
                                      permiso.EliminarVenta,
                                      permiso.CrearProducto,
                                      permiso.VerProducto,
                                      permiso.EditarProducto,
                                      permiso.EliminarProducto,
                                      permiso.CrearCategoria,
                                      permiso.VerCategoria,
                                      permiso.EditarCategoria,
                                      permiso.EliminarCategoria,
                                      permiso.CrearUsuario,
                                      permiso.VerUsuario,
                                      permiso.EditarUsuario,
                                      permiso.EliminarUsuario,
                                      permiso.CrearSuplidor,
                                      permiso.VerSuplidor,
                                      permiso.EditarSuplidor,
                                      permiso.EliminarSuplidor,
                                      permiso.VerRoles,
                                      permiso.CrearRoles,
                                      permiso.EditarRoles,
                                      permiso.EliminarRoles,
                                      permiso.VerEmpresa,
                                      permiso.CrearEmpresa,
                                      permiso.EditarEmpresa
                                  })
                                  .FirstOrDefaultAsync();

            if (permisos == null)
            {
                return NotFound("Permisos no encontrados para el rol asociado.");
            }

            var permisosDto = new Permiso_DTO
            {
                IdRol = permisos.Id,
                Nombre = permisos.Name,
                VerEstadistica = permisos.VerEstadistica,
                VerReportes = permisos.VerReportes,
                ExportalExcel = permisos.ExportalExcel,
                ExportalPdf = permisos.ExportalPdf,
                CrearEntrada = permisos.CrearEntrada,
                EditarEntrada = permisos.EditarEntrada,
                VerEntrada = permisos.VerEntrada,
                EliminarEntrada = permisos.EliminarEntrada,
                CrearVenta = permisos.CrearVenta,
                EditarVenta = permisos.EditarVenta,
                VerVenta = permisos.VerVenta,
                EliminarVenta = permisos.EliminarVenta,
                CrearProducto = permisos.CrearProducto,
                VerProducto = permisos.VerProducto,
                EditarProducto = permisos.EditarProducto,
                EliminarProducto = permisos.EliminarProducto,
                CrearCategoria = permisos.CrearCategoria,
                VerCategoria = permisos.VerCategoria,
                EditarCategoria = permisos.EditarCategoria,
                EliminarCategoria = permisos.EliminarCategoria,
                CrearUsuario = permisos.CrearUsuario,
                VerUsuario = permisos.VerUsuario,
                EditarUsuario = permisos.EditarUsuario,
                EliminarUsuario = permisos.EliminarUsuario,
                CrearSuplidor = permisos.CrearSuplidor,
                VerSuplidor = permisos.VerSuplidor,
                EditarSuplidor = permisos.EditarSuplidor,
                EliminarSuplidor = permisos.EliminarSuplidor,
                VerRoles = permisos.VerRoles,
                CrearRoles = permisos.CrearRoles,
                EditarRoles = permisos.EditarRoles,
                EliminarRoles = permisos.EliminarRoles,
                VerEmpresa = permisos.VerEmpresa,
                CrearEmpresa = permisos.CrearEmpresa,
                EditarEmpresa = permisos.EditarEmpresa
            };

            return Ok(permisosDto);
        }


    }
}
