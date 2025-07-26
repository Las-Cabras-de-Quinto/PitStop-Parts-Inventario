using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Models.ViewModels;

namespace PitStop_Parts_Inventario.Services.Interfaces
{
    public interface IEntradaProductoService
    {
        Task<IEnumerable<EntradaProductoModel>> GetAllAsync(EntradaProductoFilterOptions? filters = null);
        Task<PagedResult<EntradaProductoModel>> GetPagedAsync(int pageNumber, int pageSize, EntradaProductoFilterOptions? filters = null);
        Task<EntradaProductoModel?> GetByIdAsync(int id);
        Task<EntradaProductoModel> CreateAsync(EntradaProductoModel entradaProducto, string userId);
        Task<EntradaProductoModel> UpdateAsync(EntradaProductoModel entradaProducto, string userId);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        
        // Mantener métodos legacy por compatibilidad
        [Obsolete("Use GetAllAsync with EntradaProductoFilterOptions instead")]
        Task<IEnumerable<EntradaProductoModel>> GetByUsuarioAsync(string userId);
        [Obsolete("Use GetAllAsync with EntradaProductoFilterOptions instead")]
        Task<IEnumerable<EntradaProductoModel>> GetByFechaAsync(DateTime fechaInicio, DateTime fechaFin);
        [Obsolete("Use GetAllAsync with EntradaProductoFilterOptions instead")]
        Task<IEnumerable<EntradaProductoModel>> GetByProductoAsync(int productoId);
        [Obsolete("Use GetAllAsync with EntradaProductoFilterOptions instead")]
        Task<IEnumerable<EntradaProductoModel>> GetByProductoIdAsync(int productoId);
        [Obsolete("Use GetAllAsync with EntradaProductoFilterOptions instead")]
        Task<IEnumerable<EntradaProductoModel>> GetByProveedorIdAsync(int proveedorId);
    }
}
