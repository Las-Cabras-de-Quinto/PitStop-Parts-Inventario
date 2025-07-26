using Microsoft.EntityFrameworkCore;
using PitStop_Parts_Inventario.Data;
using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Models.ViewModels;
using PitStop_Parts_Inventario.Services.Interfaces;
using PitStop_Parts_Inventario.Services.Helpers;

namespace PitStop_Parts_Inventario.Services
{
    public class ProveedorService : IProveedorService
    {
        private readonly PitStopDbContext _context;

        public ProveedorService(PitStopDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProveedorModel>> GetAllAsync(ProveedorFilterOptions? filters = null)
        {
            var query = _context.Proveedores
                .Include(p => p.Estado)
                .Include(p => p.ProveedorProductos).ThenInclude(pp => pp.Producto)
                .AsQueryable();

            query = FilterHelper.ApplyProveedorFilters(query, filters);

            return await query.ToListAsync();
        }

        public async Task<PagedResult<ProveedorModel>> GetPagedAsync(int pageNumber, int pageSize, ProveedorFilterOptions? filters = null)
        {
            var query = _context.Proveedores
                .Include(p => p.Estado)
                .Include(p => p.ProveedorProductos).ThenInclude(pp => pp.Producto)
                .AsQueryable();

            query = FilterHelper.ApplyProveedorFilters(query, filters);

            var totalRecords = await query.CountAsync();
            var data = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ProveedorModel>(data, totalRecords, pageNumber, pageSize);
        }

        public async Task<ProveedorModel?> GetByIdAsync(int id)
        {
            return await _context.Proveedores
                .Include(p => p.Estado)
                .Include(p => p.ProveedorProductos).ThenInclude(pp => pp.Producto)
                .FirstOrDefaultAsync(p => p.IdProveedor == id);
        }

        public async Task<ProveedorModel> CreateAsync(ProveedorModel proveedor, string userId)
        {
            _context.Proveedores.Add(proveedor);
            await _context.SaveChangesAsync();
            return proveedor;
        }

        public async Task<ProveedorModel> UpdateAsync(ProveedorModel proveedor, string userId)
        {
            var existingProveedor = await _context.Proveedores.FindAsync(proveedor.IdProveedor);
            if (existingProveedor == null)
                throw new ArgumentException("Proveedor no encontrado");

            existingProveedor.Nombre = proveedor.Nombre;
            existingProveedor.Contacto = proveedor.Contacto;
            existingProveedor.Direccion = proveedor.Direccion;
            existingProveedor.IdEstado = proveedor.IdEstado;

            await _context.SaveChangesAsync();
            return existingProveedor;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor == null)
                return false;

            // Soft delete - cambiar estado en lugar de eliminar físicamente
            var estadoInactivo = await _context.Estados.FirstOrDefaultAsync(e => e.Nombre == "Inactivo");
            if (estadoInactivo != null)
            {
                proveedor.IdEstado = estadoInactivo.IdEstado;
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Proveedores.AnyAsync(p => p.IdProveedor == id);
        }

        [Obsolete("Use GetAllAsync with ProveedorFilterOptions instead")]
        public async Task<IEnumerable<ProveedorModel>> GetActivosAsync()
        {
            return await GetAllAsync(new ProveedorFilterOptions { SoloActivos = true });
        }

        public async Task<IEnumerable<ProductoModel>> GetProductosByProveedorAsync(int proveedorId)
        {
            return await _context.ProveedorProductos
                .Where(pp => pp.IdProveedor == proveedorId)
                .Include(pp => pp.Producto).ThenInclude(p => p.Marca)
                .Include(pp => pp.Producto).ThenInclude(p => p.Estado)
                .Select(pp => pp.Producto)
                .ToListAsync();
        }

        public async Task<bool> AsignarProductoAsync(int proveedorId, int productoId)
        {
            var proveedorProducto = await _context.ProveedorProductos
                .FirstOrDefaultAsync(pp => pp.IdProveedor == proveedorId && pp.IdProducto == productoId);

            if (proveedorProducto != null)
                return false; // Ya existe la relación

            proveedorProducto = new ProveedorProductoModel
            {
                IdProveedor = proveedorId,
                IdProducto = productoId
            };

            _context.ProveedorProductos.Add(proveedorProducto);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoverProductoAsync(int proveedorId, int productoId)
        {
            var proveedorProducto = await _context.ProveedorProductos
                .FirstOrDefaultAsync(pp => pp.IdProveedor == proveedorId && pp.IdProducto == productoId);

            if (proveedorProducto == null)
                return false;

            _context.ProveedorProductos.Remove(proveedorProducto);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
