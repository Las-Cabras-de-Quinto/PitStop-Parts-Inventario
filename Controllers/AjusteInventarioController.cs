using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Services;
using PitStop_Parts_Inventario.Services.Interfaces;
using PitStop_Parts_Inventario.Models.ViewModels;



namespace PitStop_Parts_Inventario.Controllers
{
    public class AjusteInventarioController : BaseController
    {
        private readonly ILogger<AjusteInventarioController> _logger;
        private readonly IAjusteInventarioService _AjusteinventarioService;

        public AjusteInventarioController(ILogger<AjusteInventarioController> logger, IAjusteInventarioService ajusteInventarioService)
        {
            _logger = logger;
            _AjusteinventarioService = ajusteInventarioService;
        }

        public async Task<IActionResult> Index(AjusteInventarioFilterOptions filtros, int numeroPagina = 1)
        {
            // Usar los parámetros recibidos para consultar el servicio
            var resultado = await _AjusteinventarioService.GetPagedAsync(
                numeroPagina,
                10,
                filtros
            );
            return View(resultado);
        }
        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        // Acción POST para procesar el formulario de creación
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(AjusteInventarioModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Obtener el ID del usuario actual
            var userId = CurrentUserId ?? User?.Identity?.Name ?? string.Empty;

            var ajusteCreado = await _AjusteinventarioService.CreateAsync(model, userId);

            if (ajusteCreado != null)
            {
                // Redirigir al listado o detalle del ajuste creado
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "No se pudo crear el ajuste de inventario.");
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            // Verificar permisos de administrador primero
            if (!IsCurrentUserAdmin)
            {
                return Json(new { success = false, message = "No tiene permisos para eliminar ajustes de inventario." });
            }

            try
            {
                // Verificar si el ajuste existe
                var existe = await _AjusteinventarioService.ExistsAsync(id);
                if (!existe)
                {
                    return Json(new { success = false, message = "El ajuste de inventario no existe." });
                }

                // Eliminar el ajuste de inventario
                var eliminado = await _AjusteinventarioService.DeleteAsync(id);

                if (eliminado)
                {
                    TempData["Success"] = "Ajuste de inventario eliminado correctamente.";
                    return Json(new { success = true, message = "Ajuste eliminado correctamente." });
                }
                else
                {
                    return Json(new { success = false, message = "No se pudo eliminar el ajuste de inventario." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar ajuste de inventario con ID: {Id}", id);
                return Json(new { success = false, message = "Error interno del servidor." });
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
