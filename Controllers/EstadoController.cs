using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Services.Interfaces;
using PitStop_Parts_Inventario.Models.ViewModels;

namespace PitStop_Parts_Inventario.Controllers
{
    public class EstadoController : BaseController
    {
        private readonly ILogger<EstadoController> _logger;
        private readonly IEstadoService _estadoService;

        public EstadoController(ILogger<EstadoController> logger, IEstadoService estadoService)
        {
            _logger = logger;
            _estadoService = estadoService;
        }

        public async Task<IActionResult> Index(EstadoFilterOptions? filtros, int numeroPagina = 1)
        {
            // Inicializar filtros si son null para evitar errores en los servicios
            filtros ??= new EstadoFilterOptions();
            
            // Usar los parámetros recibidos para consultar el servicio
            var resultado = await _estadoService.GetPagedAsync(
                numeroPagina,
                10,
                filtros
            );
            return View(resultado);
        }

        // Acción POST para procesar el formulario de creación
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([FromBody] EstadoModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Datos inválidos", errors = ModelState });
            }

            var userId = await GetCurrentUserIdAsync();
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "No se pudo obtener el usuario actual" });
            }

            try
            {
                var estadoCreado = await _estadoService.CreateAsync(model, userId);
                if (estadoCreado != null)
                {
                    return Json(new { success = true, message = "Estado creado correctamente." });
                }
                else
                {
                    return Json(new { success = false, message = "No se pudo crear el estado." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear estado");
                return Json(new { success = false, message = "Error interno del servidor." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            try
            {
                var estado = await _estadoService.GetByIdAsync(id);
                if (estado == null)
                {
                    return Json(new { success = false, message = "El estado no existe." });
                }
                return Json(new { success = true, data = estado });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estado con ID: {Id}", id);
                return Json(new { success = false, message = "Error interno del servidor." });
            }
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar([FromBody] EstadoModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Datos inválidos", errors = ModelState });
            }

            var userId = await GetCurrentUserIdAsync();
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "No se pudo obtener el usuario actual" });
            }

            try
            {
                var estadoActualizado = await _estadoService.UpdateAsync(model, userId);
                if (estadoActualizado != null)
                {
                    return Json(new { success = true, message = "Estado actualizado correctamente." });
                }
                else
                {
                    return Json(new { success = false, message = "No se pudo actualizar el estado." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar estado con ID: {Id}", model.IdEstado);
                return Json(new { success = false, message = "Error interno del servidor." });
            }
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                // Verificar si el estado existe
                var existe = await _estadoService.ExistsAsync(id);
                if (!existe)
                {
                    return Json(new { success = false, message = "El estado no existe." });
                }

                // Eliminar el estado
                var eliminado = await _estadoService.DeleteAsync(id);

                if (eliminado)
                {
                    return Json(new { success = true, message = "Estado eliminado correctamente." });
                }
                else
                {
                    return Json(new { success = false, message = "No se pudo eliminar el estado." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar estado con ID: {Id}", id);
                return Json(new { success = false, message = "Error interno del servidor." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerParaSelect()
        {
            try
            {
                var estados = await _estadoService.GetAllAsync();
                var estadosSelect = estados.Select(e => new { 
                    id = e.IdEstado, 
                    nombre = e.Nombre 
                });
                return Json(new { success = true, data = estadosSelect });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estados para select");
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
