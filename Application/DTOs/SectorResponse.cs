namespace Application.DTOs;

// DTO para mostrar los sectores cuando el usuario selecciona un evento 
public class SectorResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Capacity { get; set; }
}
