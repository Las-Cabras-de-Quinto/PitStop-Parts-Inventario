using Microsoft.EntityFrameworkCore;
using PitStop_Parts_Inventario.Data;
using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Models.ViewModels;
using PitStop_Parts_Inventario.Services.Interfaces;
using PitStop_Parts_Inventario.Services.Helpers;

namespace PitStop_Parts_Inventario.Services
{
    public class AjusteInventarioService : IAjusteInventarioService
    {
        private readonly PitStopDbContext _context;
        private readonly IProductoService _productoService;

        public AjusteInventarioService(PitStopDbContext context, IProductoService productoService)
        {
            _context = context;
            _productoService = productoService;
        }

        public async Task<IEnumerable<AjusteInventarioModel>> GetAllAsync(AjusteInventarioFilterOptions? filters = null)
        {
            var query = _context.AjusteInventarios
                .Include(ai => ai.Usuario)
                .Include(ai => ai.Bodega)
                .Include(ai => ai.AjusteInventarioProductos).ThenInclude(aip => aip.Producto)
                .AsQueryable();

            query = FilterHelper.ApplyAjusteInventarioFilters(query, filters);

            return await query.OrderByDescending(ai => ai.Fecha).ToListAsync();
        }

        public async Task<PagedResult<AjusteInventarioModel>> GetPagedAsync(int pageNumber, int pageSize, AjusteInventarioFilterOptions? filters = null)
        {
            var query = _context.AjusteInventarios
                .Include(ai => ai.Usuario)
                .Include(ai => ai.Bodega)
                .Include(ai => ai.AjusteInventarioProductos).ThenInclude(aip => aip.Producto)
                .AsQueryable();

            query = FilterHelper.ApplyAjusteInventarioFilters(query, filters);

            var totalRecords = await query.CountAsync();
            var data = await query
                .OrderByDescending(ai => ai.Fecha)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<AjusteInventarioModel>(data, totalRecords, pageNumber, pageSize);
        }

        public async Task<AjusteInventarioModel?> GetByIdAsync(int id)
        {
            return await _context.AjusteInventarios
                .Include(ai => ai.Usuario)
                .Include(ai => ai.Bodega)
                .Include(ai => ai.AjusteInventarioProductos).ThenInclude(aip => aip.Producto).ThenInclude(p => p.Marca)
                .FirstOrDefaultAsync(ai => ai.IdAjusteInventario == id);
        }

        /// <summary>
        /// Crea un nuevo ajuste de inventario
        /// </summary>
        /// <param name="ajusteInventario">Ajuste de inventario a crear</param>
        /// <param name="userId">ID del usuario que crea el ajuste</param>
        /// <returns>Ajuste de inventario creado</returns>
        public async Task<AjusteInventarioModel> CreateAsync(AjusteInventarioModel ajusteInventario, string userId)
        {
            ajusteInventario.Fecha = DateTime.Now;
            ajusteInventario.IdUsuario = userId;

            _context.AjusteInventarios.Add(ajusteInventario);
            await _context.SaveChangesAsync();
            return ajusteInventario;
        }

