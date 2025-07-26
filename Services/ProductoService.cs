using Microsoft.EntityFrameworkCore;
using PitStop_Parts_Inventario.Data;
using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Models.ViewModels;
using PitStop_Parts_Inventario.Services.Interfaces;

namespace PitStop_Parts_Inventario.Services
{
    public class ProductoService : IProductoService
    {
        private readonly PitStopDbContext _context;

        public ProductoService(PitStopDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductoModel>> GetAllAsync()
        {
            return await _context.Productos
                .Include(p => p.Marca)
                .Include(p => p.Estado)
                .ToListAsync();
        }

        public async Task<PagedResult<ProductoModel>> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm = null)
        {
            var query = _context.Productos
                .Include(p => p.Marca)
                .Include(p => p.Estado)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.Nombre.Contains(searchTerm) || 
                                        (p.Descripcion != null && p.Descripcion.Contains(searchTerm)) || 
                                        p.SKU.ToString().Contains(searchTerm) ||
                                        (p.Marca != null && p.Marca.Nombre.Contains(searchTerm)));
            }

            var totalRecords = await query.CountAsync();
            var data = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ProductoModel>(data, totalRecords, pageNumber, pageSize);
        }

        public async Task<ProductoModel?> GetByIdAsync(int id)
        {
            return await _context.Productos
                .Include(p => p.Marca)
                .Include(p => p.Estado)
                .Include(p => p.CategoriaProductos!)
                    .ThenInclude(cp => cp.Categoria)
                .Include(p => p.ProveedorProductos!)
                    .ThenInclude(pp => pp.Proveedor)
                .Include(p => p.BodegaProductos!)
                    .ThenInclude(bp => bp.Bodega)
                .FirstOrDefaultAsync(p => p.IdProducto == id);
        }

        public async Task<ProductoModel> CreateAsync(ProductoModel producto, string userId)
        {
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
            return producto;
        }

        public async Task<ProductoModel> UpdateAsync(ProductoModel producto, string userId)
        {
            var existingProducto = await _context.Productos.FindAsync(producto.IdProducto);
            if (existingProducto == null)
                throw new ArgumentException("Producto no encontrado");

            existingProducto.Nombre = producto.Nombre;
            existingProducto.SKU = producto.SKU;
            existingProducto.Descripcion = producto.Descripcion;
            existingProducto.PrecioVenta = producto.PrecioVenta;
            existingProducto.PrecioCompra = producto.PrecioCompra;
            existingProducto.StockMin = producto.StockMin;
            existingProducto.StockMax = producto.StockMax;
            existingProducto.IdMarca = producto.IdMarca;
            existingProducto.IdEstado = producto.IdEstado;

            await _context.SaveChangesAsync();
            return existingProducto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
                return false;

            // Soft delete - cambiar estado en lugar de eliminar físicamente
            var estadoInactivo = await _context.Estados.FirstOrDefaultAsync(e => e.Nombre == "Inactivo");
            if (estadoInactivo != null)
            {
                producto.IdEstado = estadoInactivo.IdEstado;
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Productos.AnyAsync(p => p.IdProducto == id);
        }

        public async Task<IEnumerable<ProductoModel>> GetByMarcaAsync(int marcaId)
        {
            return await _context.Productos
                .Include(p => p.Marca)
                .Include(p => p.Estado)
                .Where(p => p.IdMarca == marcaId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductoModel>> GetByCategoriaAsync(int categoriaId)
        {
            return await _context.Productos
                .Include(p => p.Marca)
                .Include(p => p.Estado)
                .Include(p => p.CategoriaProductos!)
                .Where(p => p.CategoriaProductos != null && p.CategoriaProductos.Any(cp => cp.IdCategoria == categoriaId))
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductoModel>> GetByProveedorAsync(int proveedorId)
        {
            return await _context.Productos
                .Include(p => p.Marca)
                .Include(p => p.Estado)
                .Include(p => p.ProveedorProductos!)
                .Where(p => p.ProveedorProductos != null && p.ProveedorProductos.Any(pp => pp.IdProveedor == proveedorId))
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductoModel>> GetByBodegaAsync(int bodegaId)
        {
            return await _context.Productos
                .Include(p => p.Marca)
                .Include(p => p.Estado)
                .Include(p => p.BodegaProductos!)
                .Where(p => p.BodegaProductos != null && p.BodegaProductos.Any(bp => bp.IdBodega == bodegaId))
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductoModel>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return await GetAllAsync();

            return await _context.Productos
                .Include(p => p.Marca)
                .Include(p => p.Estado)
                .Where(p => p.Nombre.Contains(searchTerm) || 
                           (p.Descripcion != null && p.Descripcion.Contains(searchTerm)) || 
                           p.SKU.ToString().Contains(searchTerm) ||
                           (p.Marca != null && p.Marca.Nombre.Contains(searchTerm)))
                .ToListAsync();
        }

        /// <summary>
        /// Asigna múltiples proveedores a un producto, reemplazando las asignaciones existentes
        /// </summary>
        /// <param name="productoId">ID del producto</param>
        /// <param name="proveedorIds">Lista de IDs de proveedores</param>
        /// <returns>True si se asignaron exitosamente</returns>
        public async Task<bool> AsignarProveedoresAsync(int productoId, List<int> proveedorIds)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Remover asignaciones existentes
                var asignacionesExistentes = await _context.ProveedorProductos
                    .Where(pp => pp.IdProducto == productoId)
                    .ToListAsync();

                _context.ProveedorProductos.RemoveRange(asignacionesExistentes);

                // Agregar nuevas asignaciones
                foreach (var proveedorId in proveedorIds.Distinct())
                {
                    var proveedorProducto = new ProveedorProductoModel
                    {
                        IdProveedor = proveedorId,
                        IdProducto = productoId
                    };
                    _context.ProveedorProductos.Add(proveedorProducto);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Remueve la asignación de un proveedor específico a un producto
        /// </summary>
        /// <param name="productoId">ID del producto</param>
        /// <param name="proveedorId">ID del proveedor</param>
        /// <returns>True si se removió exitosamente</returns>
        public async Task<bool> RemoverProveedorAsync(int productoId, int proveedorId)
        {
            var proveedorProducto = await _context.ProveedorProductos
                .FirstOrDefaultAsync(pp => pp.IdProducto == productoId && pp.IdProveedor == proveedorId);

            if (proveedorProducto == null)
                return false;

            _context.ProveedorProductos.Remove(proveedorProducto);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Obtiene todos los proveedores asignados a un producto
        /// </summary>
        /// <param name="productoId">ID del producto</param>
        /// <returns>Lista de proveedores</returns>
        public async Task<IEnumerable<ProveedorModel>> GetProveedoresByProductoAsync(int productoId)
        {
            return await _context.ProveedorProductos
                .Where(pp => pp.IdProducto == productoId)
                .Include(pp => pp.Proveedor!)
                    .ThenInclude(p => p.Estado)
                .Select(pp => pp.Proveedor)
                .Where(p => p != null)
                .ToListAsync();
        }

        /// <summary>
        /// Recalcula el stock actual de un producto sumando el stock de todas las bodegas
        /// </summary>
        /// <param name="productoId">ID del producto</param>
        /// <returns>Stock recalculado</returns>
        public async Task<int> RecalcularStockAsync(int productoId)
        {
            // Sumar el stock total de todas las bodegas para este producto
            var stockTotal = await _context.BodegaProductos
                .Where(bp => bp.IdProducto == productoId)
                .SumAsync(bp => bp.StockTotal);

            // Actualizar el producto
            var producto = await _context.Productos.FindAsync(productoId);
            if (producto != null)
            {
                producto.StockActual = stockTotal < 0 ? 0 : stockTotal;
                _context.Productos.Update(producto);
                await _context.SaveChangesAsync();
                return producto.StockActual;
            }

            return 0;
        }

        /// <summary>
        /// Recalcula el stock de todos los productos en el sistema
        /// </summary>
        public async Task RecalcularTodosLosStocksAsync()
        {
            var productos = await _context.Productos.ToListAsync();
            
            foreach (var producto in productos)
            {
                await RecalcularStockAsync(producto.IdProducto);
            }
        }
    }
}
