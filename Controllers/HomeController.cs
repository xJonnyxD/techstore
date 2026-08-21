using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TechStore.Models;

namespace TechStore.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        // Muestra en el inicio los primeros productos como destacados.
        var destacados = TiendaDatos.Productos.Take(4).ToList();
        ViewBag.Categorias = TiendaDatos.Categorias;
        return View(destacados);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
