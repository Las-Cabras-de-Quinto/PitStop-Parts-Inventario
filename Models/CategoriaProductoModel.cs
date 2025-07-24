using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PitStop_Parts_Inventario.Models
{
    [Table("Categoria_Producto")]
    public class CategoriaProductoModel
    {
        [Key, Column(Order = 0)]
        [ForeignKey("Categoria")]
        public int IdCategoria { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey("Producto")]
        public int IdProducto { get; set; }

        // Navegación
        public virtual CategoriaModel Categoria { get; set; }
        public virtual ProductoModel Producto { get; set; }
    }
}
