using Application.DTOs;
using Application.Interfaces;

namespace Application.UseCases.Sectors.Queries.GetSectorsByEventId;


public class GetSectorsByEventIdHandler : IGetSectorsByEventIdHandler
{
    private readonly IEventRepository _repository;

    public GetSectorsByEventIdHandler(IEventRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<SectorResponse>?> HandleAsync(GetSectorsByEventIdQuery query)
    {
        // 1. Verificamos si el evento existe en la DB
        var eventEntity = await _repository.GetByIdAsync(query.EventId);

        // 2. Si el evento no existe, devolvemos null para señalizar el 404
        if (eventEntity == null)
        {
            return null;
        }

        // 3. Si existe, buscamos sus sectores
        var sectors = await _repository.GetSectorsByEventIdAsync(query.EventId);

        return sectors.Select(s => new SectorResponse
        {
            Id = s.Id,
            Name = s.Name,
            Price = s.Price,
            Capacity = s.Capacity
        });
    }
}
