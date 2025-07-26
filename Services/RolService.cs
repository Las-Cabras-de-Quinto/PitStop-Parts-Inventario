using Microsoft.EntityFrameworkCore;
using PitStop_Parts_Inventario.Data;
using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Models.ViewModels;
using PitStop_Parts_Inventario.Services.Interfaces;
using PitStop_Parts_Inventario.Services.Helpers;

namespace PitStop_Parts_Inventario.Services
{
    public class RolService : IRolService
    {
        private readonly PitStopDbContext _context;

        public RolService(PitStopDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RolModel>> GetAllAsync(RolFilterOptions? filters = null)
        {
            var query = _context.Roles
                .Include(r => r.Estado)
                .Include(r => r.Usuarios)
                .AsQueryable();

            query = FilterHelper.ApplyRolFilters(query, filters);

            return await query.ToListAsync();
        }

        public async Task<PagedResult<RolModel>> GetPagedAsync(int pageNumber, int pageSize, RolFilterOptions? filters = null)
        {
            var query = _context.Roles
                .Include(r => r.Estado)
                .Include(r => r.Usuarios)
                .AsQueryable();

            query = FilterHelper.ApplyRolFilters(query, filters);

            var totalRecords = await query.CountAsync();
            var data = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<RolModel>(data, totalRecords, pageNumber, pageSize);
        }

        public async Task<RolModel?> GetByIdAsync(int id)
        {
            return await _context.Roles
                .Include(r => r.Estado)
                .Include(r => r.Usuarios)
                .FirstOrDefaultAsync(r => r.IdRol == id);
        }

        public async Task<RolModel> CreateAsync(RolModel rol, string userId)
        {
            _context.Roles.Add(rol);
            await _context.SaveChangesAsync();
            return rol;
        }

        public async Task<RolModel> UpdateAsync(RolModel rol, string userId)
        {
            var existingRol = await _context.Roles.FindAsync(rol.IdRol);
            if (existingRol == null)
                throw new ArgumentException("Rol no encontrado");

            existingRol.Nombre = rol.Nombre;
            existingRol.Funcion = rol.Funcion;
            existingRol.IdEstado = rol.IdEstado;

            await _context.SaveChangesAsync();
            return existingRol;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var rol = await _context.Roles.FindAsync(id);
            if (rol == null)
                return false;

            // Verificar si hay usuarios asignados a este rol
            var tieneUsuarios = await _context.Usuarios.AnyAsync(u => u.IdRol == id);
            if (tieneUsuarios)
                return false; // No se puede eliminar porque tiene usuarios asignados

            // Soft delete - cambiar estado en lugar de eliminar físicamente
            var estadoInactivo = await _context.Estados.FirstOrDefaultAsync(e => e.Nombre == "Inactivo");
            if (estadoInactivo != null)
            {
                rol.IdEstado = estadoInactivo.IdEstado;
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Roles.AnyAsync(r => r.IdRol == id);
        }

        [Obsolete("Use GetAllAsync with RolFilterOptions instead")]
        public async Task<IEnumerable<RolModel>> GetActivosAsync()
        {
            return await GetAllAsync(new RolFilterOptions { SoloActivos = true });
        }

        public async Task<IEnumerable<UsuarioModel>> GetUsuariosByRolAsync(int rolId)
        {
            return await _context.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Estado)
                .Where(u => u.IdRol == rolId)
                .ToListAsync();
        }
    }
}
