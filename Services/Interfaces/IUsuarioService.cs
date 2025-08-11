using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Models.ViewModels;

namespace PitStop_Parts_Inventario.Services.Interfaces
{
    public interface IUsuarioService
    {
        Task<IEnumerable<UsuarioModel>> GetAllAsync();
        Task<PagedResult<UsuarioModel>> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm = null);
        Task<UsuarioModel?> GetByIdAsync(string id);
        Task<UsuarioModel> CreateAsync(UsuarioModel usuario, string password);
        Task<UsuarioModel> UpdateAsync(UsuarioModel usuario);
        Task<bool> DeleteAsync(string id);
        Task<bool> ExistsAsync(string id);
        Task<bool> UpdateStatusAsync(string userId, int statusId);
        Task<IEnumerable<RolModel>> GetAllRolesAsync();
        Task<IEnumerable<EstadoModel>> GetAllStatusAsync();
        Task<bool> ChangePasswordAsync(string userId, string newPassword);
    }
}
