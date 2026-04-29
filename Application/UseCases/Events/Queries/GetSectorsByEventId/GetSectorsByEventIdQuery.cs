namespace Application.UseCases.Sectors.Queries.GetSectorsByEventId;

public class GetSectorsByEventIdQuery
{
    public int EventId { get; set; }

    public GetSectorsByEventIdQuery(int eventId)
    {
        EventId = eventId;
    }
}