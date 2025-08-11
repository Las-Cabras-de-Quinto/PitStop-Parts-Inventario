using PitStop_Parts_Inventario.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.AspNetCore.Identity;
using PitStop_Parts_Inventario.Models;
using PitStop_Parts_Inventario.Services;
using PitStop_Parts_Inventario.Services.Interfaces;
using PitStop_Parts_Inventario.Extensions;

using Microsoft.AspNetCore.Identity.UI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        // Configure DateTime handling to accept ISO strings
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddDbContext<PitStopDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.Configure<AuthMessageSenderOptions>(
    builder.Configuration.GetSection("SendGrid"));

// Configurar Identity con servicios personalizados
builder.Services.AddDefaultIdentity<UsuarioModel>(options => {
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = false;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<PitStopDbContext>();

// Registrar servicios personalizados
builder.Services.AddScoped<IUserStore<UsuarioModel>, CustomUserStore>();
builder.Services.AddScoped<UserManager<UsuarioModel>, CustomUserManager>();
builder.Services.AddScoped<SignInManager<UsuarioModel>, CustomSignInManager>();

// Registrar servicios de la aplicación
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IBodegaService, BodegaService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IMarcaService, MarcaService>();
builder.Services.AddScoped<IProveedorService, ProveedorService>();
builder.Services.AddScoped<IEntradaProductoService, EntradaProductoService>();
builder.Services.AddScoped<IAjusteInventarioService, AjusteInventarioService>();
builder.Services.AddScoped<IEstadoService, EstadoService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(Options =>
{
    Options.IdleTimeout = TimeSpan.FromMinutes(10);
    Options.Cookie.HttpOnly = true;
    Options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

// Usar el middleware de validación de sesión y roles
app.UseSessionValidation();

app.UseSession();
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Ejecutar el seeder
using (var scope = app.Services.CreateScope())
{
    try
    {
        await DbSeeder.SeedAsync(scope.ServiceProvider);
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error al ejecutar el seeder de la base de datos");
    }
}

app.Run();
