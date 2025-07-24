using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using PitStop_Parts_Inventario.Models;

namespace PitStop_Parts_Inventario.Services
{
    public class CustomUserManager : UserManager<UsuarioModel>
    {
        public CustomUserManager(
            IUserStore<UsuarioModel> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<UsuarioModel> passwordHasher,
            IEnumerable<IUserValidator<UsuarioModel>> userValidators,
            IEnumerable<IPasswordValidator<UsuarioModel>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<UsuarioModel>> logger)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }

        public override async Task<IdentityResult> CreateAsync(UsuarioModel user, string password)
        {
            // Establecer estado activo por defecto al registrar
            if (user.IdEstado == 0)
            {
                user.IdEstado = 1; // Activo
            }

            // Establecer rol por defecto si no tiene uno
            if (user.IdRol == 0)
            {
                user.IdRol = 2; // Usuario estándar
            }

            // Establecer fecha de ingreso si no está establecida
            if (user.FechaDeIngreso == default(DateTime))
            {
                user.FechaDeIngreso = DateTime.Now;
            }

            return await base.CreateAsync(user, password);
        }

        public override async Task<IdentityResult> CreateAsync(UsuarioModel user)
        {
            // Establecer estado activo por defecto al registrar
            if (user.IdEstado == 0)
            {
                user.IdEstado = 1; // Activo
            }

            // Establecer rol por defecto si no tiene uno
            if (user.IdRol == 0)
            {
                user.IdRol = 2; // Usuario estándar
            }

            // Establecer fecha de ingreso si no está establecida
            if (user.FechaDeIngreso == default(DateTime))
            {
                user.FechaDeIngreso = DateTime.Now;
            }

            return await base.CreateAsync(user);
        }
    }
}
