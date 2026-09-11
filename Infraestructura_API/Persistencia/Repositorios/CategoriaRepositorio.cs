using Dominio_API.Clases;
using Dominio_API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura_API.Persistencia.Repositorios
{
    public class CategoriaRepositorio : RepositorioBase<Categoria>, ICategoria
    {
        public CategoriaRepositorio(AppDbContext context) : base(context) { }

        public async Task<Categoria?> GetCategoryWithBooks(string name)
        {
            return await _context.Categorias
                .Include(c => c.CategoriaLibros)
                .ThenInclude(lc => lc.Libro)
                .FirstOrDefaultAsync(c => c.Name == name);
        }
    }
}