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

    public async Task<PagedResponse<EventResponse>> HandleAsync(GetAllEventsQuery query)
    {
        // 1. Traemos todos los eventos
        var events = await _repository.GetAllAsync();
        var totalItems = events.Count();

        // 2. Aplicamos paginación correctamente
        var paginatedEvents = events
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(e => new EventResponse
            {
                Id = e.Id,
                Name = e.Name,
                EventDate = e.EventDate,
                Venue = e.Venue,
                Status = e.Status
            });

        // 3. Calculamos metadata de paginación
        var totalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize);

        return new PagedResponse<EventResponse>
        {
            Data = paginatedEvents,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = totalItems,
            TotalPages = totalPages,
            HasNext = query.Page < totalPages,
            HasPrevious = query.Page > 1
        };
    }
}