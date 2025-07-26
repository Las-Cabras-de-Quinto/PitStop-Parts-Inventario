using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Models.ViewModels;

namespace PitStop_Parts_Inventario.Services.Interfaces
{
    public interface ICategoriaService
    {
        Task<IEnumerable<CategoriaModel>> GetAllAsync();
        Task<CategoriaModel?> GetByIdAsync(int id);
        Task<CategoriaModel> CreateAsync(CategoriaModel categoria, string userId);
        Task<CategoriaModel> UpdateAsync(CategoriaModel categoria, string userId);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<CategoriaModel>> GetActivasAsync();
        Task<IEnumerable<ProductoModel>> GetProductosByCategoriaAsync(int categoriaId);
        Task<PagedResult<CategoriaModel>> GetPagedAsync(int pageNumber, int pageSize, string? searchString = null);
    }
}
