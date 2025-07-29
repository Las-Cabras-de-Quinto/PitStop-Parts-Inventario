using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Services;
using PitStop_Parts_Inventario.Models.ViewModels;
using PitStop_Parts_Inventario.Services.Interfaces;



namespace PitStop_Parts_Inventario.Controllers
{
    public class ProveedorController : BaseController
    {
        private readonly ILogger<ProveedorController> _logger;
        private readonly IProveedorService _proveedorService;


        public ProveedorController(ILogger<ProveedorController> logger, IProveedorService proveedorService)
        {
            _logger = logger;
            _proveedorService = proveedorService;
        }

        public async Task<IActionResult> Index(int numeroPagina = 1, ProveedorFilterOptions? filtros = null)
        {
            // Validar que la página mínima sea 1 para evitar offset negativo
            if (numeroPagina < 1)
            {
                numeroPagina = 1;
            }

            filtros ??= new ProveedorFilterOptions();

            var resultado = await _proveedorService.GetPagedAsync(
                numeroPagina,
                4,
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

                await _proveedorService.CreateAsync(proveedor, CurrentUserId ?? "");
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
                await _proveedorService.DeleteAsync(id);
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
