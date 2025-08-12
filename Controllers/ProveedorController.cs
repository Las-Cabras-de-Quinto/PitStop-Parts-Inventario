using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Services;
using PitStop_Parts_Inventario.Services.Interfaces;
using PitStop_Parts_Inventario.Models.ViewModels;



namespace PitStop_Parts_Inventario.Controllers
{
    public class ProveedorController : BaseController
    {
        private readonly ILogger<ProveedorController> _logger;
        private readonly IProveedorService _ProveedorService;

        public ProveedorController(ILogger<ProveedorController> logger, IProveedorService proveedorService)

        {
            _logger = logger;
            _ProveedorService = proveedorService;
        }

        public async Task<IActionResult> Index(ProveedorFilterOptions? filtros, int numeroPagina = 1)
        {
            // Inicializar filtros si son null para evitar errores en los servicios
            filtros ??= new ProveedorFilterOptions();
            
            // Usar los parámetros recibidos para consultar el servicio
            var resultado = await _ProveedorService.GetPagedAsync(
                numeroPagina,
                10,
                filtros
            );
            return View(resultado);
        }

        // POST: Proveedor/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([FromBody] ProveedorEditRequest request)
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
                // Crear el modelo base
                var proveedor = new ProveedorModel
                {
                    Nombre = request.Nombre,
                    Contacto = request.Contacto,
                    Direccion = request.Direccion,
                    IdEstado = request.IdEstado
                };

                await _ProveedorService.CreateAsync(proveedor, userId ?? "");
                return Json(new { success = true, message = "Proveedor creado correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear proveedor");
                return Json(new { success = false, message = "Error interno del servidor" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            try
            {
                var proveedor = await _ProveedorService.GetByIdAsync(id);
                if (proveedor == null)
                {
                    return Json(new { success = false, message = "El proveedor no existe." });
                }

                // Incluir los productos relacionados
                var productos = proveedor.ProveedorProductos?.Select(pp => new {
                    Id = pp.Producto.IdProducto,
                    Nombre = pp.Producto.Nombre
                }).ToList();

                var proveedorConRelaciones = new
                {
                    proveedor.IdProveedor,
                    proveedor.Nombre,
                    proveedor.Contacto,
                    proveedor.Direccion,
                    proveedor.IdEstado,
                    proveedor.Estado,
                    Productos = productos
                };

                return Json(new { success = true, data = proveedorConRelaciones });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener proveedor con ID: {Id}", id);
                return Json(new { success = false, message = "Error interno del servidor." });
            }
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar([FromBody] ProveedorEditRequest request)
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
                // Crear el modelo de proveedor base
                var model = new ProveedorModel
                {
                    IdProveedor = request.IdProveedor,
                    Nombre = request.Nombre,
                    Contacto = request.Contacto,
                    Direccion = request.Direccion,
                    IdEstado = request.IdEstado
                };

                var proveedorActualizado = await _ProveedorService.UpdateAsync(model, userId);
                if (proveedorActualizado != null)
                {
                    // Actualizar relaciones de productos si se proporcionaron
                    if (request.IdsProductos?.Any() == true)
                    {
                        foreach (var productoId in request.IdsProductos)
                        {
                            await _ProveedorService.AsignarProductoAsync(request.IdProveedor, productoId);
                        }
                    }

                    return Json(new { 
                        success = true, 
                        message = "Proveedor actualizado correctamente."
                    });
                }
                else
                {
                    return Json(new { success = false, message = "No se pudo actualizar el proveedor." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar proveedor con ID: {Id}", request.IdProveedor);
                return Json(new { success = false, message = "Error interno del servidor." });
            }
        }

        // DELETE: Solo administradores
        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                await _ProveedorService.DeleteAsync(id);
                return Json(new { success = true, message = "Proveedor eliminado correctamente." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar proveedor con ID: {Id}", id);
                return Json(new { success = false, message = "Error interno del servidor." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerParaSelect()
        {
            try
            {
                var proveedores = await _ProveedorService.GetAllAsync();
                var proveedoresSelect = proveedores.Select(p => new { 
                    id = p.IdProveedor, 
                    nombre = p.Nombre 
                });
                return Json(new { success = true, data = proveedoresSelect });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener proveedores para select");
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
