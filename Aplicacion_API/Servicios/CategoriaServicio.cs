using Dominio_API.Clases;
using Dominio_API.Interfaces;

namespace Aplicacion_API.Servicios
{
    public class CategoriaServicio
    {
        private readonly ICategoria _categoriaRepositorio;

        public CategoriaServicio(ICategoria categoriaRepositorio)
        {
            _categoriaRepositorio = categoriaRepositorio;
        }

        public async Task<IEnumerable<Categoria>> GetAllCategoriasAsync(int page = 1, string search = "")
        {
            var query = await _categoriaRepositorio.GetAll(
                x => !x.IsDeleted && (string.IsNullOrEmpty(search) || x.Name.Contains(search)),
                page,
                10,
                search
            );
            return query.ToList();
        }

        public async Task<Categoria?> GetCategoriaByNameAsync(string name)
        {
            return await _categoriaRepositorio.GetCategoryWithBooks(name);
        }

        public async Task<Categoria?> CreateCategoriaAsync(Categoria categoria)
        {
            return await _categoriaRepositorio.AddAsync(categoria);
        }

        public async Task<Categoria?> UpdateCategoriaAsync(int id, Categoria categoria)
        {
            return await _categoriaRepositorio.UpdateAsync(id, categoria);
        }

        public async Task<bool> DeleteCategoriaAsync(int id)
        {
            return await _categoriaRepositorio.Delete(id);
        }

        public async Task<Categoria?> GetCategoriaByIdAsync(int id)
        {
            return await _categoriaRepositorio.FindFirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
