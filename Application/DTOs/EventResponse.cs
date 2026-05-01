namespace Application.DTOs;

// DTO para mostrar la información básica del evento en el catálogo 
public class EventResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public string Venue { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}