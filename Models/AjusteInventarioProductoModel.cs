using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PitStop_Parts_Inventario.Models
{
    [Table("AjusteInventario_Producto")]
    public class AjusteInventarioProductoModel
    {
        [Key, Column(Order = 0)]
        [ForeignKey("AjusteInventario")]
        public int IdAjusteInventario { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey("Producto")]
        public int IdProducto { get; set; }

        public int CantidadProducto { get; set; }

        // Navegación
        public virtual AjusteInventarioModel AjusteInventario { get; set; } = null!;
        public virtual ProductoModel Producto { get; set; } = null!;
    }
}
