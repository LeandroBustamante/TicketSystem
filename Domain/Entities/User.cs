using System;

namespace Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public List<Reservation> Reservations { get; set; } = new();
    public List<Audit_Log> Audit_Logs { get; set; } = new();
}