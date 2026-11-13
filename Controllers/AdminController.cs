using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TechStore.Services;

namespace TechStore.Controllers;

[Authorize(Roles = "Administrador")]
public class AdminController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<AdminController> _logger;
    private readonly IRegistroEventoService _registro;

    public AdminController(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ILogger<AdminController> logger,
        IRegistroEventoService registro)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
        _registro = registro;
    }

    public async Task<IActionResult> Usuarios()
    {
        var usuarios = _userManager.Users.ToList();
        var lista = new List<(IdentityUser User, IList<string> Roles)>();

        foreach (var u in usuarios)
        {
            var roles = await _userManager.GetRolesAsync(u);
            lista.Add((u, roles));
        }

        return View(lista);
    }

    public IActionResult Roles()
    {
        var roles = _roleManager.Roles.ToList();
        return View(roles);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AsignarRol(string userId, string rol)
    {
        var usuario = await _userManager.FindByIdAsync(userId);
        if (usuario is null)
            return NotFound();

        if (!await _roleManager.RoleExistsAsync(rol))
        {
            TempData["Error"] = $"El rol «{rol}» no existe.";
            return RedirectToAction(nameof(Usuarios));
        }

        if (!await _userManager.IsInRoleAsync(usuario, rol))
        {
            await _userManager.AddToRoleAsync(usuario, rol);
            _logger.LogInformation("Rol {Rol} asignado a {Usuario}.", rol, usuario.UserName);
            await _registro.RegistrarAsync("Info", "Admin/AsignarRol",
                $"Rol '{rol}' asignado a '{usuario.UserName}'.", User.Identity?.Name);
        }

        TempData["Exito"] = $"Rol «{rol}» asignado a «{usuario.UserName}».";
        return RedirectToAction(nameof(Usuarios));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> QuitarRol(string userId, string rol)
    {
        var usuario = await _userManager.FindByIdAsync(userId);
        if (usuario is null)
            return NotFound();

        if (await _userManager.IsInRoleAsync(usuario, rol))
        {
            await _userManager.RemoveFromRoleAsync(usuario, rol);
            _logger.LogInformation("Rol {Rol} removido de {Usuario}.", rol, usuario.UserName);
            await _registro.RegistrarAsync("Info", "Admin/QuitarRol",
                $"Rol '{rol}' removido de '{usuario.UserName}'.", User.Identity?.Name);
        }

        TempData["Exito"] = $"Rol «{rol}» removido de «{usuario.UserName}».";
        return RedirectToAction(nameof(Usuarios));
    }

    public async Task<IActionResult> Logs()
    {
        var eventos = await _registro.ObtenerRecientesAsync(100);
        return View(eventos);
    }
}
