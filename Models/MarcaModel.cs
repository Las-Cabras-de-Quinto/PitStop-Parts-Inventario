using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PitStop_Parts_Inventario.Models
{
    public class MarcaModel
    {
        [Key]
        [Column("IdMarca")]
        public int IdMarca { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Descripcion { get; set; }

        [ForeignKey("Estado")]
        public int IdEstado { get; set; }

        // Navegación uno a muchos
        public virtual EstadoModel? Estado { get; set; }

        // Colecciones
        public virtual ICollection<ProductoModel>? Productos { get; set; }
    }
}
