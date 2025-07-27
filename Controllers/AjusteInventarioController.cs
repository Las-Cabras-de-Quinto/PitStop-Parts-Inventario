using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Services;
using PitStop_Parts_Inventario.Models.ViewModels;



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
