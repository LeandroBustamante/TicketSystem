using Application.DTOs;
using Application.Interfaces;

namespace Application.UseCases.Seats.Queries.GetSeatsBySectorId;

public class GetSeatsBySectorIdHandler : IGetSeatsBySectorIdHandler
{
    private readonly ISeatRepository _repository;

    public GetSeatsBySectorIdHandler(ISeatRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<SeatResponse>?> HandleAsync(GetSeatsBySectorIdQuery query)
    {
        // 1. Validamos si el sector existe
        var sectorExists = await _repository.SectorExistsAsync(query.SectorId);

        if (!sectorExists)
        {
            return null; // Devolvemos null para que el Controller sepa que es un 404
        }

        // 2. Si existe, obtenemos las butacas
        var seats = await _repository.GetBySectorIdAsync(query.SectorId);

        return seats.Select(s => new SeatResponse
        {
            Id = s.Id,
            RowIdentifier = s.RowIdentifier,
            SeatNumber = s.SeatNumber,
            Status = s.Status,
            Version = s.Version
        });
    }


}