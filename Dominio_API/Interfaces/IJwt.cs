using Dominio_API.Clases;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio_API.Interfaces
{
    public interface IJwt
    {
        /// <summary>
        /// Para generar un token de acceso a recursos.
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        string GenerateToken(Usuario usuario, IList<string> roles);

    }
}
