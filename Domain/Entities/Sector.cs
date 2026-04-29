namespace Domain.Entities;

public class Sector
{
    // IDENTIFICADOR ÚNICO (PRIMARY KEY) 
    public int Id { get; set; }

    // RELACIÓN CON EL EVENTO: CLAVE FORÁNEA (FK)
    public int EventId { get; set; }

    // NOMBRE DEL SECTOR 
    public string Name { get; set; } = string.Empty;

    // PRECIO DE LA ENTRADA 
    public decimal Price { get; set; }

    // CAPACIDAD TOTAL DEL SECTOR 
    public int Capacity { get; set; }

    // PROPIEDAD DE NAVEGACIÓN HACIA EL EVENTO 
    public Event? Event { get; set; }

    // RELACIÓN "CONTIENE": UN SECTOR TIENE MUCHAS BUTACAS 
    public List<Seat> Seats { get; set; } = new();
}