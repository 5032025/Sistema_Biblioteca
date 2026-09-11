using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio_API.Interfaces
{
    public interface IRole
    {
        /// <summary>
        /// Para revisar que un rol exista.
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        Task<bool> RoleExistAsync(string roleName);

        /// <summary>
        /// Para crear roles.
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>

        Task<bool> CreateRole(string roleName);
    }
}
