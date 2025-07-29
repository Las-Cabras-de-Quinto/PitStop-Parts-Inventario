using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Services;
using PitStop_Parts_Inventario.Models.ViewModels;
using PitStop_Parts_Inventario.Services.Interfaces;
using System.IO;

namespace PitStop_Parts_Inventario.Controllers
{
    public class ProductoController : BaseController
    {
        private readonly ILogger<ProductoController> _logger;
        private readonly IProductoService _ProductoService;

        public ProductoController(ILogger<ProductoController> logger, ProductoService productoService)
        {
            _logger = logger;
            _ProductoService = productoService;
        }

        // GET: Producto
        public async Task<IActionResult> Index(int numeroPagina, ProductoFilterOptions filtros)
        {
            var productos = await _ProductoService.GetPagedAsync(numeroPagina, 10, filtros);
            return View(productos);
        }
        // GET: Producto/Details/5
        public async Task<IActionResult> Details(int id)
        {
            return await ExecuteIfHasRole("Empleado", async () => {
                var producto = await _ProductoService.GetByIdAsync(id);

                if (producto == null)
                {
                    return NotFound();
                }

                return View(producto);
            });
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
                    return BadRequest(ModelState);
                }

                await _ProductoService.CreateAsync(producto, CurrentUserId ?? "");
                TempData["Success"] = "Producto creado correctamente";
                return RedirectToAction(nameof(Index));
            });
        }

        // DELETE: Solo administradores
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            return await ExecuteIfAdmin(async () => {
                await _ProductoService.DeleteAsync(id);
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
