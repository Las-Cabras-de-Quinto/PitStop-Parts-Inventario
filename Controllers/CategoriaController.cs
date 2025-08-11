using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Services.Interfaces;
using PitStop_Parts_Inventario.Models.ViewModels;

namespace PitStop_Parts_Inventario.Controllers
{
    public class CategoriaController : BaseController
    {
        private readonly ILogger<CategoriaController> _logger;
        private readonly ICategoriaService _categoriaService;

        public CategoriaController(ILogger<CategoriaController> logger, ICategoriaService categoriaService)
        {
            _logger = logger;
            _categoriaService = categoriaService;
        }

        public async Task<IActionResult> Index(CategoriaFilterOptions? filtros, int numeroPagina = 1)
        {
            // Inicializar filtros si son null para evitar errores en los servicios
            filtros ??= new CategoriaFilterOptions();
            
            // Usar los parámetros recibidos para consultar el servicio
            var resultado = await _categoriaService.GetPagedAsync(
                numeroPagina,
                10,
                filtros
            );
            return View(resultado);
        }

        // Acción POST para procesar el formulario de creación
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([FromBody] CategoriaEditRequest request)
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
                var model = new CategoriaModel
                {
                    Nombre = request.Nombre,
                    Descripcion = request.Descripcion,
                    IdEstado = request.IdEstado
                };

                var categoriaCreada = await _categoriaService.CreateAsync(model, userId);
                if (categoriaCreada != null)
                {
                    return Json(new { success = true, message = "Categoría creada correctamente." });
                }
                else
                {
                    return Json(new { success = false, message = "No se pudo crear la categoría." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear categoría");
                return Json(new { success = false, message = "Error interno del servidor." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            try
            {
                var categoria = await _categoriaService.GetByIdAsync(id);
                if (categoria == null)
                {
                    return Json(new { success = false, message = "La categoría no existe." });
                }

                // Incluir los productos relacionados
                var productos = categoria.CategoriaProductos?.Select(cp => new {
                    Id = cp.Producto.IdProducto,
                    Nombre = cp.Producto.Nombre
                }).ToList();

                var categoriaConRelaciones = new
                {
                    categoria.IdCategoria,
                    categoria.Nombre,
                    categoria.Descripcion,
                    categoria.IdEstado,
                    categoria.Estado,
                    Productos = productos
                };

                return Json(new { success = true, data = categoriaConRelaciones });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener categoría con ID: {Id}", id);
                return Json(new { success = false, message = "Error interno del servidor." });
            }
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar([FromBody] CategoriaEditRequest request)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Datos inválidos", errors = ModelState });
            }

            // Obtener el ID del usuario actual
            var userId = CurrentUserId ?? User?.Identity?.Name ?? string.Empty;

            try
            {
                // Crear el modelo de categoría base
                var model = new CategoriaModel
                {
                    IdCategoria = request.IdCategoria,
                    Nombre = request.Nombre,
                    Descripcion = request.Descripcion,
                    IdEstado = request.IdEstado
                };

                var categoriaActualizada = await _categoriaService.UpdateAsync(model, userId);
                if (categoriaActualizada != null)
                {
                    return Json(new { 
                        success = true, 
                        message = "Categoría actualizada correctamente."
                    });
                }
                else
                {
                    return Json(new { success = false, message = "No se pudo actualizar la categoría." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar categoría con ID: {Id}", request.IdCategoria);
                return Json(new { success = false, message = "Error interno del servidor." });
            }
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                // Verificar si la categoría existe
                var existe = await _categoriaService.ExistsAsync(id);
                if (!existe)
                {
                    return Json(new { success = false, message = "La categoría no existe." });
                }

                // Eliminar la categoría
                var eliminado = await _categoriaService.DeleteAsync(id);

                if (eliminado)
                {
                    return Json(new { success = true, message = "Categoría eliminada correctamente." });
                }
                else
                {
                    return Json(new { success = false, message = "No se pudo eliminar la categoría." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar categoría con ID: {Id}", id);
                return Json(new { success = false, message = "Error interno del servidor." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerParaSelect()
        {
            try
            {
                var categorias = await _categoriaService.GetAllAsync();
                var categoriasSelect = categorias.Select(c => new { 
                    id = c.IdCategoria, 
                    nombre = c.Nombre 
                });
                return Json(new { success = true, data = categoriasSelect });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener categorías para select");
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
