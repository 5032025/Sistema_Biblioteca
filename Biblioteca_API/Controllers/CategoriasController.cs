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
            if (categoria == null) return NotFound();
            return Ok(categoria);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CategoriaDto dto)
        {
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
            var categoria = new Categoria
            {
                Name = dto.Name,
                Description = dto.Description
            };
            var actualizada = await _categoriaServicio.UpdateCategoriaAsync(id, categoria);
            if (actualizada == null) return NotFound();
            return Ok(actualizada);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var eliminada = await _categoriaServicio.DeleteCategoriaAsync(id);
            if (!eliminada) return NotFound();
            return NoContent();
        }
    }
}