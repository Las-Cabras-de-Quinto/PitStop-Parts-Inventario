using System.ComponentModel.DataAnnotations;
using PitStop_Parts_Inventario.Models;

namespace PitStop_Parts_Inventario.Models.ViewModels
{
    public class UsuarioViewModel
    {
        public string Id { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [Display(Name = "Nombre de Usuario")]
        public string UserName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "El email no es válido")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
        
        [Display(Name = "Contraseña")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        
        [Display(Name = "Confirmar Contraseña")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        public string? ConfirmPassword { get; set; }
        
        [Required(ErrorMessage = "El rol es requerido")]
        [Display(Name = "Rol")]
        public int IdRol { get; set; }
        
        [Required(ErrorMessage = "El estado es requerido")]
        [Display(Name = "Estado")]
        public int IdEstado { get; set; }
        
        [Display(Name = "Fecha de Ingreso")]
        public DateTime FechaDeIngreso { get; set; }
        
        // Para mostrar información
        public string? RolNombre { get; set; }
        public string? EstadoNombre { get; set; }
        
        // Para los dropdowns
        public IEnumerable<RolModel> Roles { get; set; } = new List<RolModel>();
        public IEnumerable<EstadoModel> Estados { get; set; } = new List<EstadoModel>();
        
        public bool IsEditMode => !string.IsNullOrEmpty(Id);
    }
    
    public class ChangePasswordViewModel
    {
        public string UserId { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "La nueva contraseña es requerida")]
        [Display(Name = "Nueva Contraseña")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "La contraseña debe tener al menos {2} caracteres.", MinimumLength = 6)]
        public string NewPassword { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Debe confirmar la nueva contraseña")]
        [Display(Name = "Confirmar Nueva Contraseña")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; } = string.Empty;
        
        public string UserName { get; set; } = string.Empty;
    }
    
    public class UsuarioFilterOptions
    {
        public string? SearchTerm { get; set; }
        public int? RolId { get; set; }
        public int? EstadoId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
