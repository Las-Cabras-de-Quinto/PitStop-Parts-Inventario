using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace PitStop_Parts_Inventario.Models
{
    public class UsuarioModel : IdentityUser
    {
        [Required]
        public DateTime FechaDeIngreso { get; set; }

        [ForeignKey("Rol")]
        public int IdRol { get; set; }

        [ForeignKey("Estado")]
        public int IdEstado { get; set; }

        // Navegación uno a muchos
        public virtual RolModel Rol { get; set; } = null!;
        public virtual EstadoModel Estado { get; set; } = null!;

        // Colecciones
        public virtual ICollection<EntradaProductoModel> EntradaProductos { get; set; } = new List<EntradaProductoModel>();
        public virtual ICollection<AjusteInventarioModel> AjusteInventarios { get; set; } = new List<AjusteInventarioModel>();
    }
}
