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

        public async Task<IActionResult> Index(ProveedorFilterOptions filtros, int numeroPagina = 1)
        {
            // Usar los parámetros recibidos para consultar el servicio
            var resultado = await _ProveedorService.GetPagedAsync(
                numeroPagina,
                10,
                filtros
            );
            return View(resultado);
        }

        // GET: Proveedor/Create
        public IActionResult Create()
        {
            return ExecuteIfHasRole("Empleado", () =>
            {
                return View(new ProveedorModel());
            });
        }

        // POST: Proveedor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProveedorModel proveedor)
        {
            return await ExecuteIfHasRole("Administrador", async () =>
            {
                if (!ModelState.IsValid)
                {
                    return View(proveedor);
                }

                await _ProveedorService.CreateAsync(proveedor, CurrentUserId ?? "");
                TempData["Success"] = "Proveedor creado correctamente";
                return RedirectToAction(nameof(Index));
            });
        }

        // DELETE: Solo administradores
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            return await ExecuteIfAdmin(async () =>
            {
                await _ProveedorService.DeleteAsync(id);
                return Json(new { success = true });
            });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
