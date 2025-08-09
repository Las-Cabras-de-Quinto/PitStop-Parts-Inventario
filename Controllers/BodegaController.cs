using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Services;
using PitStop_Parts_Inventario.Models.ViewModels;
using PitStop_Parts_Inventario.Services.Interfaces;

namespace PitStop_Parts_Inventario.Controllers
{
    public class BodegaController : BaseController
    {
        private readonly ILogger<BodegaController> _logger;
        private readonly IBodegaService _bodegaService;

        public BodegaController(ILogger<BodegaController> logger, IBodegaService bodegaService)
        {
            _logger = logger;
            _bodegaService = bodegaService;
        }

        public async Task<IActionResult> Index(BodegaFilterOptions? filtros, int numeroPagina = 1)
        {
            // Inicializar filtros si son null para evitar errores en los servicios
            filtros ??= new BodegaFilterOptions();
            
            // Usar los parámetros recibidos para consultar el servicio
            var resultado = await _bodegaService.GetPagedAsync(
                numeroPagina,
                10,
                filtros
            );
            return View(resultado);
        }

        // POST: Bodega/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] BodegaModel bodega)
        {
            return await ExecuteIfHasRole("Administrador", async () => {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Datos inválidos", errors = ModelState });
                }

                try
                {
                    await _bodegaService.CreateAsync(bodega, CurrentUserId ?? "");
                    return Json(new { success = true, message = "Bodega creada correctamente" });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al crear bodega");
                    return Json(new { success = false, message = "Error interno del servidor" });
                }
            });
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            try
            {
                var bodega = await _bodegaService.GetByIdAsync(id);
                if (bodega == null)
                {
                    return Json(new { success = false, message = "La bodega no existe." });
                }
                return Json(new { success = true, data = bodega });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener bodega con ID: {Id}", id);
                return Json(new { success = false, message = "Error interno del servidor." });
            }
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar([FromBody] BodegaModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Datos inválidos", errors = ModelState });
            }

            // Obtener el ID del usuario actual
            var userId = CurrentUserId ?? User?.Identity?.Name ?? string.Empty;

            try
            {
                var bodegaActualizada = await _bodegaService.UpdateAsync(model, userId);
                if (bodegaActualizada != null)
                {
                    return Json(new { success = true, message = "Bodega actualizada correctamente." });
                }
                else
                {
                    return Json(new { success = false, message = "No se pudo actualizar la bodega." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar bodega con ID: {Id}", model.IdBodega);
                return Json(new { success = false, message = "Error interno del servidor." });
            }
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            // Verificar permisos de administrador primero
            if (!IsCurrentUserAdmin)
            {
                return Json(new { success = false, message = "No tiene permisos para eliminar bodegas." });
            }

            try
            {
                // Verificar si la bodega existe
                var existe = await _bodegaService.ExistsAsync(id);
                if (!existe)
                {
                    return Json(new { success = false, message = "La bodega no existe." });
                }

                // Eliminar la bodega
                var eliminado = await _bodegaService.DeleteAsync(id);

                if (eliminado)
                {
                    return Json(new { success = true, message = "Bodega eliminada correctamente." });
                }
                else
                {
                    return Json(new { success = false, message = "No se pudo eliminar la bodega." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar bodega con ID: {Id}", id);
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
