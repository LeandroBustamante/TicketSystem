using Application.UseCases.Events.Queries.GetAllEvents;
using Application.UseCases.Sectors.Queries.GetSectorsByEventId;
using Application.UseCases.Seats.Queries.GetSeatsBySectorId;
using Application.UseCases.Seats.Commands.ReserveSeat;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/v1/events")] // URL base siguiendo estándar REST 
public class EventsController : ControllerBase
{
    private readonly IGetAllEventsHandler _getAllEventsHandler;
    private readonly IGetSectorsByEventIdHandler _getSectorsHandler;
    private readonly IGetSeatsBySectorIdHandler _getSeatsHandler;
    private readonly IReserveSeatHandler _reserveSeatHandler;

    public EventsController(IGetAllEventsHandler getAllEventsHandler, IGetSectorsByEventIdHandler getSectorsHandler,
        IGetSeatsBySectorIdHandler getSeatsHandler,
        IReserveSeatHandler reserveSeatHandler)
    {
        _getAllEventsHandler = getAllEventsHandler;
        _getSectorsHandler = getSectorsHandler;
        _getSeatsHandler = getSeatsHandler;
        _reserveSeatHandler = reserveSeatHandler;
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
        var success = await _reserveSeatHandler.HandleAsync(command);

        if (!success)
        {
            // Si falla por concurrencia o disponibilidad, devolvemos 409 Conflict 
            return Conflict(new { message = "La butaca no está disponible o ya fue reservada por otro usuario." });
        }

        return Ok(new { message = "Reserva realizada con éxito. Tenés 5 minutos para pagar." });
    }
}