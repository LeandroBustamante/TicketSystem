using Application.DTOs;

namespace Application.UseCases.Sectors.Queries.GetSectorsByEventId;

public interface IGetSectorsByEventIdHandler
{
    // Agregamos el "?" para permitir el retorno de null
    Task<IEnumerable<SectorResponse>?> HandleAsync(GetSectorsByEventIdQuery query);
   
}