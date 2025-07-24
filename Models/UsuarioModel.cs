using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace PitStop_Parts_Inventario.Models
{
    public class UsuarioModel : IdentityUser
    {
        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(50)]
        public string Apellido { get; set; }

        [Required]
        public DateTime FechaDeIngreso { get; set; }

        [ForeignKey("Rol")]
        public int IdRol { get; set; }

        [ForeignKey("Estado")]
        public int IdEstado { get; set; }

        // Navegación uno a muchos
        public virtual RolModel Rol { get; set; }
        public virtual EstadoModel Estado { get; set; }

        // Colecciones
        public virtual ICollection<EntradaProductoModel> EntradaProductos { get; set; }
        public virtual ICollection<AjusteInventarioModel> AjusteInventarios { get; set; }
    }
}
