using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using TechStore.Controllers;
using TechStore.Models;
using TechStore.Services;

namespace TechStore.Tests;

public class ProductosControllerTests
{
    private readonly Mock<IProductoService> _productoServiceMock;
    private readonly Mock<ICategoriaService> _categoriaServiceMock;
    private readonly Mock<ILogger<ProductosController>> _loggerMock;
    private readonly Mock<IRegistroEventoService> _registroMock;
    private readonly ProductosController _controller;

    public ProductosControllerTests()
    {
        _productoServiceMock = new Mock<IProductoService>();
        _categoriaServiceMock = new Mock<ICategoriaService>();
        _loggerMock = new Mock<ILogger<ProductosController>>();
        _registroMock = new Mock<IRegistroEventoService>();

        _controller = new ProductosController(
            _productoServiceMock.Object,
            _categoriaServiceMock.Object,
            _loggerMock.Object,
            _registroMock.Object);

        var claims = new List<Claim> { new(ClaimTypes.Name, "admin") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
        _controller.TempData = new TempDataDictionary(
            _controller.HttpContext, Mock.Of<ITempDataProvider>());
    }

    [Fact]
    public async Task Index_RetornaVistaConProductos()
    {
        var productos = new List<Producto>
        {
            new() { Id = 1, Nombre = "Laptop HP", Descripcion = "15 pulgadas", Precio = 799m, CategoriaId = 1 },
            new() { Id = 2, Nombre = "Monitor", Descripcion = "24 pulgadas", Precio = 299m, CategoriaId = 1 }
        };
        _productoServiceMock.Setup(s => s.ObtenerTodosAsync(null))
            .ReturnsAsync(productos);

        var resultado = await _controller.Index(null);

        var vista = Assert.IsType<ViewResult>(resultado);
        var modelo = Assert.IsAssignableFrom<IEnumerable<Producto>>(vista.Model);
        Assert.Equal(2, modelo.Count());
    }

    [Fact]
    public async Task Detalle_IdExistente_RetornaVista()
    {
        var producto = new Producto { Id = 1, Nombre = "Laptop HP", Descripcion = "Test", Precio = 799m, CategoriaId = 1 };
        _productoServiceMock.Setup(s => s.ObtenerPorIdAsync(1)).ReturnsAsync(producto);

        var resultado = await _controller.Detalle(1);

        var vista = Assert.IsType<ViewResult>(resultado);
        Assert.Equal(producto, vista.Model);
    }

    [Fact]
    public async Task Detalle_IdInexistente_RetornaNotFound()
    {
        _productoServiceMock.Setup(s => s.ObtenerPorIdAsync(999)).ReturnsAsync((Producto?)null);

        var resultado = await _controller.Detalle(999);

        Assert.IsType<NotFoundResult>(resultado);
    }

    [Fact]
    public async Task Crear_Post_ModeloValido_RedirigeAIndex()
    {
        var producto = new Producto { Nombre = "Nuevo", Descripcion = "Desc", Precio = 100m, Stock = 5, CategoriaId = 1 };
        _productoServiceMock.Setup(s => s.CrearAsync(producto)).Returns(Task.CompletedTask);
        _registroMock.Setup(r => r.RegistrarAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>()))
            .Returns(Task.CompletedTask);

        var resultado = await _controller.Crear(producto);

        var redireccion = Assert.IsType<RedirectToActionResult>(resultado);
        Assert.Equal("Index", redireccion.ActionName);
        _productoServiceMock.Verify(s => s.CrearAsync(producto), Times.Once);
    }

    [Fact]
    public async Task Crear_Post_ModeloInvalido_RetornaVista()
    {
        _controller.ModelState.AddModelError("Nombre", "Requerido");
        var producto = new Producto();
        _categoriaServiceMock.Setup(s => s.ObtenerTodasAsync()).ReturnsAsync(new List<Categoria>());

        var resultado = await _controller.Crear(producto);

        var vista = Assert.IsType<ViewResult>(resultado);
        Assert.Equal(producto, vista.Model);
    }

    [Fact]
    public async Task EliminarConfirmado_ProductoExistente_RedirigeAIndex()
    {
        var producto = new Producto { Id = 1, Nombre = "Laptop", Descripcion = "Test", Precio = 100m, CategoriaId = 1 };
        _productoServiceMock.Setup(s => s.ObtenerPorIdAsync(1)).ReturnsAsync(producto);
        _productoServiceMock.Setup(s => s.EliminarAsync(1)).Returns(Task.CompletedTask);
        _registroMock.Setup(r => r.RegistrarAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>()))
            .Returns(Task.CompletedTask);

        var resultado = await _controller.EliminarConfirmado(1);

        var redireccion = Assert.IsType<RedirectToActionResult>(resultado);
        Assert.Equal("Index", redireccion.ActionName);
    }
}
