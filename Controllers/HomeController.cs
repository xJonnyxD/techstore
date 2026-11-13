using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TechStore.Models;
using TechStore.Services;

namespace TechStore.Controllers;

public class HomeController : Controller
{
    private readonly IProductoService _productoService;
    private readonly ICategoriaService _categoriaService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IProductoService productoService, ICategoriaService categoriaService,
        ILogger<HomeController> logger)
    {
        _productoService = productoService;
        _categoriaService = categoriaService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var productos = await _productoService.ObtenerTodosAsync();
        var destacados = productos.Take(4).ToList();
        ViewBag.Categorias = await _categoriaService.ObtenerTodasAsync();
        return View(destacados);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(int? code)
    {
        var statusCode = code ?? Response.StatusCode;

        _logger.LogError("Error HTTP {Codigo} en la ruta {Ruta}.", statusCode, HttpContext.Request.Path);

        ViewData["StatusCode"] = statusCode;
        ViewData["Mensaje"] = statusCode switch
        {
            404 => "La página que buscas no existe o fue movida.",
            403 => "No tienes permiso para acceder a este recurso.",
            500 => "Ocurrió un error interno en el servidor.",
            _ => "Ocurrió un error inesperado."
        };

        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
