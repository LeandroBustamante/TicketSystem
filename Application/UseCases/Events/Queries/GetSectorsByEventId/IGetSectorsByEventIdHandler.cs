using Application.DTOs;

namespace Application.UseCases.Sectors.Queries.GetSectorsByEventId;

public interface IGetSectorsByEventIdHandler
{
    Task<IEnumerable<SectorResponse>> HandleAsync(GetSectorsByEventIdQuery query);
}