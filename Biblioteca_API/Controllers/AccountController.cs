using Biblioteca_API.DTOs;
using BookApplication.Services;
using Dominio_API.Clases;
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
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            try
            {
                var result = await _authService.Login(loginDto.Email, loginDto.Password, loginDto.RemenberMe);

                if (string.IsNullOrEmpty(result))
                {
                    return Unauthorized(new { message = "Credenciales incorrectas." });
                }

                return Ok(new { Token = result });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = "Credenciales incorrectas." });
            }
        }
    }
}