using Microsoft.EntityFrameworkCore;
using TechStore.Data;
using TechStore.Models;
using TechStore.Services;

namespace TechStore.Tests;

public class IntegracionProductoTests
{
    private TechStoreDbContext CrearContexto()
    {
        var opciones = new DbContextOptionsBuilder<TechStoreDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new TechStoreDbContext(opciones);
    }

    [Fact]
    public async Task CrearYObtenerProducto_FlujoCompleto()
    {
        using var contexto = CrearContexto();

        contexto.Categorias.Add(new Categoria { Id = 1, Nombre = "Laptops", Descripcion = "Portátiles" });
        await contexto.SaveChangesAsync();

        var servicio = new ProductoService(contexto);
        var producto = new Producto
        {
            Nombre = "Laptop Lenovo",
            Descripcion = "ThinkPad T14",
            Precio = 1099.99m,
            Stock = 8,
            CategoriaId = 1,
            Estado = true
        };

        await servicio.CrearAsync(producto);
        var obtenido = await servicio.ObtenerPorIdAsync(producto.Id);

        Assert.NotNull(obtenido);
        Assert.Equal("Laptop Lenovo", obtenido.Nombre);
        Assert.Equal(1099.99m, obtenido.Precio);
        Assert.Equal("Laptops", obtenido.Categoria?.Nombre);
    }

    [Fact]
    public async Task ActualizarYVerificar_FlujoCompleto()
    {
        using var contexto = CrearContexto();

        contexto.Categorias.Add(new Categoria { Id = 1, Nombre = "Monitores", Descripcion = "Pantallas" });
        contexto.Productos.Add(new Producto
        {
            Id = 1, Nombre = "Monitor LG", Descripcion = "27 pulgadas",
            Precio = 349.99m, Stock = 12, CategoriaId = 1, Estado = true
        });
        await contexto.SaveChangesAsync();

        var servicio = new ProductoService(contexto);
        var producto = await servicio.ObtenerPorIdAsync(1);
        producto!.Precio = 299.99m;
        producto.Stock = 15;

        await servicio.ActualizarAsync(producto);
        var actualizado = await servicio.ObtenerPorIdAsync(1);

        Assert.Equal(299.99m, actualizado!.Precio);
        Assert.Equal(15, actualizado.Stock);
    }

    [Fact]
    public async Task EliminarYVerificar_FlujoCompleto()
    {
        using var contexto = CrearContexto();

        contexto.Categorias.Add(new Categoria { Id = 1, Nombre = "Accesorios", Descripcion = "Periféricos" });
        contexto.Productos.Add(new Producto
        {
            Id = 1, Nombre = "Mouse Logitech", Descripcion = "Inalámbrico",
            Precio = 49.99m, Stock = 30, CategoriaId = 1, Estado = true
        });
        await contexto.SaveChangesAsync();

        var servicio = new ProductoService(contexto);

        await servicio.EliminarAsync(1);
        var eliminado = await servicio.ObtenerPorIdAsync(1);
        var total = await servicio.ObtenerTodosAsync();

        Assert.Null(eliminado);
        Assert.Empty(total);
    }

    [Fact]
    public async Task BuscarProductos_PorCategoria_RetornaCorrectamente()
    {
        using var contexto = CrearContexto();

        contexto.Categorias.AddRange(
            new Categoria { Id = 1, Nombre = "Laptops", Descripcion = "Portátiles" },
            new Categoria { Id = 2, Nombre = "Monitores", Descripcion = "Pantallas" }
        );
        contexto.Productos.AddRange(
            new Producto { Id = 1, Nombre = "Laptop HP", Descripcion = "HP Pavilion", Precio = 799m, Stock = 5, CategoriaId = 1 },
            new Producto { Id = 2, Nombre = "Monitor LG", Descripcion = "UltraWide", Precio = 499m, Stock = 3, CategoriaId = 2 }
        );
        await contexto.SaveChangesAsync();

        var servicio = new ProductoService(contexto);

        var resultadoLaptops = await servicio.ObtenerTodosAsync("Laptops");
        var resultadoMonitores = await servicio.ObtenerTodosAsync("Monitor");

        Assert.Single(resultadoLaptops);
        Assert.Single(resultadoMonitores);
    }

    [Fact]
    public async Task RegistroEventoService_RegistrarYObtener()
    {
        using var contexto = CrearContexto();
        var servicio = new RegistroEventoService(contexto);

        await servicio.RegistrarAsync("Info", "Test", "Evento de prueba", "admin");
        await servicio.RegistrarAsync("Error", "Test", "Error de prueba", "admin");

        var eventos = await servicio.ObtenerRecientesAsync(10);

        Assert.Equal(2, eventos.Count());
        Assert.Equal("Error", eventos.First().Nivel);
    }
}
