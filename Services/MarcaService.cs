using Microsoft.EntityFrameworkCore;
using PitStop_Parts_Inventario.Data;
using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Models.ViewModels;
using PitStop_Parts_Inventario.Services.Interfaces;
using PitStop_Parts_Inventario.Services.Helpers;

namespace PitStop_Parts_Inventario.Services
{
    public class MarcaService : IMarcaService
    {
        private readonly PitStopDbContext _context;

        public MarcaService(PitStopDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MarcaModel>> GetAllAsync(MarcaFilterOptions? filters = null)
        {
            var query = _context.Marcas
                .Include(m => m.Estado)
                .Include(m => m.Productos)
                .AsQueryable();

            query = FilterHelper.ApplyMarcaFilters(query, filters);

            return await query.ToListAsync();
        }

        public async Task<PagedResult<MarcaModel>> GetPagedAsync(int pageNumber, int pageSize, MarcaFilterOptions? filters = null)
        {
            var query = _context.Marcas
                .Include(m => m.Estado)
                .Include(m => m.Productos)
                .AsQueryable();

            query = FilterHelper.ApplyMarcaFilters(query, filters);

            var totalRecords = await query.CountAsync();
            var data = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<MarcaModel>(data, totalRecords, pageNumber, pageSize);
        }

        public async Task<MarcaModel?> GetByIdAsync(int id)
        {
            return await _context.Marcas
                .Include(m => m.Estado)
                .Include(m => m.Productos)
                .FirstOrDefaultAsync(m => m.IdMarca == id);
        }

        public async Task<MarcaModel> CreateAsync(MarcaModel marca, string userId)
        {
            _context.Marcas.Add(marca);
            await _context.SaveChangesAsync();
            return marca;
        }

        public async Task<MarcaModel> UpdateAsync(MarcaModel marca, string userId)
        {
            var existingMarca = await _context.Marcas.FindAsync(marca.IdMarca);
            if (existingMarca == null)
                throw new ArgumentException("Marca no encontrada");

            existingMarca.Nombre = marca.Nombre;
            existingMarca.Descripcion = marca.Descripcion;
            existingMarca.IdEstado = marca.IdEstado;

            await _context.SaveChangesAsync();
            return existingMarca;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var marca = await _context.Marcas.FindAsync(id);
            if (marca == null)
                return false;

            // Soft delete - cambiar estado en lugar de eliminar físicamente
            var estadoInactivo = await _context.Estados.FirstOrDefaultAsync(e => e.Nombre == "Inactivo");
            if (estadoInactivo != null)
            {
                marca.IdEstado = estadoInactivo.IdEstado;
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Marcas.AnyAsync(m => m.IdMarca == id);
        }

        [Obsolete("Use GetAllAsync with MarcaFilterOptions instead")]
        public async Task<IEnumerable<MarcaModel>> GetActivasAsync()
        {
            return await GetAllAsync(new MarcaFilterOptions { SoloActivos = true });
        }

        public async Task<IEnumerable<ProductoModel>> GetProductosByMarcaAsync(int marcaId)
        {
            return await _context.Productos
                .Include(p => p.Marca)
                .Include(p => p.Estado)
                .Where(p => p.IdMarca == marcaId)
                .ToListAsync();
        }
    }
}
