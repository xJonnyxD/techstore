using Microsoft.AspNetCore.Mvc;

namespace TechStore.Controllers;

public class NosotrosController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
