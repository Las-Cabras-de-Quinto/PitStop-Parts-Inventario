using System.ComponentModel.DataAnnotations;

namespace PitStop_Parts_Inventario.Models.ViewModels
{
    public class CategoriaEditRequest
    {
        public int IdCategoria { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Descripcion { get; set; } = string.Empty;

        public int IdEstado { get; set; }

        // IDs de los productos relacionados
        public List<int> IdsProductos { get; set; } = new List<int>();
    }
}
