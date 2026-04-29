using System;

namespace Domain.Entities;

// La clase Seat representa una butaca específica dentro de un sector 
public class Seat
{
    // Identificador único de tipo UUID (Guid en C#) 
    public Guid Id { get; set; }

    // Relación con el Sector: Clave foránea que indica a qué área pertenece la butaca 
    public int SectorId { get; set; }

    // Identificador de la fila (ej: "A", "B", "Fila 10") 
    public string RowIdentifier { get; set; } = string.Empty;

    // Número físico del asiento en esa fila 
    public int SeatNumber { get; set; }

    // Estado actual: "Available", "Reserved" o "Sold" 
    public string Status { get; set; } = "Available";

    // Campo fundamental para el control de concurrencia (Optimistic Locking) 
    // EF Core lo usa para asegurar que dos personas no pisen la misma butaca al mismo tiempo 
    public int Version { get; set; }

    // Propiedad de navegación para acceder a los datos del Sector desde la butaca
    public Sector? Sector { get; set; }
}