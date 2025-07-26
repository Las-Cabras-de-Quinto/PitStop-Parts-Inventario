using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Models.ViewModels;

namespace PitStop_Parts_Inventario.Services.Interfaces
{
    public interface IProductoService
    {
        Task<IEnumerable<ProductoModel>> GetAllAsync();
        Task<PagedResult<ProductoModel>> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm = null);
        Task<ProductoModel?> GetByIdAsync(int id);
        Task<ProductoModel> CreateAsync(ProductoModel producto, string userId);
        Task<ProductoModel> UpdateAsync(ProductoModel producto, string userId);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<ProductoModel>> GetByMarcaAsync(int marcaId);
        Task<IEnumerable<ProductoModel>> GetByCategoriaAsync(int categoriaId);
        Task<IEnumerable<ProductoModel>> GetByProveedorAsync(int proveedorId);
        Task<IEnumerable<ProductoModel>> GetByBodegaAsync(int bodegaId);
        Task<IEnumerable<ProductoModel>> SearchAsync(string searchTerm);
        
        // Métodos para manejar relación con proveedores
        Task<bool> AsignarProveedoresAsync(int productoId, List<int> proveedorIds);
        Task<bool> RemoverProveedorAsync(int productoId, int proveedorId);
        Task<IEnumerable<ProveedorModel>> GetProveedoresByProductoAsync(int productoId);
        
        // Métodos para recalcular stock
        Task<int> RecalcularStockAsync(int productoId);
        Task RecalcularTodosLosStocksAsync();
    }
}
