using Microsoft.AspNetCore.Mvc;
using PitStop_Parts_Inventario.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PitStop_Parts_Inventario.Data;

namespace PitStop_Parts_Inventario.Controllers
{
    /// <summary>
    /// Controlador base que proporciona funcionalidades comunes para el manejo de usuarios y roles
    /// </summary>
    [Authorize] // Requiere autenticación por defecto
    public abstract class BaseController : Controller
    {
        /// <summary>
        /// Obtiene el usuario actual desde Identity
        /// </summary>
        protected async Task<UsuarioModel?> GetCurrentUserAsync()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                return null;

            try
            {
                var userManager = HttpContext.RequestServices.GetRequiredService<UserManager<UsuarioModel>>();
                var dbContext = HttpContext.RequestServices.GetRequiredService<PitStopDbContext>();

                var user = await userManager.GetUserAsync(User);
                if (user == null) 
                {
                    var logger = HttpContext.RequestServices.GetService<ILogger<BaseController>>();
                    logger?.LogWarning("UserManager.GetUserAsync retornó null para usuario autenticado: {UserName}", User.Identity?.Name ?? "Unknown");
                    return null;
                }

                var fullUser = await dbContext.Usuarios
                    .Include(u => u.Rol)
                    .ThenInclude(r => r.Estado)
                    .Include(u => u.Estado)
                    .FirstOrDefaultAsync(u => u.Id == user.Id);

                if (fullUser == null)
                {
                    var logger = HttpContext.RequestServices.GetService<ILogger<BaseController>>();
                    logger?.LogError("Usuario con ID {UserId} no encontrado en tabla Usuarios", user.Id);
                }

                return user;
            }
            catch (Exception ex)
            {
                var logger = HttpContext.RequestServices.GetService<ILogger<BaseController>>();
                logger?.LogError(ex, "Error al obtener el usuario actual completo");
                return null;
            }
        }

        /// <summary>
        /// Obtiene el ID del usuario actual de forma asíncrona
        /// </summary>
        protected async Task<string?> GetCurrentUserIdAsync()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                return null;

