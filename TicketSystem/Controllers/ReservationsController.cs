using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Seats.Commands.PayReservation;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/v1/reservations")]
[Tags("Reservations")]
public class ReservationsController : ControllerBase
{
    private readonly IPayReservationHandler _payReservationHandler;

    public ReservationsController(IPayReservationHandler payReservationHandler)
    {
        _payReservationHandler = payReservationHandler;
    }

    /// <summary>
    /// Confirma el pago de una reserva activa. Cambia el asiento a Sold y la reserva a Completed bajo una transacción ACID. Si algo falla se ejecuta un rollback completo.
    /// </summary>
    /// <param name="reservationId">ID de la reserva a pagar</param>
    /// <param name="command">Datos del pago (userId)</param>

    [HttpPost("{reservationId}/pay")]
    [ProducesResponseType(typeof(PaymentResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    [ProducesResponseType(402)]
    public async Task<IActionResult> Pay(Guid reservationId, [FromBody] PayReservationCommand command)
    {
        command.ReservationId = reservationId;

        var result = await _payReservationHandler.HandleAsync(command);

        if (!result.Success)
        {
            if (result.ErrorCode == "NOT_FOUND")
                return NotFound(new { message = result.Message });

            if (result.ErrorCode == "EXPIRED")
                return Conflict(new { message = result.Message });

            if (result.ErrorCode == "PAYMENT_REJECTED")
                return StatusCode(402, new { message = result.Message });

            return StatusCode(500, new { message = result.Message });
        }

        return Ok(new PaymentResponse { Message = result.Message });
    }
}