using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio_API.Clases
{
    public class Categoria : EntidadBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<LibroCategoria> CategoriaLibros { get; set; }


    }
}
