using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TechStore.Data;
using TechStore.Services;
using QuestPDF.Infrastructure;

QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("TechStoreDB");
if (connectionString != null && connectionString.Contains("(localdb)", StringComparison.OrdinalIgnoreCase))
{
    var dbPath = Path.Combine(builder.Environment.ContentRootPath, "TechStore.db");
    builder.Services.AddDbContext<TechStoreDbContext>(options =>
        options.UseSqlite($"Data Source={dbPath}"));
}
else
{
    builder.Services.AddDbContext<TechStoreDbContext>(options =>
        options.UseSqlServer(connectionString));
}

builder.Services
    .AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 6;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<TechStoreDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccesoDenegado";
});

builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IRegistroEventoService, RegistroEventoService>();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TechStoreDbContext>();
    try
    {
        db.Database.Migrate();
    }
    catch
    {
        db.Database.EnsureCreated();
    }

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    foreach (var rol in new[] { "Administrador", "Usuario" })
    {
        if (!await roleManager.RoleExistsAsync(rol))
            await roleManager.CreateAsync(new IdentityRole(rol));
    }

    var adminUser = await userManager.FindByNameAsync("admin");
    if (adminUser is null)
    {
        adminUser = new IdentityUser
        {
            UserName = "admin",
            Email = "admin@techstore.com",
            EmailConfirmed = true
        };
        var creado = await userManager.CreateAsync(adminUser, "Admin123");
        if (creado.Succeeded)
            await userManager.AddToRoleAsync(adminUser, "Administrador");
    }
}

app.UseForwardedHeaders();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Home/Error", "?code={0}");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
