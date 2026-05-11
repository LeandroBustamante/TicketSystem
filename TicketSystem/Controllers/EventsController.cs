using Application.Interfaces;
using Application.UseCases.Events.Queries.GetAllEvents;
using Application.UseCases.Seats.Commands.PayReservation;
using Application.UseCases.Seats.Commands.ReserveSeat;
using Application.UseCases.Seats.Queries.GetSeatsBySectorId;
using Application.UseCases.Sectors.Queries.GetSectorsByEventId;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/v1/events")] // URL base siguiendo estándar REST 
public class EventsController : ControllerBase
{
    private readonly IPayReservationHandler _payReservationHandler;
    private readonly IGetAllEventsHandler _getAllEventsHandler;
    private readonly IGetSectorsByEventIdHandler _getSectorsHandler;
    private readonly IGetSeatsBySectorIdHandler _getSeatsHandler;
    private readonly IReserveSeatHandler _reserveSeatHandler;

    public EventsController(IGetAllEventsHandler getAllEventsHandler, IGetSectorsByEventIdHandler getSectorsHandler,
        IGetSeatsBySectorIdHandler getSeatsHandler,
        IReserveSeatHandler reserveSeatHandler, IPayReservationHandler payReservationHandler)
    {
        _getAllEventsHandler = getAllEventsHandler;
        _getSectorsHandler = getSectorsHandler;
        _getSeatsHandler = getSeatsHandler;
        _reserveSeatHandler = reserveSeatHandler;
        _payReservationHandler = payReservationHandler;
    }


    // 1. Listar todos los eventos (Catálogo inicial)
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _getAllEventsHandler.HandleAsync(new GetAllEventsQuery
        {
            Page = page,
            PageSize = pageSize
        });
        return Ok(result);
    }


    // 2. Listar sectores de un evento específico 
    [HttpGet("{id}/sectors")]
    public async Task<IActionResult> GetSectors(int id)
    {
        var result = await _getSectorsHandler.HandleAsync(new GetSectorsByEventIdQuery(id));

        // Si el handler nos devuelve null, es porque el evento no existe
        if (result == null)
        {
            return NotFound(new { message = $"El evento con ID {id} no existe." });
        }

        return Ok(result);
    }



    // 3. Listar estado de asientos de un sector (Mapa de asientos) 
    // Siguiendo jerarquía: /api/v1/events/{id}/sectors/{sectorId}/seats 
    [HttpGet("sectors/{sectorId}/seats")]
    public async Task<IActionResult> GetSeats(int sectorId)
    {
        var result = await _getSeatsHandler.HandleAsync(new GetSeatsBySectorIdQuery(sectorId));

        if (result == null)
        {
            return NotFound(new { message = $"El sector con ID {sectorId} no existe." });
        }

        return Ok(result);
    }


    // 4. Intento de reserva 
    [HttpPost("seats/reserve")]
    public async Task<IActionResult> Reserve([FromBody] ReserveSeatCommand command)
    {
        var reservationId = await _reserveSeatHandler.HandleAsync(command);

        if (reservationId == null)
        {
            // Si falla por concurrencia o disponibilidad, devolvemos 409 Conflict 
            return Conflict(new { message = "La butaca no está disponible o ya fue reservada por otro usuario." });
        }

        return Ok(new
        {
            message = "Reserva realizada con éxito. Tenés 5 minutos para pagar.",
            reservationId = reservationId
        });
    }

    // 5. Confirmar pago de una reserva
    [HttpPost("reservations/{reservationId}/pay")]
    public async Task<IActionResult> Pay(Guid reservationId, [FromBody] PayReservationCommand command)
    {
        // Asignamos el ID de la URL al command
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

        return Ok(new { message = result.Message });
    }
}