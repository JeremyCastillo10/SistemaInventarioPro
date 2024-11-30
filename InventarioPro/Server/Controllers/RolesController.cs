﻿using InventarioPro.Server.DAL;
using InventarioPro.Shared.DTOS.Permiso;
using InventarioPro.Shared.DTOS.Roles;
using InventarioPro.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventarioPro.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly Contexto _db;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        // Inyección de dependencias
        public RolesController(Contexto db, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // Crear un nuevo rol si no existe
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
                        EliminarUsuario = permiso_DTO.EliminarUsuario
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
            // Lista para almacenar los roles en formato DTO
            List<Roles_DTO> listroles_DTO = new List<Roles_DTO>();

            // Obtener los roles desde el RoleManager
            var roles = _roleManager.Roles.ToList();

            // Iterar sobre los roles y convertirlos en DTOs
            foreach (var role in roles)
            {
                listroles_DTO.Add(new Roles_DTO()
                {
                    Id=role.Id,
                    Nombre = role.Name,
                });
            }

            // Devolver la lista de roles en formato DTO
            return Ok(listroles_DTO);
        }
    }
}