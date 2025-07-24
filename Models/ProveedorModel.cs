using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PitStop_Parts_Inventario.Models
{
    public class ProveedorModel
    {
        [Key]
        [Column("IdProveedor")]
        public int IdProveedor { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }

        [StringLength(100)]
        public string Contacto { get; set; }

        [StringLength(50)]
        public string Direccion { get; set; }

        [ForeignKey("Estado")]
        public int IdEstado { get; set; }

        // Navegación uno a muchos
        public virtual EstadoModel Estado { get; set; }

        // Navegación muchos a muchos
        public virtual ICollection<ProveedorProductoModel> ProveedorProductos { get; set; }
    }
}
