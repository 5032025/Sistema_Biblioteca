using Dominio_API.Clases;
using Dominio_API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura_API.Persistencia.Repositorios
{
    public class AutorRepositorio : RepositorioBase<Autor>, IAutor
    {
        public AutorRepositorio(AppDbContext context) : base(context) { }

        public async Task AddBookToAuthorAsync(AutorLibro autorLibro)
        {
            await _context.Set<AutorLibro>().AddAsync(autorLibro);
            await _context.SaveChangesAsync();
        }

        public async Task<Autor?> GetAuthorWithBooks(int id)
        {
            return await _context.Autores
                .Include(a => a.LibrosAutor)
                .ThenInclude(ba => ba.Libro)
                .FirstOrDefaultAsync(a => a.Id == id);
        }
    }
}