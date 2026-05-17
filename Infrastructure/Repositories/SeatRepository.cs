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
        try
        {
            return await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new Domain.Exceptions.ConcurrencyException("Conflicto de concurrencia al guardar.");
        }
        catch (DbUpdateException)
        {
            // DbUpdateException también puede indicar conflicto de concurrencia
            throw new Domain.Exceptions.ConcurrencyException("Conflicto al guardar. La butaca puede haber sido tomada.");
        }
    }

    public async Task<bool> SectorExistsAsync(int sectorId)
    {
        return await _context.Sectors.AnyAsync(s => s.Id == sectorId);
    }
}