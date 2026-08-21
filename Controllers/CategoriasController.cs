using Microsoft.AspNetCore.Mvc;
using TechStore.Models;

namespace TechStore.Controllers;

public class CategoriasController : Controller
{
    public IActionResult Index()
    {
        return View(TiendaDatos.Categorias);
    }
}
