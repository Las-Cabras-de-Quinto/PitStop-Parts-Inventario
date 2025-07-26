using System.Linq.Expressions;
using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Models.ViewModels;

namespace PitStop_Parts_Inventario.Services.Helpers
{
    public static class FilterHelper
    {
        public static IQueryable<T> ApplyBaseFilters<T>(IQueryable<T> query, BaseFilterOptions? filters) 
            where T : class
        {
            if (filters == null) return query;

            // Filtro por fechas (si la entidad tiene propiedades de fecha)
            if (filters.FechaDesde.HasValue)
            {
                var property = typeof(T).GetProperty("FechaCreacion") ?? typeof(T).GetProperty("Fecha");
                if (property != null && (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?)))
                {
                    var parameter = Expression.Parameter(typeof(T), "x");
                    var propertyAccess = Expression.Property(parameter, property);
                    var constant = Expression.Constant(filters.FechaDesde.Value);
                    var comparison = Expression.GreaterThanOrEqual(propertyAccess, constant);
                    var lambda = Expression.Lambda<Func<T, bool>>(comparison, parameter);
                    query = query.Where(lambda);
                }
            }

            if (filters.FechaHasta.HasValue)
            {
                var property = typeof(T).GetProperty("FechaCreacion") ?? typeof(T).GetProperty("Fecha");
                if (property != null && (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?)))
                {
                    var parameter = Expression.Parameter(typeof(T), "x");
                    var propertyAccess = Expression.Property(parameter, property);
                    var constant = Expression.Constant(filters.FechaHasta.Value.AddDays(1)); // Incluir todo el día
                    var comparison = Expression.LessThan(propertyAccess, constant);
                    var lambda = Expression.Lambda<Func<T, bool>>(comparison, parameter);
                    query = query.Where(lambda);
                }
            }

