using TechStore.Models;

namespace TechStore.Services;

public interface IRegistroEventoService
{
    Task RegistrarAsync(string nivel, string origen, string mensaje, string? usuario = null);
    Task<IEnumerable<RegistroEvento>> ObtenerRecientesAsync(int cantidad = 50);
}
