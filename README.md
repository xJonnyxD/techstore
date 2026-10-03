# TechStore

Tienda tecnológica desarrollada con **ASP.NET Core MVC**.
Examen práctico de **Programación Web II (Frontend)**:

- **Unidad I** — frontend MVC con datos en memoria.
- **Unidad II** — persistencia con Entity Framework Core, arquitectura por capas,
  Servicios e Interfaces, Inversión de Control e Inyección de Dependencias.

## Tecnologías

| Componente | Versión |
|---|---|
| .NET | 10.0 |
| ASP.NET Core MVC | 10.0 |
| Entity Framework Core | 10.0 (SQL Server) |
| Bootstrap | 5 |
| C# · Razor | — |

## Requisitos previos

- SDK de .NET 10.0
- SQL Server LocalDB (incluido con Visual Studio) o una instancia de SQL Server
- Herramienta `dotnet-ef` (solo si se van a recrear las migraciones):
  `dotnet tool install --global dotnet-ef`

## Ejecución

```bash
dotnet restore
dotnet run
```

Al iniciar, la aplicación **aplica las migraciones automáticamente** (`db.Database.Migrate()`),
por lo que crea la base de datos `TechStoreDB`, su esquema y los datos iniciales la primera vez.
Queda disponible en la URL que indica la consola (por defecto `http://localhost:5035`).

La cadena de conexión está en `appsettings.json` (clave `TechStoreDB`); por defecto usa LocalDB.

## Arquitectura por capas

| Capa | Responsabilidad | Carpeta |
|---|---|---|
| Presentación | Controladores y vistas Razor | `Controllers/`, `Views/` |
| Servicios (lógica de negocio) | Reglas y operaciones del dominio | `Services/` |
| Acceso a datos | DbContext de EF Core | `Data/` |
| Modelo del dominio | Entidades | `Models/` |

El flujo es: **Vista → Controlador → Servicio (interfaz) → DbContext → Base de datos**.
Los controladores dependen únicamente de las **interfaces** de servicio, nunca de las
implementaciones concretas (principio de inversión de dependencias / SOLID).

## Entity Framework Core

- **Entidades:** `Producto` y `Categoria` (`Models/`).
- **DbContext:** `TechStoreDbContext` (`Data/`), con `DbSet<Producto>` y `DbSet<Categoria>`.
- **Relación:** `Categoria` **1 — N** `Producto` (clave foránea `Producto.CategoriaId`,
  configurada en `OnModelCreating` con `DeleteBehavior.Restrict`).
- **Datos semilla:** 4 categorías y 8 productos cargados con `HasData`.
- **Migraciones:** carpeta `Migrations/` (`InicialTechStore`).

Comandos útiles:

```bash
dotnet ef migrations add NombreMigracion
dotnet ef database update
```

## Inyección de Dependencias

Registrada en `Program.cs` mediante el contenedor de dependencias:

```csharp
builder.Services.AddDbContext<TechStoreDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TechStoreDB")));

builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
```

## Servicios e Interfaces

| Interfaz | Implementación | Operaciones |
|---|---|---|
| `IProductoService` | `ProductoService` | CRUD de productos + búsqueda |
| `ICategoriaService` | `CategoriaService` | CRUD de categorías + validación de borrado |

## Operaciones CRUD

| Entidad | Crear | Leer | Editar | Eliminar |
|---|---|---|---|---|
| Producto | ✅ `/Productos/Crear` | ✅ `/Productos` · `/Productos/Detalle/{id}` | ✅ `/Productos/Editar/{id}` | ✅ `/Productos/Eliminar/{id}` |
| Categoría | ✅ `/Categorias/Crear` | ✅ `/Categorias` | ✅ `/Categorias/Editar/{id}` | ✅ `/Categorias/Eliminar/{id}` |

> No se permite eliminar una categoría que tenga productos asociados (integridad referencial).

## Estructura del proyecto

```
TechStore/
├── Controllers/   Home, Productos (CRUD), Categorias (CRUD), Contacto, Nosotros
├── Data/          TechStoreDbContext (EF Core)
├── Models/        Producto, Categoria, Contacto, ErrorViewModel
├── Services/      IProductoService/ProductoService, ICategoriaService/CategoriaService
├── Migrations/    Migraciones de EF Core
├── Views/
│   ├── Productos/   Index, Crear, Editar, Eliminar, Detalle, _FormProducto
│   ├── Categorias/  Index, Crear, Editar, Eliminar, _FormCategoria
│   └── Shared/      _Layout, _ProductCard, _ValidationScriptsPartial
├── wwwroot/       css/site.css · js/site.js · images/
├── appsettings.json
└── Program.cs
```
