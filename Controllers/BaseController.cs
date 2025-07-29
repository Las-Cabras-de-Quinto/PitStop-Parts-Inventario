using Microsoft.AspNetCore.Mvc;
using PitStop_Parts_Inventario.Controllers.Helpers;
using PitStop_Parts_Inventario.Models;

namespace PitStop_Parts_Inventario.Controllers
{
    /// <summary>
    /// Controlador base que proporciona funcionalidades comunes para el manejo de usuarios y roles
    /// </summary>
    public abstract class BaseController : Controller
    {
        /// <summary>
        /// Obtiene el usuario actual
        /// </summary>
        protected UsuarioModel? CurrentUser => UserContextHelper.GetCurrentUser(HttpContext);

        /// <summary>
        /// Obtiene el rol del usuario actual
        /// </summary>
        protected string? CurrentUserRole => UserContextHelper.GetCurrentUserRole(HttpContext);

        /// <summary>
        /// Verifica si el usuario actual es administrador
        /// </summary>
        protected bool IsCurrentUserAdmin => UserContextHelper.IsCurrentUserAdmin(HttpContext);

        /// <summary>
        /// Obtiene el ID del usuario actual
        /// </summary>
        protected string? CurrentUserId => UserContextHelper.GetCurrentUserId(HttpContext);

        /// <summary>
        /// Obtiene el nombre de usuario actual
        /// </summary>
        protected string? CurrentUserName => UserContextHelper.GetCurrentUserName(HttpContext);

        /// <summary>
        /// Obtiene el email del usuario actual
        /// </summary>
        protected string? CurrentUserEmail => UserContextHelper.GetCurrentUserEmail(HttpContext);

        /// <summary>
        /// Verifica si el usuario actual tiene un rol específico
        /// </summary>
        /// <param name="roleName">El nombre del rol a verificar</param>
        /// <returns>True si el usuario tiene el rol especificado</returns>
        protected bool CurrentUserHasRole(string roleName)
        {
            return UserContextHelper.CurrentUserHasRole(HttpContext, roleName);
        }

        /// <summary>
        /// Verifica si el usuario actual tiene alguno de los roles especificados
        /// </summary>
        /// <param name="roleNames">Los nombres de los roles a verificar</param>
        /// <returns>True si el usuario tiene alguno de los roles especificados</returns>
        protected bool CurrentUserHasAnyRole(params string[] roleNames)
        {
            return UserContextHelper.CurrentUserHasAnyRole(HttpContext, roleNames);
        }

        /// <summary>
        /// Establece información del usuario en ViewBag para las vistas
        /// </summary>
        protected void SetUserInfoInViewBag()
        {
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.CurrentUserRole = CurrentUserRole;
            ViewBag.IsCurrentUserAdmin = IsCurrentUserAdmin;
            ViewBag.CurrentUserId = CurrentUserId;
            ViewBag.CurrentUserName = CurrentUserName;
            ViewBag.CurrentUserEmail = CurrentUserEmail;
        }

        /// <summary>
        /// Ejecuta una acción solo si el usuario tiene el rol especificado
        /// </summary>
        /// <param name="roleName">El rol requerido</param>
        /// <param name="action">La acción a ejecutar</param>
        /// <param name="unauthorizedAction">Acción a ejecutar si no tiene autorización (opcional)</param>
        protected IActionResult ExecuteIfHasRole(string roleName, Func<IActionResult> action, Func<IActionResult>? unauthorizedAction = null)
        {
            if (CurrentUserHasRole(roleName) || IsCurrentUserAdmin)
            {
                return action();
            }

            return unauthorizedAction?.Invoke() ?? RedirectToAction("AccessDenied", "Account", new { area = "Identity" });
        }

        protected async Task<IActionResult> ExecuteIfHasRole(string roleName, Func<Task<IActionResult>> action, Func<IActionResult>? unauthorizedAction = null)
        {
            if (CurrentUserHasRole(roleName) || IsCurrentUserAdmin)
            {
                return await action();
            }

            return unauthorizedAction?.Invoke() ?? RedirectToAction("AccessDenied", "Account", new { area = "Identity" });
        }

        /// <summary>
        /// Ejecuta una acción solo si el usuario es administrador
        /// </summary>
        /// <param name="action">La acción a ejecutar</param>
        /// <param name="unauthorizedAction">Acción a ejecutar si no tiene autorización (opcional)</param>
        protected IActionResult ExecuteIfAdmin(Func<IActionResult> action, Func<IActionResult>? unauthorizedAction = null)
        {
            if (IsCurrentUserAdmin)
            {
                return action();
            }

            return unauthorizedAction?.Invoke() ?? RedirectToAction("AccessDenied", "Account", new { area = "Identity" });
        }

        protected async Task<IActionResult> ExecuteIfAdmin(Func<Task<IActionResult>> action, Func<IActionResult>? unauthorizedAction = null)
        {
            if (IsCurrentUserAdmin)
            {
                return await action();
            }

            return unauthorizedAction?.Invoke() ?? RedirectToAction("AccessDenied", "Account", new { area = "Identity" });
        }

        /// <summary>
        /// Override del método OnActionExecuting para establecer información del usuario automáticamente
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            SetUserInfoInViewBag();
            base.OnActionExecuting(context);
        }
    }
}
