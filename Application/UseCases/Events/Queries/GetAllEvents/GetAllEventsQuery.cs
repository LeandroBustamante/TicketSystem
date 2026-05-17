namespace Application.UseCases.Events.Queries.GetAllEvents;

// Parámetros de paginación para el listado de eventos. Los valores por defecto evitan que el cliente tenga que enviarlos siempre
public class GetAllEventsQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}