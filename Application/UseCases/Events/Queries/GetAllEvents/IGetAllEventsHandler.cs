using Application.DTOs;
using Application.UseCases.Events.Queries.GetAllEvents;

namespace Application.Interfaces;

public interface IGetAllEventsHandler
{
    Task<PagedResponse<EventResponse>> HandleAsync(GetAllEventsQuery query);
}