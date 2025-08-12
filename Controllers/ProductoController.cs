using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Services.Interfaces;
using PitStop_Parts_Inventario.Models.ViewModels;

namespace PitStop_Parts_Inventario.Controllers
{
    public class ProductoController : BaseController
    {
        private readonly ILogger<ProductoController> _logger;
        private readonly IProductoService _productoService;

        public ProductoController(ILogger<ProductoController> logger, IProductoService productoService)
        {
            _logger = logger;
            _productoService = productoService;
        }

        public async Task<IActionResult> Index(ProductoFilterOptions? filtros, int numeroPagina = 1)
        {
            // Inicializar filtros si son null para evitar errores en los servicios
            filtros ??= new ProductoFilterOptions();
            
            // Usar los parámetros recibidos para consultar el servicio
            var resultado = await _productoService.GetPagedAsync(
                numeroPagina,
                10,
                filtros
            );
            return View(resultado);
        }

        // Acción POST para procesar el formulario de creación
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([FromBody] ProductoEditRequest request)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Datos inválidos", errors = ModelState });
            }

            var userId = await GetCurrentUserIdAsync();
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "No se pudo obtener el usuario actual" });
            }

            try
            {
                // Crear el modelo de producto base
                var model = new ProductoModel
                {
                    Nombre = request.Nombre,
                    Descripcion = request.Descripcion,
                    SKU = request.SKU,
                    PrecioVenta = request.PrecioVenta,
                    PrecioCompra = request.PrecioCompra,
                    StockMin = request.StockMin,
                    StockMax = request.StockMax,
                    IdMarca = request.IdMarca,
                    IdEstado = request.IdEstado
                };

                var productoCreado = await _productoService.CreateAsync(model, userId);
                if (productoCreado != null)
                {
                    // Asignar proveedores si se proporcionaron
                    if (request.IdsProveedores?.Any() == true)
                    {
                        await _productoService.AsignarProveedoresAsync(productoCreado.IdProducto, request.IdsProveedores);
                    }

                    // Asignar categorías si se proporcionaron
                    if (request.IdsCategorias?.Any() == true)
                    {
                        await _productoService.AsignarCategoriasAsync(productoCreado.IdProducto, request.IdsCategorias);
                    }

                    return Json(new { success = true, message = "Producto creado correctamente." });
                }
                else
                {
                    return Json(new { success = false, message = "No se pudo crear el producto." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear producto");
                return Json(new { success = false, message = "Error interno del servidor." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            try
            {
                var producto = await _productoService.GetByIdAsync(id);
                if (producto == null)
                {
                    return Json(new { success = false, message = "El producto no existe." });
                }

                // Incluir las relaciones muchos a muchos
                var proveedores = producto.ProveedorProductos?.Where(pp => pp.Proveedor != null)
                    .Select(pp => new {
                        Id = pp.Proveedor!.IdProveedor,
                        Nombre = pp.Proveedor.Nombre
                    }).ToList();

                var categorias = producto.CategoriaProductos?.Where(cp => cp.Categoria != null)
                    .Select(cp => new {
                        Id = cp.Categoria!.IdCategoria,
                        Nombre = cp.Categoria.Nombre
                    }).ToList();

                var bodegas = producto.BodegaProductos?.Where(bp => bp.Bodega != null)
                    .Select(bp => new {
                        Id = bp.Bodega!.IdBodega,
                        Nombre = bp.Bodega.Nombre,
                        Stock = bp.StockTotal
                    }).ToList();

                var productoConRelaciones = new
                {
                    producto.IdProducto,
                    producto.Nombre,
                    producto.SKU,
                    producto.Descripcion,
                    producto.IdMarca,
                    producto.IdEstado,
                    producto.PrecioVenta,
                    producto.PrecioCompra,
                    producto.StockMin,
                    producto.StockMax,
                    producto.StockActual,
                    Marca = new {
                        Id = producto.Marca?.IdMarca,
                        Nombre = producto.Marca?.Nombre
                    },
                    Estado = new {
                        Id = producto.Estado?.IdEstado,
                        Nombre = producto.Estado?.Nombre
                    },
                    Proveedores = proveedores,
                    Categorias = categorias,
                    Bodegas = bodegas
                };

                return Json(new { success = true, data = productoConRelaciones });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener producto con ID: {Id}", id);
                return Json(new { success = false, message = "Error interno del servidor." });
            }
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar([FromBody] ProductoEditRequest request)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Datos inválidos", errors = ModelState });
            }

            var userId = await GetCurrentUserIdAsync();
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "No se pudo obtener el usuario actual" });
            }

            try
            {
                // Crear el modelo de producto base
                var model = new ProductoModel
                {
                    IdProducto = request.IdProducto,
                    Nombre = request.Nombre,
                    SKU = request.SKU,
                    Descripcion = request.Descripcion,
                    IdMarca = request.IdMarca,
                    IdEstado = request.IdEstado,
                    PrecioVenta = request.PrecioVenta,
                    PrecioCompra = request.PrecioCompra,
                    StockMin = request.StockMin,
                    StockMax = request.StockMax,
                    StockActual = request.StockActual
                };

                var productoActualizado = await _productoService.UpdateAsync(model, userId);
                if (productoActualizado != null)
                {
                    // Actualizar relaciones de proveedores si se proporcionaron
                    if (request.IdsProveedores?.Any() == true)
                    {
                        await _productoService.AsignarProveedoresAsync(request.IdProducto, request.IdsProveedores);
                    }

                    // Actualizar relaciones de categorías si se proporcionaron
                    if (request.IdsCategorias?.Any() == true)
                    {
                        await _productoService.AsignarCategoriasAsync(request.IdProducto, request.IdsCategorias);
                    }

                    return Json(new { 
                        success = true, 
                        message = "Producto actualizado correctamente."
                    });
                }
                else
                {
                    return Json(new { success = false, message = "No se pudo actualizar el producto." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar producto con ID: {Id}", request.IdProducto);
                return Json(new { success = false, message = "Error interno del servidor." });
            }
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                // Verificar si el producto existe
                var existe = await _productoService.ExistsAsync(id);
                if (!existe)
                {
                    return Json(new { success = false, message = "El producto no existe." });
                }

                // Eliminar el producto
                var eliminado = await _productoService.DeleteAsync(id);

                if (eliminado)
                {
                    return Json(new { success = true, message = "Producto eliminado correctamente." });
                }
                else
                {
                    return Json(new { success = false, message = "No se pudo eliminar el producto." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar producto con ID: {Id}", id);
                return Json(new { success = false, message = "Error interno del servidor." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerParaSelect()
        {
            try
            {
                var productos = await _productoService.GetAllAsync();
                var productosSelect = productos.Select(p => new { 
                    id = p.IdProducto, 
                    nombre = p.Nombre 
                });
                return Json(new { success = true, data = productosSelect });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos para select");
                return Json(new { success = false, message = "Error interno del servidor." });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
