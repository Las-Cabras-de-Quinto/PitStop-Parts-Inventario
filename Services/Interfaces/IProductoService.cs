using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Models.ViewModels;

namespace PitStop_Parts_Inventario.Services.Interfaces
{
    public interface IProductoService
    {
        Task<IEnumerable<ProductoModel>> GetAllAsync(ProductoFilterOptions? filters = null);
        Task<PagedResult<ProductoModel>> GetPagedAsync(int pageNumber, int pageSize, ProductoFilterOptions? filters = null);
        Task<ProductoModel?> GetByIdAsync(int id);
        Task<ProductoModel> CreateAsync(ProductoModel producto, string userId);
        Task<ProductoModel> UpdateAsync(ProductoModel producto, string userId);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        
        // Mantener métodos legacy por compatibilidad
        [Obsolete("Use GetAllAsync with ProductoFilterOptions instead")]
        Task<IEnumerable<ProductoModel>> GetByMarcaAsync(int marcaId);
        [Obsolete("Use GetAllAsync with ProductoFilterOptions instead")]
        Task<IEnumerable<ProductoModel>> GetByCategoriaAsync(int categoriaId);
        [Obsolete("Use GetAllAsync with ProductoFilterOptions instead")]
        Task<IEnumerable<ProductoModel>> GetByProveedorAsync(int proveedorId);
        [Obsolete("Use GetAllAsync with ProductoFilterOptions instead")]
        Task<IEnumerable<ProductoModel>> GetByBodegaAsync(int bodegaId);
        [Obsolete("Use GetAllAsync with ProductoFilterOptions instead")]
        Task<IEnumerable<ProductoModel>> SearchAsync(string searchTerm);
        
        // Métodos para manejar relación con proveedores
        Task<bool> AsignarProveedoresAsync(int productoId, List<int> proveedorIds);
        Task<bool> RemoverProveedorAsync(int productoId, int proveedorId);
        Task<IEnumerable<ProveedorModel>> GetProveedoresByProductoAsync(int productoId);
        
        // Métodos para manejar relación con categorías
        Task<bool> AsignarCategoriasAsync(int productoId, List<int> categoriaIds);
        Task<bool> RemoverCategoriaAsync(int productoId, int categoriaId);
        Task<IEnumerable<CategoriaModel>> GetCategoriasByProductoAsync(int productoId);
        
        // Métodos para recalcular stock
        Task<int> RecalcularStockAsync(int productoId);
        Task<int> RecalcularStockSinGuardarAsync(int productoId);
        Task RecalcularTodosLosStocksAsync();
        Task ActualizarStockBodegaAsync(int bodegaId, int productoId, int cantidad, string descripcion = "");
    }
}
