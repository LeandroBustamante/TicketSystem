using Application.DTOs;
using Application.Interfaces;

namespace Application.UseCases.Events.Queries.GetAllEvents;

public class GetAllEventsHandler : IGetAllEventsHandler
{
    private readonly IEventRepository _repository;

    // Inyectamos el repositorio para acceder a los datos
    public GetAllEventsHandler(IEventRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<EventResponse>> HandleAsync(GetAllEventsQuery query)
    {
        // 1. Buscamos las entidades en la base de datos
        var events = await _repository.GetAllAsync();


        // 2. Mapeamos las entidades a DTOs de respuesta
        return events.Select(e => new EventResponse
        {
            Id = e.Id,
            Name = e.Name,
            EventDate = e.EventDate,
            Venue = e.Venue
        });
    }
}