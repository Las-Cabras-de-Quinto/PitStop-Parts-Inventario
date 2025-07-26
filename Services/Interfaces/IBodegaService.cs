using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Models.ViewModels;

namespace PitStop_Parts_Inventario.Services.Interfaces
{
    public interface IBodegaService
    {
        Task<IEnumerable<BodegaModel>> GetAllAsync(BodegaFilterOptions? filters = null);
        Task<PagedResult<BodegaModel>> GetPagedAsync(int pageNumber, int pageSize, BodegaFilterOptions? filters = null);
        Task<BodegaModel?> GetByIdAsync(int id);
        Task<BodegaModel> CreateAsync(BodegaModel bodega, string userId);
        Task<BodegaModel> UpdateAsync(BodegaModel bodega, string userId);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        
        // Mantener métodos legacy por compatibilidad
        [Obsolete("Use GetAllAsync with BodegaFilterOptions instead")]
        Task<IEnumerable<BodegaModel>> GetActivasAsync();
        Task<IEnumerable<ProductoModel>> GetProductosByBodegaAsync(int bodegaId);
        Task<bool> AsignarProductoAsync(int bodegaId, int productoId, int cantidad);
        Task<bool> RemoverProductoAsync(int bodegaId, int productoId);
    }
}
