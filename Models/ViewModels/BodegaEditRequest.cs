using System.ComponentModel.DataAnnotations;

namespace PitStop_Parts_Inventario.Models.ViewModels
{
    public class BodegaEditRequest
    {
        public int IdBodega { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(100)]
        public string Descripcion { get; set; } = string.Empty;

        [StringLength(50)]
        public string Ubicacion { get; set; } = string.Empty;

        public int IdEstado { get; set; }

        // IDs de los productos relacionados con sus stocks
        public List<BodegaProductoRequest> Productos { get; set; } = new List<BodegaProductoRequest>();
    }

    public class BodegaProductoRequest
    {
        public int IdProducto { get; set; }
        public int StockTotal { get; set; }
    }
}
