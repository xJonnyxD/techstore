using Microsoft.EntityFrameworkCore;
using TechStore.Data;
using TechStore.Models;

namespace TechStore.Services;

public class RegistroEventoService : IRegistroEventoService
{
    private readonly TechStoreDbContext _context;

    public RegistroEventoService(TechStoreDbContext context)
    {
        _context = context;
    }

    public async Task RegistrarAsync(string nivel, string origen, string mensaje, string? usuario = null)
    {
        var evento = new RegistroEvento
        {
            Fecha = DateTime.Now,
            Nivel = nivel,
            Origen = origen,
            Mensaje = mensaje,
            Usuario = usuario
        };

        _context.RegistroEventos.Add(evento);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<RegistroEvento>> ObtenerRecientesAsync(int cantidad = 50)
    {
        return await _context.RegistroEventos
            .OrderByDescending(e => e.Fecha)
            .Take(cantidad)
            .ToListAsync();
    }
}
