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
        private readonly CategoriaServicio _categoriaServicio;
        private readonly AutorServicio _autorServicio;

        public LibrosController(
            LibroServicio libroServicio,
            CategoriaServicio categoriaServicio,
            AutorServicio autorServicio)
        {
            _libroServicio = libroServicio;
            _categoriaServicio = categoriaServicio;
            _autorServicio = autorServicio;
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
            if (libro == null) return NotFound(new { message = $"El libro con ID {id} no existe." });
            return Ok(libro);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(LibroDto dto)
        {
            // 0. NUEVO: Validar si ya existe un libro registrado con el mismo título exacto
            var librosExistentes = await _libroServicio.GetAllLibrosAsync(1, dto.Title);
            bool libroDuplicado = librosExistentes.Any(l => l.Title.Equals(dto.Title, StringComparison.OrdinalIgnoreCase));

            if (libroDuplicado)
            {
                return Conflict(new { message = $"Ya existe un libro registrado con el título '{dto.Title}'." });
            }

            // 1. Validar que se proporcione al menos un autor y una categoría
            if (dto.AutorIds == null || !dto.AutorIds.Any())
            {
                return BadRequest(new { message = "El libro debe tener al menos un autor asociado." });
            }

            if (dto.CategoriaIds == null || !dto.CategoriaIds.Any())
            {
                return BadRequest(new { message = "El libro debe tener al menos una categoría asociada." });
            }

            // 2. Validar CADA ID de Autor individualmente
            foreach (var autorId in dto.AutorIds)
            {
                var autorExiste = await _autorServicio.GetAutorByIdAsync(autorId);
                if (autorExiste == null)
                {
                    return BadRequest(new { message = $"El Autor con ID ({autorId}) no existe en el sistema." });
                }
            }

            // 3. Validar CADA ID de Categoría individualmente
            foreach (var categoriaId in dto.CategoriaIds)
            {
                var categoriaExiste = await _categoriaServicio.GetCategoriaByIdAsync(categoriaId);
                if (categoriaExiste == null)
                {
                    return BadRequest(new { message = $"La Categoría con ID ({categoriaId}) no existe en el sistema." });
                }
            }

            var primeraCategoria = await _categoriaServicio.GetCategoriaByIdAsync(dto.CategoriaIds.First());

            var libro = new Libro
            {
                Title = dto.Title,
                Genre = primeraCategoria.Name,
                PublicationYear = dto.PublicationYear,
                Description = dto.Description,
                Status = BookStatus.Available
            };

            var creado = await _libroServicio.CreateLibroAsync(libro);

            foreach (var autorId in dto.AutorIds)
            {
                await _libroServicio.AddAuthorsToBookAsync(new AutorLibro { BookId = creado.Id, AuthorId = autorId });
            }

            foreach (var categoriaId in dto.CategoriaIds)
            {
                await _libroServicio.AddCategoriesToBookAsync(new LibroCategoria { LibroId = creado.Id, CategoriaId = categoriaId });
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
            if (actualizado == null) return NotFound(new { message = $"No se puede actualizar. El libro con ID {id} no existe." });
            return Ok(actualizado);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var eliminado = await _libroServicio.DeleteLibroAsync(id);
            if (!eliminado) return NotFound(new { message = $"No se pudo eliminar. El libro con ID {id} no existe." });
            return NoContent();
        }
    }
}