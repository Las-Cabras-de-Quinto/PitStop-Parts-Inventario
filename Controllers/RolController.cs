using Microsoft.AspNetCore.Mvc;
using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Models.ViewModels;
using PitStop_Parts_Inventario.Services;
using PitStop_Parts_Inventario.Services.Interfaces;
using System.Diagnostics;

namespace PitStop_Parts_Inventario.Controllers
{
    public class RolController : BaseController
    {
        private readonly ILogger<RolController> _logger;
        private readonly IRolService _rolService;

        public RolController(ILogger<RolController> logger, IRolService rolService)
        {
            _logger = logger;
            _rolService = rolService;

        }

        public async Task<IActionResult> Index(RolFilterOptions? filtros, int numeroPagina = 1)
        {
            // Inicializar filtros si son null para evitar errores en los servicios
            filtros ??= new RolFilterOptions();
            
            // Usar los parámetros recibidos para consultar el servicio
            var resultado = await _rolService.GetPagedAsync(
                numeroPagina,
                10,
                filtros
            );
            return View(resultado);
        }

        // Acción POST para procesar el formulario de creación
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([FromBody] RolModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Datos inválidos", errors = ModelState });
            }

            // Obtener el ID del usuario actual
            var userId = CurrentUserId ?? User?.Identity?.Name ?? string.Empty;

            try
            {
                var rolCreado = await _rolService.CreateAsync(model, userId);
                if (rolCreado != null)
                {
                    return Json(new { success = true, message = "Rol creado correctamente." });
                }
                else
                {
                    return Json(new { success = false, message = "No se pudo crear el rol." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear rol");
                return Json(new { success = false, message = "Error interno del servidor." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            try
            {
                var rol = await _rolService.GetByIdAsync(id);
                if (rol == null)
                {
                    return Json(new { success = false, message = "El rol no existe." });
                }
                return Json(new { success = true, data = rol });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener rol con ID: {Id}", id);
                return Json(new { success = false, message = "Error interno del servidor." });
            }
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar([FromBody] RolModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Datos inválidos", errors = ModelState });
            }

            // Obtener el ID del usuario actual
            var userId = CurrentUserId ?? User?.Identity?.Name ?? string.Empty;

            try
            {
                var rolActualizado = await _rolService.UpdateAsync(model, userId);
                if (rolActualizado != null)
                {
                    return Json(new { success = true, message = "Rol actualizado correctamente." });
                }
                else
                {
                    return Json(new { success = false, message = "No se pudo actualizar el rol." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar rol con ID: {Id}", model.IdRol);
                return Json(new { success = false, message = "Error interno del servidor." });
            }
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                // Verificar si el rol existe
                var existe = await _rolService.ExistsAsync(id);
                if (!existe)
                {
                    return Json(new { success = false, message = "El rol no existe." });
                }

                // Eliminar el rol
                var eliminado = await _rolService.DeleteAsync(id);

                if (eliminado)
                {
                    return Json(new { success = true, message = "Rol eliminado correctamente." });
                }
                else
                {
                    return Json(new { success = false, message = "No se pudo eliminar el rol." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar rol con ID: {Id}", id);
                return Json(new { success = false, message = "Error interno del servidor." });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
