using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Models.ViewModels;

namespace PitStop_Parts_Inventario.Services.Interfaces
{
    public interface IEntradaProductoService
    {
        Task<IEnumerable<EntradaProductoModel>> GetAllAsync();
        Task<PagedResult<EntradaProductoModel>> GetPagedAsync(int pageNumber, int pageSize, string? searchString = null);
        Task<EntradaProductoModel?> GetByIdAsync(int id);
        Task<EntradaProductoModel> CreateAsync(EntradaProductoModel entradaProducto, string userId);
        Task<EntradaProductoModel> UpdateAsync(EntradaProductoModel entradaProducto, string userId);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<EntradaProductoModel>> GetByUsuarioAsync(string userId);
        Task<IEnumerable<EntradaProductoModel>> GetByFechaAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<IEnumerable<EntradaProductoModel>> GetByProductoAsync(int productoId);
        Task<IEnumerable<EntradaProductoModel>> GetByProductoIdAsync(int productoId);
        Task<IEnumerable<EntradaProductoModel>> GetByProveedorIdAsync(int proveedorId);
    }
}
