using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PitStop_Parts_Inventario.Models
{
    [Table("Entrada_Producto")]
    public class EntradaProductoModel
    {
        [Key]
        [Column("IdEntrada")]
        public int IdEntrada { get; set; }

        [ForeignKey("Bodega")]
        public int IdBodega { get; set; }

        [ForeignKey("Usuario")]
        public int IdUsuario { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [ForeignKey("Producto")]
        public int IdProducto { get; set; }

        public int CantidadProducto { get; set; }

        // Navegación
        public virtual BodegaModel Bodega { get; set; }
        public virtual UsuarioModel Usuario { get; set; }
        public virtual ProductoModel Producto { get; set; }
    }
}
