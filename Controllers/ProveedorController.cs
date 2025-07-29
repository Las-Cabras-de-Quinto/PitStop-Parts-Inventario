using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Services;
using PitStop_Parts_Inventario.Models.ViewModels;



namespace PitStop_Parts_Inventario.Controllers
{
    public class ProveedorController : BaseController
    {
        private readonly ILogger<ProveedorController> _logger;
        private readonly ProveedorService _ProveedorService;

        public ProveedorController(ILogger<ProveedorController> logger, ProveedorService proveedorService)

        {
            _logger = logger;
            _ProveedorService = proveedorService;
        }

        public async Task<IActionResult> Index(int numeroPagina, ProveedorFilterOptions filtros)
        {
            // Usar los parámetros recibidos para consultar el servicio
            var resultado = await _ProveedorService.GetPagedAsync(
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
