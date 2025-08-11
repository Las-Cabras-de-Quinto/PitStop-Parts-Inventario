using Microsoft.EntityFrameworkCore;
using PitStop_Parts_Inventario.Data;
using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Models.ViewModels;
using PitStop_Parts_Inventario.Services.Interfaces;
using PitStop_Parts_Inventario.Services.Helpers;

namespace PitStop_Parts_Inventario.Services
{
    public class ProductoService : IProductoService
    {
        private readonly PitStopDbContext _context;

        public ProductoService(PitStopDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductoModel>> GetAllAsync(ProductoFilterOptions? filters = null)
        {
            var query = _context.Productos
                .Include(p => p.Marca)
                .Include(p => p.Estado)
                .Include(p => p.CategoriaProductos!)
                    .ThenInclude(cp => cp.Categoria)
                .Include(p => p.ProveedorProductos!)
                    .ThenInclude(pp => pp.Proveedor)
                .Include(p => p.BodegaProductos!)
                    .ThenInclude(bp => bp.Bodega)
                .AsQueryable();

            query = FilterHelper.ApplyProductoFilters(query, filters);

            return await query.ToListAsync();
        }

        public async Task<PagedResult<ProductoModel>> GetPagedAsync(int pageNumber, int pageSize, ProductoFilterOptions? filters = null)
        {
            var query = _context.Productos
                .Include(p => p.Marca)
                .Include(p => p.Estado)
                .Include(p => p.CategoriaProductos!)
                    .ThenInclude(cp => cp.Categoria)
                .Include(p => p.ProveedorProductos!)
                    .ThenInclude(pp => pp.Proveedor)
                .Include(p => p.BodegaProductos!)
                    .ThenInclude(bp => bp.Bodega)
                .AsQueryable();

            query = FilterHelper.ApplyProductoFilters(query, filters);

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

        [Obsolete("Use GetAllAsync with ProductoFilterOptions instead")]
        public async Task<IEnumerable<ProductoModel>> GetByMarcaAsync(int marcaId)
        {
            return await GetAllAsync(new ProductoFilterOptions { MarcaId = marcaId });
        }

        [Obsolete("Use GetAllAsync with ProductoFilterOptions instead")]
        public async Task<IEnumerable<ProductoModel>> GetByCategoriaAsync(int categoriaId)
        {
            return await GetAllAsync(new ProductoFilterOptions { CategoriaId = categoriaId });
        }

        [Obsolete("Use GetAllAsync with ProductoFilterOptions instead")]
        public async Task<IEnumerable<ProductoModel>> GetByProveedorAsync(int proveedorId)
        {
            return await GetAllAsync(new ProductoFilterOptions { ProveedorId = proveedorId });
        }

        [Obsolete("Use GetAllAsync with ProductoFilterOptions instead")]
        public async Task<IEnumerable<ProductoModel>> GetByBodegaAsync(int bodegaId)
        {
            return await GetAllAsync(new ProductoFilterOptions { BodegaId = bodegaId });
        }

        [Obsolete("Use GetAllAsync with ProductoFilterOptions instead")]
        public async Task<IEnumerable<ProductoModel>> SearchAsync(string searchTerm)
        {
            return await GetAllAsync(new ProductoFilterOptions { SearchTerm = searchTerm });
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
        /// Este método solo debe usarse cuando sea necesario sincronizar el stock actual
        /// </summary>
        /// <param name="productoId">ID del producto</param>
        /// <returns>Stock recalculado</returns>
        public async Task<int> RecalcularStockAsync(int productoId)
        {
            return await RecalcularStockInternoAsync(productoId, true);
        }

        /// <summary>
        /// Recalcula el stock actual de un producto sin guardar automáticamente
        /// Útil cuando se hace dentro de una transacción existente
        /// </summary>
        /// <param name="productoId">ID del producto</param>
        /// <returns>Stock recalculado</returns>
        public async Task<int> RecalcularStockSinGuardarAsync(int productoId)
        {
            return await RecalcularStockInternoAsync(productoId, false);
        }

        /// <summary>
        /// Método interno para recalcular stock con opción de guardar
        /// </summary>
        private async Task<int> RecalcularStockInternoAsync(int productoId, bool guardar)
        {
            // Sumar el stock total de todas las bodegas activas para este producto
            var stockTotal = await _context.BodegaProductos
                .Where(bp => bp.IdProducto == productoId && bp.IdEstado == 1)
                .SumAsync(bp => bp.StockTotal);

            // Actualizar el producto
            var producto = await _context.Productos.FindAsync(productoId);
            if (producto != null)
            {
                producto.StockActual = stockTotal < 0 ? 0 : stockTotal;
                _context.Productos.Update(producto);
                
                if (guardar)
                {
                    await _context.SaveChangesAsync();
                }
                
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
                await RecalcularStockSinGuardarAsync(producto.IdProducto);
            }
            
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Actualiza el stock en BodegaProducto y recalcula el stock total del producto
        /// Este es el método principal para actualizaciones de stock
        /// </summary>
        /// <param name="bodegaId">ID de la bodega</param>
        /// <param name="productoId">ID del producto</param>
        /// <param name="cantidad">Cantidad a agregar o quitar (puede ser negativa)</param>
        /// <param name="descripcion">Descripción del movimiento</param>
        public async Task ActualizarStockBodegaAsync(int bodegaId, int productoId, int cantidad, string descripcion = "")
        {
            var bodegaProducto = await _context.BodegaProductos
                .FirstOrDefaultAsync(bp => bp.IdBodega == bodegaId && bp.IdProducto == productoId);

            if (bodegaProducto != null)
            {
                bodegaProducto.StockTotal += cantidad;
                
                // Asegurar que el stock no sea negativo
                if (bodegaProducto.StockTotal < 0)
                    bodegaProducto.StockTotal = 0;
                    
                if (!string.IsNullOrEmpty(descripcion))
                    bodegaProducto.Descripcion = descripcion;
                    
                _context.BodegaProductos.Update(bodegaProducto);
            }
            else if (cantidad > 0)
            {
                // Solo crear nuevo registro si la cantidad es positiva
                bodegaProducto = new BodegaProductoModel
                {
                    IdBodega = bodegaId,
                    IdProducto = productoId,
                    StockTotal = cantidad,
                    IdEstado = 1, // Estado activo por defecto
                    Descripcion = string.IsNullOrEmpty(descripcion) ? "Stock inicial" : descripcion
                };
                _context.BodegaProductos.Add(bodegaProducto);
            }

            // Recalcular el stock total del producto
            await RecalcularStockSinGuardarAsync(productoId);
        }

        /// <summary>
        /// Asigna múltiples categorías a un producto
        /// </summary>
        /// <param name="productoId">ID del producto</param>
        /// <param name="categoriaIds">Lista de IDs de categorías</param>
        /// <returns>True si se asignaron exitosamente</returns>
        public async Task<bool> AsignarCategoriasAsync(int productoId, List<int> categoriaIds)
        {
            try
            {
                // Remover todas las categorías existentes del producto
                var categoriasExistentes = await _context.CategoriaProductos
                    .Where(cp => cp.IdProducto == productoId)
                    .ToListAsync();
                
                _context.CategoriaProductos.RemoveRange(categoriasExistentes);

                // Agregar las nuevas categorías
                foreach (var categoriaId in categoriaIds)
                {
                    var categoriaProducto = new CategoriaProductoModel
                    {
                        IdProducto = productoId,
                        IdCategoria = categoriaId
                    };
                    _context.CategoriaProductos.Add(categoriaProducto);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Remueve una categoría específica de un producto
        /// </summary>
        /// <param name="productoId">ID del producto</param>
        /// <param name="categoriaId">ID de la categoría a remover</param>
        /// <returns>True si se removió exitosamente</returns>
        public async Task<bool> RemoverCategoriaAsync(int productoId, int categoriaId)
        {
            var categoriaProducto = await _context.CategoriaProductos
                .FirstOrDefaultAsync(cp => cp.IdProducto == productoId && cp.IdCategoria == categoriaId);

            if (categoriaProducto == null)
                return false;

            _context.CategoriaProductos.Remove(categoriaProducto);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Obtiene todas las categorías asignadas a un producto
        /// </summary>
        /// <param name="productoId">ID del producto</param>
        /// <returns>Lista de categorías</returns>
        public async Task<IEnumerable<CategoriaModel>> GetCategoriasByProductoAsync(int productoId)
        {
            return await _context.CategoriaProductos
                .Where(cp => cp.IdProducto == productoId)
                .Include(cp => cp.Categoria!)
                    .ThenInclude(c => c.Estado)
                .Select(cp => cp.Categoria)
                .Where(c => c != null)
                .ToListAsync();
        }
    }
}
