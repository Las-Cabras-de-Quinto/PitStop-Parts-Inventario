using Microsoft.EntityFrameworkCore;
using PitStop_Parts_Inventario.Data;
using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Models.ViewModels;
using PitStop_Parts_Inventario.Services.Interfaces;
using PitStop_Parts_Inventario.Services.Helpers;

namespace PitStop_Parts_Inventario.Services
{
    public class BodegaService : IBodegaService
    {
        private readonly PitStopDbContext _context;

        public BodegaService(PitStopDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BodegaModel>> GetAllAsync(BodegaFilterOptions? filters = null)
        {
            var query = _context.Bodegas
                .Include(b => b.Estado)
                .Include(b => b.BodegaProductos).ThenInclude(bp => bp.Producto)
                .AsQueryable();

            query = FilterHelper.ApplyBodegaFilters(query, filters);

            return await query.ToListAsync();
        }

        public async Task<PagedResult<BodegaModel>> GetPagedAsync(int pageNumber, int pageSize, BodegaFilterOptions? filters = null)
        {
            var query = _context.Bodegas
                .Include(b => b.Estado)
                .Include(b => b.BodegaProductos).ThenInclude(bp => bp.Producto)
                .AsQueryable();

            query = FilterHelper.ApplyBodegaFilters(query, filters);

            var totalRecords = await query.CountAsync();
            var data = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<BodegaModel>(data, totalRecords, pageNumber, pageSize);
        }

        public async Task<BodegaModel?> GetByIdAsync(int id)
        {
            return await _context.Bodegas
                .Include(b => b.Estado)
                .Include(b => b.BodegaProductos).ThenInclude(bp => bp.Producto)
                .FirstOrDefaultAsync(b => b.IdBodega == id);
        }

        public async Task<BodegaModel> CreateAsync(BodegaModel bodega, string userId)
        {
            _context.Bodegas.Add(bodega);
            await _context.SaveChangesAsync();
            return bodega;
        }

        public async Task<BodegaModel> UpdateAsync(BodegaModel bodega, string userId)
        {
            var existingBodega = await _context.Bodegas.FindAsync(bodega.IdBodega);
            if (existingBodega == null)
                throw new ArgumentException("Bodega no encontrada");

            existingBodega.Nombre = bodega.Nombre;
            existingBodega.Descripcion = bodega.Descripcion;
            existingBodega.Ubicacion = bodega.Ubicacion;
            existingBodega.IdEstado = bodega.IdEstado;

            await _context.SaveChangesAsync();
            return existingBodega;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var bodega = await _context.Bodegas.FindAsync(id);
            if (bodega == null)
                return false;

            // Soft delete - cambiar estado en lugar de eliminar físicamente
            var estadoInactivo = await _context.Estados.FirstOrDefaultAsync(e => e.Nombre == "Inactivo");
            if (estadoInactivo != null)
            {
                bodega.IdEstado = estadoInactivo.IdEstado;
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Bodegas.AnyAsync(b => b.IdBodega == id);
        }

        [Obsolete("Use GetAllAsync with BodegaFilterOptions instead")]
        public async Task<IEnumerable<BodegaModel>> GetActivasAsync()
        {
            return await GetAllAsync(new BodegaFilterOptions { SoloActivos = true });
        }

        public async Task<IEnumerable<ProductoModel>> GetProductosByBodegaAsync(int bodegaId)
        {
            return await _context.BodegaProductos
                .Where(bp => bp.IdBodega == bodegaId)
                .Include(bp => bp.Producto).ThenInclude(p => p.Marca)
                .Include(bp => bp.Producto).ThenInclude(p => p.Estado)
                .Select(bp => bp.Producto)
                .ToListAsync();
        }

        public async Task<bool> AsignarProductoAsync(int bodegaId, int productoId, int cantidad)
        {
            var bodegaProducto = await _context.BodegaProductos
                .FirstOrDefaultAsync(bp => bp.IdBodega == bodegaId && bp.IdProducto == productoId);

            if (bodegaProducto != null)
            {
                // Si ya existe, actualizar cantidad
                bodegaProducto.StockTotal += cantidad;
            }
            else
            {
                // Si no existe, crear nueva relación
                bodegaProducto = new BodegaProductoModel
                {
                    IdBodega = bodegaId,
                    IdProducto = productoId,
                    StockTotal = cantidad,
                    IdEstado = 1 // Asumiendo que 1 es el ID del estado activo
                };
                _context.BodegaProductos.Add(bodegaProducto);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoverProductoAsync(int bodegaId, int productoId)
        {
            var bodegaProducto = await _context.BodegaProductos
                .FirstOrDefaultAsync(bp => bp.IdBodega == bodegaId && bp.IdProducto == productoId);

            if (bodegaProducto == null)
                return false;

            _context.BodegaProductos.Remove(bodegaProducto);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PagedResult<BodegaModel>> GetPagedAsync(int pageNumber, int pageSize, string? searchString = null)
        {
            var query = _context.Bodegas
                .Include(b => b.Estado)
                .AsQueryable();

            // Aplicar filtro de búsqueda si se proporciona
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(b => b.Nombre.Contains(searchString) || 
                                        b.Descripcion.Contains(searchString) ||
                                        b.Ubicacion.Contains(searchString) ||
                                        b.Estado.Nombre.Contains(searchString));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<BodegaModel>(items, totalCount, pageNumber, pageSize);
        }
    }
}
