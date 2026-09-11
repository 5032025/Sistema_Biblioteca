using System;
using System.Collections.Generic;
using System.Text;


namespace Dominio_API.Clases
{
    public class Autor : EntidadBase
    {
        public string Name { get; set; }
        public string BirthPlace { get; set; }
        public DateTime BirthDate { get; set; }
        public string Description { get; set; }
        public string Nationality { get; set; }



        // Propiedad de navegación: Un autor puede tener muchos libros
        public ICollection<AutorLibro> LibrosAutor { get; set; }
    }
}
