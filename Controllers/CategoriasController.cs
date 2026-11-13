using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechStore.Models;
using TechStore.Services;

namespace TechStore.Controllers;

[Authorize]
public class CategoriasController : Controller
{
    private readonly ICategoriaService _categoriaService;
    private readonly ILogger<CategoriasController> _logger;
    private readonly IRegistroEventoService _registro;

    public CategoriasController(ICategoriaService categoriaService,
        ILogger<CategoriasController> logger, IRegistroEventoService registro)
    {
        _categoriaService = categoriaService;
        _logger = logger;
        _registro = registro;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _categoriaService.ObtenerTodasAsync());
    }

    [HttpGet]
    [Authorize(Roles = "Administrador")]
    public IActionResult Crear()
    {
        return View(new Categoria());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Crear(Categoria categoria)
    {
        if (!ModelState.IsValid)
            return View(categoria);

        await _categoriaService.CrearAsync(categoria);
        _logger.LogInformation("Categoría creada: {Nombre}.", categoria.Nombre);
        await _registro.RegistrarAsync("Info", "Categorias/Crear",
            $"Categoría '{categoria.Nombre}' creada.", User.Identity?.Name);

        TempData["Exito"] = $"La categoría «{categoria.Nombre}» se agregó correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Editar(int id)
    {
        var categoria = await _categoriaService.ObtenerPorIdAsync(id);
        if (categoria is null)
            return NotFound();

        return View(categoria);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Editar(Categoria categoria)
    {
        if (!ModelState.IsValid)
            return View(categoria);

        await _categoriaService.ActualizarAsync(categoria);
        _logger.LogInformation("Categoría actualizada: {Nombre}.", categoria.Nombre);
        await _registro.RegistrarAsync("Info", "Categorias/Editar",
            $"Categoría '{categoria.Nombre}' actualizada.", User.Identity?.Name);

        TempData["Exito"] = $"La categoría «{categoria.Nombre}» se actualizó correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var categoria = await _categoriaService.ObtenerPorIdAsync(id);
        if (categoria is null)
            return NotFound();

        return View(categoria);
    }

    [HttpPost, ActionName("Eliminar")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> EliminarConfirmado(int id)
    {
        if (await _categoriaService.TieneProductosAsync(id))
        {
            TempData["Error"] = "No se puede eliminar la categoría porque tiene productos asociados.";
            return RedirectToAction(nameof(Index));
        }

        var categoria = await _categoriaService.ObtenerPorIdAsync(id);
        await _categoriaService.EliminarAsync(id);

        if (categoria is not null)
        {
            _logger.LogInformation("Categoría eliminada: {Nombre}.", categoria.Nombre);
            await _registro.RegistrarAsync("Info", "Categorias/Eliminar",
                $"Categoría '{categoria.Nombre}' eliminada.", User.Identity?.Name);
            TempData["Exito"] = $"La categoría «{categoria.Nombre}» se eliminó correctamente.";
        }

        return RedirectToAction(nameof(Index));
    }
}
