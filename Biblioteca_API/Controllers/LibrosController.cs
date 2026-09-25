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
        private readonly CategoriaServicio _categoriaServicio; // 1. Declara el servicio aquí

       
        public LibrosController(LibroServicio libroServicio, CategoriaServicio categoriaServicio)
        {
            _libroServicio = libroServicio;
            _categoriaServicio = categoriaServicio; // 3. Inicialízalo
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
            // 1. Validar que se proporcione al menos un autor
            if (dto.AutorIds == null || !dto.AutorIds.Any())
            {
                return BadRequest("El libro debe tener al menos un autor asociado.");
            }

            // 2. Validar que se proporcione al menos una categoría
            if (dto.CategoriaIds == null || !dto.CategoriaIds.Any())
            {
                return BadRequest("El libro debe tener al menos una categoría asociada.");
            }

            // 3. Opcional: Obtener la primera categoría para asignarla como el "Genre" automáticamente
            // (Asegúrate de inyectar ICategoria o el servicio correspondiente en tu controlador si no lo tienes)
            var primeraCategoria = await _categoriaServicio.GetCategoriaByIdAsync(dto.CategoriaIds.First());

            if (primeraCategoria == null)
            {
                return BadRequest("Una de las categorías proporcionadas no existe.");
            }

            var libro = new Libro
            {
                Title = dto.Title,
                Genre = primeraCategoria.Name, // Se asigna automáticamente el nombre de la categoría
                PublicationYear = dto.PublicationYear,
                Description = dto.Description,
                Status = BookStatus.Available
            };

            // 4. Crear el libro primero para obtener su ID generado
            var creado = await _libroServicio.CreateLibroAsync(libro);

            // 5. Asociar cada autor al libro creado en la tabla intermedia
            foreach (var autorId in dto.AutorIds)
            {
                await _libroServicio.AddAuthorsToBookAsync(new AutorLibro
                {
                    BookId = creado.Id,
                    AuthorId = autorId
                });
            }

            // 6. Asociar cada categoría al libro creado en la tabla intermedia
            foreach (var categoriaId in dto.CategoriaIds)
            {
                await _libroServicio.AddCategoriesToBookAsync(new LibroCategoria
                {
                    LibroId = creado.Id,
                    CategoriaId = categoriaId
                });
            }

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