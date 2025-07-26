using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Models.ViewModels;

namespace PitStop_Parts_Inventario.Services.Interfaces
{
    public interface IMarcaService
    {
        Task<IEnumerable<MarcaModel>> GetAllAsync();
        Task<PagedResult<MarcaModel>> GetPagedAsync(int pageNumber, int pageSize, string? searchString = null);
        Task<MarcaModel?> GetByIdAsync(int id);
        Task<MarcaModel> CreateAsync(MarcaModel marca, string userId);
        Task<MarcaModel> UpdateAsync(MarcaModel marca, string userId);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<MarcaModel>> GetActivasAsync();
        Task<IEnumerable<ProductoModel>> GetProductosByMarcaAsync(int marcaId);
    }
}
