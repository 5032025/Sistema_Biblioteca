using Dominio_API.Clases;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio_API.Interfaces
{
    public interface IAutor : IBase<Autor>
    {
        
        Task<Autor?> GetAuthorWithBooks(int id);
    }
}
