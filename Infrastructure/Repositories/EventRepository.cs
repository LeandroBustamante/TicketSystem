using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

// Accede a la base de datos a través del DbContext siguiendo el patrón Repository para desacoplar la capa de datos de la lógica de negocio.
public class EventRepository : IEventRepository
{
    private readonly AppDbContext _context;

    public EventRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Event>> GetAllAsync()
    {
        return await _context.Events.ToListAsync();
    }

    public async Task<Event?> GetByIdAsync(int id)
    {
        return await _context.Events.FindAsync(id);
    }

    // Filtra por EventId en lugar de traer todos los sectores para evitar cargar datos innecesarios.
    public async Task<IEnumerable<Sector>> GetSectorsByEventIdAsync(int eventId)
    {
        return await _context.Sectors
            .Where(s => s.EventId == eventId)
            .ToListAsync();
    }
}