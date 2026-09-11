using Aplicacion_API.Servicios;
using Biblioteca_API.DTOs;
using Dominio_API.Clases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibrosController : ControllerBase
    {
        private readonly LibroServicio _libroServicio;

        public LibrosController(LibroServicio libroServicio)
        {
            _libroServicio = libroServicio;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] string search = "")
        {
            var libros = await _libroServicio.GetAllLibrosAsync(page, search);
            return Ok(libros);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var libro = await _libroServicio.GetLibroByIdAsync(id);
            if (libro == null) return NotFound();
            return Ok(libro);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(LibroDto dto)
        {
            var libro = new Libro
            {
                Title = dto.Title,
                Genre = dto.Genre,
                PublicationYear = dto.PublicationYear,
                Description = dto.Description,
                Status = BookStatus.Available
            };
            var creado = await _libroServicio.CreateLibroAsync(libro);
            return Ok(creado);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, LibroDto dto)
        {
            var libro = new Libro
            {
                Title = dto.Title,
                Genre = dto.Genre,
                PublicationYear = dto.PublicationYear,
                Description = dto.Description
            };
            var actualizado = await _libroServicio.UpdateLibroAsync(id, libro);
            if (actualizado == null) return NotFound();
            return Ok(actualizado);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var eliminado = await _libroServicio.DeleteLibroAsync(id);
            if (!eliminado) return NotFound();
            return NoContent();
        }
    }
}