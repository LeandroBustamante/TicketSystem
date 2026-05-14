using Application.Interfaces;
using Application.UseCases.Seats.Commands.ReserveSeat;
using Application.UseCases.Seats.Queries.GetSeatsBySectorId;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/v1/sectors")]
public class SeatsController : ControllerBase
{
    private readonly IGetSeatsBySectorIdHandler _getSeatsHandler;
    private readonly IReserveSeatHandler _reserveSeatHandler;

    public SeatsController(
        IGetSeatsBySectorIdHandler getSeatsHandler,
        IReserveSeatHandler reserveSeatHandler)
    {
        _getSeatsHandler = getSeatsHandler;
        _reserveSeatHandler = reserveSeatHandler;
    }

    // GET /api/v1/sectors/{sectorId}/seats
    [HttpGet("{sectorId}/seats")]
    public async Task<IActionResult> GetSeats(int sectorId)
    {
        var result = await _getSeatsHandler.HandleAsync(new GetSeatsBySectorIdQuery(sectorId));

        if (result == null)
            return NotFound(new { message = $"El sector con ID {sectorId} no existe." });

        return Ok(result);
    }

    // POST /api/v1/sectors/{sectorId}/seats/reserve
    [HttpPost("{sectorId}/seats/reserve")]
    public async Task<IActionResult> Reserve(int sectorId, [FromBody] ReserveSeatCommand command)
    {
        var reservationId = await _reserveSeatHandler.HandleAsync(command);

        if (reservationId == null)
            return Conflict(new { message = "La butaca no está disponible o ya fue reservada por otro usuario." });

        return CreatedAtAction(nameof(GetSeats), new { sectorId },
            new
            {
                message = "Reserva realizada con éxito. Tenés 5 minutos para pagar.",
                reservationId
            });
    }
}
