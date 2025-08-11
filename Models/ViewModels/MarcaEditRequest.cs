using System.ComponentModel.DataAnnotations;

namespace PitStop_Parts_Inventario.Models.ViewModels
{
    public class MarcaEditRequest
    {
        public int IdMarca { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Descripcion { get; set; }

        public int IdEstado { get; set; }
    }
}
