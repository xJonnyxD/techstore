using Microsoft.AspNetCore.Mvc;
using TechStore.Models;

namespace TechStore.Controllers;

public class ContactoController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View(new Contacto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index(Contacto contacto)
    {
        if (!ModelState.IsValid)
        {
            return View(contacto);
        }

        // No se envía correo real: solo confirmamos que el formulario funciona.
        TempData["Enviado"] = $"Gracias {contacto.Nombre}, tu mensaje fue recibido.";
        return RedirectToAction(nameof(Index));
    }
}
