using Microsoft.EntityFrameworkCore;
using PitStop_Parts_Inventario.Data;
using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Models.ViewModels;
using PitStop_Parts_Inventario.Services.Interfaces;

namespace PitStop_Parts_Inventario.Services
{
    public class AjusteInventarioService : IAjusteInventarioService
    {
        private readonly PitStopDbContext _context;

        public AjusteInventarioService(PitStopDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AjusteInventarioModel>> GetAllAsync()
        {
            return await _context.AjusteInventarios
                .Include(ai => ai.Usuario)
                .Include(ai => ai.Bodega)
                .Include(ai => ai.AjusteInventarioProductos).ThenInclude(aip => aip.Producto)
                .OrderByDescending(ai => ai.Fecha)
                .ToListAsync();
        }

        public async Task<PagedResult<AjusteInventarioModel>> GetPagedAsync(int pageNumber, int pageSize, string? searchString = null)
        {
            var query = _context.AjusteInventarios
                .Include(ai => ai.Usuario)
                .Include(ai => ai.Bodega)
                .Include(ai => ai.AjusteInventarioProductos).ThenInclude(aip => aip.Producto)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                query = query.Where(ai => 
                    ai.Bodega.Nombre.ToLower().Contains(searchString) ||
                    (ai.Usuario.UserName != null && ai.Usuario.UserName.ToLower().Contains(searchString)) ||
                    ai.Fecha.ToString("dd/MM/yyyy").Contains(searchString)
                );
            }

            var totalRecords = await query.CountAsync();
            var data = await query
                .OrderByDescending(ai => ai.Fecha)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

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
            var ajusteInventario = await _context.AjusteInventarios
                .Include(ai => ai.AjusteInventarioProductos)
                .FirstOrDefaultAsync(ai => ai.IdAjusteInventario == id);

            if (ajusteInventario == null)
                return false;

            _context.AjusteInventarios.Remove(ajusteInventario);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.AjusteInventarios.AnyAsync(ai => ai.IdAjusteInventario == id);
        }

        public async Task<IEnumerable<AjusteInventarioModel>> GetByUsuarioAsync(string userId)
        {
            return await _context.AjusteInventarios
                .Include(ai => ai.Usuario)
                .Include(ai => ai.Bodega)
                .Include(ai => ai.AjusteInventarioProductos).ThenInclude(aip => aip.Producto)
                .Where(ai => ai.IdUsuario == userId)
                .OrderByDescending(ai => ai.Fecha)
                .ToListAsync();
        }

        public async Task<IEnumerable<AjusteInventarioModel>> GetByFechaAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _context.AjusteInventarios
                .Include(ai => ai.Usuario)
                .Include(ai => ai.Bodega)
                .Include(ai => ai.AjusteInventarioProductos).ThenInclude(aip => aip.Producto)
                .Where(ai => ai.Fecha >= fechaInicio && ai.Fecha <= fechaFin)
                .OrderByDescending(ai => ai.Fecha)
                .ToListAsync();
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

                // Actualizar stock en BodegaProducto
                var bodegaProducto = await _context.BodegaProductos
                    .FirstOrDefaultAsync(bp => bp.IdBodega == ajuste.IdBodega && bp.IdProducto == productoId);

                if (bodegaProducto != null)
                {
                    bodegaProducto.StockTotal += diferencia;
                    // Asegurar que el stock no sea negativo
                    if (bodegaProducto.StockTotal < 0)
                        bodegaProducto.StockTotal = 0;
                    
                    _context.BodegaProductos.Update(bodegaProducto);
                }
                else if (diferencia > 0)
                {
                    // Si no existe el registro y la diferencia es positiva, crear uno nuevo
                    bodegaProducto = new BodegaProductoModel
                    {
                        IdBodega = ajuste.IdBodega,
                        IdProducto = productoId,
                        StockTotal = diferencia,
                        IdEstado = 1,
                        Descripcion = $"Ajuste de inventario: {motivo}"
                    };
                    _context.BodegaProductos.Add(bodegaProducto);
                }

                // Recalcular el stock actual del producto sumando todas las bodegas
                var producto = await _context.Productos.FindAsync(productoId);
                if (producto != null)
                {
                    var stockTotal = await _context.BodegaProductos
                        .Where(bp => bp.IdProducto == productoId)
                        .SumAsync(bp => bp.StockTotal);
                    
                    producto.StockActual = stockTotal;
                    _context.Productos.Update(producto);
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

                // Revertir el cambio en el stock de BodegaProducto
                var bodegaProducto = await _context.BodegaProductos
                    .FirstOrDefaultAsync(bp => bp.IdBodega == ajuste.IdBodega && bp.IdProducto == productoId);

                if (bodegaProducto != null)
                {
                    bodegaProducto.StockTotal -= ajusteProducto.CantidadProducto;
                    // Asegurar que el stock no sea negativo
                    if (bodegaProducto.StockTotal < 0)
                        bodegaProducto.StockTotal = 0;
                    
                    _context.BodegaProductos.Update(bodegaProducto);
                }

                // Recalcular el stock actual del producto
                var producto = await _context.Productos.FindAsync(productoId);
                if (producto != null)
                {
                    var stockTotal = await _context.BodegaProductos
                        .Where(bp => bp.IdProducto == productoId)
                        .SumAsync(bp => bp.StockTotal);
                    
                    producto.StockActual = stockTotal;
                    _context.Productos.Update(producto);
                }

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
    }
}
