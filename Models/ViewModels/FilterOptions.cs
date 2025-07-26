namespace PitStop_Parts_Inventario.Models.ViewModels
{
    public class BaseFilterOptions
    {
        public string? SearchTerm { get; set; }
        public int? EstadoId { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public bool? SoloActivos { get; set; } = true;
    }

    public class ProductoFilterOptions : BaseFilterOptions
    {
        public int? MarcaId { get; set; }
        public int? CategoriaId { get; set; }
        public int? ProveedorId { get; set; }
        public int? BodegaId { get; set; }
        public decimal? PrecioMinimo { get; set; }
        public decimal? PrecioMaximo { get; set; }
        public int? StockMinimo { get; set; }
        public int? StockMaximo { get; set; }
        public bool? ConStock { get; set; }
        public bool? StockBajo { get; set; } // Stock menor al mínimo
    }

    public class ProveedorFilterOptions : BaseFilterOptions
    {
        public string? Contacto { get; set; }
        public string? Direccion { get; set; }
        public bool? TieneProductos { get; set; }
    }

    public class CategoriaFilterOptions : BaseFilterOptions
    {
        public bool? TieneProductos { get; set; }
    }

    public class MarcaFilterOptions : BaseFilterOptions
    {
        public bool? TieneProductos { get; set; }
    }

    public class BodegaFilterOptions : BaseFilterOptions
    {
        public string? Ubicacion { get; set; }
        public bool? TieneProductos { get; set; }
    }

    public class EntradaProductoFilterOptions : BaseFilterOptions
    {
        public int? ProductoId { get; set; }
        public int? BodegaId { get; set; }
        public int? CantidadMinima { get; set; }
        public int? CantidadMaxima { get; set; }
    }

    public class AjusteInventarioFilterOptions : BaseFilterOptions
    {
        public int? BodegaId { get; set; }
        public int? ProductoId { get; set; } // A través de la tabla intermedia
        public string? TipoAjuste { get; set; } // Si se define un campo para esto
        public int? CantidadMinima { get; set; } // De la tabla intermedia
        public int? CantidadMaxima { get; set; } // De la tabla intermedia
    }

    public class EstadoFilterOptions : BaseFilterOptions
    {
        public bool? EsActivo { get; set; }
        public bool? TieneEntidades { get; set; } // Si tiene entidades asociadas
    }

    public class RolFilterOptions : BaseFilterOptions
    {
        public bool? TieneUsuarios { get; set; } // Si tiene usuarios asociados
        public string? Tipo { get; set; } // Tipo de rol si existe
    }
}
