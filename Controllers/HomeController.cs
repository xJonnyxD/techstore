using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TechStore.Models;
using TechStore.Services;

namespace TechStore.Controllers;

public class HomeController : Controller
{
    private readonly IProductoService _productoService;
    private readonly ICategoriaService _categoriaService;

    // Los servicios llegan por Inyección de Dependencias a través del constructor.
    public HomeController(IProductoService productoService, ICategoriaService categoriaService)
    {
        _productoService = productoService;
        _categoriaService = categoriaService;
    }

    public async Task<IActionResult> Index()
    {
        // Muestra en el inicio los primeros productos como destacados.
        var productos = await _productoService.ObtenerTodosAsync();
        var destacados = productos.Take(4).ToList();
        ViewBag.Categorias = await _categoriaService.ObtenerTodasAsync();
        return View(destacados);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
