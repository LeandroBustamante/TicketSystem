namespace Domain.Entities;

public class Event
{
    // IDENTIFICADOR ÚNICO (PRIMARY KEY) DE TIPO ENTERO 
    public int Id { get; set; }

    // NOMBRE DEL EVENTO 
    public string Name { get; set; } = string.Empty;

    // FECHA Y HORA PROGRAMADA 
    public DateTime EventDate { get; set; }

    // NOMBRE DEL RECINTO O LUGAR 
    public string Venue { get; set; } = string.Empty;

    // ESTADO DEL EVENTO 
    public string Status { get; set; } = string.Empty;

    // PROPIEDAD DE NAVEGACIÓN: UN EVENTO TIENE VARIOS SECTORES
    public List<Sector> Sectors { get; set; } = new();
}