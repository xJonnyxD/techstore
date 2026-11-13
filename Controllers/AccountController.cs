using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TechStore.Models;
using TechStore.Services;

namespace TechStore.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ILogger<AccountController> _logger;
    private readonly IRegistroEventoService _registro;

    public AccountController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        ILogger<AccountController> logger,
        IRegistroEventoService registro)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _registro = registro;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel modelo, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
            return View(modelo);

        var resultado = await _signInManager.PasswordSignInAsync(
            modelo.UserName, modelo.Password, modelo.RememberMe, lockoutOnFailure: false);

        if (resultado.Succeeded)
        {
            _logger.LogInformation("Usuario {Usuario} inició sesión.", modelo.UserName);
            await _registro.RegistrarAsync("Info", "Account/Login",
                $"Inicio de sesión exitoso.", modelo.UserName);

            return LocalRedirect(returnUrl ?? "/");
        }

        _logger.LogWarning("Intento de login fallido para {Usuario}.", modelo.UserName);
        await _registro.RegistrarAsync("Advertencia", "Account/Login",
            $"Intento de login fallido.", modelo.UserName);

        ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
        return View(modelo);
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel modelo)
    {
        if (!ModelState.IsValid)
            return View(modelo);

        var usuario = new IdentityUser
        {
            UserName = modelo.UserName,
            Email = modelo.Email,
            EmailConfirmed = true
        };

        var resultado = await _userManager.CreateAsync(usuario, modelo.Password);

        if (resultado.Succeeded)
        {
            await _userManager.AddToRoleAsync(usuario, "Usuario");
            await _signInManager.SignInAsync(usuario, isPersistent: false);

            _logger.LogInformation("Nuevo usuario registrado: {Usuario}", modelo.UserName);
            await _registro.RegistrarAsync("Info", "Account/Register",
                $"Nuevo usuario registrado.", modelo.UserName);

            return RedirectToAction("Index", "Home");
        }

        foreach (var error in resultado.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(modelo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var usuario = User.Identity?.Name;
        await _signInManager.SignOutAsync();

        _logger.LogInformation("Usuario {Usuario} cerró sesión.", usuario);
        await _registro.RegistrarAsync("Info", "Account/Logout",
            "Cierre de sesión.", usuario);

        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccesoDenegado()
    {
        return View();
    }
}
