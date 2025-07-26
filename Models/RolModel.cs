using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PitStop_Parts_Inventario.Models
{
    public class RolModel
    {
        [Key]
        [Column("IdRol")]
        public int IdRol { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Funcion { get; set; } = string.Empty;

        public bool Admin { get; set; }

        [ForeignKey("Estado")]
        public int IdEstado { get; set; }

        // Navegación uno a muchos
        public virtual EstadoModel Estado { get; set; } = null!;

        // Colecciones
        public virtual ICollection<UsuarioModel> Usuarios { get; set; } = new List<UsuarioModel>();
    }
}
