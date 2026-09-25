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
        private readonly IConfiguration _configuration;

        public JWTService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(Usuario usuario, IList<string> roles)
        {
            // Reclamaciones o características que identifican al usuario
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Email ?? string.Empty),
                new Claim(ClaimTypes.GivenName, usuario.FullName ?? string.Empty),
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Lectura segura de la configuración con valores por defecto para evitar nulos
            var jwtKey = _configuration["JwtSettings:JwtKey"] ?? "LlaveSecretaPorDefectoSuperSegura123*!";
            var jwtIssuer = _configuration["JwtSettings:JwtIssuer"] ?? "BibliotecaAPI";
            var jwtAudience = _configuration["JwtSettings:JwtAudience"] ?? "BibliotecaUsuarios";
            var lifeTimeConfig = _configuration["JwtSettings:JwtLifeTime"] ?? "7";

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            int lifeTimeDays = int.TryParse(lifeTimeConfig, out var days) ? days : 7;

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(lifeTimeDays),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}