using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly AppDbContext _context;

    public ReservationRepository(AppDbContext context)
    {
        _context = context;
    }

    // Trae la reserva con su butaca incluida para poder modificar ambas en la misma operación
    public async Task<Reservation?> GetByIdWithSeatAsync(Guid reservationId)
    {
        return await _context.Reservations
            .Include(r => r.Seat)
            .FirstOrDefaultAsync(r => r.Id == reservationId);
    }

    public void UpdateReservation(Reservation reservation)
    {
        _context.Reservations.Update(reservation);
    }

    public void UpdateSeat(Seat seat)
    {
        _context.Seats.Update(seat);
    }

    public void AddAuditLog(Audit_Log log)
    {
        _context.Audit_Logs.Add(log);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
