using TechStore.Models;

namespace TechStore.Services;

/// <summary>
/// Abstracción del servicio de productos. Los controladores dependen de esta
/// interfaz y no de una implementación concreta (Inversión de Dependencias).
/// </summary>
public interface IProductoService
{
    Task<IEnumerable<Producto>> ObtenerTodosAsync(string? buscar = null);
    Task<Producto?> ObtenerPorIdAsync(int id);
    Task CrearAsync(Producto producto);
    Task ActualizarAsync(Producto producto);
    Task EliminarAsync(int id);
}
