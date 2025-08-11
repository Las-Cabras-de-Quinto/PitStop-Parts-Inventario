using Microsoft.EntityFrameworkCore;
using PitStop_Parts_Inventario.Data;
using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Models.ViewModels;
using PitStop_Parts_Inventario.Services.Interfaces;
using PitStop_Parts_Inventario.Services.Helpers;

namespace PitStop_Parts_Inventario.Services
{
    public class EntradaProductoService : IEntradaProductoService
    {
        private readonly PitStopDbContext _context;
        private readonly IProductoService _productoService;

        public EntradaProductoService(PitStopDbContext context, IProductoService productoService)
        {
            _context = context;
            _productoService = productoService;
        }

        public async Task<IEnumerable<EntradaProductoModel>> GetAllAsync(EntradaProductoFilterOptions? filters = null)
        {
            var query = _context.EntradaProductos
                .Include(ep => ep.Producto).ThenInclude(p => p.Marca)
                .Include(ep => ep.Usuario)
                .Include(ep => ep.Bodega)
                .AsQueryable();

            query = FilterHelper.ApplyEntradaProductoFilters(query, filters);

            return await query.OrderByDescending(ep => ep.Fecha).ToListAsync();
        }

        public async Task<PagedResult<EntradaProductoModel>> GetPagedAsync(int pageNumber, int pageSize, EntradaProductoFilterOptions? filters = null)
        {
            var query = _context.EntradaProductos
                .Include(ep => ep.Producto)
                    .ThenInclude(p => p!.Marca)
                .Include(ep => ep.Usuario)
                .Include(ep => ep.Bodega)
                .AsQueryable();

            query = FilterHelper.ApplyEntradaProductoFilters(query, filters);

            var totalRecords = await query.CountAsync();
            var data = await query
                .OrderByDescending(ep => ep.Fecha)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<EntradaProductoModel>(data, totalRecords, pageNumber, pageSize);
        }

        public async Task<EntradaProductoModel?> GetByIdAsync(int id)
        {
            return await _context.EntradaProductos
                .Include(ep => ep.Producto).ThenInclude(p => p.Marca)
                .Include(ep => ep.Usuario)
                .Include(ep => ep.Bodega)
                .FirstOrDefaultAsync(ep => ep.IdEntrada == id);
        }

