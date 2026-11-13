using Microsoft.EntityFrameworkCore;
using TechStore.Data;
using TechStore.Models;
using TechStore.Services;

namespace TechStore.Tests;

public class ProductoServiceTests
{
    private TechStoreDbContext CrearContexto()
    {
        var opciones = new DbContextOptionsBuilder<TechStoreDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var contexto = new TechStoreDbContext(opciones);

        contexto.Categorias.Add(new Categoria
        {
            Id = 1,
            Nombre = "Laptops",
            Descripcion = "Computadoras portátiles"
        });

        contexto.Productos.AddRange(
            new Producto { Id = 1, Nombre = "Laptop HP", Descripcion = "15 pulgadas", Precio = 799.99m, Stock = 10, CategoriaId = 1, Estado = true },
            new Producto { Id = 2, Nombre = "Laptop Dell", Descripcion = "14 pulgadas", Precio = 899.99m, Stock = 5, CategoriaId = 1, Estado = true },
            new Producto { Id = 3, Nombre = "Monitor Samsung", Descripcion = "24 pulgadas", Precio = 299.99m, Stock = 0, CategoriaId = 1, Estado = false }
        );

        contexto.SaveChanges();
        return contexto;
    }

    [Fact]
    public async Task ObtenerTodosAsync_SinFiltro_RetornaTodosLosProductos()
    {
        using var contexto = CrearContexto();
        var servicio = new ProductoService(contexto);

        var resultado = await servicio.ObtenerTodosAsync();

        Assert.Equal(3, resultado.Count());
    }

    [Fact]
    public async Task ObtenerTodosAsync_ConFiltro_RetornaProductosFiltrados()
    {
        using var contexto = CrearContexto();
        var servicio = new ProductoService(contexto);

        var resultado = await servicio.ObtenerTodosAsync("HP");

        Assert.Single(resultado);
        Assert.Equal("Laptop HP", resultado.First().Nombre);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_IdExistente_RetornaProducto()
    {
        using var contexto = CrearContexto();
        var servicio = new ProductoService(contexto);

        var producto = await servicio.ObtenerPorIdAsync(1);

        Assert.NotNull(producto);
        Assert.Equal("Laptop HP", producto.Nombre);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_IdInexistente_RetornaNull()
    {
        using var contexto = CrearContexto();
        var servicio = new ProductoService(contexto);

        var producto = await servicio.ObtenerPorIdAsync(999);

        Assert.Null(producto);
    }

    [Fact]
    public async Task CrearAsync_ProductoValido_SeAgregaAlContexto()
    {
        using var contexto = CrearContexto();
        var servicio = new ProductoService(contexto);
        var nuevo = new Producto
        {
            Nombre = "Teclado Logitech",
            Descripcion = "Mecánico RGB",
            Precio = 89.99m,
            Stock = 20,
            CategoriaId = 1,
            Estado = true
        };

        await servicio.CrearAsync(nuevo);

        Assert.Equal(4, contexto.Productos.Count());
        Assert.True(nuevo.Id > 0);
    }

    [Fact]
    public async Task ActualizarAsync_ProductoExistente_SeActualizan()
    {
        using var contexto = CrearContexto();
        var servicio = new ProductoService(contexto);
        var producto = await contexto.Productos.FindAsync(1);
        producto!.Precio = 749.99m;

        await servicio.ActualizarAsync(producto);

        var actualizado = await contexto.Productos.FindAsync(1);
        Assert.Equal(749.99m, actualizado!.Precio);
    }

    [Fact]
    public async Task EliminarAsync_IdExistente_EliminaProducto()
    {
        using var contexto = CrearContexto();
        var servicio = new ProductoService(contexto);

        await servicio.EliminarAsync(1);

        Assert.Equal(2, contexto.Productos.Count());
        Assert.Null(await contexto.Productos.FindAsync(1));
    }

    [Fact]
    public async Task EliminarAsync_IdInexistente_NoLanzaExcepcion()
    {
        using var contexto = CrearContexto();
        var servicio = new ProductoService(contexto);

        await servicio.EliminarAsync(999);

        Assert.Equal(3, contexto.Productos.Count());
    }
}
