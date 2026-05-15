namespace Application.UseCases.Seats.Commands.ReserveSeat;

public interface IReserveSeatHandler
{
    Task<ReserveSeatResult> HandleAsync(ReserveSeatCommand command);
}