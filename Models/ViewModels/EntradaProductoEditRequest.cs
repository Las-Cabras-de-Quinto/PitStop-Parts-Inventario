using System.ComponentModel.DataAnnotations;

namespace PitStop_Parts_Inventario.Models.ViewModels
{
    public class EntradaProductoEditRequest
    {
        public int IdEntrada { get; set; }

        [Required(ErrorMessage = "La bodega es requerida")]
        public int IdBodega { get; set; }

        [Required(ErrorMessage = "La fecha es requerida")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "El producto es requerido")]
        public int IdProducto { get; set; }

        [Required(ErrorMessage = "La cantidad es requerida")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int CantidadProducto { get; set; }
    }
}
