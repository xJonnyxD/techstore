using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechStore.Data;

namespace TechStore.ViewComponents;

public class ResumenInventarioViewComponent : ViewComponent
{
    private readonly TechStoreDbContext _context;

    public ResumenInventarioViewComponent(TechStoreDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var productos = await _context.Productos.ToListAsync();

        var modelo = new ResumenInventarioModel
        {
            TotalProductos = productos.Count,
            ProductosActivos = productos.Count(p => p.Estado),
            ProductosAgotados = productos.Count(p => p.Stock == 0),
            ValorInventario = productos.Sum(p => p.Precio * p.Stock),
            TotalCategorias = await _context.Categorias.CountAsync()
        };

        return View(modelo);
    }
}

public class ResumenInventarioModel
{
    public int TotalProductos { get; set; }
    public int ProductosActivos { get; set; }
    public int ProductosAgotados { get; set; }
    public decimal ValorInventario { get; set; }
    public int TotalCategorias { get; set; }
}
