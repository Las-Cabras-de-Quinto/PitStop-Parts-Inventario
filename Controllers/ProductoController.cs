using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Services;
using PitStop_Parts_Inventario.Models.ViewModels;
using PitStop_Parts_Inventario.Services.Interfaces;

namespace PitStop_Parts_Inventario.Controllers
{
    public class ProductoController : BaseController
    {
        private readonly ILogger<ProductoController> _logger;
        private readonly ProductoService _ProductoService;

        public ProductoController(ILogger<ProductoController> logger, ProductoService productoService)
        {
            _logger = logger;
            _ProductoService = productoService;
        }
        public async Task<IActionResult> Index(int numeroPagina, ProductoFilterOptions filtros)
        {
            // Usar los parámetros recibidos para consultar el servicio
            var resultado = await _ProductoService.GetPagedAsync(
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
        public async Task<IActionResult> Crear(ProductoModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Obtener el ID del usuario actual
            var userId = CurrentUserId ?? User?.Identity?.Name ?? string.Empty;

            var ajusteCreado = await _ProductoService.CreateAsync(model, userId);

            if (ajusteCreado != null)
            {
                // Redirigir al listado o detalle del producto creado
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "No se pudo crear Producto.");
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
                // Verificar si el prducto existe
                var existe = await _ProductoService.ExistsAsync(id);
                if (!existe)
                {
                    return Json(new { success = false, message = "El ajuste de inventario no existe." });
                }

                // Eliminar el producto
                var eliminado = await _ProductoService.DeleteAsync(id);

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
