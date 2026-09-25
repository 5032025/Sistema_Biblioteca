using Dominio_API.Clases;
using Dominio_API.Interfaces;
using Microsoft.AspNetCore.Http; 
using Microsoft.EntityFrameworkCore;

namespace Infraestructura_API.Persistencia.Repositorios
{
    public class AutorRepositorio : RepositorioBase<Autor>, IAutor
    {
       
        public AutorRepositorio(AppDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor) { }

       

        public async Task<Autor?> GetAuthorWithBooks(int id)
        {
            return await _context.Autores
                .Include(a => a.LibrosAutor)
                .ThenInclude(ba => ba.Libro)
                .FirstOrDefaultAsync(a => a.Id == id);
        }
    }
}