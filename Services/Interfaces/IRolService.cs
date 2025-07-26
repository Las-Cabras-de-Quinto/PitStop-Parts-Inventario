using PitStop_Parts_Inventario.Models;

namespace PitStop_Parts_Inventario.Services.Interfaces
{
    public interface IRolService
    {
        Task<IEnumerable<RolModel>> GetAllAsync();
        Task<RolModel?> GetByIdAsync(int id);
        Task<RolModel> CreateAsync(RolModel rol, string userId);
        Task<RolModel> UpdateAsync(RolModel rol, string userId);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<RolModel>> GetActivosAsync();
        Task<IEnumerable<UsuarioModel>> GetUsuariosByRolAsync(int rolId);
    }
}
