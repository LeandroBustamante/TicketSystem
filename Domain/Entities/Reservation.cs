using System;

namespace Domain.Entities;

// Representa el bloqueo temporal de una butaca mientras el usuario completa el pago.
public class Reservation
{
    public Guid Id { get; set; }
    public int UserId { get; set; }
    public Guid SeatId { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime ReservedAt { get; set; }

    // Límite de tiempo para completar el pago. El Background Job libera la butaca si este tiempo vence.
    public DateTime ExpiresAt { get; set; }

    public User? User { get; set; }
    public Seat? Seat { get; set; }
}