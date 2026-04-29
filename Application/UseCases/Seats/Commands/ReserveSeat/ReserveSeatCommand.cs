namespace Application.UseCases.Seats.Commands.ReserveSeat;

public class ReserveSeatCommand
{
    public Guid SeatId { get; set; }
    public int UserId { get; set; }
    // Se agrega la versión para validar concurrencia desde el frontend
    public int Version { get; set; }

    public ReserveSeatCommand(Guid seatId, int userId, int version)
    {
        SeatId = seatId;
        UserId = userId;
        Version = version;
    }
}