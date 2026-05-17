using System;

namespace Domain.Entities;

public class Seat
{
    public Guid Id { get; set; }
    public int SectorId { get; set; }
    public string RowIdentifier { get; set; } = string.Empty;
    public int SeatNumber { get; set; }
    public string Status { get; set; } = "Available";

    // Version se incrementa en cada modificación. EF Core lo usa en el WHERE del UPDATE para detectar si otro usuario modificó el registro antes — Optimistic Locking.
    public int Version { get; set; }

    public Sector? Sector { get; set; }
}