# TechStore

Tienda tecnológica desarrollada con **ASP.NET Core MVC** como examen práctico de la
Unidad I de Programación Web II (Frontend).

## Tecnologías

- ASP.NET Core MVC (.NET 10)
- C# · Razor
- Bootstrap 5
- HTML5 · CSS3 (Flexbox y CSS Grid)
- JavaScript mínimo

## Ejecución

```bash
cd TechStore
dotnet restore
dotnet run
```

La aplicación queda disponible en la URL que indica la consola.

## Estructura

```
TechStore/
├── Controllers/   Home, Productos, Categorias, Contacto, Nosotros
├── Models/        Producto, Categoria, Contacto, TiendaDatos (listas en memoria)
├── Views/
│   ├── Home · Productos · Categorias · Contacto · Nosotros
│   └── Shared/    _Layout · _ProductCard (partial reutilizable)
├── wwwroot/       css/site.css · js/site.js · images/
└── Program.cs
```

## Secciones

Inicio · Productos · Categorías · Contáctenos · Nosotros

Los productos y categorías se manejan con listas en memoria (sin base de datos).
La tarjeta de producto se define una sola vez en `_ProductCard.cshtml` y se reutiliza
en Inicio y Productos.
