using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using PitStop_Parts_Inventario.Models;

namespace PitStop_Parts_Inventario.Data
{
    public class CustomUserStore : UserStore<UsuarioModel>
    {
        private readonly PitStopDbContext _context;

        public CustomUserStore(PitStopDbContext context, IdentityErrorDescriber describer = null)
            : base(context, describer)
        {
            _context = context;
        }

        public override async Task<UsuarioModel> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
        {
            var user = await base.FindByEmailAsync(normalizedEmail, cancellationToken);
            
            if (user != null)
            {
                // Cargar el estado del usuario
                await _context.Entry(user)
                    .Reference(u => u.Estado)
                    .LoadAsync(cancellationToken);
            }

            return user;
        }

        public override async Task<UsuarioModel> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default)
        {
            var user = await base.FindByNameAsync(normalizedUserName, cancellationToken);
            
            if (user != null)
            {
                // Cargar el estado del usuario
                await _context.Entry(user)
                    .Reference(u => u.Estado)
                    .LoadAsync(cancellationToken);
            }

            return user;
        }
    }
}
