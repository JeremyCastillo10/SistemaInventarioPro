using InventarioPro.Server.Models;
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
        public async Task<ActionResult<RespuestaAuthenticacion>> Registrar(UserRegister userRegister)
        {
            var usuario = new ApplicationUser
            {
                UserName = userRegister.Email,
                Email = userRegister.Email,
                FirstName = userRegister.FirstName,
                LastName = userRegister.LastName,
            };

            var resultado = await userManager.CreateAsync(usuario, userRegister.Password);

            if (resultado.Succeeded)
            {
                // Verificar si el rol existe
                var roleExists = await roleManager.RoleExistsAsync(userRegister.RolId);
                if (!roleExists)
                {
                    return BadRequest($"El rol '{userRegister.RolId}' no existe.");
                }

                // Asignar el rol al usuario
                await userManager.AddToRoleAsync(usuario, userRegister.RolId);

                // Crear el objeto UsuarioAutenticado para pasar a la función ConstruirToken
                var usuarioAutenticado = new UsuarioAutenticado
                {
                    Email = userRegister.Email,
                    Password = userRegister.Password
                };

                // Retornar el token JWT
                return await ConstruirToken(usuarioAutenticado);
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
                // Usar un objeto UsuarioAutenticado para construir el token
                var usuarioAutenticado = new UsuarioAutenticado
                {
                    Email = credencialesUsuario.Email,
                    Password = credencialesUsuario.Password
                };

                return await ConstruirToken(usuarioAutenticado);
            }
            else
            {
                return BadRequest("Login Incorrecto");
            }
        }


        // Generar el token JWT
        private async Task<RespuestaAuthenticacion> ConstruirToken(UsuarioAutenticado usuarioAutenticado)
        {
           

            var usuario = await userManager.FindByEmailAsync(usuarioAutenticado.Email);
            var roles = await userManager.GetRolesAsync(usuario);
            var claims = new List<Claim>
            {
                new Claim("email", usuarioAutenticado.Email),
                new Claim(ClaimTypes.NameIdentifier, usuario.Id) // userId aquí

            };
            // Agregar roles como claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Crear el token JWT
            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtKey"]));
            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);
            var expiracion = DateTime.UtcNow.AddMinutes(1);

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

        // Listar todos los usuarios
        [HttpGet("listar")]
        public async Task<ActionResult<List<CredencialesUsuario>>> ListarUsuarios()
        {
            var usuarios = userManager.Users.ToList();

            var usuariosDTO = usuarios.Select(u => new CredencialesUsuario
            {
                Email = u.Email,
            }).ToList();

            return Ok(usuariosDTO);
        }

        // Obtener todos los roles
        [HttpGet("roles")]
        public async Task<ActionResult<List<DropdownOption>>> GetRoles()
        {
            var roles = roleManager.Roles.Select(r => new DropdownOption
            {
                Value = r.Name,
                Text = r.Name
            }).ToList();

            return Ok(roles);
        }

        // Clase para representar las opciones de rol
        public class DropdownOption
        {
            public string Value { get; set; } = string.Empty;
            public string Text { get; set; } = string.Empty;
        }
    }
}
