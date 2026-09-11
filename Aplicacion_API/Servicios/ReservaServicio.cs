using Dominio_API.Clases;
using Dominio_API.Interfaces;

namespace Aplicacion_API.Servicios
{
    public class ReservaServicio
    {
        private readonly IReserva _reservaRepositorio;

        public ReservaServicio(IReserva reservaRepositorio)
        {
            _reservaRepositorio = reservaRepositorio;
        }

        public async Task<IEnumerable<Reserva>> GetAllReservasAsync(int page = 1, string search = "", string userId = null, bool isAdmin = false)
        {
            var query = await _reservaRepositorio.GetAll(
                x => !x.IsDeleted && (isAdmin || x.UserId == userId),
                page,
                10,
                search
            );
            return query.ToList();
        }

        public async Task<Reserva?> GetReservaByIdAsync(int id)
        {
            return await _reservaRepositorio.GetReserveWithBooks(id);
        }

        public async Task<Reserva?> CreateReservaAsync(Reserva reserva)
        {
            return await _reservaRepositorio.AddAsync(reserva);
        }

        public async Task AddBookToReserveAsync(LibroReserva libroReserva)
        {
            await _reservaRepositorio.AddBookToReserve(libroReserva);
        }

        public async Task<Reserva?> UpdateReservaAsync(int id, Reserva reserva)
        {
            return await _reservaRepositorio.UpdateAsync(id, reserva);
        }

        public async Task<bool> DeleteReservaAsync(int id)
        {
            return await _reservaRepositorio.Delete(id);
        }
    }
}