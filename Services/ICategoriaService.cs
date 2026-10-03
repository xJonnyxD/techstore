using TechStore.Models;

namespace TechStore.Services;

/// <summary>
/// Abstracción del servicio de categorías. Los controladores dependen de esta
/// interfaz (Inversión de Dependencias / principio de inversión de dependencias).
/// </summary>
public interface ICategoriaService
{
    Task<IEnumerable<Categoria>> ObtenerTodasAsync();
    Task<Categoria?> ObtenerPorIdAsync(int id);
    Task CrearAsync(Categoria categoria);
    Task ActualizarAsync(Categoria categoria);
    Task EliminarAsync(int id);
    Task<bool> TieneProductosAsync(int categoriaId);
}
