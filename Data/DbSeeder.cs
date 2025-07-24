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
                        IdEstado = 1,
                        Nombre = "Activo",
                        Descripcion = "Usuario activo en el sistema",
                        Funcion = "Permitir acceso al sistema"
                    },
                    new EstadoModel
                    {
                        IdEstado = 2,
                        Nombre = "Inactivo",
                        Descripcion = "Usuario inactivo en el sistema",
                        Funcion = "Bloquear acceso al sistema"
                    }
                };

                context.Estados.AddRange(estados);
                await context.SaveChangesAsync();
            }
        }
    }
}
