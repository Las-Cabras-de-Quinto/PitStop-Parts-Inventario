using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PitStop_Parts_Inventario.Models
{
    public class EstadoModel
    {
        [Key]
        [Column("IdEstado")]
        public int IdEstado { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }

        [StringLength(100)]
        public string Descripcion { get; set; }

        [Required]
        [StringLength(50)]
        public string Funcion { get; set; }

        // Colecciones - Estado es utilizado por múltiples entidades
        public virtual ICollection<RolModel> Roles { get; set; }
        public virtual ICollection<UsuarioModel> Usuarios { get; set; }
        public virtual ICollection<BodegaModel> Bodegas { get; set; }
        public virtual ICollection<CategoriaModel> Categorias { get; set; }
        public virtual ICollection<MarcaModel> Marcas { get; set; }
        public virtual ICollection<ProveedorModel> Proveedores { get; set; }
        public virtual ICollection<ProductoModel> Productos { get; set; }
        public virtual ICollection<BodegaProductoModel> BodegaProductos { get; set; }
    }
}
