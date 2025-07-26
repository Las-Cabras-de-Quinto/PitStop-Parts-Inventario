using PitStop_Parts_Inventario.Middleware;

namespace PitStop_Parts_Inventario.Extensions
{
    public static class MiddlewareExtensions
    {
        /// <summary>
        /// Registra el middleware de validación de sesión y roles
        /// </summary>
        /// <param name="builder">El WebApplicationBuilder</param>
        /// <returns>El mismo WebApplicationBuilder para permitir encadenamiento</returns>
        public static IApplicationBuilder UseSessionValidation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SessionValidationMiddleware>();
        }
    }
}
