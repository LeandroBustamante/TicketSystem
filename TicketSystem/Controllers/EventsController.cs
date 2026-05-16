using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Events.Queries.GetAllEvents;
using Application.UseCases.Sectors.Queries.GetSectorsByEventId;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/v1/events")]
[Tags("Events")]
public class EventsController : ControllerBase
{
    private readonly IGetAllEventsHandler _getAllEventsHandler;
    private readonly IGetSectorsByEventIdHandler _getSectorsHandler;

    public EventsController(
        IGetAllEventsHandler getAllEventsHandler,
        IGetSectorsByEventIdHandler getSectorsHandler)
    {
        _getAllEventsHandler = getAllEventsHandler;
        _getSectorsHandler = getSectorsHandler;
    }

    /// <summary>
    /// Devuelve una lista paginada de todos los eventos activos.
    /// </summary>
    /// <param name="page">Número de página (default: 1)</param>
    /// <param name="pageSize">Cantidad de resultados por página (default: 10)</param>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<EventResponse>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _getAllEventsHandler.HandleAsync(new GetAllEventsQuery
        {
            Page = page,
            PageSize = pageSize
        });
        return Ok(result);
    }

    /// <summary>
    /// Devuelve todos los sectores de un evento dado su ID.
    /// </summary>
    /// <param name="id">ID del evento</param>
    [HttpGet("{id}/sectors")]
    [ProducesResponseType(typeof(IEnumerable<SectorResponse>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetSectors(int id)
    {
        var result = await _getSectorsHandler.HandleAsync(new GetSectorsByEventIdQuery(id));

        if (result == null)
            return NotFound(new { message = $"El evento con ID {id} no existe." });

        return Ok(result);
    }
}