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

        public async Task<IActionResult> Index(AjusteInventarioFilterOptions? filtros, int numeroPagina = 1)
        {
            // Inicializar filtros si son null para evitar errores en los servicios
            filtros ??= new AjusteInventarioFilterOptions();
            
            // Usar los parámetros recibidos para consultar el servicio
            var resultado = await _AjusteinventarioService.GetPagedAsync(
                numeroPagina,
                10,
                filtros
            );
            return View(resultado);
        }

        // Acción POST para procesar el formulario de creación
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] AjusteInventarioEditRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Model state is invalid: {@ModelState}", ModelState);
                    return Json(new { success = false, message = "Datos inválidos", errors = ModelState });
                }

                // Obtener el ID del usuario actual
                var userId = CurrentUserId ?? User?.Identity?.Name ?? string.Empty;

                // Crear el modelo base
                var model = new AjusteInventarioModel
                {
                    IdBodega = request.IdBodega,
                    IdUsuario = userId,
                    Fecha = request.Fecha
                };

                var ajusteCreado = await _AjusteinventarioService.CreateAsync(model, userId);
                if (ajusteCreado != null)
                {
                    // Procesar los productos del ajuste
                    foreach (var producto in request.Productos)
                    {
                        // Obtener la cantidad anterior del stock actual en la bodega
                        var cantidadAnterior = await _AjusteinventarioService.ObtenerStockActualAsync(request.IdBodega, producto.IdProducto);
                        
                        // Usar CantidadProducto como la nueva cantidad del ajuste
                        await _AjusteinventarioService.AgregarProductoAsync(
                            ajusteCreado.IdAjusteInventario,
                            producto.IdProducto,
                            cantidadAnterior, // Cantidad obtenida de la BD
                            producto.CantidadProducto, // La cantidad nueva viene del frontend
                            producto.Motivo ?? "Ajuste de inventario"
                        );
                    }

                    return Json(new { success = true, message = "Ajuste de inventario creado correctamente." });
                }
                else
                {
                    return Json(new { success = false, message = "No se pudo crear el ajuste de inventario." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear ajuste de inventario");
                return Json(new { success = false, message = "Error interno del servidor: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            try
            {
                var ajuste = await _AjusteinventarioService.GetByIdAsync(id);
                if (ajuste == null)
                {
                    return Json(new { success = false, message = "El ajuste de inventario no existe." });
                }

                // Estructurar los datos con nombres que coincidan con la respuesta esperada
                var ajusteEstructurado = new
                {
                    idAjusteInventario = ajuste.IdAjusteInventario,
                    idBodega = ajuste.IdBodega,
                    idUsuario = ajuste.IdUsuario,
                    fecha = ajuste.Fecha,
                    bodega = new
                    {
                        idBodega = ajuste.Bodega?.IdBodega,
                        nombre = ajuste.Bodega?.Nombre,
                        descripcion = ajuste.Bodega?.Descripcion,
                        ubicacion = ajuste.Bodega?.Ubicacion
                    },
                    usuario = new
                    {
                        id = ajuste.Usuario?.Id,
                        userName = ajuste.Usuario?.UserName,
                        email = ajuste.Usuario?.Email
                    },
                    ajusteInventarioProductos = ajuste.AjusteInventarioProductos?.Select(aip => new
                    {
                        idAjusteInventario = aip.IdAjusteInventario,
                        idProducto = aip.IdProducto,
                        cantidadProducto = aip.CantidadProducto,
                        producto = new
                        {
                            idProducto = aip.Producto?.IdProducto,
                            nombre = aip.Producto?.Nombre,
                            sku = aip.Producto?.SKU,
                            descripcion = aip.Producto?.Descripcion
                        }
                    }).ToList()
                };

                return Json(new { success = true, data = ajusteEstructurado });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener ajuste de inventario con ID: {Id}", id);
                return Json(new { success = false, message = "Error interno del servidor." });
            }
        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromBody] AjusteInventarioEditRequest request)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Datos inválidos", errors = ModelState });
            }

            // Obtener el ID del usuario actual
            var userId = CurrentUserId ?? User?.Identity?.Name ?? string.Empty;

            try
            {
                // Crear el modelo base para actualizar
                var model = new AjusteInventarioModel
                {
                    IdAjusteInventario = request.IdAjusteInventario,
                    IdBodega = request.IdBodega,
                    IdUsuario = userId,
                    Fecha = request.Fecha
                };

                var ajusteActualizado = await _AjusteinventarioService.UpdateAsync(model, userId);
                if (ajusteActualizado != null)
                {
                    // Limpiar productos existentes del ajuste
                    var productosExistentes = await _AjusteinventarioService.GetByIdAsync(request.IdAjusteInventario);
                    if (productosExistentes?.AjusteInventarioProductos != null)
                    {
                        foreach (var productoExistente in productosExistentes.AjusteInventarioProductos)
                        {
                            await _AjusteinventarioService.RemoverProductoAsync(request.IdAjusteInventario, productoExistente.IdProducto);
                        }
                    }

                    // Agregar los nuevos productos
                    foreach (var producto in request.Productos)
                    {
                        // Obtener la cantidad anterior del stock actual en la bodega
                        var cantidadAnterior = await _AjusteinventarioService.ObtenerStockActualAsync(request.IdBodega, producto.IdProducto);
                        
                        // Usar CantidadProducto como la nueva cantidad del ajuste
                        await _AjusteinventarioService.AgregarProductoAsync(
                            request.IdAjusteInventario,
                            producto.IdProducto,
                            cantidadAnterior, // Cantidad obtenida de la BD
                            producto.CantidadProducto, // La cantidad nueva viene del frontend
                            producto.Motivo ?? "Ajuste de inventario"
                        );
                    }

                    return Json(new { success = true, message = "Ajuste de inventario actualizado correctamente." });
                }
                else
                {
                    return Json(new { success = false, message = "No se pudo actualizar el ajuste de inventario." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar ajuste de inventario con ID: {Id}", request.IdAjusteInventario);
                return Json(new { success = false, message = "Error interno del servidor." });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                _logger.LogInformation("Iniciando eliminación de ajuste de inventario con ID: {Id}", id);
                
                // Verificar si el ajuste existe
                var existe = await _AjusteinventarioService.ExistsAsync(id);
                if (!existe)
                {
                    _logger.LogWarning("Ajuste de inventario con ID {Id} no existe", id);
                    return Json(new { success = false, message = "El ajuste de inventario no existe." });
                }

                // Eliminar el ajuste de inventario
                var eliminado = await _AjusteinventarioService.DeleteAsync(id);

                if (eliminado)
                {
                    _logger.LogInformation("Ajuste de inventario con ID {Id} eliminado exitosamente", id);
                    return Json(new { success = true, message = "Ajuste de inventario eliminado correctamente." });
                }
                else
                {
                    _logger.LogWarning("No se pudo eliminar el ajuste de inventario con ID {Id}", id);
                    return Json(new { success = false, message = "No se pudo eliminar el ajuste de inventario." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar ajuste de inventario con ID: {Id}. Detalles: {Message}", id, ex.Message);
                return Json(new { success = false, message = $"Error interno del servidor: {ex.Message}" });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
