using Dominio_API.Clases;
using Dominio_API.Interfaces;
using Microsoft.AspNetCore.Http; 
using Microsoft.EntityFrameworkCore;

namespace Infraestructura_API.Persistencia.Repositorios
{
    public class LibroRepositorio : RepositorioBase<Libro>, ILibro
    {
       
        public LibroRepositorio(AppDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor) { }

        public async Task AddAuthorsToBookAsync(AutorLibro autorLibro)
        {
            await _context.Set<AutorLibro>().AddAsync(autorLibro);
            await _context.SaveChangesAsync();
        }

        public async Task AddCategoriesToBookAsync(LibroCategoria libroCategoria)
        {
            await _context.Set<LibroCategoria>().AddAsync(libroCategoria);
            await _context.SaveChangesAsync();
        }

        public async Task<Libro?> GetBookWithAuthors(int id)
        {
            return await _context.Libros
                .Include(l => l.AutoresLibro)
                .ThenInclude(al => al.Autor)
                .Include(l => l.LibroCategorias)
                .ThenInclude(lc => lc.Categoria)
                .FirstOrDefaultAsync(l => l.Id == id);
        }
    }
}