using Dominio_API.Clases;
using Dominio_API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura_API.Persistencia.Repositorios
{
    public class LibroRepositorio : RepositorioBase<Libro>, ILibro
    {
        public LibroRepositorio(AppDbContext context) : base(context) { }

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