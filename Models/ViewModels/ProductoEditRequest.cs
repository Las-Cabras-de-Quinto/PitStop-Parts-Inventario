using System.ComponentModel.DataAnnotations;

namespace PitStop_Parts_Inventario.Models.ViewModels
{
    public class ProductoEditRequest
    {
        public int IdProducto { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;

        public int SKU { get; set; }

        [StringLength(1000)]
        public string? Descripcion { get; set; }

        public int IdMarca { get; set; }

        public int IdEstado { get; set; }

        public decimal PrecioVenta { get; set; }

        public decimal PrecioCompra { get; set; }

        public int StockMin { get; set; }

        public int StockMax { get; set; }

        public int StockActual { get; set; } = 0;

        // IDs de las relaciones muchos a muchos
        public List<int> IdsProveedores { get; set; } = new List<int>();
        public List<int> IdsCategorias { get; set; } = new List<int>();
        public List<int> IdsBodegas { get; set; } = new List<int>();
    }
}
