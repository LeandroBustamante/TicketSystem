namespace Application.UseCases.Seats.Queries.GetSeatsBySectorId;

// Transporta el ID del sector seleccionado para obtener sus butacas
public class GetSeatsBySectorIdQuery
{
    public int SectorId { get; set; }

    public GetSeatsBySectorIdQuery(int sectorId)
    {
        SectorId = sectorId;
    }
}