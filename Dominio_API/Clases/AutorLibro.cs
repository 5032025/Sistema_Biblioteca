using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio_API.Clases
{
    public class AutorLibro 
    {
        public int BookId { get; set; }
        public Libro? Libro { get; set; }
        public int AuthorId { get; set; }
        public Autor? Autor { get; set; }
    }
}
