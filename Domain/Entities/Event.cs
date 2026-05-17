using System;

namespace Domain.Entities;

public class Event
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public string Venue { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public List<Sector> Sectors { get; set; } = new();
}