using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Services;
using PitStop_Parts_Inventario.Models.ViewModels;
using PitStop_Parts_Inventario.Services.Interfaces;



namespace PitStop_Parts_Inventario.Controllers
{
    public class AjusteInventarioController : BaseController
    {
        private readonly ILogger<AjusteInventarioController> _logger;
        private readonly AjusteInventarioService _AjusteinventarioService;

        public AjusteInventarioController(ILogger<AjusteInventarioController> logger, AjusteInventarioService ajusteInventarioService)
        {
            _logger = logger;
            _AjusteinventarioService = ajusteInventarioService;
        }

        public async Task<IActionResult> Index(int numeroPagina , AjusteInventarioFilterOptions filtros)
        {
            // Usar los parámetros recibidos para consultar el servicio
            var resultado = await _AjusteinventarioService.GetPagedAsync(
                numeroPagina,
                10,
                filtros
            );
            return View(resultado);
        }
        // GET: Producto/Create
        public IActionResult Create()
        {
            return ExecuteIfHasRole("Empleado", () => {
                return View(new AjusteInventarioModel());
            });
        }

        // POST: Producto/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AjusteInventarioModel Ajuste)
        {
            return await ExecuteIfHasRole("Administrador", async () => {
                if (!ModelState.IsValid)
                {
                    return View(Ajuste);
                }

                await _AjusteinventarioService.CreateAsync(Ajuste,CurrentUserId ?? "");
                TempData["Success"] = "Producto creado correctamente";
                return RedirectToAction(nameof(Index));
            });
        }
        // DELETE: Solo administradores
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            return await ExecuteIfAdmin(async () => {
                await _AjusteinventarioService.DeleteAsync(id);
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
