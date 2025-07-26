using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Models.ViewModels;

namespace PitStop_Parts_Inventario.Services.Interfaces
{
    public interface IEstadoService
    {
        Task<IEnumerable<EstadoModel>> GetAllAsync(EstadoFilterOptions? filters = null);
        Task<PagedResult<EstadoModel>> GetPagedAsync(int pageNumber, int pageSize, EstadoFilterOptions? filters = null);
        Task<EstadoModel?> GetByIdAsync(int id);
        Task<EstadoModel> CreateAsync(EstadoModel estado, string userId);
        Task<EstadoModel> UpdateAsync(EstadoModel estado, string userId);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        
        // Mantener métodos legacy por compatibilidad
        [Obsolete("Use GetAllAsync with EstadoFilterOptions instead")]
        Task<IEnumerable<EstadoModel>> GetActivosAsync();
    }
}
