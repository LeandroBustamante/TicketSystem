using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

// Esta clase implementa la interfaz definida en Application
public class EventRepository : IEventRepository
{
    private readonly AppDbContext _context;

    // Inyectamos el DbContext para interactuar con SQL Server
    public EventRepository(AppDbContext context)
    {
        _context = context;
    }

    // Obtiene todos los eventos de la tabla Events
    public async Task<IEnumerable<Event>> GetAllAsync()
    {
        return await _context.Events.ToListAsync();
    }

    // Busca un evento por su ID
    public async Task<Event?> GetByIdAsync(int id)
    {
        return await _context.Events.FindAsync(id);
    }

    // Filtra los sectores que pertenecen a un EventId específico
    public async Task<IEnumerable<Sector>> GetSectorsByEventIdAsync(int eventId)
    {
        return await _context.Sectors
            .Where(s => s.EventId == eventId)
            .ToListAsync();
    }
}