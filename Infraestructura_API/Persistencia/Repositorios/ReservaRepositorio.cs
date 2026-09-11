using Dominio_API.Clases;
using Dominio_API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura_API.Persistencia.Repositorios
{
    public class ReservaRepositorio : RepositorioBase<Reserva>, IReserva
    {
        public ReservaRepositorio(AppDbContext context) : base(context) { }

        public async Task AddBookToReserve(LibroReserva libroReserva)
        {
            await _context.Set<LibroReserva>().AddAsync(libroReserva);
            await _context.SaveChangesAsync();
        }

        public async Task<Reserva?> GetReserveWithBooks(int id)
        {
            return await _context.Reservas
                .Include(r => r.ReservaLibros)
                .ThenInclude(rl => rl.Libro)
                .FirstOrDefaultAsync(r => r.Id == id);
        }
    }
}