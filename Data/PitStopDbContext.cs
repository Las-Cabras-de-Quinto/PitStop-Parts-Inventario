using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PitStop_Parts_Inventario.Models;

namespace PitStop_Parts_Inventario.Data
{
    public class PitStopDbContext : IdentityDbContext<UsuarioModel>
    {
        public PitStopDbContext(DbContextOptions<PitStopDbContext> options) : base(options)
        {
        }

        // DbSets para todas las entidades
        public DbSet<EstadoModel> Estados { get; set; }
        public new DbSet<RolModel> Roles { get; set; }
        public DbSet<UsuarioModel> Usuarios { get; set; }
        public DbSet<BodegaModel> Bodegas { get; set; }
        public DbSet<CategoriaModel> Categorias { get; set; }
        public DbSet<MarcaModel> Marcas { get; set; }
        public DbSet<ProveedorModel> Proveedores { get; set; }
        public DbSet<ProductoModel> Productos { get; set; }
        public DbSet<EntradaProductoModel> EntradaProductos { get; set; }
        public DbSet<AjusteInventarioModel> AjusteInventarios { get; set; }

        // DbSets para las tablas de relación
        public DbSet<BodegaProductoModel> BodegaProductos { get; set; }
        public DbSet<CategoriaProductoModel> CategoriaProductos { get; set; }
        public DbSet<ProveedorProductoModel> ProveedorProductos { get; set; }
        public DbSet<AjusteInventarioProductoModel> AjusteInventarioProductos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de claves compuestas para relaciones muchos a muchos
            modelBuilder.Entity<BodegaProductoModel>()
                .HasKey(bp => new { bp.IdBodega, bp.IdProducto });

            modelBuilder.Entity<CategoriaProductoModel>()
                .HasKey(cp => new { cp.IdCategoria, cp.IdProducto });

            modelBuilder.Entity<ProveedorProductoModel>()
                .HasKey(pp => new { pp.IdProveedor, pp.IdProducto });

            modelBuilder.Entity<AjusteInventarioProductoModel>()
                .HasKey(aip => new { aip.IdAjusteInventario, aip.IdProducto });

            // Configuración de relaciones muchos a muchos

            // BodegaProducto
            modelBuilder.Entity<BodegaProductoModel>()
                .HasOne(bp => bp.Bodega)
                .WithMany(b => b.BodegaProductos)
                .HasForeignKey(bp => bp.IdBodega)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BodegaProductoModel>()
                .HasOne(bp => bp.Producto)
                .WithMany(p => p.BodegaProductos)
                .HasForeignKey(bp => bp.IdProducto)
                .OnDelete(DeleteBehavior.Restrict);

            // CategoriaProducto
            modelBuilder.Entity<CategoriaProductoModel>()
                .HasOne(cp => cp.Categoria)
                .WithMany(c => c.CategoriaProductos)
                .HasForeignKey(cp => cp.IdCategoria)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CategoriaProductoModel>()
                .HasOne(cp => cp.Producto)
                .WithMany(p => p.CategoriaProductos)
                .HasForeignKey(cp => cp.IdProducto)
                .OnDelete(DeleteBehavior.Restrict);

            // ProveedorProducto
            modelBuilder.Entity<ProveedorProductoModel>()
                .HasOne(pp => pp.Proveedor)
                .WithMany(pr => pr.ProveedorProductos)
                .HasForeignKey(pp => pp.IdProveedor)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProveedorProductoModel>()
                .HasOne(pp => pp.Producto)
                .WithMany(p => p.ProveedorProductos)
                .HasForeignKey(pp => pp.IdProducto)
                .OnDelete(DeleteBehavior.Restrict);

            // AjusteInventarioProducto
            modelBuilder.Entity<AjusteInventarioProductoModel>()
                .HasOne(aip => aip.AjusteInventario)
                .WithMany(ai => ai.AjusteInventarioProductos)
                .HasForeignKey(aip => aip.IdAjusteInventario)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AjusteInventarioProductoModel>()
                .HasOne(aip => aip.Producto)
                .WithMany(p => p.AjusteInventarioProductos)
                .HasForeignKey(aip => aip.IdProducto)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración de relaciones uno a muchos con DeleteBehavior.Restrict
            // para evitar cascadas múltiples

