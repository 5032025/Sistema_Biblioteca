using Dominio_API.Clases;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio_API.Interfaces
{
    public interface ICategoria : IBase<Categoria>
    {

        Task<Categoria?> GetCategoryWithBooks(string name);

    }
}
