using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Models.ViewModels;
using PitStop_Parts_Inventario.Services;

namespace PitStop_Parts_Inventario.Controllers
{
    public class MarcaController : BaseController
    {
        private readonly ILogger<MarcaController> _logger;
        private readonly MarcaService _marcaService;

        public MarcaController(ILogger<MarcaController> logger, MarcaService marcaService)
        {
            _logger = logger;
            _marcaService = marcaService;
        }

        public async Task<IActionResult> Index(int numeroPagina, MarcaFilterOptions filtros)
        {
            // Usar los parámetros recibidos para consultar el servicio
            var resultado = await _marcaService.GetPagedAsync(
                numeroPagina,
                10,
                filtros
            );
            return View(resultado);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // ------------------- MÉTODOS AGREGADOS -------------------

        // GET: Marca/Create
        public IActionResult Create()
        {
            return ExecuteIfHasRole("Empleado", () =>
            {
                return View(new MarcaModel());
            });
        }

        // POST: Marca/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MarcaModel marca)
        {
            return await ExecuteIfHasRole("Administrador", async () =>
            {
                if (!ModelState.IsValid)
                {
                    return View(marca);
                }

                await _marcaService.CreateAsync(marca, CurrentUserId ?? "");
                TempData["Success"] = "Marca creada correctamente";
                return RedirectToAction(nameof(Index));
            });
        }

        // DELETE: Solo administradores
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            return await ExecuteIfAdmin(async () =>
            {
                await _marcaService.DeleteAsync(id);
                return Json(new { success = true });
            });
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
