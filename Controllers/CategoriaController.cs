using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Services;
using PitStop_Parts_Inventario.Models.ViewModels;

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

        public IActionResult Index()
        {
            return View();
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
