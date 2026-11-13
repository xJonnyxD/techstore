using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TechStore.Controllers;
using TechStore.Data;
using TechStore.Models;
using TechStore.Services;

namespace TechStore.Tests;

public class E2EProductoTests
{
    private TechStoreDbContext CrearContexto()
    {
        var opciones = new DbContextOptionsBuilder<TechStoreDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var contexto = new TechStoreDbContext(opciones);
        contexto.Categorias.Add(new Categoria { Id = 1, Nombre = "Laptops", Descripcion = "Portátiles" });
        contexto.Productos.AddRange(
            new Producto { Id = 1, Nombre = "Laptop HP", Descripcion = "15 pulgadas", Precio = 799m, Stock = 10, CategoriaId = 1, Estado = true },
            new Producto { Id = 2, Nombre = "Laptop Dell", Descripcion = "14 pulgadas", Precio = 899m, Stock = 5, CategoriaId = 1, Estado = true }
        );
        contexto.SaveChanges();
        return contexto;
    }

    private ProductosController CrearController(TechStoreDbContext contexto)
    {
        var productoService = new ProductoService(contexto);
        var categoriaServiceMock = new Mock<ICategoriaService>();
        categoriaServiceMock.Setup(s => s.ObtenerTodasAsync())
            .ReturnsAsync(contexto.Categorias.ToList());

        var controller = new ProductosController(
            productoService,
            categoriaServiceMock.Object,
            Mock.Of<ILogger<ProductosController>>(),
            Mock.Of<IRegistroEventoService>());

        var claims = new List<Claim> { new(ClaimTypes.Name, "admin") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
        controller.TempData = new TempDataDictionary(
            controller.HttpContext, Mock.Of<ITempDataProvider>());

        return controller;
    }

    [Fact]
    public async Task FlujoCompleto_ListarCrearEditarEliminar()
    {
        using var contexto = CrearContexto();
        var controller = CrearController(contexto);

        var listarResult = await controller.Index(null);
        var vista = Assert.IsType<ViewResult>(listarResult);
        var productos = Assert.IsAssignableFrom<IEnumerable<Producto>>(vista.Model);
        Assert.Equal(2, productos.Count());

        var nuevoProducto = new Producto
        {
            Nombre = "Laptop Lenovo",
            Descripcion = "ThinkPad",
            Precio = 1099m,
            Stock = 8,
            CategoriaId = 1,
            Estado = true
        };
        var crearResult = await controller.Crear(nuevoProducto);
        Assert.IsType<RedirectToActionResult>(crearResult);

        listarResult = await controller.Index(null);
        vista = Assert.IsType<ViewResult>(listarResult);
        productos = Assert.IsAssignableFrom<IEnumerable<Producto>>(vista.Model);
        Assert.Equal(3, productos.Count());

        var productoEditar = await contexto.Productos.FirstAsync(p => p.Nombre == "Laptop Lenovo");
        productoEditar.Precio = 999m;
        var editarResult = await controller.Editar(productoEditar);
        Assert.IsType<RedirectToActionResult>(editarResult);

        var productoActualizado = await contexto.Productos.FindAsync(productoEditar.Id);
        Assert.Equal(999m, productoActualizado!.Precio);

        var eliminarResult = await controller.EliminarConfirmado(productoEditar.Id);
        Assert.IsType<RedirectToActionResult>(eliminarResult);

        listarResult = await controller.Index(null);
        vista = Assert.IsType<ViewResult>(listarResult);
        productos = Assert.IsAssignableFrom<IEnumerable<Producto>>(vista.Model);
        Assert.Equal(2, productos.Count());
    }

    [Fact]
    public async Task FlujoCompleto_BuscarProducto()
    {
        using var contexto = CrearContexto();
        var controller = CrearController(contexto);

        var buscarResult = await controller.Index("HP");
        var vista = Assert.IsType<ViewResult>(buscarResult);
        var productos = Assert.IsAssignableFrom<IEnumerable<Producto>>(vista.Model);

        Assert.Single(productos);
        Assert.Equal("Laptop HP", productos.First().Nombre);
    }

    [Fact]
    public async Task FlujoCompleto_DetalleProductoExistenteEInexistente()
    {
        using var contexto = CrearContexto();
        var controller = CrearController(contexto);

        var detalleResult = await controller.Detalle(1);
        var vista = Assert.IsType<ViewResult>(detalleResult);
        var producto = Assert.IsType<Producto>(vista.Model);
        Assert.Equal("Laptop HP", producto.Nombre);

        var notFoundResult = await controller.Detalle(999);
        Assert.IsType<NotFoundResult>(notFoundResult);
    }
}
