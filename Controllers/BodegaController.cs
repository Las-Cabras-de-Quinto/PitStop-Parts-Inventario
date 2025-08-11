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

        // POST: Bodega/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([FromBody] BodegaEditRequest request)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Datos inválidos", errors = ModelState });
            }

            try
            {
                // Crear el modelo base
                var bodega = new BodegaModel
                {
                    Nombre = request.Nombre,
                    Descripcion = request.Descripcion,
                    Ubicacion = request.Ubicacion,
                    IdEstado = request.IdEstado
                };

                await _bodegaService.CreateAsync(bodega, CurrentUserId ?? "");
                return Json(new { success = true, message = "Bodega creada correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear bodega");
                return Json(new { success = false, message = "Error interno del servidor" });
            }
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

                // Incluir los productos relacionados
                var productos = bodega.BodegaProductos?.Select(bp => new {
                    Id = bp.Producto?.IdProducto ?? 0,
                    Nombre = bp.Producto?.Nombre ?? "Sin nombre",
                    StockTotal = bp.StockTotal
                }).ToList();

                var bodegaConRelaciones = new
                {
                    bodega.IdBodega,
                    bodega.Nombre,
                    bodega.Descripcion,
                    bodega.Ubicacion,
                    bodega.IdEstado,
                    bodega.Estado,
                    BodegaProductos = productos,
                    TotalProductos = productos?.Count ?? 0, // Para debugging
                    TotalStock = productos?.Sum(p => p.StockTotal) ?? 0 // Para debugging
                };

                return Json(new { success = true, data = bodegaConRelaciones });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener bodega con ID: {Id}", id);
                return Json(new { success = false, message = "Error interno del servidor." });
            }
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar([FromBody] BodegaEditRequest request)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Datos inválidos", errors = ModelState });
            }

            // Obtener el ID del usuario actual
            var userId = CurrentUserId ?? User?.Identity?.Name ?? string.Empty;

            try
            {
                // Crear el modelo de bodega base
                var model = new BodegaModel
                {
                    IdBodega = request.IdBodega,
                    Nombre = request.Nombre,
                    Descripcion = request.Descripcion,
                    Ubicacion = request.Ubicacion,
                    IdEstado = request.IdEstado
                };

                var bodegaActualizada = await _bodegaService.UpdateAsync(model, userId);
                if (bodegaActualizada != null)
                {
                    // Actualizar relaciones de productos si se proporcionaron
                    if (request.Productos?.Any() == true)
                    {
                        foreach (var producto in request.Productos)
                        {
                            await _bodegaService.AsignarProductoAsync(request.IdBodega, producto.IdProducto, producto.StockTotal);
                        }
                    }

                    return Json(new { 
                        success = true, 
                        message = "Bodega actualizada correctamente."
                    });
                }
                else
                {
                    return Json(new { success = false, message = "No se pudo actualizar la bodega." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar bodega con ID: {Id}", request.IdBodega);
                return Json(new { success = false, message = "Error interno del servidor." });
            }
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
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

        [HttpGet]
        public async Task<IActionResult> ObtenerParaSelect()
        {
            try
            {
                var bodegas = await _bodegaService.GetAllAsync();
                var bodegasSelect = bodegas.Select(b => new { 
                    id = b.IdBodega, 
                    nombre = b.Nombre 
                });
                return Json(new { success = true, data = bodegasSelect });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener bodegas para select");
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
