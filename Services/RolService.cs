using Microsoft.EntityFrameworkCore;
using PitStop_Parts_Inventario.Data;
using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Services.Interfaces;

namespace PitStop_Parts_Inventario.Services
{
    public class RolService : IRolService
    {
        private readonly PitStopDbContext _context;

        public RolService(PitStopDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RolModel>> GetAllAsync()
        {
            return await _context.Roles
                .Include(r => r.Estado)
                .Include(r => r.Usuarios)
                .ToListAsync();
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

        public async Task<IEnumerable<RolModel>> GetActivosAsync()
        {
            var estadoActivo = await _context.Estados.FirstOrDefaultAsync(e => e.Nombre == "Activo");
            if (estadoActivo == null)
                return new List<RolModel>();

            return await _context.Roles
                .Include(r => r.Estado)
                .Where(r => r.IdEstado == estadoActivo.IdEstado)
                .ToListAsync();
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
