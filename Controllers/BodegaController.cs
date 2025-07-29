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
        private readonly BodegaService _bodegaService;

        public BodegaController(ILogger<BodegaController> logger, BodegaService bodegaService)
        {
            _logger = logger;
            _bodegaService = bodegaService;
        }

        public async Task<IActionResult> Index(int numeroPagina, BodegaFilterOptions filtros)
        {
            // Usar los par�metros recibidos para consultar el servicio
            var resultado = await _bodegaService.GetPagedAsync(
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
                return View(new BodegaModel());
            });
        }

        // POST: Producto/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BodegaModel bodega)
        {
            return await ExecuteIfHasRole("Administrador", async () => {
                if (!ModelState.IsValid)
                {
                    return View(bodega);
                }

                await _bodegaService.CreateAsync(bodega, CurrentUserId ?? "");
                TempData["Success"] = "Producto creado correctamente";
                return RedirectToAction(nameof(Index));
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
