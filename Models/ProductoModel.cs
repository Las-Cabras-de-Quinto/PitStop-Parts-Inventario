using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PitStop_Parts_Inventario.Models
{
    public class ProductoModel
    {
        [Key]
        [Column("IdProducto")]
        public int IdProducto { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;

        public int SKU { get; set; }

        [StringLength(1000)]
        public string? Descripcion { get; set; }

        [ForeignKey("Marca")]
        public int IdMarca { get; set; }

        [ForeignKey("Estado")]
        public int IdEstado { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal PrecioVenta { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal PrecioCompra { get; set; }

        public int StockMin { get; set; }

        public int StockMax { get; set; }
        public int StockActual { get; set; } = 0;

        // Navegación uno a muchos
        public virtual MarcaModel? Marca { get; set; }
        public virtual EstadoModel? Estado { get; set; }

        // Navegación muchos a muchos
        public virtual ICollection<BodegaProductoModel>? BodegaProductos { get; set; }
        public virtual ICollection<CategoriaProductoModel>? CategoriaProductos { get; set; }
        public virtual ICollection<ProveedorProductoModel>? ProveedorProductos { get; set; }
        public virtual ICollection<AjusteInventarioProductoModel>? AjusteInventarioProductos { get; set; }
        public virtual ICollection<EntradaProductoModel>? EntradaProductos { get; set; }
    }
}
