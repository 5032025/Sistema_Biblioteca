using Aplicacion_API.Servicios;
using Biblioteca_API.DTOs;
using Dominio_API.Clases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Biblioteca_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Requiere estar autenticado para realizar reservas y verlas
    public class ReservasController : ControllerBase
    {
        private readonly ReservaServicio _reservaServicio;
        private readonly LibroServicio _libroServicio; 
        public ReservasController(ReservaServicio reservaServicio, LibroServicio libroServicio)
        {
            _reservaServicio = reservaServicio;
            _libroServicio = libroServicio;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyReservas([FromQuery] int page = 1, [FromQuery] string search = "")
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            var isAdmin = User.IsInRole("Admin");

            var reservas = await _reservaServicio.GetAllReservasAsync(page, search, userId, isAdmin);
            return Ok(reservas);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var reserva = await _reservaServicio.GetReservaByIdAsync(id);
            if (reserva == null) return NotFound(new { message = $"La reserva con ID {id} no existe." });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            var isAdmin = User.IsInRole("Admin");

            // Validar que el usuario común solo acceda a su propia reserva
            if (!isAdmin && reserva.UserId != userId)
            {
                return Forbid();
            }

            return Ok(reserva);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ReservaDto dto)
        {
            if (dto.BookIds == null || !dto.BookIds.Any())
            {
                return BadRequest(new { message = "La reserva debe incluir al menos un libro." });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");

            // Validar que cada ID de libro exista en el sistema usando el servicio correcto
            foreach (var bookId in dto.BookIds)
            {
                var libroExiste = await _libroServicio.GetLibroByIdAsync(bookId);
                if (libroExiste == null)
                {
                    return BadRequest(new { message = $"El libro con ID {bookId} no existe en el sistema." });
                }
            }

            var reserva = new Reserva
            {
                UserId = userId,
                ReservationDate = dto.ReservationDate,
                ExpirationDate = dto.ExpirationDate,
                Status = ReserveStatus.Active
            };

            var creada = await _reservaServicio.CreateReservaAsync(reserva);

            foreach (var bookId in dto.BookIds)
            {
                await _reservaServicio.AddBookToReserveAsync(new LibroReserva
                {
                    ReserveId = creada.Id,
                    BookId = bookId
                });
            }

            return Ok(creada);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var eliminada = await _reservaServicio.DeleteReservaAsync(id);
            if (!eliminada) return NotFound(new { message = $"No se pudo eliminar. La reserva con ID {id} no existe." });
            return NoContent();
        }
    }
}