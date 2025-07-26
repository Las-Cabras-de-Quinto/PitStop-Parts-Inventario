using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Models.ViewModels;

namespace PitStop_Parts_Inventario.Services.Interfaces
{
    public interface IMarcaService
    {
        Task<IEnumerable<MarcaModel>> GetAllAsync(MarcaFilterOptions? filters = null);
        Task<PagedResult<MarcaModel>> GetPagedAsync(int pageNumber, int pageSize, MarcaFilterOptions? filters = null);
        Task<MarcaModel?> GetByIdAsync(int id);
        Task<MarcaModel> CreateAsync(MarcaModel marca, string userId);
        Task<MarcaModel> UpdateAsync(MarcaModel marca, string userId);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        
        // Mantener métodos legacy por compatibilidad
        [Obsolete("Use GetAllAsync with MarcaFilterOptions instead")]
        Task<IEnumerable<MarcaModel>> GetActivasAsync();
        Task<IEnumerable<ProductoModel>> GetProductosByMarcaAsync(int marcaId);
    }
}
