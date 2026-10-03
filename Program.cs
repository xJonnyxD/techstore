using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using TechStore.Data;
using TechStore.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Entity Framework Core: registra el DbContext y lo configura para usar SQL Server
// con la cadena de conexión "TechStoreDB".
builder.Services.AddDbContext<TechStoreDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TechStoreDB")));

// Inyección de Dependencias: se asocia cada abstracción (interfaz) con su
// implementación concreta. Ciclo de vida Scoped = una instancia por petición.
// Para cambiar de implementación basta con modificar esta línea: los
// controladores no se tocan (principio de inversión de dependencias / SOLID).
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

// Aplica las migraciones pendientes al iniciar: crea la base de datos, su
// esquema y los datos iniciales la primera vez, sin intervención manual.
// Se reintenta porque, en contenedores, el servidor SQL puede tardar en
// aceptar conexiones cuando ambos servicios arrancan a la vez.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TechStoreDbContext>();
    for (var intento = 1; intento <= 12; intento++)
    {
        try
        {
            db.Database.Migrate();
            break;
        }
        catch (Exception ex) when (intento < 12)
        {
            app.Logger.LogWarning("Base de datos no disponible (intento {Intento}): {Mensaje}", intento, ex.Message);
            Thread.Sleep(5000);
        }
    }
}

app.UseForwardedHeaders();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
