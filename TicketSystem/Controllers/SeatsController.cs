using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Seats.Commands.ReserveSeat;
using Application.UseCases.Seats.Queries.GetSeatsBySectorId;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/v1/sectors")]
[Tags("Seats")]
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

    /// <summary>
    /// Devuelve todos los asientos de un sector con su estado actual: Available, Reserved o Sold.
    /// </summary>
    /// <param name="sectorId">ID del sector</param>
    [HttpGet("{sectorId}/seats")]
    [ProducesResponseType(typeof(IEnumerable<SeatResponse>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetSeats(int sectorId)
    {
        var result = await _getSeatsHandler.HandleAsync(new GetSeatsBySectorIdQuery(sectorId));

        if (result == null)
            return NotFound(new { message = $"El sector con ID {sectorId} no existe." });

        return Ok(result);
    }

    /// <summary>
    /// Intenta reservar una butaca por 5 minutos. Implementa Optimistic Locking con el campo Version para evitar reservas duplicadas bajo alta concurrencia.
    /// </summary>
    /// <param name="sectorId">ID del sector al que pertenece la butaca</param>
    /// <param name="command">Datos de la reserva (seatId, userId, version)</param>
    [HttpPost("{sectorId}/seats/reserve")]
    [ProducesResponseType(typeof(ReservationResponse), 201)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> Reserve(int sectorId, [FromBody] ReserveSeatCommand command)
    {
        var result = await _reserveSeatHandler.HandleAsync(command);

        if (!result.Success)
        {
            if (result.ErrorCode == "NOT_FOUND")
                return NotFound(new { message = result.Message });

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
