using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Models.ViewModels;

namespace PitStop_Parts_Inventario.Services.Interfaces
{
    public interface IProveedorService
    {
        Task<IEnumerable<ProveedorModel>> GetAllAsync();
        Task<ProveedorModel?> GetByIdAsync(int id);
        Task<ProveedorModel> CreateAsync(ProveedorModel proveedor, string userId);
        Task<ProveedorModel> UpdateAsync(ProveedorModel proveedor, string userId);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<ProveedorModel>> GetActivosAsync();
        Task<IEnumerable<ProductoModel>> GetProductosByProveedorAsync(int proveedorId);
        Task<bool> AsignarProductoAsync(int proveedorId, int productoId);
        Task<bool> RemoverProductoAsync(int proveedorId, int productoId);
        Task<PagedResult<ProveedorModel>> GetPagedAsync(int pageNumber, int pageSize, string? searchString = null);
    }
}
