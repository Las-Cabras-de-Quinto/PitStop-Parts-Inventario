using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Services;
using PitStop_Parts_Inventario.Models.ViewModels;

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
