using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.Seats.Commands.ReserveSeat;

public class ReserveSeatHandler : IReserveSeatHandler
{
    private readonly ISeatRepository _repository;

    public ReserveSeatHandler(ISeatRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> HandleAsync(ReserveSeatCommand command)
    {
        // 1. Obtener la butaca
        var seat = await _repository.GetByIdAsync(command.SeatId);


        // Validar disponibilidad y versión (optimistic locking)
        // Si la versión en db es distinta a la que mandó el front, alguien cambió el asiento
        if (seat == null || seat.Status != "Available" || seat.Version != command.Version)
        {
            return false;
        }


        // 2. Modificar estado en memoria
        seat.Status = "Reserved";


        // Incrementamos la versión manualmente para el próximo control
        seat.Version++;

        _repository.Update(seat);


        // 3. Preparar reserva
        var reservation = new Reservation
        {
            Id = Guid.NewGuid(),
            UserId = command.UserId,
            SeatId = seat.Id,
            Status = "Pending",
            ReservedAt = DateTime.Now,
            ExpiresAt = DateTime.Now.AddMinutes(5)
        };
        _repository.AddReservation(reservation);


        // 4. Preparar log de auditoría
        var log = new Audit_Log
        {
            Id = Guid.NewGuid(),
            UserId = command.UserId,
            Action = "RESERVE_SUCCESS",
            EntityType = "Seat",
            EntityId = seat.Id.ToString(),
            Details = $"RESERVA CREADA: BUTACA {seat.SeatNumber} SECTOR {seat.SectorId} VERSIÓN {seat.Version}",
            CreatedAt = DateTime.Now
        };
        _repository.AddAuditLog(log);


        // 5. Intentar persistencia total (atomicidad)
        try
        {
            await _repository.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            // Si falla cualquier parte, ef core no persiste nada (rollback automático)
            return false;
        }
    }
}