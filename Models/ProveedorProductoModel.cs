using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PitStop_Parts_Inventario.Models
{
    [Table("Proveedor_Producto")]
    public class ProveedorProductoModel
    {
        [Key, Column(Order = 0)]
        [ForeignKey("Proveedor")]
        public int IdProveedor { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey("Producto")]
        public int IdProducto { get; set; }

        // Navegación
        public virtual ProveedorModel Proveedor { get; set; } = null!;
        public virtual ProductoModel Producto { get; set; } = null!;
    }
}
