using PitStop_Parts_Inventario.Models;

namespace PitStop_Parts_Inventario.Controllers.Helpers
{
    public static class UserContextHelper
    {
        /// <summary>
        /// Obtiene el usuario actual desde el contexto HTTP
        /// </summary>
        /// <param name="httpContext">El contexto HTTP actual</param>
        /// <returns>El usuario actual o null si no está disponible</returns>
        public static UsuarioModel? GetCurrentUser(HttpContext httpContext)
        {
            return httpContext.Items["CurrentUser"] as UsuarioModel;
        }

        /// <summary>
        /// Obtiene el rol del usuario actual
        /// </summary>
        /// <param name="httpContext">El contexto HTTP actual</param>
        /// <returns>El nombre del rol del usuario actual o null si no está disponible</returns>
        public static string? GetCurrentUserRole(HttpContext httpContext)
        {
            return httpContext.Items["UserRole"] as string;
        }

        /// <summary>
        /// Verifica si el usuario actual es administrador
        /// </summary>
        /// <param name="httpContext">El contexto HTTP actual</param>
        /// <returns>True si el usuario es administrador, false en caso contrario</returns>
        public static bool IsCurrentUserAdmin(HttpContext httpContext)
        {
            var isAdmin = httpContext.Items["IsAdmin"];
            return isAdmin is bool admin && admin;
        }

        /// <summary>
        /// Obtiene el ID del usuario actual
        /// </summary>
        /// <param name="httpContext">El contexto HTTP actual</param>
        /// <returns>El ID del usuario actual o null si no está disponible</returns>
        public static string? GetCurrentUserId(HttpContext httpContext)
        {
            var user = GetCurrentUser(httpContext);
            return user?.Id;
        }

        /// <summary>
        /// Obtiene el nombre de usuario actual
        /// </summary>
        /// <param name="httpContext">El contexto HTTP actual</param>
        /// <returns>El nombre de usuario actual o null si no está disponible</returns>
        public static string? GetCurrentUserName(HttpContext httpContext)
        {
            var user = GetCurrentUser(httpContext);
            return user?.UserName;
        }

        /// <summary>
        /// Obtiene el email del usuario actual
        /// </summary>
        /// <param name="httpContext">El contexto HTTP actual</param>
        /// <returns>El email del usuario actual o null si no está disponible</returns>
        public static string? GetCurrentUserEmail(HttpContext httpContext)
        {
            var user = GetCurrentUser(httpContext);
            return user?.Email;
        }

        /// <summary>
        /// Verifica si el usuario actual tiene un rol específico
        /// </summary>
        /// <param name="httpContext">El contexto HTTP actual</param>
        /// <param name="roleName">El nombre del rol a verificar</param>
        /// <returns>True si el usuario tiene el rol especificado, false en caso contrario</returns>
        public static bool CurrentUserHasRole(HttpContext httpContext, string roleName)
        {
            var userRole = GetCurrentUserRole(httpContext);
            return string.Equals(userRole, roleName, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Verifica si el usuario actual tiene alguno de los roles especificados
        /// </summary>
        /// <param name="httpContext">El contexto HTTP actual</param>
        /// <param name="roleNames">Los nombres de los roles a verificar</param>
        /// <returns>True si el usuario tiene alguno de los roles especificados, false en caso contrario</returns>
        public static bool CurrentUserHasAnyRole(HttpContext httpContext, params string[] roleNames)
        {
            var userRole = GetCurrentUserRole(httpContext);
            return roleNames.Any(role => string.Equals(userRole, role, StringComparison.OrdinalIgnoreCase));
        }
    }
}
