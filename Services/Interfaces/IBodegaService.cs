using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Models.ViewModels;

namespace PitStop_Parts_Inventario.Services.Interfaces
{
    public interface IBodegaService
    {
        Task<IEnumerable<BodegaModel>> GetAllAsync();
        Task<BodegaModel?> GetByIdAsync(int id);
        Task<BodegaModel> CreateAsync(BodegaModel bodega, string userId);
        Task<BodegaModel> UpdateAsync(BodegaModel bodega, string userId);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<BodegaModel>> GetActivasAsync();
        Task<IEnumerable<ProductoModel>> GetProductosByBodegaAsync(int bodegaId);
        Task<bool> AsignarProductoAsync(int bodegaId, int productoId, int cantidad);
        Task<bool> RemoverProductoAsync(int bodegaId, int productoId);
        Task<PagedResult<BodegaModel>> GetPagedAsync(int pageNumber, int pageSize, string? searchString = null);
    }
}
