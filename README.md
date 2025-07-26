# PitStop Parts Inventario

Sistema de gestión de inventario para repuestos automotrices desarrollado en ASP.NET Core 8.0 con Entity Framework Core y SQL Server.

## 📋 Tabla de Contenidos

- [Características Principales](#características-principales)
- [Arquitectura del Sistema](#arquitectura-del-sistema)
- [Autenticación y Autorización](#autenticación-y-autorización)
- [Uso de Servicios](#uso-de-servicios)
- [Controladores y BaseController](#controladores-y-basecontroller)
- [Filtros y Paginación](#filtros-y-paginación)
- [Vistas y ViewModels](#vistas-y-viewmodels)
- [Configuración](#configuración)

## 🚀 Características Principales

- **Gestión de Inventario**: Control completo de productos, categorías, marcas y proveedores
- **Control de Bodegas**: Manejo de múltiples ubicaciones de almacenamiento
- **Entradas y Ajustes**: Registro de movimientos de inventario
- **Sistema de Roles**: Control granular de permisos por funcionalidad
- **Autenticación Robusta**: Validación automática de sesiones y estados de usuario
- **Filtros Avanzados**: Búsqueda y filtrado dinámico en todas las entidades

## 🏗️ Arquitectura del Sistema

### Estructura del Proyecto

```
├── Controllers/          # Controladores MVC
├── Services/            # Lógica de negocio
├── Models/              # Modelos de datos
├── Data/                # Contexto de base de datos
├── Views/               # Vistas Razor
├── Middleware/          # Middleware personalizado
└── Extensions/          # Extensiones y helpers
```

### Patrón de Arquitectura

El sistema utiliza una arquitectura en capas:

1. **Presentación**: Controladores MVC
2. **Lógica de Negocio**: Servicios e Interfaces
3. **Acceso a Datos**: Entity Framework Core
4. **Base de Datos**: SQL Server

## 🔐 Autenticación y Autorización

### Middleware de Validación de Sesión

El sistema cuenta con un middleware personalizado que valida automáticamente:

- **Autenticación**: Usuario logueado
- **Estado del Usuario**: Activo/Inactivo
- **Estado del Rol**: Activo/Inactivo
- **Permisos por Ruta**: Según el rol asignado

### Rutas Públicas

Las siguientes rutas no requieren autenticación:
```csharp
/Identity/Account/Login
/Identity/Account/Register
/Identity/Account/Logout
/css/, /js/, /lib/, /assets/
```

### BaseController - Métodos de Autorización

Todos los controladores heredan de `BaseController` que proporciona:

#### Información del Usuario
```csharp
protected UsuarioModel? CurrentUser           // Usuario actual
protected string? CurrentUserRole             // Rol del usuario
protected bool IsCurrentUserAdmin             // Es administrador
protected string? CurrentUserId               // ID del usuario
```

#### Validación de Roles (Manual)
```csharp
// Verificar rol específico
if (CurrentUserHasRole("Inventario"))
{
    // Lógica para rol Inventario
}

// Verificar múltiples roles
if (CurrentUserHasAnyRole("Administrador", "Empleado"))
{
    // Lógica para cualquiera de estos roles
}
```

#### Protección de Métodos Completos

**Opción 1: ExecuteIfHasRole**
```csharp
public IActionResult GestionInventario()
{
    return ExecuteIfHasRole("Empleado", () => {
        // Todo este código solo se ejecuta si tiene el rol
        var productos = _productoService.GetAllWithStock();
        return View(productos);
    });
}
```

**Opción 2: ExecuteIfAdmin**
```csharp
public IActionResult ConfiguracionSistema()
{
    return ExecuteIfAdmin(() => {
        // Solo administradores pueden acceder
        return View();
    });
}
```

## 🔧 Uso de Servicios

### Estructura de Servicios

Cada entidad tiene su servicio correspondiente con interfaz:

```csharp
Services/
├── Interfaces/
│   ├── IProductoService.cs
│   ├── ICategoriaService.cs
│   └── ...
├── ProductoService.cs
├── CategoriaService.cs
└── ...
```

### Inyección de Dependencias

En el controlador:
```csharp
public class ProductoController : BaseController
{
    private readonly IProductoService _productoService;
    private readonly ICategoriaService _categoriaService;

    public ProductoController(
        IProductoService productoService,
        ICategoriaService categoriaService)
    {
        _productoService = productoService;
        _categoriaService = categoriaService;
    }
}
```

### Métodos Comunes de Servicios

Todos los servicios implementan operaciones CRUD estándar:

```csharp
// Obtener todos
var productos = await _productoService.GetAllAsync();

// Obtener con filtros
var productos = await _productoService.GetFilteredAsync(filtros);

// Obtener por ID
var producto = await _productoService.GetByIdAsync(id);

// Crear
await _productoService.CreateAsync(producto);

// Actualizar
await _productoService.UpdateAsync(producto);

// Eliminar
await _productoService.DeleteAsync(id);

// Verificar existencia
var existe = await _productoService.ExistsAsync(id);
```

## 🎮 Controladores y BaseController

### Ejemplo de Controlador Completo

```csharp
public class ProductoController : BaseController
{
    private readonly IProductoService _productoService;

    public ProductoController(IProductoService productoService)
    {
        _productoService = productoService;
    }

    // GET: Producto
    public async Task<IActionResult> Index(FilterOptions filtros)
    {
        var productos = await _productoService.GetFilteredAsync(filtros);
        return View(productos);
    }

    // GET: Producto/Create
    public IActionResult Create()
    {
        return ExecuteIfHasRole("Empleado", () => {
            return View(new ProductoModel());
        });
    }

    // POST: Producto/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductoModel producto)
    {
        return await ExecuteIfHasRole("Administrador", async () => {
            if (!ModelState.IsValid)
            {
                return View(producto);
            }

            await _productoService.CreateAsync(producto);
            TempData["Success"] = "Producto creado correctamente";
            return RedirectToAction(nameof(Index));
        });
    }

    // DELETE: Solo administradores
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        return await ExecuteIfAdmin(async () => {
            await _productoService.DeleteAsync(id);
            return Json(new { success = true });
        });
    }
}
```

## 📄 Filtros y Paginación

### FilterOptions

Clase base para filtros:

```csharp
public class FilterOptions
{
    public string? Search { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; } = "ASC";
}
```

### PagedResult

Resultado paginado:

```csharp
public class PagedResult<T>
{
    public List<T> Items { get; set; }
    public int TotalItems { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}
```

### Uso en Controladores

```csharp
public async Task<IActionResult> Index(FilterOptions filtros)
{
    var resultado = await _productoService.GetPagedAsync(filtros);
    return View(resultado);
}
```

### Uso en Vistas

```html
<!-- Formulario de filtros -->
<form method="get">
    <input name="Search" value="@Model.Search" placeholder="Buscar..." />
    <select name="PageSize">
        <option value="10">10 por página</option>
        <option value="25">25 por página</option>
        <option value="50">50 por página</option>
    </select>
    <button type="submit">Filtrar</button>
</form>

<!-- Paginación -->
@if (Model.TotalPages > 1)
{
    <nav>
        <ul class="pagination">
            @for (int i = 1; i <= Model.TotalPages; i++)
            {
                <li class="page-item @(i == Model.CurrentPage ? "active" : "")">
                    <a class="page-link" href="?page=@i&search=@Model.Search">@i</a>
                </li>
            }
        </ul>
    </nav>
}
```

## 🎨 Vistas y ViewModels

### Información del Usuario en Vistas

El `BaseController` automáticamente proporciona información del usuario en `ViewBag`:

```html
@{
    var currentUser = ViewBag.CurrentUser as UsuarioModel;
    var userRole = ViewBag.CurrentUserRole as string;
    var isAdmin = (bool)ViewBag.IsCurrentUserAdmin;
}

<!-- Mostrar información del usuario -->
<p>Bienvenido, @ViewBag.CurrentUserName</p>

<!-- Mostrar contenido según rol -->
@if (isAdmin)
{
    <a href="/Admin/Configuracion" class="btn btn-danger">Configuración</a>
}

@if (userRole == "Inventario")
{
    <a href="/Inventario/Ajustes" class="btn btn-warning">Ajustes</a>
}
```

### Conditional Rendering

```html
<!-- Solo mostrar si es admin -->
@if ((bool)ViewBag.IsCurrentUserAdmin)
{
    <button class="btn btn-danger" onclick="eliminar(@item.Id)">Eliminar</button>
}

<!-- Mostrar según rol -->
@switch (ViewBag.CurrentUserRole?.ToString())
{
    case "Inventario":
        <partial name="_InventarioActions" model="@Model" />
        break;
    case "Vendedor":
        <partial name="_VendedorActions" model="@Model" />
        break;
}
```

## ⚙️ Configuración

### Program.cs - Configuración de Servicios

```csharp
// Registrar servicios
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
// ... otros servicios

// Configurar middleware
app.UseMiddleware<SessionValidationMiddleware>();
```

### Roles del Sistema

- **Administrador**: Acceso completo
- **Inventario**: Gestión de productos, entradas, ajustes
- **Vendedor**: Consulta de productos, categorías, marcas
- **Supervisor**: Acceso intermedio (configurable)

## 📚 Ejemplos Avanzados

### AJAX con Autorización

```javascript
function eliminarProducto(id) {
    if (confirm('¿Está seguro de eliminar este producto?')) {
        $.post('/Producto/Delete', { id: id })
            .done(function(result) {
                if (result.success) {
                    location.reload();
                } else {
                    alert('Error: ' + result.message);
                }
            })
            .fail(function() {
                alert('Sin permisos para esta acción');
            });
    }
}
```

### Validación en JavaScript

```javascript
// Verificar permisos desde el cliente
var isAdmin = @Html.Raw(Json.Serialize((bool)ViewBag.IsCurrentUserAdmin));
var userRole = '@ViewBag.CurrentUserRole';

if (isAdmin) {
    $('.admin-only').show();
} else {
    $('.admin-only').hide();
}

if (userRole === 'Inventario') {
    $('.inventario-actions').show();
}
```