using System;

namespace Domain.Entities;

// La clase Reservation gestiona el bloqueo temporal de las butacas 
public class Reservation
{
    // Identificador único de la reserva (Primary Key) 
    public Guid Id { get; set; }

    // ID del usuario que realiza la reserva (Clave Foránea) 
    public int UserId { get; set; }

    // ID de la butaca reservada (Clave Foránea) 
    public Guid SeatId { get; set; }

    // Estado de la reserva: "Pending", "Paid" o "Expired" 
    public string Status { get; set; } = "Pending";

    // Fecha y hora exacta en la que se creó la reserva 
    public DateTime ReservedAt { get; set; }

    // Fecha y hora en la que vence el bloqueo (máximo 5 minutos) 
    public DateTime ExpiresAt { get; set; }

    // Propiedades de navegación para relacionar los objetos en C#
    public User? User { get; set; }
    public Seat? Seat { get; set; }
}