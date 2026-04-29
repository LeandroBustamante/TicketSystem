using System;

// Namespace de la capa de dominio
namespace Domain.Entities;

// La clase Audit_Log registra cada movimiento del sistema 
public class Audit_Log
{
    // Identificador único del log 
    public Guid Id { get; set; }

    // ID del usuario que realizó la acción. Puede ser nulo si es un proceso de sistema 
    public int? UserId { get; set; }

    // Acción realizada (ej: RESERVE_ATTEMPT, RESERVE_SUCCESS, EXPIRED) 
    public string Action { get; set; } = string.Empty;

    // Tipo de entidad afectada (Ejemplo: Reservation, Seat)
    public string EntityType { get; set; } = string.Empty;

    // ID de la entidad afectada guardado como texto 
    public string EntityId { get; set; } = string.Empty;

    // JSON con metadatos del evento para auditoría
    public string Details { get; set; } = string.Empty;

    // Fecha y milisegundo exacto de la creación del registro 
    public DateTime CreatedAt { get; set; }

    // Propiedad de navegación para vincular el usuario al log
    public User? User { get; set; }
}