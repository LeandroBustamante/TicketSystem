using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Seats.Commands.PayReservation;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/v1/reservations")]
public class ReservationsController : ControllerBase
{
    private readonly IPayReservationHandler _payReservationHandler;

    public ReservationsController(IPayReservationHandler payReservationHandler)
    {
        _payReservationHandler = payReservationHandler;
    }

    // POST /api/v1/reservations/{reservationId}/pay
    [HttpPost("{reservationId}/pay")]
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
