
using Dominio_API.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookInfrastructure.Persistence.Repositories
{
    public class RoleRepository : IRole
    {
        readonly RoleManager<IdentityRole> _roleManager;

        public RoleRepository(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;

        }






        public async Task<bool> CreateRole(string roleName)
        {
            if (!await RoleExistAsync(roleName))
            {

                //todo -> crear el rol usando el RoleManager de Identity

                var result = await _roleManager.CreateAsync(new IdentityRole(roleName));

                return result.Succeeded;

            }

            return false;
        }

        public async Task<bool> RoleExistAsync(string roleName)
        {

            return await _roleManager.RoleExistsAsync(roleName);
        }
    }
}
