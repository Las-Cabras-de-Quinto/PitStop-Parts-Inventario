using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Models.ViewModels;
using PitStop_Parts_Inventario.Services;
using PitStop_Parts_Inventario.Services.Interfaces;

namespace PitStop_Parts_Inventario.Controllers
{
    public class CategoriaController : BaseController
    {
       private readonly ILogger<CategoriaController> _logger;
       private readonly CategoriaService _CategoriaService;

       public CategoriaController(ILogger<CategoriaController> logger, CategoriaService categoriaService)
       {
                _logger = logger;
                _CategoriaService = categoriaService;
       }
        //GET: Categoria
        public async Task<IActionResult> Index(int numeroPagina, CategoriaFilterOptions filtros)
       {
            // Usar los parámetros recibidos para consultar el servicio
            var resultado = await _CategoriaService.GetPagedAsync(
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

        // GET: Categoria/Create
        public IActionResult Create()
        {
            return ExecuteIfHasRole("Empleado", () => {
                return View(new CategoriaModel());
            });
        }
        
        // POST: Categoria/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoriaModel categoria)
        {
            return await ExecuteIfHasRole("Administrador", async () => {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _CategoriaService.CreateAsync(categoria, CurrentUserId ?? "");
                TempData["Success"] = "Producto creado correctamente";
                return RedirectToAction(nameof(Index));
            });
        }

        // DELETE: Solo administradores
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            return await ExecuteIfAdmin(async () => {
                await _CategoriaService.DeleteAsync(id);
                return Json(new { success = true });
            });
        }
    }
}

