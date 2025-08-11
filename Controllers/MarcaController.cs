using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Services;
using PitStop_Parts_Inventario.Models.ViewModels;
using PitStop_Parts_Inventario.Services.Interfaces;

namespace PitStop_Parts_Inventario.Controllers
{
    public class MarcaController : BaseController
    {
        private readonly ILogger<MarcaController> _logger;
        private readonly IMarcaService _marcaService;

        public MarcaController(ILogger<MarcaController> logger, IMarcaService marcaService)
        {
            _logger = logger;
            _marcaService = marcaService;
        }

        public async Task<IActionResult> Index(MarcaFilterOptions? filtros, int numeroPagina = 1)
        {
            // Inicializar filtros si son null para evitar errores en los servicios
            filtros ??= new MarcaFilterOptions();
            
            // Usar los parámetros recibidos para consultar el servicio
            var resultado = await _marcaService.GetPagedAsync(
                numeroPagina,
                10,
                filtros
            );
            return View(resultado);
        }

        // Acción POST para procesar el formulario de creación
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([FromBody] MarcaEditRequest request)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Datos inválidos", errors = ModelState });
            }

            // Obtener el ID del usuario actual
            var userId = CurrentUserId ?? User?.Identity?.Name ?? string.Empty;

            try
            {
                // Crear el modelo base
                var model = new MarcaModel
                {
                    Nombre = request.Nombre,
                    Descripcion = request.Descripcion,
                    IdEstado = request.IdEstado
                };

                var marcaCreada = await _marcaService.CreateAsync(model, userId);
                if (marcaCreada != null)
                {
                    return Json(new { success = true, message = "Marca creada correctamente." });
                }
                else
                {
                    return Json(new { success = false, message = "No se pudo crear la marca." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear marca");
                return Json(new { success = false, message = "Error interno del servidor." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            try
            {
                var marca = await _marcaService.GetByIdAsync(id);
                if (marca == null)
                {
                    return Json(new { success = false, message = "La marca no existe." });
                }
                return Json(new { success = true, data = marca });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener marca con ID: {Id}", id);
                return Json(new { success = false, message = "Error interno del servidor." });
            }
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar([FromBody] MarcaEditRequest request)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Datos inválidos", errors = ModelState });
            }

            // Obtener el ID del usuario actual
            var userId = CurrentUserId ?? User?.Identity?.Name ?? string.Empty;

            try
            {
                // Obtener la marca existente
                var marcaExistente = await _marcaService.GetByIdAsync(request.IdMarca);
                if (marcaExistente == null)
                {
                    return Json(new { success = false, message = "Marca no encontrada." });
                }

                // Actualizar las propiedades
                marcaExistente.Nombre = request.Nombre;
                marcaExistente.Descripcion = request.Descripcion;
                marcaExistente.IdEstado = request.IdEstado;

                var marcaActualizada = await _marcaService.UpdateAsync(marcaExistente, userId);
                if (marcaActualizada != null)
                {
                    return Json(new { success = true, message = "Marca actualizada correctamente." });
                }
                else
                {
                    return Json(new { success = false, message = "No se pudo actualizar la marca." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar marca con ID: {Id}", request.IdMarca);
                return Json(new { success = false, message = "Error interno del servidor." });
            }
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                // Verificar si la marca existe
                var existe = await _marcaService.ExistsAsync(id);
                if (!existe)
                {
                    return Json(new { success = false, message = "La marca no existe." });
                }

                // Eliminar la marca
                var eliminado = await _marcaService.DeleteAsync(id);

                if (eliminado)
                {
                    return Json(new { success = true, message = "Marca eliminada correctamente." });
                }
                else
                {
                    return Json(new { success = false, message = "No se pudo eliminar la marca." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar marca con ID: {Id}", id);
                return Json(new { success = false, message = "Error interno del servidor." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerParaSelect()
        {
            try
            {
                var marcas = await _marcaService.GetAllAsync();
                var marcasSelect = marcas.Select(m => new { 
                    id = m.IdMarca, 
                    nombre = m.Nombre 
                });
                return Json(new { success = true, data = marcasSelect });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener marcas para select");
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
