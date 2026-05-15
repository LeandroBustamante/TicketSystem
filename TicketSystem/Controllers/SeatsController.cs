using Application.DTOs;
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
        var result = await _reserveSeatHandler.HandleAsync(command);

        if (!result.Success)
        {
            // La butaca no existe → 404
            if (result.ErrorCode == "NOT_FOUND")
                return NotFound(new { message = result.Message });

            // La butaca no está disponible o hay conflicto de concurrencia → 409
            if (result.ErrorCode == "UNAVAILABLE" || result.ErrorCode == "CONCURRENCY")
                return Conflict(new { message = result.Message });

            return StatusCode(500, new { message = result.Message });
        }

        return CreatedAtAction(nameof(GetSeats), new { sectorId },
            new ReservationResponse
            {
                ReservationId = result.ReservationId ?? Guid.Empty,
                Message = result.Message
            });
    }
}
