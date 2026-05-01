namespace Application.UseCases.Events.Queries.GetAllEvents;

// Como no necesitamos parámetros para listar todo, la clase está vacía.
// Solo sirve para identificar la intención del usuario.
public class GetAllEventsQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}