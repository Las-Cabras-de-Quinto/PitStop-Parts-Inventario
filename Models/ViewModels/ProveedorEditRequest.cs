using System.ComponentModel.DataAnnotations;

namespace PitStop_Parts_Inventario.Models.ViewModels
{
    public class ProveedorEditRequest
    {
        public int IdProveedor { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(100)]
        public string Contacto { get; set; } = string.Empty;

        [StringLength(50)]
        public string Direccion { get; set; } = string.Empty;

        public int IdEstado { get; set; }

        // IDs de los productos relacionados
        public List<int> IdsProductos { get; set; } = new List<int>();
    }
}
