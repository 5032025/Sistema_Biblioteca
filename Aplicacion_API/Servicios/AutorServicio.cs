using Dominio_API.Clases;
using Dominio_API.Interfaces;

namespace Aplicacion_API.Servicios
{
    public class AutorServicio
    {
        private readonly IAutor _autorRepositorio;

        public AutorServicio(IAutor autorRepositorio)
        {
            _autorRepositorio = autorRepositorio;
        }

        public async Task<IEnumerable<Autor>> GetAllAutoresAsync(int page = 1, string search = "")
        {
            var query = await _autorRepositorio.GetAll(
                x => !x.IsDeleted && (string.IsNullOrEmpty(search) || x.Name.Contains(search)),
                page,
                10,
                search
            );
            return query.ToList();
        }

        public async Task<Autor?> GetAutorByIdAsync(int id)
        {
            return await _autorRepositorio.GetAuthorWithBooks(id);
        }

        public async Task<Autor?> CreateAutorAsync(Autor autor)
        {
            return await _autorRepositorio.AddAsync(autor);
        }

        public async Task<Autor?> UpdateAutorAsync(int id, Autor autor)
        {
            return await _autorRepositorio.UpdateAsync(id, autor);
        }

        public async Task<bool> DeleteAutorAsync(int id)
        {
            return await _autorRepositorio.Delete(id);
        }
    }
}