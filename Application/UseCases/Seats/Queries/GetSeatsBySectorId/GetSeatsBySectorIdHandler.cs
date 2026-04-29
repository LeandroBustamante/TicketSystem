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

    public async Task<IEnumerable<SeatResponse>> HandleAsync(GetSeatsBySectorIdQuery query)
    {
        // Obtenemos las butacas de la base de datos
        var seats = await _repository.GetBySectorIdAsync(query.SectorId);


        // Mapeamos a DTOs para informar el estado (Available, Reserved, Sold) al Front
        return seats.Select(s => new SeatResponse
        {
            Id = s.Id,
            RowIdentifier = s.RowIdentifier,
            SeatNumber = s.SeatNumber,
            Status = s.Status,
            // Mapeamos la versión hacia el dto para el frontend
            Version = s.Version
        });
    }
}