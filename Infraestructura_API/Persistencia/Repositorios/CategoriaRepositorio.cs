using Dominio_API.Clases;
using Dominio_API.Interfaces;
using Microsoft.AspNetCore.Http; 
using Microsoft.EntityFrameworkCore;

namespace Infraestructura_API.Persistencia.Repositorios
{
    public class CategoriaRepositorio : RepositorioBase<Categoria>, ICategoria
    {
        // Constructor con IHttpContextAccessor
        public CategoriaRepositorio(AppDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor) { }

        public async Task<Categoria?> GetCategoryWithBooks(string name)
        {
            return await _context.Categorias
                .Include(c => c.CategoriaLibros)
                .ThenInclude(lc => lc.Libro)
                .FirstOrDefaultAsync(c => c.Name == name);
        }
    }
}