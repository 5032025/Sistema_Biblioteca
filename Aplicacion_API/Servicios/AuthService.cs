
using Dominio_API.Clases;
using Dominio_API.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookApplication.Services
{
    public class AuthService
    {

        private readonly IUsuario _userRepository;
        private readonly IJwt _jwtService;

        public AuthService(IUsuario userRepository, IJwt jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }




        public async Task<Usuario> RegisterUser(Usuario user)
        {
          

            await _userRepository.CreateUser(user);

            await _userRepository.AddToRoleAsync(user, "User");

            return user;
        }


        public async Task<string> Login(string email, string password, bool rememberme)
        {

            var user = await _userRepository.GetUserByEmail(email);

            if (user == null)
            {
                return "Credenciales invalidas";
            }


            var credencialesValidas = await _userRepository.CheckPasswordAsync(email, password);

            if (!credencialesValidas)
            {
                return "Credenciales invalidas";
            }

            // todo -> JWT



            var roles = await _userRepository.GetUserRoles(email);



            return _jwtService.GenerateToken(user, roles);
        }





    }
}
