using Microsoft.EntityFrameworkCore;
using PitStop_Parts_Inventario.Data;
using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Models.ViewModels;
using PitStop_Parts_Inventario.Services.Interfaces;
using PitStop_Parts_Inventario.Services.Helpers;

namespace PitStop_Parts_Inventario.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly PitStopDbContext _context;

        public CategoriaService(PitStopDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoriaModel>> GetAllAsync(CategoriaFilterOptions? filters = null)
        {
            var query = _context.Categorias
                .Include(c => c.Estado)
                .Include(c => c.CategoriaProductos).ThenInclude(cp => cp.Producto)
                .AsQueryable();

            query = FilterHelper.ApplyCategoriaFilters(query, filters);

            return await query.ToListAsync();
        }

        public async Task<PagedResult<CategoriaModel>> GetPagedAsync(int pageNumber, int pageSize, CategoriaFilterOptions? filters = null)
        {
            var query = _context.Categorias
                .Include(c => c.Estado)
                .Include(c => c.CategoriaProductos).ThenInclude(cp => cp.Producto)
                .AsQueryable();

            query = FilterHelper.ApplyCategoriaFilters(query, filters);

            var totalRecords = await query.CountAsync();
            var data = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<CategoriaModel>(data, totalRecords, pageNumber, pageSize);
        }

        public async Task<CategoriaModel?> GetByIdAsync(int id)
        {
            return await _context.Categorias
                .Include(c => c.Estado)
                .Include(c => c.CategoriaProductos).ThenInclude(cp => cp.Producto)
                .FirstOrDefaultAsync(c => c.IdCategoria == id);
        }

        public async Task<CategoriaModel> CreateAsync(CategoriaModel categoria, string userId)
        {
            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();
            return categoria;
        }

        public async Task<CategoriaModel> UpdateAsync(CategoriaModel categoria, string userId)
        {
            var existingCategoria = await _context.Categorias.FindAsync(categoria.IdCategoria);
            if (existingCategoria == null)
                throw new ArgumentException("Categoría no encontrada");

            existingCategoria.Nombre = categoria.Nombre;
            existingCategoria.Descripcion = categoria.Descripcion;
            existingCategoria.IdEstado = categoria.IdEstado;

            await _context.SaveChangesAsync();
            return existingCategoria;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
                return false;

            // Soft delete - cambiar estado en lugar de eliminar físicamente
            var estadoInactivo = await _context.Estados.FirstOrDefaultAsync(e => e.Nombre == "Inactivo");
            if (estadoInactivo != null)
            {
                categoria.IdEstado = estadoInactivo.IdEstado;
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Categorias.AnyAsync(c => c.IdCategoria == id);
        }

        [Obsolete("Use GetAllAsync with CategoriaFilterOptions instead")]
        public async Task<IEnumerable<CategoriaModel>> GetActivasAsync()
        {
            return await GetAllAsync(new CategoriaFilterOptions { SoloActivos = true });
        }

        public async Task<IEnumerable<ProductoModel>> GetProductosByCategoriaAsync(int categoriaId)
        {
            return await _context.CategoriaProductos
                .Where(cp => cp.IdCategoria == categoriaId)
                .Include(cp => cp.Producto).ThenInclude(p => p.Marca)
                .Include(cp => cp.Producto).ThenInclude(p => p.Estado)
                .Select(cp => cp.Producto)
                .ToListAsync();
        }

        public async Task<PagedResult<CategoriaModel>> GetPagedAsync(int pageNumber, int pageSize, string? searchString = null)
        {
            var query = _context.Categorias
                .Include(c => c.Estado)
                .AsQueryable();

            // Aplicar filtro de búsqueda si se proporciona
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(c => c.Nombre.Contains(searchString) || 
                                        c.Descripcion.Contains(searchString) ||
                                        c.Estado.Nombre.Contains(searchString));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<CategoriaModel>(items, totalCount, pageNumber, pageSize);
        }
    }
}
