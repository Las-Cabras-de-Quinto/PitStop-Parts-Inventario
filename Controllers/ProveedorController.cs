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

        // POST: Proveedor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] ProveedorModel proveedor)
        {
            return await ExecuteIfHasRole("Administrador", async () =>
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Datos inválidos", errors = ModelState });
                }

                try
                {
                    await _ProveedorService.CreateAsync(proveedor, CurrentUserId ?? "");
                    return Json(new { success = true, message = "Proveedor creado correctamente" });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al crear proveedor");
                    return Json(new { success = false, message = "Error interno del servidor" });
                }
            });
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
                return Json(new { success = true, data = proveedor });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener proveedor con ID: {Id}", id);
                return Json(new { success = false, message = "Error interno del servidor." });
            }
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar([FromBody] ProveedorModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Datos inválidos", errors = ModelState });
            }

            // Obtener el ID del usuario actual
            var userId = CurrentUserId ?? User?.Identity?.Name ?? string.Empty;

            try
            {
                var proveedorActualizado = await _ProveedorService.UpdateAsync(model, userId);
                if (proveedorActualizado != null)
                {
                    return Json(new { success = true, message = "Proveedor actualizado correctamente." });
                }
                else
                {
                    return Json(new { success = false, message = "No se pudo actualizar el proveedor." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar proveedor con ID: {Id}", model.IdProveedor);
                return Json(new { success = false, message = "Error interno del servidor." });
            }
        }

        // DELETE: Solo administradores
        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            return await ExecuteIfAdmin(async () =>
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
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
