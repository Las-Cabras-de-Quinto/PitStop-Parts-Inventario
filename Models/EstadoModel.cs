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
        public string Nombre { get; set; } = string.Empty;

        [StringLength(100)]
        public string Descripcion { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Funcion { get; set; } = string.Empty;

        // Colecciones - Estado es utilizado por múltiples entidades
        public virtual ICollection<RolModel> Roles { get; set; } = new List<RolModel>();
        public virtual ICollection<UsuarioModel> Usuarios { get; set; } = new List<UsuarioModel>();
        public virtual ICollection<BodegaModel> Bodegas { get; set; } = new List<BodegaModel>();
        public virtual ICollection<CategoriaModel> Categorias { get; set; } = new List<CategoriaModel>();
        public virtual ICollection<MarcaModel> Marcas { get; set; } = new List<MarcaModel>();
        public virtual ICollection<ProveedorModel> Proveedores { get; set; } = new List<ProveedorModel>();
        public virtual ICollection<ProductoModel> Productos { get; set; } = new List<ProductoModel>();
        public virtual ICollection<BodegaProductoModel> BodegaProductos { get; set; } = new List<BodegaProductoModel>();
    }
}
