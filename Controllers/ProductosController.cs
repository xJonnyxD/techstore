using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechStore.Models;
using TechStore.Services;

namespace TechStore.Controllers;

[Authorize]
public class ProductosController : Controller
{
    private readonly IProductoService _productoService;
    private readonly ICategoriaService _categoriaService;
    private readonly ILogger<ProductosController> _logger;
    private readonly IRegistroEventoService _registro;

    public ProductosController(IProductoService productoService, ICategoriaService categoriaService,
        ILogger<ProductosController> logger, IRegistroEventoService registro)
    {
        _productoService = productoService;
        _categoriaService = categoriaService;
        _logger = logger;
        _registro = registro;
    }

    public async Task<IActionResult> Index(string? buscar)
    {
        var productos = await _productoService.ObtenerTodosAsync(buscar);
        ViewData["Buscar"] = buscar;
        return View(productos);
    }

    public async Task<IActionResult> Detalle(int id)
    {
        var producto = await _productoService.ObtenerPorIdAsync(id);
        if (producto is null)
            return NotFound();

        return View(producto);
    }

    [HttpGet]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Crear()
    {
        await CargarCategoriasAsync();
        return View(new Producto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Crear(Producto producto)
    {
        if (!ModelState.IsValid)
        {
            await CargarCategoriasAsync(producto.CategoriaId);
            return View(producto);
        }

        await _productoService.CrearAsync(producto);
        _logger.LogInformation("Producto creado: {Nombre} (ID {Id}).", producto.Nombre, producto.Id);
        await _registro.RegistrarAsync("Info", "Productos/Crear",
            $"Producto '{producto.Nombre}' creado.", User.Identity?.Name);

        TempData["Exito"] = $"El producto «{producto.Nombre}» se agregó correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Editar(int id)
    {
        var producto = await _productoService.ObtenerPorIdAsync(id);
        if (producto is null)
            return NotFound();

        await CargarCategoriasAsync(producto.CategoriaId);
        return View(producto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Editar(Producto producto)
    {
        if (!ModelState.IsValid)
        {
            await CargarCategoriasAsync(producto.CategoriaId);
            return View(producto);
        }

        await _productoService.ActualizarAsync(producto);
        _logger.LogInformation("Producto actualizado: {Nombre} (ID {Id}).", producto.Nombre, producto.Id);
        await _registro.RegistrarAsync("Info", "Productos/Editar",
            $"Producto '{producto.Nombre}' actualizado.", User.Identity?.Name);

        TempData["Exito"] = $"El producto «{producto.Nombre}» se actualizó correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var producto = await _productoService.ObtenerPorIdAsync(id);
        if (producto is null)
            return NotFound();

        return View(producto);
    }

    [HttpPost, ActionName("Eliminar")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> EliminarConfirmado(int id)
    {
        var producto = await _productoService.ObtenerPorIdAsync(id);
        await _productoService.EliminarAsync(id);

        if (producto is not null)
        {
            _logger.LogInformation("Producto eliminado: {Nombre} (ID {Id}).", producto.Nombre, id);
            await _registro.RegistrarAsync("Info", "Productos/Eliminar",
                $"Producto '{producto.Nombre}' eliminado.", User.Identity?.Name);
            TempData["Exito"] = $"El producto «{producto.Nombre}» se eliminó correctamente.";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task CargarCategoriasAsync(int? seleccionada = null)
    {
        var categorias = await _categoriaService.ObtenerTodasAsync();
        ViewBag.Categorias = new SelectList(categorias, nameof(Categoria.Id), nameof(Categoria.Nombre), seleccionada);
    }
}
