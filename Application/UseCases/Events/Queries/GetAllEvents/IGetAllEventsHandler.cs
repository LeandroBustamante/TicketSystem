using Application.DTOs;

namespace Application.UseCases.Events.Queries.GetAllEvents;

public interface IGetAllEventsHandler
{
    // El método recibe la Query y devuelve una lista de DTOs de respuesta
    Task<IEnumerable<EventResponse>> HandleAsync(GetAllEventsQuery query);
}