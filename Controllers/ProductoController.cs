using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Services.Interfaces;
using PitStop_Parts_Inventario.Models.ViewModels;

namespace PitStop_Parts_Inventario.Controllers
{
    public class ProductoController : BaseController
    {
        private readonly ILogger<ProductoController> _logger;
        private readonly IProductoService _productoService;

        public ProductoController(ILogger<ProductoController> logger, IProductoService productoService)
        {
            _logger = logger;
            _productoService = productoService;
        }

        public async Task<IActionResult> Index(ProductoFilterOptions? filtros, int numeroPagina = 1)
        {
            // Inicializar filtros si son null para evitar errores en los servicios
            filtros ??= new ProductoFilterOptions();
            
            // Usar los parámetros recibidos para consultar el servicio
            var resultado = await _productoService.GetPagedAsync(
                numeroPagina,
                10,
                filtros
            );
            return View(resultado);
        }

        // Acción POST para procesar el formulario de creación
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([FromBody] ProductoModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Datos inválidos", errors = ModelState });
            }

            // Obtener el ID del usuario actual
            var userId = CurrentUserId ?? User?.Identity?.Name ?? string.Empty;

            try
            {
                var productoCreado = await _productoService.CreateAsync(model, userId);
                if (productoCreado != null)
                {
                    return Json(new { success = true, message = "Producto creado correctamente." });
                }
                else
                {
                    return Json(new { success = false, message = "No se pudo crear el producto." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear producto");
                return Json(new { success = false, message = "Error interno del servidor." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            try
            {
                var producto = await _productoService.GetByIdAsync(id);
                if (producto == null)
                {
                    return Json(new { success = false, message = "El producto no existe." });
                }
                return Json(new { success = true, data = producto });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener producto con ID: {Id}", id);
                return Json(new { success = false, message = "Error interno del servidor." });
            }
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar([FromBody] ProductoModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Datos inválidos", errors = ModelState });
            }

            // Obtener el ID del usuario actual
            var userId = CurrentUserId ?? User?.Identity?.Name ?? string.Empty;

            try
            {
                var productoActualizado = await _productoService.UpdateAsync(model, userId);
                if (productoActualizado != null)
                {
                    return Json(new { success = true, message = "Producto actualizado correctamente." });
                }
                else
                {
                    return Json(new { success = false, message = "No se pudo actualizar el producto." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar producto con ID: {Id}", model.IdProducto);
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
                return Json(new { success = false, message = "No tiene permisos para eliminar productos." });
            }

            try
            {
                // Verificar si el producto existe
                var existe = await _productoService.ExistsAsync(id);
                if (!existe)
                {
                    return Json(new { success = false, message = "El producto no existe." });
                }

                // Eliminar el producto
                var eliminado = await _productoService.DeleteAsync(id);

                if (eliminado)
                {
                    return Json(new { success = true, message = "Producto eliminado correctamente." });
                }
                else
                {
                    return Json(new { success = false, message = "No se pudo eliminar el producto." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar producto con ID: {Id}", id);
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
