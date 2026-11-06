using Aplicacion_API.Servicios;
using Biblioteca_API.DTOs;
using Dominio_API.Clases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutoresController : ControllerBase
    {
        private readonly AutorServicio _autorServicio;

        public AutoresController(AutorServicio autorServicio)
        {
            _autorServicio = autorServicio;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] string search = "")
        {
            var autores = await _autorServicio.GetAllAutoresAsync(page, search);
            return Ok(autores);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var autor = await _autorServicio.GetAutorByIdAsync(id);
            if (autor == null) return NotFound(new { message = $"El autor con ID {id} no existe." });
            return Ok(autor);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(AutorDto dto)
        {
            // Validar si ya existe un autor con el mismo nombre para evitar duplicados
            var autoresExistentes = await _autorServicio.GetAllAutoresAsync(1, dto.Name);
            bool yaExiste = autoresExistentes.Any(a => a.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase));

            if (yaExiste)
            {
                return Conflict(new { message = "Ya existe un autor registrado con ese mismo nombre." });
            }

            var autor = new Autor
            {
                Name = dto.Name,
                BirthPlace = dto.BirthPlace,
                BirthDate = dto.BirthDate,
                Description = dto.Description,
                Nationality = dto.Nationality
            };

            var creado = await _autorServicio.CreateAutorAsync(autor);

            // Retorna un código 201 Created apuntando al endpoint GetById
            return CreatedAtAction(nameof(GetById), new { id = creado.Id }, creado);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, AutorDto dto)
        {
            // Validar si el autor existe antes de actualizar
            var autorExistente = await _autorServicio.GetAutorByIdAsync(id);
            if (autorExistente == null)
            {
                return NotFound(new { message = $"No se puede actualizar. El autor con ID {id} no existe." });
            }

            var autor = new Autor
            {
                Name = dto.Name,
                BirthPlace = dto.BirthPlace,
                BirthDate = dto.BirthDate,
                Description = dto.Description,
                Nationality = dto.Nationality
            };

            var actualizado = await _autorServicio.UpdateAutorAsync(id, autor);
            if (actualizado == null) return NotFound();

            return Ok(actualizado);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var eliminado = await _autorServicio.DeleteAutorAsync(id);
            if (!eliminado) return NotFound(new { message = $"No se pudo eliminar. El autor con ID {id} no existe." });

            return NoContent();
        }
    }
}