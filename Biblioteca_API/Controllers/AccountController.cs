using Biblioteca_API.DTOs;
using BookApplication.Services;
using Dominio_API.Clases;
using Infraestructura_API.Seguridad;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        readonly AuthService _authService;

        public AccountController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterUserDto registerUserDto)
        {
            // Corrección en la sintaxis de instanciación e inicializador de objetos
            Usuario newUser = new Usuario
            {
               
                Email = registerUserDto.Email,
                FirstName = registerUserDto.FirstName,
                LastName = registerUserDto.LastName,
                Password = registerUserDto.Password,
                Tel = registerUserDto.Tel
            };

            var result = await _authService.RegisterUser(newUser);

            return Ok("Registro exitoso");
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var result = await _authService.Login(loginDto.Email, loginDto.Password, loginDto.RememberMe);

            if (result == "Credenciales invalidas")
            {
                return Unauthorized(new { message = "Credenciales incorrectas." });
            }

            return Ok(new { Token = result });
        }
    }
}