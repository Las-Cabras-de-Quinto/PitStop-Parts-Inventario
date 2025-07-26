using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PitStop_Parts_Inventario.Data;
using PitStop_Parts_Inventario.Models;

namespace PitStop_Parts_Inventario.Middleware
{
    public class SessionValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SessionValidationMiddleware> _logger;

        // Rutas que no requieren autenticación
        private readonly string[] _publicPaths = new[]
        {
            "/Identity/Account/Login",
            "/Identity/Account/Register",
            "/Identity/Account/Logout",
            "/Identity/Account/ForgotPassword",
            "/Identity/Account/ResetPassword",
            "/Identity/Account/ConfirmEmail",
            "/Identity/Account/AccessDenied",
            "/css/",
            "/js/",
            "/lib/",
            "/assets/",
            "/favicon.ico"
        };

        public SessionValidationMiddleware(RequestDelegate next, ILogger<SessionValidationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, UserManager<UsuarioModel> userManager, PitStopDbContext dbContext)
        {
            var path = context.Request.Path.Value ?? "";

            // Permitir acceso a rutas públicas
            if (IsPublicPath(path))
            {
                await _next(context);
                return;
            }

            // Si el usuario no está autenticado, redirigir al login
            if (!context.User.Identity?.IsAuthenticated ?? true)
            {
                context.Response.Redirect("/Identity/Account/Login");
                return;
            }

            try
            {
                // Obtener el usuario actual
                var user = await userManager.GetUserAsync(context.User);
                
                if (user == null)
                {
                    _logger.LogWarning("Usuario autenticado pero no encontrado en la base de datos");
                    SignOutAndRedirect(context, "/Identity/Account/Login");
                    return;
                }

                // Cargar información completa del usuario con rol y estado
                var usuarioCompleto = await dbContext.Usuarios
                    .Include(u => u.Rol)
                    .Include(u => u.Estado)
                    .FirstOrDefaultAsync(u => u.Id == user.Id);

                if (usuarioCompleto == null)
                {
                    _logger.LogWarning($"Usuario {user.Id} no encontrado en la base de datos");
                    SignOutAndRedirect(context, "/Identity/Account/Login");
                    return;
                }

                // Verificar que el usuario esté activo (estado activo)
                if (usuarioCompleto.Estado?.Nombre != "Activo")
                {
                    _logger.LogWarning($"Usuario {user.Id} con estado inactivo: {usuarioCompleto.Estado?.Nombre}");
                    SignOutAndRedirect(context, "/Identity/Account/AccessDenied");
                    return;
                }

                // Verificar que el rol esté activo
                if (usuarioCompleto.Rol?.Estado?.Nombre != "Activo")
                {
                    _logger.LogWarning($"Usuario {user.Id} con rol inactivo");
                    SignOutAndRedirect(context, "/Identity/Account/AccessDenied");
                    return;
                }

                // Almacenar información del usuario y rol en el contexto para uso posterior
                context.Items["CurrentUser"] = usuarioCompleto;
                context.Items["UserRole"] = usuarioCompleto.Rol.Nombre;
                context.Items["IsAdmin"] = usuarioCompleto.Rol.Admin;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante la validación de sesión");
                SignOutAndRedirect(context, "/Identity/Account/Login");
                return;
            }

            await _next(context);
        }

        private bool IsPublicPath(string path)
        {
            return _publicPaths.Any(publicPath => 
                path.StartsWith(publicPath, StringComparison.OrdinalIgnoreCase));
        }

        private void SignOutAndRedirect(HttpContext context, string redirectPath)
        {
            // Limpiar la sesión
            context.Session.Clear();
            
            // Redirigir
            context.Response.Redirect(redirectPath);
        }
    }
}
