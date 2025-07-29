using Microsoft.AspNetCore.Mvc;
using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Models.ViewModels;
using PitStop_Parts_Inventario.Services;
using PitStop_Parts_Inventario.Services.Interfaces;
using System.Diagnostics;

namespace PitStop_Parts_Inventario.Controllers
{
    public class RolController : BaseController
    {
        private readonly ILogger<RolController> _logger;
        private readonly IRolService _rolService;

        public RolController(ILogger<RolController> logger, IRolService rolService)
        {
            _logger = logger;
            _rolService = rolService;

        }

        public async Task<IActionResult> Index(RolFilterOptions filtros, int numeroPagina = 1)
        {
            // Usar los parámetros recibidos para consultar el servicio
            var resultado = await _rolService.GetPagedAsync(
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
