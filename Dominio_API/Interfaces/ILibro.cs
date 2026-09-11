using Dominio_API.Clases;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio_API.Interfaces
{
    public interface ILibro : IBase<Libro>
    {
        Task<Libro?> GetBookWithAuthors(int id);
    }
}
