using Dominio_API.Clases;
using Dominio_API.Interfaces;
using Infraestructura_API.Identidad;
using Infraestructura_API.Mapping;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestructura_API.Persistencia.Repositorios
{
    public class UsuarioRepositorio : IUsuario
    {
        public readonly UserManager<AppIdentityUser> _userManager;

        public UsuarioRepositorio(UserManager<AppIdentityUser> userManager)
        {

            _userManager = userManager;
        }

        public async Task<Usuario> AddToRoleAsync(Usuario user, string roleName)
        {
            // Buscamos al usuario de Identity por correo
            var userDb = await _userManager.FindByEmailAsync(user.Email);

            // Protección para evitar que explote si por alguna razón viene nulo
            if (userDb != null)
            {
                await _userManager.AddToRoleAsync(userDb, roleName);
            }

            return user;
        }

        public async Task<bool> CheckPasswordAsync(string email, string password)
        {
          
            var user = await _userManager.FindByEmailAsync(email);

            return user != null && await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<Usuario> CreateUser(Usuario user)
        {
            var Result = await _userManager.CreateAsync(user.ToIdentityUser(), user.Password);

            if (Result.Succeeded)

            {
                var newUser = await _userManager.FindByEmailAsync(user.Email);
                user.Id = new Guid(newUser.Id);
                return user;
            }
            return null;
        }

        public async Task<Usuario> GetUserByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            return user?.ToDomainUser();
        }

        public async Task<List<string>> GetUserRoles(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var roles = await _userManager.GetRolesAsync(user);
            return roles.ToList();
        }

        public async Task<bool> UserExists(string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }
    }
}
