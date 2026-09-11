using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio_API.Clases
{
    public class Libro : EntidadBase
    {
        public string Title { get; set; }
        public string Genre { get; set; }
        public int PublicationYear { get; set; }

        public string Description { get; set; }

        public BookStatus Status { get; set; }

        public ICollection<LibroReserva> LibroReservas { get; set; }

        public ICollection<AutorLibro> AutoresLibro { get; set; }

        public ICollection<LibroCategoria> LibroCategorias { get; set; }
    }

    public enum BookStatus
    {
        Available,
        Reserved,
        Maintenance
    }
}

