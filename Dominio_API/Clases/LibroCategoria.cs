using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio_API.Clases
{
    public class LibroCategoria 
    {
        public int LibroId { get; set; }
        public Libro Libro { get; set; }

        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; }
    }
}