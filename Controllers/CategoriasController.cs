using Microsoft.AspNetCore.Mvc;
using TechStore.Models;
using TechStore.Services;

namespace TechStore.Controllers;

/// <summary>
/// Controlador de categorías. Depende únicamente de la abstracción
/// ICategoriaService (Inyección de Dependencias).
/// </summary>
public class CategoriasController : Controller
{
    private readonly ICategoriaService _categoriaService;

    public CategoriasController(ICategoriaService categoriaService)
    {
        _categoriaService = categoriaService;
    }

    // READ
    public async Task<IActionResult> Index()
    {
        return View(await _categoriaService.ObtenerTodasAsync());
    }

    // CREATE (formulario)
    [HttpGet]
    public IActionResult Crear()
    {
        return View(new Categoria());
    }

    // CREATE (guardar)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(Categoria categoria)
    {
        if (!ModelState.IsValid)
        {
            return View(categoria);
        }

        await _categoriaService.CrearAsync(categoria);
        TempData["Exito"] = $"La categoría «{categoria.Nombre}» se agregó correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // UPDATE (formulario)
    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var categoria = await _categoriaService.ObtenerPorIdAsync(id);
        if (categoria is null)
        {
            return NotFound();
        }

        return View(categoria);
    }

    // UPDATE (guardar)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(Categoria categoria)
    {
        if (!ModelState.IsValid)
        {
            return View(categoria);
        }

        await _categoriaService.ActualizarAsync(categoria);
        TempData["Exito"] = $"La categoría «{categoria.Nombre}» se actualizó correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // DELETE (confirmación)
    [HttpGet]
    public async Task<IActionResult> Eliminar(int id)
    {
        var categoria = await _categoriaService.ObtenerPorIdAsync(id);
        if (categoria is null)
        {
            return NotFound();
        }

        return View(categoria);
    }

    // DELETE (ejecutar)
    [HttpPost, ActionName("Eliminar")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarConfirmado(int id)
    {
        // Regla de negocio: no se elimina una categoría con productos asociados.
        if (await _categoriaService.TieneProductosAsync(id))
        {
            TempData["Error"] = "No se puede eliminar la categoría porque tiene productos asociados.";
            return RedirectToAction(nameof(Index));
        }

        var categoria = await _categoriaService.ObtenerPorIdAsync(id);
        await _categoriaService.EliminarAsync(id);

        if (categoria is not null)
        {
            TempData["Exito"] = $"La categoría «{categoria.Nombre}» se eliminó correctamente.";
        }

        return RedirectToAction(nameof(Index));
    }
}
