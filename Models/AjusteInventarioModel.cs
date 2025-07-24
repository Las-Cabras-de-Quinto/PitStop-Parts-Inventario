using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PitStop_Parts_Inventario.Models
{
    public class AjusteInventarioModel
    {
        [Key]
        [Column("IdAjusteInventario")]
        public int IdAjusteInventario { get; set; }

        [ForeignKey("Bodega")]
        public int IdBodega { get; set; }

        [ForeignKey("Usuario")]
        public string IdUsuario { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        // Navegación uno a muchos
        public virtual BodegaModel Bodega { get; set; }
        public virtual UsuarioModel Usuario { get; set; }

        // Navegación muchos a muchos
        public virtual ICollection<AjusteInventarioProductoModel> AjusteInventarioProductos { get; set; }
    }
}