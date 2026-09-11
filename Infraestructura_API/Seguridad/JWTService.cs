using Dominio_API.Clases;
using Dominio_API.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infraestructura_API.Seguridad
{
    public class JWTService : IJwt
    {

        IConfiguration _configuration;
        public JWTService(IConfiguration configuritaion)
        {

            _configuration = configuritaion;

        }

        public object SegurityAlgorithms { get; private set; }

        public string GenerateToken(Usuario usuario, IList<string> roles)
        {

            //Reclamaciones o caracteristicas que identifican al usuario

            var claims = new List<Claim>() {

                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Email),
                new Claim(ClaimTypes.GivenName, usuario.FullName),

            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));


            }


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(

                issuer: _configuration["JwtIssuer"],
                audience: _configuration["JwtAudience"],
                claims: claims,
                expires: DateTime.Now.AddDays(int.Parse(_configuration["JwtLifeTime"])),
                signingCredentials: creds


                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