            try
            {
                // Primero intentamos obtener el ID directamente del ClaimsPrincipal
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userIdClaim))
                {
                    // Verificar que el usuario existe en la base de datos
                    var dbContext = HttpContext.RequestServices.GetRequiredService<PitStopDbContext>();
                    var userExists = await dbContext.Usuarios.AnyAsync(u => u.Id == userIdClaim);
                    if (userExists)
                    {
                        return userIdClaim;
                    }
                }

                // Si no funciona, usar UserManager como fallback
                var userManager = HttpContext.RequestServices.GetRequiredService<UserManager<UsuarioModel>>();
                var user = await userManager.GetUserAsync(User);
                if (user != null)
                {
                    // Verificar que el usuario existe en la tabla personalizada
                    var dbContext = HttpContext.RequestServices.GetRequiredService<PitStopDbContext>();
                    var userExists = await dbContext.Usuarios.AnyAsync(u => u.Id == user.Id);
                    if (userExists)
                    {
                        return user.Id;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                // Log del error sin exponer detalles internos
                var logger = HttpContext.RequestServices.GetService<ILogger<BaseController>>();
                logger?.LogError(ex, "Error al obtener el ID del usuario actual. Usuario autenticado: {IsAuthenticated}", User.Identity?.IsAuthenticated);
                return null;
            }
        }

        /// <summary>
        /// Obtiene el ID del usuario actual de forma directa desde los Claims
        /// </summary>
        protected string? GetCurrentUserIdFromClaims()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                return null;

            return User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        }

        /// <summary>
        /// Verifica si el usuario actual es administrador
        /// </summary>
        protected async Task<bool> IsCurrentUserAdminAsync()
        {
            var user = await GetCurrentUserAsync();
            return user?.Rol?.Admin ?? false;
        }

        /// <summary>
        /// Verifica si el usuario actual está activo
        /// </summary>
        protected async Task<bool> IsCurrentUserActiveAsync()
        {
            var user = await GetCurrentUserAsync();
            return user?.Estado?.Nombre == "Activo" && user?.Rol?.Estado?.Nombre == "Activo";
        }

        /// <summary>
        /// Verifica si el usuario actual tiene un rol específico
        /// </summary>
        protected async Task<bool> CurrentUserHasRoleAsync(string roleName)
        {
            var user = await GetCurrentUserAsync();
            return string.Equals(user?.Rol?.Nombre, roleName, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Verifica si el usuario actual tiene cualquiera de los roles especificados
        /// </summary>
        protected async Task<bool> CurrentUserHasAnyRoleAsync(params string[] roleNames)
        {
            var user = await GetCurrentUserAsync();
            if (user?.Rol?.Nombre == null) return false;
            
            return roleNames.Any(role => 
                string.Equals(user.Rol.Nombre, role, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Ejecuta una acción solo si el usuario tiene el rol especificado
        /// </summary>
        protected async Task<IActionResult> ExecuteIfHasRoleAsync(string roleName, Func<Task<IActionResult>> action)
        {
            if (!await IsCurrentUserActiveAsync())
            {
                return RedirectToPage("/Account/AccessDenied", new { area = "Identity" });
            }

            if (await CurrentUserHasRoleAsync(roleName) || await IsCurrentUserAdminAsync())
            {
                return await action();
            }

            return RedirectToPage("/Account/AccessDenied", new { area = "Identity" });
        }

        /// <summary>
        /// Ejecuta una acción solo si el usuario es administrador
        /// </summary>
        protected async Task<IActionResult> ExecuteIfAdminAsync(Func<Task<IActionResult>> action)
        {
            if (!await IsCurrentUserActiveAsync())
            {
                return RedirectToPage("/Account/AccessDenied", new { area = "Identity" });
            }

            if (await IsCurrentUserAdminAsync())
            {
                return await action();
            }

            return RedirectToPage("/Account/AccessDenied", new { area = "Identity" });
        }

        /// <summary>
        /// Establece información del usuario en ViewBag para las vistas
        /// </summary>
        protected async Task SetUserInfoInViewBagAsync()
        {
            var user = await GetCurrentUserAsync();
            ViewBag.CurrentUser = user;
            ViewBag.CurrentUserRole = user?.Rol?.Nombre;
            ViewBag.IsCurrentUserAdmin = user?.Rol?.Admin ?? false;
            ViewBag.IsCurrentUserActive = user?.Estado?.Nombre == "Activo" && user?.Rol?.Estado?.Nombre == "Activo";
            ViewBag.CurrentUserId = user?.Id;
            ViewBag.CurrentUserName = user?.UserName;
            ViewBag.CurrentUserEmail = user?.Email;
        }

        /// <summary>
        /// Obtiene el rol del usuario actual de forma asíncrona
        /// </summary>
        protected async Task<string?> GetCurrentUserRoleAsync()
        {
            var user = await GetCurrentUserAsync();
            return user?.Rol?.Nombre;
        }

        /// <summary>
        /// Obtiene el nombre de usuario actual de forma asíncrona
        /// </summary>
        protected async Task<string?> GetCurrentUserNameAsync()
        {
            var user = await GetCurrentUserAsync();
            return user?.UserName;
        }

        /// <summary>
        /// Obtiene el email del usuario actual de forma asíncrona
        /// </summary>
        protected async Task<string?> GetCurrentUserEmailAsync()
        {
            var user = await GetCurrentUserAsync();
            return user?.Email;
        }

        /// <summary>
        /// Override del método OnActionExecuting para validar sesión
        /// </summary>
        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            // Validar que el usuario esté autenticado y activo
            if (User.Identity?.IsAuthenticated == true)
            {
                var isActive = IsCurrentUserActiveAsync().GetAwaiter().GetResult();
                if (!isActive)
                {
                    context.Result = RedirectToPage("/Account/AccessDenied", new { area = "Identity" });
                    return;
                }
            }

            // Establecer información del usuario para las vistas
            SetUserInfoInViewBagAsync().GetAwaiter().GetResult();
            base.OnActionExecuting(context);
        }
    }
}
