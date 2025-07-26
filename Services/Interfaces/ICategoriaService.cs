using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Models.ViewModels;

namespace PitStop_Parts_Inventario.Services.Interfaces
{
    public interface ICategoriaService
    {
        Task<IEnumerable<CategoriaModel>> GetAllAsync(CategoriaFilterOptions? filters = null);
        Task<PagedResult<CategoriaModel>> GetPagedAsync(int pageNumber, int pageSize, CategoriaFilterOptions? filters = null);
        Task<CategoriaModel?> GetByIdAsync(int id);
        Task<CategoriaModel> CreateAsync(CategoriaModel categoria, string userId);
        Task<CategoriaModel> UpdateAsync(CategoriaModel categoria, string userId);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        
        // Mantener métodos legacy por compatibilidad
        [Obsolete("Use GetAllAsync with CategoriaFilterOptions instead")]
        Task<IEnumerable<CategoriaModel>> GetActivasAsync();
        Task<IEnumerable<ProductoModel>> GetProductosByCategoriaAsync(int categoriaId);
    }
}
