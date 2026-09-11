using Dominio_API.Clases;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio_API.Interfaces
{
    public interface IReserva : IBase<Reserva>
    {
        Task AddBookToReserve(LibroReserva libroReserva);
        Task<Reserva?> GetReserveWithBooks(int id);
    }
}
