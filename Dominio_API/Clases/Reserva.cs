using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio_API.Clases
{
    public class Reserva : EntidadBase
    {
      
        public string UserId { get; set; }

        public DateTime ReservationDate { get; set; }
        public DateTime ExpirationDate { get; set; }

        public ReserveStatus Status { get; set; }

        public virtual ICollection<LibroReserva> ReservaLibros { get; set; } = new List<LibroReserva>();
    }

    public enum ReserveStatus
    {
        Active,
        Cancelled,
        Completed
    }
}

