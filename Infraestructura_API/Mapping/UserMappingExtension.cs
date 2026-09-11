using Dominio_API.Clases;
using Infraestructura_API.Identidad;
using System;

namespace Infraestructura_API.Mapping
{
    public static class UserMappingExtension
    {
        public static AppIdentityUser ToIdentityUser(this Usuario user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user), "El objeto usuario llegó nulo al mapeador.");

            return new AppIdentityUser
            {
                UserName = user.Email,
                Email = user.Email,
                PhoneNumber = user.Tel,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }

        public static Usuario ToDomainUser(this AppIdentityUser identityUser)
        {
            return new Usuario
            {
                Id = Guid.Parse(identityUser.Id),
                Email = identityUser.Email,
                Tel = identityUser.PhoneNumber,
                FirstName = identityUser.FirstName,
                LastName = identityUser.LastName
            };
        }
    }
}