        /// <summary>
        /// Crea una nueva entrada de producto y actualiza el stock en la bodega automáticamente
        /// </summary>
        /// <param name="entradaProducto">Entrada de producto a crear</param>
        /// <param name="userId">ID del usuario que registra la entrada</param>
        /// <returns>Entrada de producto creada</returns>
        public async Task<EntradaProductoModel> CreateAsync(EntradaProductoModel entradaProducto, string userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                entradaProducto.Fecha = DateTime.Now;
                entradaProducto.IdUsuario = userId;

                // Agregar la entrada
                _context.EntradaProductos.Add(entradaProducto);

                // Actualizar el stock en BodegaProducto directamente
                var bodegaProducto = await _context.BodegaProductos
                    .FirstOrDefaultAsync(bp => bp.IdBodega == entradaProducto.IdBodega && bp.IdProducto == entradaProducto.IdProducto);

                if (bodegaProducto != null)
                {
                    bodegaProducto.StockTotal += entradaProducto.CantidadProducto;
                    _context.BodegaProductos.Update(bodegaProducto);
                }
                else
                {
                    // Si no existe el registro BodegaProducto, crear uno nuevo
                    bodegaProducto = new BodegaProductoModel
                    {
                        IdBodega = entradaProducto.IdBodega,
                        IdProducto = entradaProducto.IdProducto,
                        StockTotal = entradaProducto.CantidadProducto,
                        IdEstado = 1, // Estado activo por defecto
                        Descripcion = "Entrada de producto"
                    };
                    _context.BodegaProductos.Add(bodegaProducto);
                }

                // Guardar cambios de BodegaProducto primero
                await _context.SaveChangesAsync();

                // Recalcular el stock actual del producto con los datos actualizados
                var bodegasProducto = await _context.BodegaProductos
                    .Where(bp => bp.IdProducto == entradaProducto.IdProducto && bp.IdEstado == 1)
                    .ToListAsync();
                    
                var stockTotal = bodegasProducto.Sum(bp => bp.StockTotal);

                var producto = await _context.Productos.FindAsync(entradaProducto.IdProducto);
                if (producto != null)
                {
                    producto.StockActual = stockTotal < 0 ? 0 : stockTotal;
                    _context.Productos.Update(producto);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return entradaProducto;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Actualiza una entrada de producto existente y ajusta el stock en bodega según el cambio
        /// </summary>
        /// <param name="entradaProducto">Entrada de producto con datos actualizados</param>
        /// <param name="userId">ID del usuario que actualiza</param>
        /// <returns>Entrada de producto actualizada</returns>
        public async Task<EntradaProductoModel> UpdateAsync(EntradaProductoModel entradaProducto, string userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingEntrada = await _context.EntradaProductos.FindAsync(entradaProducto.IdEntrada);
                if (existingEntrada == null)
                    throw new ArgumentException("Entrada de producto no encontrada");

                // Calcular diferencia de cantidad para ajustar stock
                var diferenciaCantidad = entradaProducto.CantidadProducto - existingEntrada.CantidadProducto;
                var cambioProducto = entradaProducto.IdProducto != existingEntrada.IdProducto;
                var cambioBodega = entradaProducto.IdBodega != existingEntrada.IdBodega;

                // Si cambió el producto o bodega, revertir stock en la bodega anterior
                if (cambioProducto || cambioBodega)
                {
                    var bodegaAnterior = await _context.BodegaProductos
                        .FirstOrDefaultAsync(bp => bp.IdBodega == existingEntrada.IdBodega && bp.IdProducto == existingEntrada.IdProducto);
                    
                    if (bodegaAnterior != null)
                    {
                        bodegaAnterior.StockTotal -= existingEntrada.CantidadProducto;
                        if (bodegaAnterior.StockTotal < 0) bodegaAnterior.StockTotal = 0;
                        _context.BodegaProductos.Update(bodegaAnterior);
                    }
                }

                // Actualizar entrada
                existingEntrada.IdProducto = entradaProducto.IdProducto;
                existingEntrada.IdBodega = entradaProducto.IdBodega;
                existingEntrada.CantidadProducto = entradaProducto.CantidadProducto;

                // Actualizar stock en la bodega nueva o actual
                var bodegaActual = await _context.BodegaProductos
                    .FirstOrDefaultAsync(bp => bp.IdBodega == entradaProducto.IdBodega && bp.IdProducto == entradaProducto.IdProducto);

                if (bodegaActual != null)
                {
                    if (cambioProducto || cambioBodega)
                    {
                        // Si cambió producto o bodega, agregar toda la cantidad nueva
                        bodegaActual.StockTotal += entradaProducto.CantidadProducto;
                    }
                    else
                    {
                        // Si es la misma bodega y producto, agregar solo la diferencia
                        bodegaActual.StockTotal += diferenciaCantidad;
                    }
                    _context.BodegaProductos.Update(bodegaActual);
                }
                else
                {
                    // Crear nuevo registro BodegaProducto si no existe
                    bodegaActual = new BodegaProductoModel
                    {
                        IdBodega = entradaProducto.IdBodega,
                        IdProducto = entradaProducto.IdProducto,
                        StockTotal = entradaProducto.CantidadProducto,
                        IdEstado = 1,
                        Descripcion = "Entrada de producto actualizada"
                    };
                    _context.BodegaProductos.Add(bodegaActual);
                }

                // Guardar cambios de BodegaProducto primero
                await _context.SaveChangesAsync();

                // Recalcular stock de productos afectados con los datos actualizados
                var productosAfectados = new HashSet<int> { existingEntrada.IdProducto, entradaProducto.IdProducto };
                
                foreach (var productoId in productosAfectados)
                {
                    var bodegasProducto = await _context.BodegaProductos
                        .Where(bp => bp.IdProducto == productoId && bp.IdEstado == 1)
                        .ToListAsync();
                        
                    var stockTotal = bodegasProducto.Sum(bp => bp.StockTotal);

                    var producto = await _context.Productos.FindAsync(productoId);
                    if (producto != null)
                    {
                        producto.StockActual = stockTotal < 0 ? 0 : stockTotal;
                        _context.Productos.Update(producto);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return existingEntrada;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Elimina una entrada de producto y revierte el stock en la bodega correspondiente
        /// </summary>
        /// <param name="id">ID de la entrada a eliminar</param>
        /// <returns>True si se eliminó exitosamente</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var entradaProducto = await _context.EntradaProductos.FindAsync(id);
                if (entradaProducto == null)
                    return false;

                // Revertir el stock en BodegaProducto directamente
                var bodegaProducto = await _context.BodegaProductos
                    .FirstOrDefaultAsync(bp => bp.IdBodega == entradaProducto.IdBodega && bp.IdProducto == entradaProducto.IdProducto);
                
                if (bodegaProducto != null)
                {
                    bodegaProducto.StockTotal -= entradaProducto.CantidadProducto;
                    // Asegurar que el stock no sea negativo
                    if (bodegaProducto.StockTotal < 0)
                        bodegaProducto.StockTotal = 0;
                    
                    _context.BodegaProductos.Update(bodegaProducto);
                }

                // Guardar cambios de BodegaProducto primero
                await _context.SaveChangesAsync();

                // Recalcular el stock actual del producto con los datos actualizados
                var bodegasProducto = await _context.BodegaProductos
                    .Where(bp => bp.IdProducto == entradaProducto.IdProducto && bp.IdEstado == 1)
                    .ToListAsync();
                    
                var stockTotal = bodegasProducto.Sum(bp => bp.StockTotal);

                var producto = await _context.Productos.FindAsync(entradaProducto.IdProducto);
                if (producto != null)
                {
                    producto.StockActual = stockTotal < 0 ? 0 : stockTotal;
                    _context.Productos.Update(producto);
                }

                _context.EntradaProductos.Remove(entradaProducto);
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

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.EntradaProductos.AnyAsync(ep => ep.IdEntrada == id);
        }

        [Obsolete("Use GetAllAsync with EntradaProductoFilterOptions instead")]
        public async Task<IEnumerable<EntradaProductoModel>> GetByUsuarioAsync(string userId)
        {
            // Note: This would need the user ID in the filter, but our current filter doesn't support it
            // For now, keep the original implementation
            return await _context.EntradaProductos
                .Include(ep => ep.Producto).ThenInclude(p => p.Marca)
                .Include(ep => ep.Usuario)
                .Include(ep => ep.Bodega)
                .Where(ep => ep.IdUsuario == userId)
                .OrderByDescending(ep => ep.Fecha)
                .ToListAsync();
        }

        [Obsolete("Use GetAllAsync with EntradaProductoFilterOptions instead")]
        public async Task<IEnumerable<EntradaProductoModel>> GetByFechaAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            return await GetAllAsync(new EntradaProductoFilterOptions 
            { 
                FechaDesde = fechaInicio, 
                FechaHasta = fechaFin 
            });
        }

        [Obsolete("Use GetAllAsync with EntradaProductoFilterOptions instead")]
        public async Task<IEnumerable<EntradaProductoModel>> GetByProductoAsync(int productoId)
        {
            return await GetAllAsync(new EntradaProductoFilterOptions { ProductoId = productoId });
        }

        /// <summary>
        /// Obtiene las entradas de producto por ID de producto (alias para compatibilidad)
        /// </summary>
        /// <param name="productoId">ID del producto</param>
        /// <returns>Lista de entradas del producto</returns>
        [Obsolete("Use GetAllAsync with EntradaProductoFilterOptions instead")]
        public async Task<IEnumerable<EntradaProductoModel>> GetByProductoIdAsync(int productoId)
        {
            return await GetAllAsync(new EntradaProductoFilterOptions { ProductoId = productoId });
        }

        /// <summary>
        /// Obtiene las entradas de producto por ID de proveedor
        /// </summary>
        /// <param name="proveedorId">ID del proveedor</param>
        /// <returns>Lista de entradas del proveedor</returns>
        [Obsolete("Use GetAllAsync with EntradaProductoFilterOptions instead")]
        public async Task<IEnumerable<EntradaProductoModel>> GetByProveedorIdAsync(int proveedorId)
        {
            // Note: This would need a proveedor filter in EntradaProducto, but it's not directly related
            // For now, keep the original implementation
            var productosProveedor = await _context.ProveedorProductos
                .Where(pp => pp.IdProveedor == proveedorId)
                .Select(pp => pp.IdProducto)
                .ToListAsync();

            return await _context.EntradaProductos
                .Include(ep => ep.Producto).ThenInclude(p => p.Marca)
                .Include(ep => ep.Usuario)
                .Include(ep => ep.Bodega)
                .Where(ep => productosProveedor.Contains(ep.IdProducto))
                .OrderByDescending(ep => ep.Fecha)
                .ToListAsync();
        }
    }
}
