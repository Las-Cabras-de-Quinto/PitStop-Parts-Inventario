using System.ComponentModel.DataAnnotations;

namespace PitStop_Parts_Inventario.Models.ViewModels
{
    public class AjusteInventarioEditRequest
    {
        public int IdAjusteInventario { get; set; }

        [Required(ErrorMessage = "La bodega es requerida")]
        public int IdBodega { get; set; }

        [Required(ErrorMessage = "La fecha es requerida")]
        public DateTime Fecha { get; set; }

        // Lista de productos con sus cantidades para el ajuste
        public List<AjusteProductoRequest> Productos { get; set; } = new List<AjusteProductoRequest>();
    }

    public class AjusteProductoRequest
    {
        public int IdProducto { get; set; }
        public int CantidadProducto { get; set; }
        public string Motivo { get; set; } = string.Empty;
    }
}
