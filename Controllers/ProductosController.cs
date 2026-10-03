using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechStore.Models;
using TechStore.Services;

namespace TechStore.Controllers;

/// <summary>
/// Controlador de productos. No accede a la base de datos directamente: delega
/// toda la lógica en los servicios, de los que depende solo a través de sus
/// interfaces (Inyección de Dependencias + principio de inversión de dependencias).
/// </summary>
public class ProductosController : Controller
{
    private readonly IProductoService _productoService;
    private readonly ICategoriaService _categoriaService;

    public ProductosController(IProductoService productoService, ICategoriaService categoriaService)
    {
        _productoService = productoService;
        _categoriaService = categoriaService;
    }

    // READ: listado con búsqueda opcional.
    public async Task<IActionResult> Index(string? buscar)
    {
        var productos = await _productoService.ObtenerTodosAsync(buscar);
        ViewData["Buscar"] = buscar;
        return View(productos);
    }

    // READ (uno): detalle de un producto.
    public async Task<IActionResult> Detalle(int id)
    {
        var producto = await _productoService.ObtenerPorIdAsync(id);
        if (producto is null)
        {
            return NotFound();
        }

        return View(producto);
    }

    // CREATE (formulario)
    [HttpGet]
    public async Task<IActionResult> Crear()
    {
        await CargarCategoriasAsync();
        return View(new Producto());
    }

    // CREATE (guardar)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(Producto producto)
    {
        if (!ModelState.IsValid)
        {
            await CargarCategoriasAsync(producto.CategoriaId);
            return View(producto);
        }

        await _productoService.CrearAsync(producto);
        TempData["Exito"] = $"El producto «{producto.Nombre}» se agregó correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // UPDATE (formulario)
    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var producto = await _productoService.ObtenerPorIdAsync(id);
        if (producto is null)
        {
            return NotFound();
        }

        await CargarCategoriasAsync(producto.CategoriaId);
        return View(producto);
    }

    // UPDATE (guardar)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(Producto producto)
    {
        if (!ModelState.IsValid)
        {
            await CargarCategoriasAsync(producto.CategoriaId);
            return View(producto);
        }

        await _productoService.ActualizarAsync(producto);
        TempData["Exito"] = $"El producto «{producto.Nombre}» se actualizó correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // DELETE (confirmación)
    [HttpGet]
    public async Task<IActionResult> Eliminar(int id)
    {
        var producto = await _productoService.ObtenerPorIdAsync(id);
        if (producto is null)
        {
            return NotFound();
        }

        return View(producto);
    }

    // DELETE (ejecutar)
    [HttpPost, ActionName("Eliminar")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarConfirmado(int id)
    {
        var producto = await _productoService.ObtenerPorIdAsync(id);
        await _productoService.EliminarAsync(id);

        if (producto is not null)
        {
            TempData["Exito"] = $"El producto «{producto.Nombre}» se eliminó correctamente.";
        }

        return RedirectToAction(nameof(Index));
    }

    // Carga las categorías en un SelectList para el <select> de los formularios.
    private async Task CargarCategoriasAsync(int? seleccionada = null)
    {
        var categorias = await _categoriaService.ObtenerTodasAsync();
        ViewBag.Categorias = new SelectList(categorias, nameof(Categoria.Id), nameof(Categoria.Nombre), seleccionada);
    }
}
