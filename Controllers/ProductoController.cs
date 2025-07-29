using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Services;
using PitStop_Parts_Inventario.Models.ViewModels;
using PitStop_Parts_Inventario.Services.Interfaces;

namespace PitStop_Parts_Inventario.Controllers
{
    public class ProductoController : BaseController
    {
        private readonly ILogger<ProductoController> _logger;
        private readonly IProductoService _productoService;

        public ProductoController(ILogger<ProductoController> logger, ProductoService productoService)
        {
            _logger = logger;
            _productoService = productoService;
        }
        public async Task<IActionResult> Index(int numeroPagina, ProductoFilterOptions filtros)
        {
            // Usar los parámetros recibidos para consultar el servicio
            var resultado = await _productoService.GetPagedAsync(
                numeroPagina,
                10,
                filtros
            );
            return View(resultado);
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
        // GET: Producto/Create
        public IActionResult Create()
        {
            return ExecuteIfHasRole("Empleado", () => {
                return View(new ProductoModel());
            });
        }
        // POST: Producto/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductoModel producto)
        {
            return await ExecuteIfHasRole("Administrador", async () => {
                if (!ModelState.IsValid)
                {
                    return View(producto);
                }

                await _productoService.CreateAsync(producto);
                TempData["Success"] = "Producto creado correctamente";
                return RedirectToAction(nameof(Index));
            });
        }

        // DELETE: Solo administradores
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            return await ExecuteIfAdmin(async () => {
                await _productoService.DeleteAsync(id);
                return Json(new { success = true });
            });
        }
    }
}
