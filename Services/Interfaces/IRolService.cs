using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Models.ViewModels;

namespace PitStop_Parts_Inventario.Services.Interfaces
{
    public interface IRolService
    {
        Task<IEnumerable<RolModel>> GetAllAsync(RolFilterOptions? filters = null);
        Task<PagedResult<RolModel>> GetPagedAsync(int pageNumber, int pageSize, RolFilterOptions? filters = null);
        Task<RolModel?> GetByIdAsync(int id);
        Task<RolModel> CreateAsync(RolModel rol, string userId);
        Task<RolModel> UpdateAsync(RolModel rol, string userId);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        
        // Mantener métodos legacy por compatibilidad
        [Obsolete("Use GetAllAsync with RolFilterOptions instead")]
        Task<IEnumerable<RolModel>> GetActivosAsync();
        Task<IEnumerable<UsuarioModel>> GetUsuariosByRolAsync(int rolId);
    }
}
