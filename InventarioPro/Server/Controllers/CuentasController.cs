using InventarioPro.Shared.DTOs;
using InventarioPro.Shared.DTOS.Cuenta;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InventarioPro.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CuentasController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IConfiguration configuration;

        public CuentasController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
        }

        // Registrar usuario
        [HttpPost("registrar")]
        public async Task<ActionResult<RespuestaAuthenticacion>> Registrar(CredencialesUsuario credencialesUsuario)
        {
            var usuario = new IdentityUser
            {
                UserName = credencialesUsuario.Email,
                Email = credencialesUsuario.Email
            };

            var resultado = await userManager.CreateAsync(usuario, credencialesUsuario.Password);

            if (resultado.Succeeded)
            {
                // Crear un rol "User" por defecto si no existe
                var roleExists = await roleManager.RoleExistsAsync("User");
                if (!roleExists)
                {
                    var role = new IdentityRole("User");
                    await roleManager.CreateAsync(role);
                }

                // Asignar rol "User" al usuario
                await userManager.AddToRoleAsync(usuario, "User");

                // Retornar el token JWT
                return await ConstruirToken(credencialesUsuario);
            }
            else
            {
                return BadRequest(resultado.Errors);
            }
        }

        // Login
        [HttpPost("login")]
        public async Task<ActionResult<RespuestaAuthenticacion>> Login(CredencialesUsuario credencialesUsuario)
        {
            var resultado = await signInManager.PasswordSignInAsync(credencialesUsuario.Email,
                credencialesUsuario.Password,
                isPersistent: false,
                lockoutOnFailure: false);

            if (resultado.Succeeded)
            {
                return await ConstruirToken(credencialesUsuario);
            }
            else
            {
                return BadRequest("Login Incorrecto");
            }
        }

        // Asignar rol a un usuario
        [HttpPost("AsignarRol")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> AsignarRol([FromBody] AsignarRolDTO asignarRolDTO)
        {
            var usuario = await userManager.FindByEmailAsync(asignarRolDTO.Email);
            if (usuario == null)
            {
                return NotFound("Usuario no encontrado");
            }

            // Verificar si el rol existe
            var roleExists = await roleManager.RoleExistsAsync(asignarRolDTO.Role);
            if (!roleExists)
            {
                return BadRequest("Rol no existe");
            }

            // Asignar el rol al usuario
            var result = await userManager.AddToRoleAsync(usuario, asignarRolDTO.Role);
            if (result.Succeeded)
            {
                return NoContent();
            }

            return BadRequest("Error al asignar rol");
        }

        [HttpGet("roles")]
        public IActionResult ObtenerRoles()
        {
            var roles = roleManager.Roles.Select(role => role.Name).ToList();
            return Ok(roles);
        }

        // Asignar permisos a un rol
        [HttpPost("asignarPermisos")]
        public async Task<ActionResult> AsignarPermisos([FromBody] AsignarPermisosDTO asignarPermisosDTO)
        {
            if (string.IsNullOrWhiteSpace(asignarPermisosDTO.Role))
            {
                return BadRequest("El rol es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(asignarPermisosDTO.Permisos))
            {
                return BadRequest("Los permisos son obligatorios.");
            }

            // Obtener el rol
            var role = await roleManager.FindByNameAsync(asignarPermisosDTO.Role);
            if (role == null)
            {
                return BadRequest("El rol especificado no existe.");
            }

            // Asignar los permisos (claims) al rol
            var permisos = asignarPermisosDTO.Permisos.Split(',');
            foreach (var permiso in permisos)
            {
                var claim = new Claim("Permission", permiso.Trim());
                await roleManager.AddClaimAsync(role, claim);
            }

            return NoContent(); // 204 No Content
        }

        // Crear rol con permisos
        [HttpPost("crearRol")]
        public async Task<ActionResult> CrearRol([FromBody] CrearRolDTO crearRolDTO)
        {
            if (string.IsNullOrWhiteSpace(crearRolDTO.NombreRol))
            {
                return BadRequest("El nombre del rol es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(crearRolDTO.Permisos))
            {
                return BadRequest("Los permisos son obligatorios.");
            }

            // Verificar si el rol ya existe
            var roleExistente = await roleManager.RoleExistsAsync(crearRolDTO.NombreRol);
            if (roleExistente)
            {
                return BadRequest("El rol ya existe.");
            }

            // Crear el nuevo rol
            var nuevoRol = new IdentityRole(crearRolDTO.NombreRol);
            var resultado = await roleManager.CreateAsync(nuevoRol);
            if (!resultado.Succeeded)
            {
                return BadRequest("Error al crear el rol.");
            }

            // Asignar los permisos al rol
            var permisos = crearRolDTO.Permisos.Split(',');
            foreach (var permiso in permisos)
            {
                var claim = new Claim("Permission", permiso.Trim());
                await roleManager.AddClaimAsync(nuevoRol, claim);
            }

            return NoContent(); // 204 No Content
        }
        // Generar el token JWT
        private async Task<RespuestaAuthenticacion> ConstruirToken(CredencialesUsuario credencialesUsuario)
        {
            var claims = new List<Claim>
            {
                new Claim("email", credencialesUsuario.Email)
            };

            var usuario = await userManager.FindByEmailAsync(credencialesUsuario.Email);
            var roles = await userManager.GetRolesAsync(usuario);

            // Agregar roles como claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Crear el token JWT
            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtKey"]));
            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);
            var expiracion = DateTime.UtcNow.AddDays(1);

            var securityToken = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: expiracion,
                signingCredentials: creds);

            return new RespuestaAuthenticacion()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                FechaExpiracion = expiracion
            };
        }
    }
}
