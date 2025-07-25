using Microsoft.AspNetCore.Identity;
using PitStop_Parts_Inventario.Models;

namespace PitStop_Parts_Inventario.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<PitStopDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UsuarioModel>>();

            await SeedEstadosAsync(context);
            await SeedRolesAsync(context);
            await SeedDefaultAdminUserAsync(context, userManager);
        }

        private static async Task SeedEstadosAsync(PitStopDbContext context)
        {
            // Verificar si ya existen estados
            if (!context.Estados.Any())
            {
                var estados = new List<EstadoModel>
                {
                    new EstadoModel
                    {
                        Nombre = "Activo",
                        Descripcion = "Usuario activo en el sistema",
                        Funcion = "Permitir acceso al sistema"
                    },
                    new EstadoModel
                    {
                        Nombre = "Inactivo",
                        Descripcion = "Usuario inactivo en el sistema",
                        Funcion = "Bloquear acceso al sistema"
                    }
                };

                context.Estados.AddRange(estados);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedRolesAsync(PitStopDbContext context)
        {
            // Verificar si ya existen roles
            if (!context.Roles.Any())
            {
                // Obtener el ID del estado activo
                var estadoActivoId = context.Estados.First(e => e.Nombre == "Activo").IdEstado;

                var roles = new List<RolModel>
                {
                    new RolModel
                    {
                        Nombre = "Administrador",
                        Funcion = "Acceso completo al sistema, gestión de usuarios, inventario y configuración",
                        Admin = true,
                        IdEstado = estadoActivoId
                    },
                    new RolModel
                    {
                        Nombre = "Empleado",
                        Funcion = "Acceso limitado al sistema, gestión básica de inventario",
                        Admin = false,
                        IdEstado = estadoActivoId
                    }
                };

                context.Roles.AddRange(roles);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedDefaultAdminUserAsync(PitStopDbContext context, UserManager<UsuarioModel> userManager)
        {
            // Verificar si ya existe un usuario administrador
            var adminExists = await userManager.FindByEmailAsync("admin@pitstop.com");
            if (adminExists == null)
            {
                var estadoActivo = context.Estados.First(e => e.Nombre == "Activo");
                var rolAdministrador = context.Roles.First(r => r.Nombre == "Administrador");

                var adminUser = new UsuarioModel
                {
                    UserName = "admin@pitstop.com",
                    Email = "admin@pitstop.com",
                    EmailConfirmed = true,
                    FechaDeIngreso = DateTime.Now,
                    IdEstado = estadoActivo.IdEstado,
                    IdRol = rolAdministrador.IdRol
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException($"Error al crear el usuario administrador: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }
    }
}
