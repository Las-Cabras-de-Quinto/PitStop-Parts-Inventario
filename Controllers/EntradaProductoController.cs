using Microsoft.AspNetCore.Mvc;
using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Models.ViewModels;
using PitStop_Parts_Inventario.Services;
using PitStop_Parts_Inventario.Services.Interfaces;
using System.Diagnostics;

namespace PitStop_Parts_Inventario.Controllers
{
    public class EntradaProductoController : BaseController
    {
        private readonly ILogger<EntradaProductoController> _logger;
        private readonly IEntradaProductoService _entradaProductoService;

        public EntradaProductoController(ILogger<EntradaProductoController> logger, IEntradaProductoService entradaProductoService)
        {
            _logger = logger;
            _entradaProductoService = entradaProductoService;
        }

        public async Task<IActionResult> Index(EntradaProductoFilterOptions? filtros, int numeroPagina = 1)
        {
            // Inicializar filtros si son null para evitar errores en los servicios
            filtros ??= new EntradaProductoFilterOptions();
            
            // Usar los parámetros recibidos para consultar el servicio
            var resultado = await _entradaProductoService.GetPagedAsync(
                numeroPagina,
                10,
                filtros
            );
            return View(resultado);
        }

        // Acción POST para procesar el formulario de creación
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([FromBody] EntradaProductoModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Datos inválidos", errors = ModelState });
            }

            // Obtener el ID del usuario actual
            var userId = CurrentUserId ?? User?.Identity?.Name ?? string.Empty;

            try
            {
                var entradaCreada = await _entradaProductoService.CreateAsync(model, userId);
                if (entradaCreada != null)
                {
                    return Json(new { success = true, message = "Entrada de producto creada correctamente." });
                }
                else
                {
                    return Json(new { success = false, message = "No se pudo crear la entrada de producto." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear entrada de producto");
                return Json(new { success = false, message = "Error interno del servidor." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            try
            {
                var entrada = await _entradaProductoService.GetByIdAsync(id);
                if (entrada == null)
                {
                    return Json(new { success = false, message = "La entrada de producto no existe." });
                }
                return Json(new { success = true, data = entrada });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener entrada de producto con ID: {Id}", id);
                return Json(new { success = false, message = "Error interno del servidor." });
            }
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar([FromBody] EntradaProductoModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Datos inválidos", errors = ModelState });
            }

            // Obtener el ID del usuario actual
            var userId = CurrentUserId ?? User?.Identity?.Name ?? string.Empty;

            try
            {
                var entradaActualizada = await _entradaProductoService.UpdateAsync(model, userId);
                if (entradaActualizada != null)
                {
                    return Json(new { success = true, message = "Entrada de producto actualizada correctamente." });
                }
                else
                {
                    return Json(new { success = false, message = "No se pudo actualizar la entrada de producto." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar entrada de producto con ID: {Id}", model.IdEntrada);
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
                return Json(new { success = false, message = "No tiene permisos para eliminar entradas de producto." });
            }

            try
            {
                // Verificar si la entrada existe
                var existe = await _entradaProductoService.ExistsAsync(id);
                if (!existe)
                {
                    return Json(new { success = false, message = "La entrada de producto no existe." });
                }

                // Eliminar la entrada
                var eliminado = await _entradaProductoService.DeleteAsync(id);

                if (eliminado)
                {
                    return Json(new { success = true, message = "Entrada de producto eliminada correctamente." });
                }
                else
                {
                    return Json(new { success = false, message = "No se pudo eliminar la entrada de producto." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar entrada de producto con ID: {Id}", id);
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
