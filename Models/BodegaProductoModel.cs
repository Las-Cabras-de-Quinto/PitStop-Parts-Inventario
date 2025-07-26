using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PitStop_Parts_Inventario.Models
{
    [Table("Bodega_Producto")]
    public class BodegaProductoModel
    {
        [Key, Column(Order = 0)]
        [ForeignKey("Bodega")]
        public int IdBodega { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey("Producto")]
        public int IdProducto { get; set; }

        [StringLength(100)]
        public string Descripcion { get; set; } = string.Empty;

        [ForeignKey("Estado")]
        public int IdEstado { get; set; }

        public int StockTotal { get; set; }

        // Navegación
        public virtual BodegaModel Bodega { get; set; } = null!;
        public virtual ProductoModel Producto { get; set; } = null!;
        public virtual EstadoModel Estado { get; set; } = null!;
    }
}
