using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio_API.Clases
{
    public class LibroReserva
    {
        public int BookId { get; set; }
        public Libro? Libro { get; set; }


        public int ReserveId { get; set; }
        public Reserva? Reserva { get; set; }
    }
}
