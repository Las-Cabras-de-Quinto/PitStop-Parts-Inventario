using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Models.ViewModels;

namespace PitStop_Parts_Inventario.Services.Interfaces
{
    public interface IEstadoService
    {
        Task<IEnumerable<EstadoModel>> GetAllAsync();
        Task<EstadoModel?> GetByIdAsync(int id);
        Task<EstadoModel> CreateAsync(EstadoModel estado, string userId);
        Task<EstadoModel> UpdateAsync(EstadoModel estado, string userId);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<EstadoModel>> GetActivosAsync();
        Task<PagedResult<EstadoModel>> GetPagedAsync(int pageNumber, int pageSize, string? searchString = null);
    }
}
