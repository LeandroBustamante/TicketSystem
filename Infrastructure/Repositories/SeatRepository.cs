using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SeatRepository : ISeatRepository
{
    private readonly AppDbContext _context;

    public SeatRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Seat>> GetBySectorIdAsync(int sectorId)
    {
        return await _context.Seats
            .Where(s => s.SectorId == sectorId)
            .ToListAsync();
    }

    public async Task<Seat?> GetByIdAsync(Guid id)
    {
        return await _context.Seats.FindAsync(id);
    }

    public void Update(Seat seat)
    {
        // EF Core incluirá el campo Version en el WHERE del UPDATE para detectar conflictos de concurrencia.
        _context.Seats.Update(seat);
    }

    public void AddReservation(Reservation reservation)
    {
        _context.Reservations.Add(reservation);
    }

    public void AddAuditLog(Audit_Log log)
    {
        _context.Audit_Logs.Add(log);
    }

    public async Task<int> SaveChangesAsync()
    {
        // Persiste todas las operaciones pendientes en una sola transacción. Si algo falla, EF Core hace rollback automático.
        return await _context.SaveChangesAsync();
    }

    public async Task<bool> SectorExistsAsync(int sectorId)
    {
        return await _context.Sectors.AnyAsync(s => s.Id == sectorId);
    }
}