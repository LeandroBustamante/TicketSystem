namespace Domain.Entities;

public class User
{
    // IDENTIFICADOR ÚNICO NUMÉRICO (PK) 
    public int Id { get; set; }

    // NOMBRE COMPLETO DEL USUARIO 
    public string Name { get; set; } = string.Empty;

    // DIRECCIÓN DE CORREO ELECTRÓNICO 
    public string Email { get; set; } = string.Empty;

    // HASH DE LA CONTRASEÑA 
    public string PasswordHash { get; set; } = string.Empty;

    // RELACIÓN "REALIZA": UN USUARIO TIENE MUCHAS RESERVAS 
    public List<Reservation> Reservations { get; set; } = new();

    // RELACIÓN "GENERA": UN USUARIO TIENE MUCHOS LOGS DE AUDITORÍA 
    public List<Audit_Log> Audit_Logs { get; set; } = new();
}