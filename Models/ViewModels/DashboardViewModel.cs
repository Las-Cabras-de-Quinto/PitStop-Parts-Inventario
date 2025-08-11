using PitStop_Parts_Inventario.Models;

namespace PitStop_Parts_Inventario.Models.ViewModels
{
    public class DashboardViewModel
    {
        // Stats Cards Data
        public int ProductosActivos { get; set; }
        public int ProductosInactivos { get; set; }
        public int TotalBodegas { get; set; }
        public int BodegasSinEspacioMinimo { get; set; }
        public int ProveedoresActivos { get; set; }
        public int ProveedoresInactivos { get; set; }
        public int UsuariosDelSistema { get; set; }
        public int UsuariosFueraEmpresa { get; set; }

        // Recent Entries Data
        public IEnumerable<EntradaProductoModel> UltimasEntradas { get; set; } = new List<EntradaProductoModel>();

        // Daily Summary Data
        public int EntradasHoy { get; set; }
        public int ProductosDistintosHoy { get; set; }
        public int ProveedoresInvolucradosHoy { get; set; }

        // System Alerts Data
        public IEnumerable<string> AlertasSistema { get; set; } = new List<string>();
    }
}
