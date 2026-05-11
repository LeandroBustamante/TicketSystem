using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.Seats.Commands.PayReservation;

public class PayReservationHandler : IPayReservationHandler
{
    private readonly IReservationRepository _repository;

    public PayReservationHandler(IReservationRepository repository)
    {
        _repository = repository;
    }

    public async Task<PayReservationResult> HandleAsync(PayReservationCommand command)
    {
        // 1. Buscar la reserva junto con su butaca
        var reservation = await _repository.GetByIdWithSeatAsync(command.ReservationId);

        // Validar que existe y pertenece al usuario
        if (reservation == null || reservation.UserId != command.UserId)
        {
            return new PayReservationResult
            {
                Success = false,
                ErrorCode = "NOT_FOUND",
                Message = "Reserva no encontrada."
            };
        }

        // Validar que no expiró
        if (reservation.Status != "Pending" || reservation.ExpiresAt < DateTime.Now)
        {
            return new PayReservationResult
            {
                Success = false,
                ErrorCode = "EXPIRED",
                Message = "La reserva expiró. La butaca fue liberada."
            };
        }

        // Validar que la butaca sigue reservada
        if (reservation.Seat == null || reservation.Seat.Status != "Reserved")
        {
            return new PayReservationResult
            {
                Success = false,
                ErrorCode = "SEAT_UNAVAILABLE",
                Message = "La butaca no está en estado válido para pagar."
            };
        }

        // 2. Simular pasarela de pago (siempre aprueba)
        var paymentApproved = SimulatePayment();

        if (!paymentApproved)
        {
            _repository.AddAuditLog(new Audit_Log
            {
                Id = Guid.NewGuid(),
                UserId = command.UserId,
                Action = "PAYMENT_FAILED",
                EntityType = "Reservation",
                EntityId = reservation.Id.ToString(),
                Details = $"PAGO RECHAZADO: RESERVA {reservation.Id} BUTACA {reservation.Seat.SeatNumber}",
                CreatedAt = DateTime.Now
            });

            await _repository.SaveChangesAsync();

            return new PayReservationResult
            {
                Success = false,
                ErrorCode = "PAYMENT_REJECTED",
                Message = "El pago fue rechazado."
            };
        }

        // 3. Confirmar: cambiar estados
        reservation.Status = "Completed";
        reservation.Seat.Status = "Sold";
        reservation.Seat.Version++;

        _repository.UpdateReservation(reservation);
        _repository.UpdateSeat(reservation.Seat);

        _repository.AddAuditLog(new Audit_Log
        {
            Id = Guid.NewGuid(),
            UserId = command.UserId,
            Action = "PAYMENT_SUCCESS",
            EntityType = "Reservation",
            EntityId = reservation.Id.ToString(),
            Details = $"PAGO CONFIRMADO: BUTACA {reservation.Seat.SeatNumber} VENDIDA",
            CreatedAt = DateTime.Now
        });

        // 4. Persistir todo junto (transacción ACID)
        try
        {
            await _repository.SaveChangesAsync();
            return new PayReservationResult
            {
                Success = true,
                Message = "Pago confirmado. ¡Disfrutá el evento!"
            };
        }
        catch (Exception)
        {
            return new PayReservationResult
            {
                Success = false,
                ErrorCode = "DB_ERROR",
                Message = "Error al confirmar el pago. Intentá nuevamente."
            };
        }
    }

    // Simulación de pasarela — siempre aprueba
    private bool SimulatePayment() => true;
}