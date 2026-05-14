using Application.Interfaces;
using Application.UseCases.Events.Queries.GetAllEvents;
using Application.UseCases.Sectors.Queries.GetSectorsByEventId;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/v1/events")]
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

    // GET /api/v1/events
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

    // GET /api/v1/events/{id}/sectors
    [HttpGet("{id}/sectors")]
    public async Task<IActionResult> GetSectors(int id)
    {
        var result = await _getSectorsHandler.HandleAsync(new GetSectorsByEventIdQuery(id));

        if (result == null)
            return NotFound(new { message = $"El evento con ID {id} no existe." });

        return Ok(result);
    }
}