            return query;
        }

        public static IQueryable<ProductoModel> ApplyProductoFilters(IQueryable<ProductoModel> query, ProductoFilterOptions? filters)
        {
            if (filters == null) return query;

            query = ApplyBaseFilters(query, filters);

            // Filtro por búsqueda
            if (!string.IsNullOrEmpty(filters.SearchTerm))
            {
                var searchTerm = filters.SearchTerm.ToLower();
                query = query.Where(p => 
                    p.Nombre.ToLower().Contains(searchTerm) ||
                    (p.Descripcion != null && p.Descripcion.ToLower().Contains(searchTerm)) ||
                    p.SKU.ToString().Contains(searchTerm) ||
                    (p.Marca != null && p.Marca.Nombre.ToLower().Contains(searchTerm)));
            }

            // Filtro por marca
            if (filters.MarcaId.HasValue)
            {
                query = query.Where(p => p.IdMarca == filters.MarcaId.Value);
            }

            // Filtro por estado
            if (filters.EstadoId.HasValue)
            {
                query = query.Where(p => p.IdEstado == filters.EstadoId.Value);
            }
            else if (filters.SoloActivos == true)
            {
                query = query.Where(p => p.Estado != null && p.Estado.Nombre == "Activo");
            }

            // Filtro por categoría
            if (filters.CategoriaId.HasValue)
            {
                query = query.Where(p => p.CategoriaProductos != null && 
                    p.CategoriaProductos.Any(cp => cp.IdCategoria == filters.CategoriaId.Value));
            }

            // Filtro por proveedor
            if (filters.ProveedorId.HasValue)
            {
                query = query.Where(p => p.ProveedorProductos != null && 
                    p.ProveedorProductos.Any(pp => pp.IdProveedor == filters.ProveedorId.Value));
            }

            // Filtro por bodega
            if (filters.BodegaId.HasValue)
            {
                query = query.Where(p => p.BodegaProductos != null && 
                    p.BodegaProductos.Any(bp => bp.IdBodega == filters.BodegaId.Value));
            }

            // Filtros por precio
            if (filters.PrecioMinimo.HasValue)
            {
                query = query.Where(p => p.PrecioVenta >= filters.PrecioMinimo.Value);
            }

            if (filters.PrecioMaximo.HasValue)
            {
                query = query.Where(p => p.PrecioVenta <= filters.PrecioMaximo.Value);
            }

            // Filtros por stock
            if (filters.StockMinimo.HasValue)
            {
                query = query.Where(p => p.StockActual >= filters.StockMinimo.Value);
            }

            if (filters.StockMaximo.HasValue)
            {
                query = query.Where(p => p.StockActual <= filters.StockMaximo.Value);
            }

            if (filters.ConStock == true)
            {
                query = query.Where(p => p.StockActual > 0);
            }

            if (filters.StockBajo == true)
            {
                query = query.Where(p => p.StockActual < p.StockMin);
            }

            return query;
        }

        public static IQueryable<ProveedorModel> ApplyProveedorFilters(IQueryable<ProveedorModel> query, ProveedorFilterOptions? filters)
        {
            if (filters == null) return query;

            query = ApplyBaseFilters(query, filters);

            // Filtro por búsqueda
            if (!string.IsNullOrEmpty(filters.SearchTerm))
            {
                var searchTerm = filters.SearchTerm.ToLower();
                query = query.Where(p => 
                    p.Nombre.ToLower().Contains(searchTerm) ||
                    (p.Contacto != null && p.Contacto.ToLower().Contains(searchTerm)) ||
                    (p.Direccion != null && p.Direccion.ToLower().Contains(searchTerm)));
            }

            // Filtro por estado
            if (filters.EstadoId.HasValue)
            {
                query = query.Where(p => p.IdEstado == filters.EstadoId.Value);
            }
            else if (filters.SoloActivos == true)
            {
                query = query.Where(p => p.Estado != null && p.Estado.Nombre == "Activo");
            }

            // Filtro por contacto
            if (!string.IsNullOrEmpty(filters.Contacto))
            {
                query = query.Where(p => p.Contacto != null && p.Contacto.ToLower().Contains(filters.Contacto.ToLower()));
            }

            // Filtro por dirección
            if (!string.IsNullOrEmpty(filters.Direccion))
            {
                query = query.Where(p => p.Direccion != null && p.Direccion.ToLower().Contains(filters.Direccion.ToLower()));
            }

            // Filtro por tener productos
            if (filters.TieneProductos == true)
            {
                query = query.Where(p => p.ProveedorProductos != null && p.ProveedorProductos.Any());
            }
            else if (filters.TieneProductos == false)
            {
                query = query.Where(p => p.ProveedorProductos == null || !p.ProveedorProductos.Any());
            }

            return query;
        }

        public static IQueryable<CategoriaModel> ApplyCategoriaFilters(IQueryable<CategoriaModel> query, CategoriaFilterOptions? filters)
        {
            if (filters == null) return query;

            query = ApplyBaseFilters(query, filters);

            // Filtro por búsqueda
            if (!string.IsNullOrEmpty(filters.SearchTerm))
            {
                var searchTerm = filters.SearchTerm.ToLower();
                query = query.Where(c => 
                    c.Nombre.ToLower().Contains(searchTerm) ||
                    (c.Descripcion != null && c.Descripcion.ToLower().Contains(searchTerm)));
            }

            // Filtro por estado
            if (filters.EstadoId.HasValue)
            {
                query = query.Where(c => c.IdEstado == filters.EstadoId.Value);
            }
            else if (filters.SoloActivos == true)
            {
                query = query.Where(c => c.Estado != null && c.Estado.Nombre == "Activo");
            }

            // Filtro por tener productos
            if (filters.TieneProductos == true)
            {
                query = query.Where(c => c.CategoriaProductos != null && c.CategoriaProductos.Any());
            }
            else if (filters.TieneProductos == false)
            {
                query = query.Where(c => c.CategoriaProductos == null || !c.CategoriaProductos.Any());
            }

            return query;
        }

        public static IQueryable<MarcaModel> ApplyMarcaFilters(IQueryable<MarcaModel> query, MarcaFilterOptions? filters)
        {
            if (filters == null) return query;

            query = ApplyBaseFilters(query, filters);

            // Filtro por búsqueda
            if (!string.IsNullOrEmpty(filters.SearchTerm))
            {
                var searchTerm = filters.SearchTerm.ToLower();
                query = query.Where(m => 
                    m.Nombre.ToLower().Contains(searchTerm) ||
                    (m.Descripcion != null && m.Descripcion.ToLower().Contains(searchTerm)));
            }

            // Filtro por estado
            if (filters.EstadoId.HasValue)
            {
                query = query.Where(m => m.IdEstado == filters.EstadoId.Value);
            }
            else if (filters.SoloActivos == true)
            {
                query = query.Where(m => m.Estado != null && m.Estado.Nombre == "Activo");
            }

            // Filtro por tener productos
            if (filters.TieneProductos == true)
            {
                query = query.Where(m => m.Productos != null && m.Productos.Any());
            }
            else if (filters.TieneProductos == false)
            {
                query = query.Where(m => m.Productos == null || !m.Productos.Any());
            }

            return query;
        }

        public static IQueryable<BodegaModel> ApplyBodegaFilters(IQueryable<BodegaModel> query, BodegaFilterOptions? filters)
        {
            if (filters == null) return query;

            query = ApplyBaseFilters(query, filters);

            // Filtro por búsqueda
            if (!string.IsNullOrEmpty(filters.SearchTerm))
            {
                var searchTerm = filters.SearchTerm.ToLower();
                query = query.Where(b => 
                    b.Nombre.ToLower().Contains(searchTerm) ||
                    (b.Ubicacion != null && b.Ubicacion.ToLower().Contains(searchTerm)) ||
                    (b.Descripcion != null && b.Descripcion.ToLower().Contains(searchTerm)));
            }

            // Filtro por estado
            if (filters.EstadoId.HasValue)
            {
                query = query.Where(b => b.IdEstado == filters.EstadoId.Value);
            }
            else if (filters.SoloActivos == true)
            {
                query = query.Where(b => b.Estado != null && b.Estado.Nombre == "Activo");
            }

            // Filtro por ubicación
            if (!string.IsNullOrEmpty(filters.Ubicacion))
            {
                query = query.Where(b => b.Ubicacion != null && b.Ubicacion.ToLower().Contains(filters.Ubicacion.ToLower()));
            }

            // Filtro por tener productos
            if (filters.TieneProductos == true)
            {
                query = query.Where(b => b.BodegaProductos != null && b.BodegaProductos.Any());
            }
            else if (filters.TieneProductos == false)
            {
                query = query.Where(b => b.BodegaProductos == null || !b.BodegaProductos.Any());
            }

            return query;
        }

        public static IQueryable<EntradaProductoModel> ApplyEntradaProductoFilters(IQueryable<EntradaProductoModel> query, EntradaProductoFilterOptions? filters)
        {
            if (filters == null) return query;

            query = ApplyBaseFilters(query, filters);

            // Filtro por búsqueda (usando campos disponibles)
            if (!string.IsNullOrEmpty(filters.SearchTerm))
            {
                var searchTerm = filters.SearchTerm.ToLower();
                query = query.Where(e => 
                    (e.Producto != null && e.Producto.Nombre.ToLower().Contains(searchTerm)) ||
                    (e.Bodega != null && e.Bodega.Nombre.ToLower().Contains(searchTerm)) ||
                    (e.Usuario != null && e.Usuario.UserName != null && e.Usuario.UserName.ToLower().Contains(searchTerm)));
            }

            // Filtro por producto
            if (filters.ProductoId.HasValue)
            {
                query = query.Where(e => e.IdProducto == filters.ProductoId.Value);
            }

            // Filtro por bodega
            if (filters.BodegaId.HasValue)
            {
                query = query.Where(e => e.IdBodega == filters.BodegaId.Value);
            }

            // Filtros por cantidad (usar CantidadProducto que es el campo correcto)
            if (filters.CantidadMinima.HasValue)
            {
                query = query.Where(e => e.CantidadProducto >= filters.CantidadMinima.Value);
            }

            if (filters.CantidadMaxima.HasValue)
            {
                query = query.Where(e => e.CantidadProducto <= filters.CantidadMaxima.Value);
            }

            // Nota: CostoUnitario no existe en EntradaProductoModel, se omiten estos filtros
            // Si se necesitan, se deberían agregar campos al modelo

            return query;
        }

        public static IQueryable<AjusteInventarioModel> ApplyAjusteInventarioFilters(IQueryable<AjusteInventarioModel> query, AjusteInventarioFilterOptions? filters)
        {
            if (filters == null) return query;

            query = ApplyBaseFilters(query, filters);

            // Filtro por búsqueda
            if (!string.IsNullOrEmpty(filters.SearchTerm))
            {
                var searchTerm = filters.SearchTerm.ToLower();
                query = query.Where(a => 
                    (a.Usuario != null && a.Usuario.UserName != null && a.Usuario.UserName.ToLower().Contains(searchTerm)) ||
                    (a.Bodega != null && a.Bodega.Nombre.ToLower().Contains(searchTerm)));
            }

            // Filtro por bodega
            if (filters.BodegaId.HasValue)
            {
                query = query.Where(a => a.IdBodega == filters.BodegaId.Value);
            }

            // Filtro por producto (a través de la tabla intermedia)
            if (filters.ProductoId.HasValue)
            {
                query = query.Where(a => a.AjusteInventarioProductos.Any(ap => ap.IdProducto == filters.ProductoId.Value));
            }

            // Filtros por cantidad (de la tabla intermedia)
            if (filters.CantidadMinima.HasValue)
            {
                query = query.Where(a => a.AjusteInventarioProductos.Any(ap => ap.CantidadProducto >= filters.CantidadMinima.Value));
            }

            if (filters.CantidadMaxima.HasValue)
            {
                query = query.Where(a => a.AjusteInventarioProductos.Any(ap => ap.CantidadProducto <= filters.CantidadMaxima.Value));
            }

            return query;
        }

        public static IQueryable<EstadoModel> ApplyEstadoFilters(IQueryable<EstadoModel> query, EstadoFilterOptions? filters)
        {
            if (filters == null) return query;

            query = ApplyBaseFilters(query, filters);

            // Filtro por búsqueda
            if (!string.IsNullOrEmpty(filters.SearchTerm))
            {
                var searchTerm = filters.SearchTerm.ToLower();
                query = query.Where(e => 
                    e.Nombre.ToLower().Contains(searchTerm) ||
                    (e.Descripcion != null && e.Descripcion.ToLower().Contains(searchTerm)));
            }

            // Filtro por estado activo (si es el estado "Activo")
            if (filters.EsActivo == true)
            {
                query = query.Where(e => e.Nombre == "Activo");
            }
            else if (filters.EsActivo == false)
            {
                query = query.Where(e => e.Nombre != "Activo");
            }

            return query;
        }

        public static IQueryable<RolModel> ApplyRolFilters(IQueryable<RolModel> query, RolFilterOptions? filters)
        {
            if (filters == null) return query;

            query = ApplyBaseFilters(query, filters);

            // Filtro por búsqueda
            if (!string.IsNullOrEmpty(filters.SearchTerm))
            {
                var searchTerm = filters.SearchTerm.ToLower();
                query = query.Where(r => 
                    r.Nombre.ToLower().Contains(searchTerm) ||
                    (r.Funcion != null && r.Funcion.ToLower().Contains(searchTerm)));
            }

            // Filtro por estado
            if (filters.EstadoId.HasValue)
            {
                query = query.Where(r => r.IdEstado == filters.EstadoId.Value);
            }
            else if (filters.SoloActivos == true)
            {
                query = query.Where(r => r.Estado != null && r.Estado.Nombre == "Activo");
            }

            // Filtro por tipo (si existe la propiedad)
            if (!string.IsNullOrEmpty(filters.Tipo))
            {
                // Asumir que existe una propiedad Tipo en el RolModel, si no se ignora
                // query = query.Where(r => r.Tipo != null && r.Tipo.ToLower() == filters.Tipo.ToLower());
            }

            return query;
        }
    }
}
