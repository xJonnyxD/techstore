using Microsoft.EntityFrameworkCore;
using TechStore.Data;
using TechStore.Models;

namespace TechStore.Services;

/// <summary>
/// Implementación de <see cref="ICategoriaService"/>. Gestiona la lógica de
/// negocio de las categorías mediante Entity Framework Core.
/// </summary>
public class CategoriaService : ICategoriaService
{
    private readonly TechStoreDbContext _context;

    public CategoriaService(TechStoreDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Categoria>> ObtenerTodasAsync()
    {
        return await _context.Categorias.OrderBy(c => c.Nombre).ToListAsync();
    }

    public async Task<Categoria?> ObtenerPorIdAsync(int id)
    {
        return await _context.Categorias
            .Include(c => c.Productos)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task CrearAsync(Categoria categoria)
    {
        _context.Categorias.Add(categoria);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Categoria categoria)
    {
        _context.Categorias.Update(categoria);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(int id)
    {
        var categoria = await _context.Categorias.FindAsync(id);
        if (categoria is not null)
        {
            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();
        }
    }

    // Regla de negocio: indica si la categoría tiene productos asociados, para
    // impedir que se elimine y se rompa la integridad referencial.
    public async Task<bool> TieneProductosAsync(int categoriaId)
    {
        return await _context.Productos.AnyAsync(p => p.CategoriaId == categoriaId);
    }
}