            // Usuario -> Rol
            modelBuilder.Entity<UsuarioModel>()
                .HasOne(u => u.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(u => u.IdRol)
                .OnDelete(DeleteBehavior.Restrict);

            // Usuario -> Estado
            modelBuilder.Entity<UsuarioModel>()
                .HasOne(u => u.Estado)
                .WithMany(e => e.Usuarios)
                .HasForeignKey(u => u.IdEstado)
                .OnDelete(DeleteBehavior.Restrict);

            // Rol -> Estado
            modelBuilder.Entity<RolModel>()
                .HasOne(r => r.Estado)
                .WithMany(e => e.Roles)
                .HasForeignKey(r => r.IdEstado)
                .OnDelete(DeleteBehavior.Restrict);

            // Bodega -> Estado
            modelBuilder.Entity<BodegaModel>()
                .HasOne(b => b.Estado)
                .WithMany(e => e.Bodegas)
                .HasForeignKey(b => b.IdEstado)
                .OnDelete(DeleteBehavior.Restrict);

            // Categoria -> Estado
            modelBuilder.Entity<CategoriaModel>()
                .HasOne(c => c.Estado)
                .WithMany(e => e.Categorias)
                .HasForeignKey(c => c.IdEstado)
                .OnDelete(DeleteBehavior.Restrict);

            // Marca -> Estado
            modelBuilder.Entity<MarcaModel>()
                .HasOne(m => m.Estado)
                .WithMany(e => e.Marcas)
                .HasForeignKey(m => m.IdEstado)
                .OnDelete(DeleteBehavior.Restrict);

            // Proveedor -> Estado
            modelBuilder.Entity<ProveedorModel>()
                .HasOne(p => p.Estado)
                .WithMany(e => e.Proveedores)
                .HasForeignKey(p => p.IdEstado)
                .OnDelete(DeleteBehavior.Restrict);

            // Producto -> Marca
            modelBuilder.Entity<ProductoModel>()
                .HasOne(p => p.Marca)
                .WithMany(m => m.Productos)
                .HasForeignKey(p => p.IdMarca)
                .OnDelete(DeleteBehavior.Restrict);

            // Producto -> Estado
            modelBuilder.Entity<ProductoModel>()
                .HasOne(p => p.Estado)
                .WithMany(e => e.Productos)
                .HasForeignKey(p => p.IdEstado)
                .OnDelete(DeleteBehavior.Restrict);

            // EntradaProducto -> Bodega
            modelBuilder.Entity<EntradaProductoModel>()
                .HasOne(ep => ep.Bodega)
                .WithMany(b => b.EntradaProductos)
                .HasForeignKey(ep => ep.IdBodega)
                .OnDelete(DeleteBehavior.Restrict);

            // EntradaProducto -> Usuario
            modelBuilder.Entity<EntradaProductoModel>()
                .HasOne(ep => ep.Usuario)
                .WithMany(u => u.EntradaProductos)
                .HasForeignKey(ep => ep.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            // EntradaProducto -> Producto
            modelBuilder.Entity<EntradaProductoModel>()
                .HasOne(ep => ep.Producto)
                .WithMany(p => p.EntradaProductos)
                .HasForeignKey(ep => ep.IdProducto)
                .OnDelete(DeleteBehavior.Restrict);

            // AjusteInventario -> Bodega
            modelBuilder.Entity<AjusteInventarioModel>()
                .HasOne(ai => ai.Bodega)
                .WithMany(b => b.AjusteInventarios)
                .HasForeignKey(ai => ai.IdBodega)
                .OnDelete(DeleteBehavior.Restrict);

            // AjusteInventario -> Usuario
            modelBuilder.Entity<AjusteInventarioModel>()
                .HasOne(ai => ai.Usuario)
                .WithMany(u => u.AjusteInventarios)
                .HasForeignKey(ai => ai.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            // BodegaProducto -> Estado
            modelBuilder.Entity<BodegaProductoModel>()
                .HasOne(bp => bp.Estado)
                .WithMany(e => e.BodegaProductos)
                .HasForeignKey(bp => bp.IdEstado)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
