using Microsoft.EntityFrameworkCore;
using TechStore.Data;
using TechStore.Models;

namespace TechStore.Services;

/// <summary>
/// Implementación de <see cref="IProductoService"/>. Contiene la lógica de
/// negocio de los productos y usa el DbContext de Entity Framework Core para
/// realizar las operaciones CRUD contra la base de datos.
/// </summary>
public class ProductoService : IProductoService
{
    private readonly TechStoreDbContext _context;

    // El DbContext llega por Inyección de Dependencias.
    public ProductoService(TechStoreDbContext context)
    {
        _context = context;
    }

    // READ: lista los productos (con su categoría) y permite filtrar por texto.
    public async Task<IEnumerable<Producto>> ObtenerTodosAsync(string? buscar = null)
    {
        var consulta = _context.Productos
            .Include(p => p.Categoria)   // trae los datos de la categoría relacionada
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var termino = buscar.Trim();
            consulta = consulta.Where(p =>
                p.Nombre.Contains(termino) ||
                p.Descripcion.Contains(termino) ||
                p.Categoria!.Nombre.Contains(termino));
        }

        return await consulta.OrderBy(p => p.Nombre).ToListAsync();
    }

    // READ (uno): localiza un producto por su clave primaria, con su categoría.
    public async Task<Producto?> ObtenerPorIdAsync(int id)
    {
        return await _context.Productos
            .Include(p => p.Categoria)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    // CREATE: inserta un nuevo producto.
    public async Task CrearAsync(Producto producto)
    {
        _context.Productos.Add(producto);
        await _context.SaveChangesAsync();
    }

    // UPDATE: actualiza los campos de un producto existente.
    public async Task ActualizarAsync(Producto producto)
    {
        _context.Productos.Update(producto);
        await _context.SaveChangesAsync();
    }

    // DELETE: elimina el producto indicado si existe.
    public async Task EliminarAsync(int id)
    {
        var producto = await _context.Productos.FindAsync(id);
        if (producto is not null)
        {
            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
        }
    }
}
