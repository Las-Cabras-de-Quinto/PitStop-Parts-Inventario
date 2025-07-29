using Microsoft.AspNetCore.Mvc;
using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Models.ViewModels;
using PitStop_Parts_Inventario.Services;
using PitStop_Parts_Inventario.Services.Interfaces;
using System.Diagnostics;

namespace PitStop_Parts_Inventario.Controllers
{
    public class EntradaProductoController : BaseController
    {
        private readonly ILogger<EntradaProductoController> _logger;
        private readonly EntradaProductoService _entradaProductoService;

        public EntradaProductoController(ILogger<EntradaProductoController> logger, EntradaProductoService entradaProductoService)
        {
            _logger = logger;
            _entradaProductoService = entradaProductoService;
        }

        public async Task<IActionResult> Index(int numeroPagina, EntradaProductoFilterOptions filtros)
        {
            // Usar los parámetros recibidos para consultar el servicio
            var resultado = await _entradaProductoService.GetPagedAsync(
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
