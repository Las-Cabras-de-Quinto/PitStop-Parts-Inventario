using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using PitStop_Parts_Inventario.Models;

namespace PitStop_Parts_Inventario.Services
{
    public class CustomSignInManager : SignInManager<UsuarioModel>
    {
        private readonly UserManager<UsuarioModel> _userManager;

        public CustomSignInManager(
            UserManager<UsuarioModel> userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<UsuarioModel> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<UsuarioModel>> logger,
            IAuthenticationSchemeProvider schemes,
            IUserConfirmation<UsuarioModel> confirmation)
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
        {
            _userManager = userManager;
        }

        public override async Task<SignInResult> CheckPasswordSignInAsync(UsuarioModel user, string password, bool lockoutOnFailure)
        {
            // Verificar primero si el usuario está activo
            if (user.IdEstado == 2) // 2 = Inactivo
            {
                Logger.LogWarning("Usuario {Email} intentó iniciar sesión pero está inactivo", user.Email);
                return SignInResult.NotAllowed;
            }

            // Si está activo, proceder con la validación normal
            return await base.CheckPasswordSignInAsync(user, password, lockoutOnFailure);
        }

        public override async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return SignInResult.Failed;
            }

            // Verificar si el usuario está activo antes de intentar el login
            if (user.IdEstado == 2) // 2 = Inactivo
            {
                Logger.LogWarning("Usuario {UserName} intentó iniciar sesión pero está inactivo", userName);
                return SignInResult.NotAllowed;
            }

            return await base.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);
        }
    }
}
