using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PitStop_Parts_Inventario.Data;
using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Models.ViewModels;
using PitStop_Parts_Inventario.Services.Interfaces;

namespace PitStop_Parts_Inventario.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductoService _productoService;
        private readonly IBodegaService _bodegaService;
        private readonly IProveedorService _proveedorService;
        private readonly IEntradaProductoService _entradaProductoService;
        private readonly PitStopDbContext _context;

        public HomeController(
            ILogger<HomeController> logger,
            IProductoService productoService,
            IBodegaService bodegaService,
            IProveedorService proveedorService,
            IEntradaProductoService entradaProductoService,
            PitStopDbContext context)
        {
            _logger = logger;
            _productoService = productoService;
            _bodegaService = bodegaService;
            _proveedorService = proveedorService;
            _entradaProductoService = entradaProductoService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var dashboard = new DashboardViewModel();

                // Obtener datos de productos
                var todosProductos = await _productoService.GetAllAsync();
                dashboard.ProductosActivos = todosProductos.Count(p => p.IdEstado == 1);
                dashboard.ProductosInactivos = todosProductos.Count(p => p.IdEstado != 1);

                // Obtener datos de bodegas
                var todasBodegas = await _bodegaService.GetAllAsync();
                dashboard.TotalBodegas = todasBodegas.Count();
                // Para simplificar, asumimos que 2 bodegas no tienen espacio mínimo (puedes ajustar esta lógica)
                dashboard.BodegasSinEspacioMinimo = 2;

                // Obtener datos de proveedores
                var todosProveedores = await _proveedorService.GetAllAsync();
                dashboard.ProveedoresActivos = todosProveedores.Count(p => p.IdEstado == 1);
                dashboard.ProveedoresInactivos = todosProveedores.Count(p => p.IdEstado != 1);

                // Obtener datos de usuarios
                var totalUsuarios = await _context.Usuarios.CountAsync();
                dashboard.UsuariosDelSistema = totalUsuarios;
                // Para simplificar, asumimos que 2 usuarios están fuera de la empresa
                dashboard.UsuariosFueraEmpresa = 2;

                // Obtener últimas entradas (últimas 10)
                var hoy = DateTime.Today;
                var entradasPaginadas = await _entradaProductoService.GetPagedAsync(1, 10);
                dashboard.UltimasEntradas = entradasPaginadas.Data;

                // Obtener resumen del día
                var filtroHoy = new EntradaProductoFilterOptions
                {
                    FechaDesde = hoy,
                    FechaHasta = hoy.AddDays(1).AddTicks(-1)
                };
                var entradasHoy = await _entradaProductoService.GetAllAsync(filtroHoy);
                dashboard.EntradasHoy = entradasHoy.Count();
                dashboard.ProductosDistintosHoy = entradasHoy.Select(e => e.IdProducto).Distinct().Count();
                
                // Para proveedores involucrados, necesitamos obtenerlos a través de los productos
                var productosConEntradaHoy = entradasHoy.Select(e => e.IdProducto).Distinct().ToList();
                var proveedoresCount = 0;
                if (productosConEntradaHoy.Any())
                {
                    // Obtener proveedores únicos de los productos que tuvieron entradas hoy
                    var proveedoresIds = new HashSet<int>();
                    foreach (var productoId in productosConEntradaHoy)
                    {
                        var proveedoresProducto = await _productoService.GetProveedoresByProductoAsync(productoId);
                        foreach (var proveedor in proveedoresProducto)
                        {
                            proveedoresIds.Add(proveedor.IdProveedor);
                        }
                    }
                    proveedoresCount = proveedoresIds.Count;
                }
                dashboard.ProveedoresInvolucradosHoy = proveedoresCount;

                // Generar alertas del sistema
                var alertas = new List<string>();
                
                // Alerta por productos con stock bajo (menos de 10 unidades)
                var productosStockBajo = todosProductos.Where(p => p.StockActual < 10 && p.IdEstado == 1).Count();
                if (productosStockBajo > 0)
                {
                    alertas.Add($"{productosStockBajo} producto(s) con stock bajo (menos de 10 unidades)");
                }

                // Alerta por productos sin stock
                var productosSinStock = todosProductos.Where(p => p.StockActual == 0 && p.IdEstado == 1).Count();
                if (productosSinStock > 0)
                {
                    alertas.Add($"{productosSinStock} producto(s) sin stock disponible");
                }

                // Alerta por entradas sin registrar hoy
                if (dashboard.EntradasHoy == 0)
                {
                    alertas.Add("No se han registrado entradas de productos hoy");
                }

                dashboard.AlertasSistema = alertas;

                var user = await GetCurrentUserAsync();

                // La información del usuario ya está disponible a través del BaseController
                ViewBag.WelcomeMessage = $"Bienvenido, {user?.UserName ?? "Usuario"}";
                ViewBag.UserRole = user?.Rol?.Nombre ?? "Invitado";

                return View(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar el dashboard");
                
                // En caso de error, retornar un dashboard con datos por defecto
                var dashboardError = new DashboardViewModel
                {
                    ProductosActivos = 0,
                    ProductosInactivos = 0,
                    TotalBodegas = 0,
                    BodegasSinEspacioMinimo = 0,
                    ProveedoresActivos = 0,
                    ProveedoresInactivos = 0,
                    UsuariosDelSistema = 0,
                    UsuariosFueraEmpresa = 0,
                    EntradasHoy = 0,
                    ProductosDistintosHoy = 0,
                    ProveedoresInvolucradosHoy = 0,
                    AlertasSistema = new List<string> { "Error al cargar datos del sistema" }
                };

                var user = await GetCurrentUserAsync();

                ViewBag.WelcomeMessage = $"Bienvenido, {user?.UserName ?? "Usuario"}";
                ViewBag.UserRole = user?.Rol?.Nombre ?? "Sin rol asignado";

                return View(dashboardError);
            }
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
