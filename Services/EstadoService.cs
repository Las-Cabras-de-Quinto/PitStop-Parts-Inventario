using Microsoft.EntityFrameworkCore;
using PitStop_Parts_Inventario.Data;
using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Models.ViewModels;
using PitStop_Parts_Inventario.Services.Interfaces;
using PitStop_Parts_Inventario.Services.Helpers;

namespace PitStop_Parts_Inventario.Services
{
    public class EstadoService : IEstadoService
    {
        private readonly PitStopDbContext _context;

        public EstadoService(PitStopDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EstadoModel>> GetAllAsync(EstadoFilterOptions? filters = null)
        {
            var query = _context.Estados.AsQueryable();

            query = FilterHelper.ApplyEstadoFilters(query, filters);

            return await query.ToListAsync();
        }

        public async Task<PagedResult<EstadoModel>> GetPagedAsync(int pageNumber, int pageSize, EstadoFilterOptions? filters = null)
        {
            var query = _context.Estados.AsQueryable();

            query = FilterHelper.ApplyEstadoFilters(query, filters);

            var totalRecords = await query.CountAsync();
            var data = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<EstadoModel>(data, totalRecords, pageNumber, pageSize);
        }

        public async Task<EstadoModel?> GetByIdAsync(int id)
        {
            return await _context.Estados.FindAsync(id);
        }

        public async Task<EstadoModel> CreateAsync(EstadoModel estado, string userId)
        {
            _context.Estados.Add(estado);
            await _context.SaveChangesAsync();
            return estado;
        }

        public async Task<EstadoModel> UpdateAsync(EstadoModel estado, string userId)
        {
            var existingEstado = await _context.Estados.FindAsync(estado.IdEstado);
            if (existingEstado == null)
                throw new ArgumentException("Estado no encontrado");

            existingEstado.Nombre = estado.Nombre;
            existingEstado.Descripcion = estado.Descripcion;
            existingEstado.Funcion = estado.Funcion;

            await _context.SaveChangesAsync();
            return existingEstado;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var estado = await _context.Estados.FindAsync(id);
            if (estado == null)
                return false;

            // Verificar si hay registros que dependen de este estado
            var tieneProductos = await _context.Productos.AnyAsync(p => p.IdEstado == id);
            var tieneBodegas = await _context.Bodegas.AnyAsync(b => b.IdEstado == id);
            var tieneCategorias = await _context.Categorias.AnyAsync(c => c.IdEstado == id);
            var tieneMarcas = await _context.Marcas.AnyAsync(m => m.IdEstado == id);
            var tieneProveedores = await _context.Proveedores.AnyAsync(p => p.IdEstado == id);
            var tieneUsuarios = await _context.Usuarios.AnyAsync(u => u.IdEstado == id);

            if (tieneProductos || tieneBodegas || tieneCategorias || tieneMarcas || tieneProveedores || tieneUsuarios)
                return false; // No se puede eliminar porque tiene dependencias

            _context.Estados.Remove(estado);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Estados.AnyAsync(e => e.IdEstado == id);
        }

        [Obsolete("Use GetAllAsync with EstadoFilterOptions instead")]
        public async Task<IEnumerable<EstadoModel>> GetActivosAsync()
        {
            return await GetAllAsync(new EstadoFilterOptions { EsActivo = true });
        }
    }
}
