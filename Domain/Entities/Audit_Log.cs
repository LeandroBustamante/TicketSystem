using System;

namespace Domain.Entities;

// Registro inmutable de cada acción relevante del sistema para trazabilidad completa.
public class Audit_Log
{
    public Guid Id { get; set; }

    // Nulo cuando la acción la ejecuta el sistema (ej: Background Job liberando reservas vencidas).
    public int? UserId { get; set; }

    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public User? User { get; set; }
}