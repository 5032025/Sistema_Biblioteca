using Aplicacion_API.Servicios;
using Biblioteca_API.DTOs;
using Dominio_API.Clases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly CategoriaServicio _categoriaServicio;

        public CategoriasController(CategoriaServicio categoriaServicio)
        {
            _categoriaServicio = categoriaServicio;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] string search = "")
        {
            var categorias = await _categoriaServicio.GetAllCategoriasAsync(page, search);
            return Ok(categorias);
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            var categoria = await _categoriaServicio.GetCategoriaByNameAsync(name);
            if (categoria == null) return NotFound(new { message = $"La categoría '{name}' no existe." });
            return Ok(categoria);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CategoriaDto dto)
        {
            // Validar si ya existe una categoría con el mismo nombre para evitar duplicados
            var categoriaExistente = await _categoriaServicio.GetCategoriaByNameAsync(dto.Name);
            if (categoriaExistente != null)
            {
                return Conflict(new { message = "Ya existe una categoría registrada con ese mismo nombre." });
            }

            var categoria = new Categoria
            {
                Name = dto.Name,
                Description = dto.Description
            };

            var creada = await _categoriaServicio.CreateCategoriaAsync(categoria);
            return Ok(creada);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, CategoriaDto dto)
        {
           
            var actualizada = await _categoriaServicio.UpdateCategoriaAsync(id, new Categoria
            {
                Name = dto.Name,
                Description = dto.Description
            });

            if (actualizada == null)
            {
                return NotFound(new { message = $"No se puede actualizar. La categoría con ID {id} no existe." });
            }

            return Ok(actualizada);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var eliminada = await _categoriaServicio.DeleteCategoriaAsync(id);
            if (!eliminada)
            {
                return NotFound(new { message = $"No se pudo eliminar. La categoría con ID {id} no existe." });
            }

            return NoContent();
        }
    }
}