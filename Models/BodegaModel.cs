using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PitStop_Parts_Inventario.Models
{
    public class BodegaModel
    {
        [Key]
        [Column("IdBodega")]
        public int IdBodega { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }

        [StringLength(100)]
        public string Descripcion { get; set; }

        [ForeignKey("Estado")]
        public int IdEstado { get; set; }

        [StringLength(50)]
        public string Ubicacion { get; set; }

        // Navegación uno a muchos
        public virtual EstadoModel Estado { get; set; }

        // Navegación muchos a muchos y uno a muchos
        public virtual ICollection<BodegaProductoModel> BodegaProductos { get; set; }
        public virtual ICollection<EntradaProductoModel> EntradaProductos { get; set; }
        public virtual ICollection<AjusteInventarioModel> AjusteInventarios { get; set; }
    }
}
