using Dominio_API.Clases;
using Dominio_API.Interfaces;

namespace Aplicacion_API.Servicios
{
    public class LibroServicio
    {
        private readonly ILibro _libroRepositorio;

        public LibroServicio(ILibro libroRepositorio)
        {
            _libroRepositorio = libroRepositorio;
        }

        public async Task<IEnumerable<Libro>> GetAllLibrosAsync(int page = 1, string search = "")
        {
            var query = await _libroRepositorio.GetAll(
                x => !x.IsDeleted && (string.IsNullOrEmpty(search) || x.Title.Contains(search)),
                page,
                10,
                search
            );
            return query.ToList();
        }

        public async Task<Libro?> GetLibroByIdAsync(int id)
        {
            return await _libroRepositorio.GetBookWithAuthors(id);
        }

        public async Task<Libro?> CreateLibroAsync(Libro libro)
        {
            return await _libroRepositorio.AddAsync(libro);
        }

        public async Task<Libro?> UpdateLibroAsync(int id, Libro libro)
        {
            return await _libroRepositorio.UpdateAsync(id, libro);
        }

        public async Task<bool> DeleteLibroAsync(int id)
        {
            return await _libroRepositorio.Delete(id);
        }
    }
}