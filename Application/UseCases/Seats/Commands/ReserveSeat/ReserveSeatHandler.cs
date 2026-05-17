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

    public async Task<ReserveSeatResult> HandleAsync(ReserveSeatCommand command)
    {
        // 1. Obtener la butaca
        var seat = await _repository.GetByIdAsync(command.SeatId);

        // Caso: la butaca no existe → 404
        if (seat == null)
        {
            _repository.AddAuditLog(new Audit_Log
            {
                Id = Guid.NewGuid(),
                UserId = command.UserId,
                Action = "RESERVE_FAILED_NOT_FOUND",
                EntityType = "Seat",
                EntityId = command.SeatId.ToString(),
                Details = $"INTENTO FALLIDO: LA BUTACA {command.SeatId} NO EXISTE",
                CreatedAt = DateTime.Now
            });
            await _repository.SaveChangesAsync();
            return new ReserveSeatResult
            {
                Success = false,
                ErrorCode = "NOT_FOUND",
                Message = "La butaca no existe."
            };
        }

        // Caso: la butaca no está disponible → 409
        if (seat.Status != "Available")
        {
            _repository.AddAuditLog(new Audit_Log
            {
                Id = Guid.NewGuid(),
                UserId = command.UserId,
                Action = "RESERVE_FAILED_UNAVAILABLE",
                EntityType = "Seat",
                EntityId = seat.Id.ToString(),
                Details = $"INTENTO FALLIDO: BUTACA {seat.SeatNumber} EN ESTADO {seat.Status}",
                CreatedAt = DateTime.Now
            });
            await _repository.SaveChangesAsync();
            return new ReserveSeatResult
            {
                Success = false,
                ErrorCode = "UNAVAILABLE",
                Message = "La butaca no está disponible."
            };
        }

        // Caso: versión incorrecta → 409
        if (seat.Version != command.Version)
        {
            _repository.AddAuditLog(new Audit_Log
            {
                Id = Guid.NewGuid(),
                UserId = command.UserId,
                Action = "RESERVE_FAILED_CONCURRENCY",
                EntityType = "Seat",
                EntityId = seat.Id.ToString(),
                Details = $"INTENTO FALLIDO: CONFLICTO DE VERSIÓN EN BUTACA {seat.SeatNumber} (ESPERADA: {command.Version}, ACTUAL: {seat.Version})",
                CreatedAt = DateTime.Now
            });
            await _repository.SaveChangesAsync();
            return new ReserveSeatResult
            {
                Success = false,
                ErrorCode = "CONCURRENCY",
                Message = "La butaca fue modificada por otro usuario. Intentá nuevamente."
            };
        }

        // 2. Modificar estado en memoria
        seat.Status = "Reserved";
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

        // 4. Auditoría del camino feliz
        _repository.AddAuditLog(new Audit_Log
        {
            Id = Guid.NewGuid(),
            UserId = command.UserId,
            Action = "RESERVE_SUCCESS",
            EntityType = "Seat",
            EntityId = seat.Id.ToString(),
            Details = $"RESERVA CREADA: BUTACA {seat.SeatNumber} SECTOR {seat.SectorId} VERSIÓN {seat.Version}",
            CreatedAt = DateTime.Now
        });

        // 5. Persistencia total (atomicidad)
        try
        {
            await _repository.SaveChangesAsync();
            return new ReserveSeatResult
            {
                Success = true,
                ReservationId = reservation.Id,
                Message = "Reserva realizada con éxito. Tenés 5 minutos para pagar."
            };
        }
        catch (Domain.Exceptions.ConcurrencyException)
        {
            return new ReserveSeatResult
            {
                Success = false,
                ErrorCode = "CONCURRENCY",
                Message = "La butaca fue tomada por otro usuario. Intentá nuevamente."
            };
        }
        catch (Exception)
        {
            return new ReserveSeatResult
            {
                Success = false,
                ErrorCode = "DB_ERROR",
                Message = "Error al guardar la reserva. Intentá nuevamente."
            };
        }
    }
}