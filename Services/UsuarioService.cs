using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PitStop_Parts_Inventario.Data;
using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Models.ViewModels;
using PitStop_Parts_Inventario.Services.Interfaces;

namespace PitStop_Parts_Inventario.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly PitStopDbContext _context;
        private readonly UserManager<UsuarioModel> _userManager;

        public UsuarioService(PitStopDbContext context, UserManager<UsuarioModel> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IEnumerable<UsuarioModel>> GetAllAsync()
        {
            return await _context.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Estado)
                .OrderBy(u => u.UserName)
                .ToListAsync();
        }

        public async Task<PagedResult<UsuarioModel>> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm = null)
        {
            var query = _context.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Estado)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(u => u.UserName!.Contains(searchTerm) || 
                                        u.Email!.Contains(searchTerm));
            }

            var totalRecords = await query.CountAsync();
            var data = await query
                .OrderBy(u => u.UserName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<UsuarioModel>(data, totalRecords, pageNumber, pageSize);
        }

        public async Task<UsuarioModel?> GetByIdAsync(string id)
        {
            return await _context.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Estado)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<UsuarioModel> CreateAsync(UsuarioModel usuario, string password)
        {
            usuario.FechaDeIngreso = DateTime.Now;
            usuario.EmailConfirmed = true; // Por simplicidad

            var result = await _userManager.CreateAsync(usuario, password);
            if (result.Succeeded)
            {
                return usuario;
            }

            throw new InvalidOperationException($"Error al crear usuario: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        public async Task<UsuarioModel> UpdateAsync(UsuarioModel usuario)
        {
            var existingUser = await _context.Usuarios.FindAsync(usuario.Id);
            if (existingUser == null)
                throw new InvalidOperationException("Usuario no encontrado");

            existingUser.UserName = usuario.UserName;
            existingUser.Email = usuario.Email;
            existingUser.IdRol = usuario.IdRol;
            existingUser.IdEstado = usuario.IdEstado;

            _context.Usuarios.Update(existingUser);
            await _context.SaveChangesAsync();

            return existingUser;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return false;

            // En lugar de eliminar, cambiar estado a inactivo
            usuario.IdEstado = 2; // Asumiendo que 2 es inactivo
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _context.Usuarios.AnyAsync(u => u.Id == id);
        }

        public async Task<bool> UpdateStatusAsync(string userId, int statusId)
        {
            var usuario = await _context.Usuarios.FindAsync(userId);
            if (usuario == null)
                return false;

            usuario.IdEstado = statusId;
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<RolModel>> GetAllRolesAsync()
        {
            return await _context.Roles
                .Where(r => r.IdEstado == 1) // Solo roles activos
                .OrderBy(r => r.Nombre)
                .ToListAsync();
        }

        public async Task<IEnumerable<EstadoModel>> GetAllStatusAsync()
        {
            return await _context.Estados
                .OrderBy(e => e.Nombre)
                .ToListAsync();
        }

        public async Task<bool> ChangePasswordAsync(string userId, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            // Remover la contraseña actual y establecer la nueva
            var removePasswordResult = await _userManager.RemovePasswordAsync(user);
            if (!removePasswordResult.Succeeded)
                return false;

            var addPasswordResult = await _userManager.AddPasswordAsync(user, newPassword);
            return addPasswordResult.Succeeded;
        }
    }
}
