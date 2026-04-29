using Application.DTOs;

namespace Application.UseCases.Seats.Queries.GetSeatsBySectorId;

public interface IGetSeatsBySectorIdHandler
{
    Task<IEnumerable<SeatResponse>> HandleAsync(GetSeatsBySectorIdQuery query);
}