using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PitStop_Parts_Inventario.Models
{
    public class CategoriaModel
    {
        [Key]
        [Column("IdCategoria")]
        public int IdCategoria { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }

        [StringLength(1000)]
        public string Descripcion { get; set; }

        [ForeignKey("Estado")]
        public int IdEstado { get; set; }

        // Navegación uno a muchos
        public virtual EstadoModel Estado { get; set; }

        // Navegación muchos a muchos
        public virtual ICollection<CategoriaProductoModel> CategoriaProductos { get; set; }
    }
}
