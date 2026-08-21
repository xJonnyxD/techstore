using Microsoft.AspNetCore.Mvc;
using TechStore.Models;

namespace TechStore.Controllers;

public class ProductosController : Controller
{
    public IActionResult Index()
    {
        return View(TiendaDatos.Productos);
    }
}
