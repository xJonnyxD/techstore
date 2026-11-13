using System.ComponentModel.DataAnnotations;
using TechStore.Models;

namespace TechStore.Tests;

public class ProductoModelTests
{
    private static List<ValidationResult> ValidarModelo(Producto producto)
    {
        var contexto = new ValidationContext(producto);
        var resultados = new List<ValidationResult>();
        Validator.TryValidateObject(producto, contexto, resultados, true);
        return resultados;
    }

    [Fact]
    public void Producto_Valido_NoTieneErrores()
    {
        var producto = new Producto
        {
            Nombre = "Laptop HP",
            Descripcion = "15 pulgadas",
            Precio = 799.99m,
            Stock = 10,
            CategoriaId = 1,
            Estado = true
        };

        var errores = ValidarModelo(producto);

        Assert.Empty(errores);
    }

    [Fact]
    public void Producto_SinNombre_TieneError()
    {
        var producto = new Producto
        {
            Nombre = "",
            Descripcion = "Descripción",
            Precio = 100m,
            CategoriaId = 1
        };

        var errores = ValidarModelo(producto);

        Assert.Contains(errores, e => e.MemberNames.Contains("Nombre"));
    }

    [Fact]
    public void Producto_PrecioNegativo_TieneError()
    {
        var producto = new Producto
        {
            Nombre = "Test",
            Descripcion = "Desc",
            Precio = -5m,
            CategoriaId = 1
        };

        var errores = ValidarModelo(producto);

        Assert.Contains(errores, e => e.MemberNames.Contains("Precio"));
    }

    [Fact]
    public void Producto_StockNegativo_TieneError()
    {
        var producto = new Producto
        {
            Nombre = "Test",
            Descripcion = "Desc",
            Precio = 100m,
            Stock = -1,
            CategoriaId = 1
        };

        var errores = ValidarModelo(producto);

        Assert.Contains(errores, e => e.MemberNames.Contains("Stock"));
    }

    [Fact]
    public void Disponible_ConStockYActivo_RetornaTrue()
    {
        var producto = new Producto { Stock = 5, Estado = true };

        Assert.True(producto.Disponible);
    }

    [Fact]
    public void Disponible_SinStock_RetornaFalse()
    {
        var producto = new Producto { Stock = 0, Estado = true };

        Assert.False(producto.Disponible);
    }

    [Fact]
    public void Disponible_Inactivo_RetornaFalse()
    {
        var producto = new Producto { Stock = 10, Estado = false };

        Assert.False(producto.Disponible);
    }
}
