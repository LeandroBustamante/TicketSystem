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

    public async Task<IEnumerable<SectorResponse>> HandleAsync(GetSectorsByEventIdQuery query)
    {
        // Buscamos los sectores asociados al evento
        var sectors = await _repository.GetSectorsByEventIdAsync(query.EventId);


        // Mapeamos a DTOs de salida
        return sectors.Select(s => new SectorResponse
        {
            Id = s.Id,
            Name = s.Name,
            Price = s.Price,
            Capacity = s.Capacity
        });
    }
}