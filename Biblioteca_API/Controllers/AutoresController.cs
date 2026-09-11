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
            if (autor == null) return NotFound();
            return Ok(autor);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(AutorDto dto)
        {
            var autor = new Autor
            {
                Name = dto.Name,
                BirthPlace = dto.BirthPlace,
                BirthDate = dto.BirthDate,
                Description = dto.Description,
                Nationality = dto.Nationality
            };
            var creado = await _autorServicio.CreateAutorAsync(autor);
            return Ok(creado);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, AutorDto dto)
        {
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
            if (!eliminado) return NotFound();
            return NoContent();
        }
    }
}
