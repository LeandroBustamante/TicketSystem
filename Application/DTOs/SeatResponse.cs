namespace Application.DTOs;

// DTO para armar el "Mapa de Asientos" en el Frontend 
public class SeatResponse
{
    public Guid Id { get; set; }
    public string RowIdentifier { get; set; } = string.Empty;
    public int SeatNumber { get; set; }

    // El estado ("Available", "Reserved", "Sold") para el color de la butaca en el mapa 
    public string Status { get; set; } = string.Empty;

    // Agregamos la versión para que el front la envie en el comando de rserva 
    public int Version { get; set; }
}