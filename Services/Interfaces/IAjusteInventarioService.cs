using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Models.ViewModels;

namespace PitStop_Parts_Inventario.Services.Interfaces
{
    public interface IAjusteInventarioService
    {
        Task<IEnumerable<AjusteInventarioModel>> GetAllAsync();
        Task<PagedResult<AjusteInventarioModel>> GetPagedAsync(int pageNumber, int pageSize, string? searchString = null);
        Task<AjusteInventarioModel?> GetByIdAsync(int id);
        Task<AjusteInventarioModel> CreateAsync(AjusteInventarioModel ajusteInventario, string userId);
        Task<AjusteInventarioModel> UpdateAsync(AjusteInventarioModel ajusteInventario, string userId);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<AjusteInventarioModel>> GetByUsuarioAsync(string userId);
        Task<IEnumerable<AjusteInventarioModel>> GetByFechaAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<bool> AgregarProductoAsync(int ajusteId, int productoId, int cantidadAnterior, int cantidadNueva, string motivo);
        Task<bool> RemoverProductoAsync(int ajusteId, int productoId);
    }
}
