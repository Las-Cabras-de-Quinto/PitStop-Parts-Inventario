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
        public string Descripcion { get; set; }

        [ForeignKey("Estado")]
        public int IdEstado { get; set; }

        public int StockTotal { get; set; }

        // Navegación
        public virtual BodegaModel Bodega { get; set; }
        public virtual ProductoModel Producto { get; set; }
        public virtual EstadoModel Estado { get; set; }
    }
}