        public async Task<AjusteInventarioModel> UpdateAsync(AjusteInventarioModel ajusteInventario, string userId)
        {
            var existingAjuste = await _context.AjusteInventarios.FindAsync(ajusteInventario.IdAjusteInventario);
            if (existingAjuste == null)
                throw new ArgumentException("Ajuste de inventario no encontrado");

            existingAjuste.IdBodega = ajusteInventario.IdBodega;

            await _context.SaveChangesAsync();
            return existingAjuste;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var ajusteInventario = await _context.AjusteInventarios
                    .Include(ai => ai.AjusteInventarioProductos)
                    .FirstOrDefaultAsync(ai => ai.IdAjusteInventario == id);

                if (ajusteInventario == null)
                    return false;

                // Primero eliminar todos los productos del ajuste y revertir cambios en stock
                if (ajusteInventario.AjusteInventarioProductos != null)
                {
                    foreach (var ajusteProducto in ajusteInventario.AjusteInventarioProductos.ToList())
                    {
                        // Revertir cambios en el stock antes de eliminar
                        await RevertirCambiosStockAsync(ajusteInventario.IdBodega, ajusteProducto.IdProducto, ajusteProducto.CantidadProducto);
                        
                        // Eliminar el registro de ajuste-producto
                        _context.AjusteInventarioProductos.Remove(ajusteProducto);
                    }
                }

                // Luego eliminar el ajuste principal
                _context.AjusteInventarios.Remove(ajusteInventario);
                
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
        /// Revierte los cambios de stock realizados por un ajuste
        /// </summary>
        private async Task RevertirCambiosStockAsync(int idBodega, int idProducto, int cantidadAjuste)
        {
            // Revertir usando el método centralizado (cantidad negativa para revertir)
            await _productoService.ActualizarStockBodegaAsync(
                idBodega, 
                idProducto, 
                -cantidadAjuste, 
                "Reversión de ajuste de inventario"
            );
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.AjusteInventarios.AnyAsync(ai => ai.IdAjusteInventario == id);
        }

        [Obsolete("Use GetAllAsync with AjusteInventarioFilterOptions instead")]
        public async Task<IEnumerable<AjusteInventarioModel>> GetByUsuarioAsync(string userId)
        {
            // Note: This would need the user ID in the filter, but our current filter doesn't support it
            // For now, keep the original implementation
            return await _context.AjusteInventarios
                .Include(ai => ai.Usuario)
                .Include(ai => ai.Bodega)
                .Include(ai => ai.AjusteInventarioProductos).ThenInclude(aip => aip.Producto)
                .Where(ai => ai.IdUsuario == userId)
                .OrderByDescending(ai => ai.Fecha)
                .ToListAsync();
        }

        [Obsolete("Use GetAllAsync with AjusteInventarioFilterOptions instead")]
        public async Task<IEnumerable<AjusteInventarioModel>> GetByFechaAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            return await GetAllAsync(new AjusteInventarioFilterOptions 
            { 
                FechaDesde = fechaInicio, 
                FechaHasta = fechaFin 
            });
        }

        /// <summary>
        /// Agrega un producto a un ajuste de inventario y actualiza el stock en la bodega correspondiente
        /// </summary>
        /// <param name="ajusteId">ID del ajuste de inventario</param>
        /// <param name="productoId">ID del producto</param>
        /// <param name="cantidadAnterior">Cantidad anterior en inventario</param>
        /// <param name="cantidadNueva">Nueva cantidad después del ajuste</param>
        /// <param name="motivo">Motivo del ajuste</param>
        /// <returns>True si se agregó exitosamente</returns>
        public async Task<bool> AgregarProductoAsync(int ajusteId, int productoId, int cantidadAnterior, int cantidadNueva, string motivo)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Obtener el ajuste para saber en qué bodega trabajar
                var ajuste = await _context.AjusteInventarios.FindAsync(ajusteId);
                if (ajuste == null)
                    return false;

                // Calcular diferencia (puede ser positiva o negativa)
                var diferencia = cantidadNueva - cantidadAnterior;

                var ajusteProducto = new AjusteInventarioProductoModel
                {
                    IdAjusteInventario = ajusteId,
                    IdProducto = productoId,
                    CantidadProducto = diferencia // Guardar la diferencia, no la cantidad nueva
                };

                _context.AjusteInventarioProductos.Add(ajusteProducto);

                // Actualizar stock usando el método centralizado
                await _productoService.ActualizarStockBodegaAsync(
                    ajuste.IdBodega, 
                    productoId, 
                    diferencia, 
                    $"Ajuste de inventario: {motivo}"
                );

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
        /// Remueve un producto de un ajuste de inventario y revierte el cambio de stock en la bodega
        /// </summary>
        /// <param name="ajusteId">ID del ajuste de inventario</param>
        /// <param name="productoId">ID del producto</param>
        /// <returns>True si se removió exitosamente</returns>
        public async Task<bool> RemoverProductoAsync(int ajusteId, int productoId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var ajusteProducto = await _context.AjusteInventarioProductos
                    .FirstOrDefaultAsync(aip => aip.IdAjusteInventario == ajusteId && aip.IdProducto == productoId);

                if (ajusteProducto == null)
                    return false;

                // Obtener el ajuste para saber en qué bodega trabajar
                var ajuste = await _context.AjusteInventarios.FindAsync(ajusteId);
                if (ajuste == null)
                    return false;

                // Revertir el cambio usando el método centralizado
                await _productoService.ActualizarStockBodegaAsync(
                    ajuste.IdBodega, 
                    productoId, 
                    -ajusteProducto.CantidadProducto, 
                    "Eliminación de producto de ajuste"
                );

                _context.AjusteInventarioProductos.Remove(ajusteProducto);
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

        public async Task<int> ObtenerStockActualAsync(int idBodega, int idProducto)
        {
            var bodegaProducto = await _context.BodegaProductos
                .FirstOrDefaultAsync(bp => bp.IdBodega == idBodega && bp.IdProducto == idProducto);
            
            return bodegaProducto?.StockTotal ?? 0;
        }
    }
}
