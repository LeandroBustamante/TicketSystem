namespace Application.UseCases.Seats.Commands.ReserveSeat;

public interface IReserveSeatHandler
{
    // Devuelve true si la reserva fue exitosa o false si falló (Ejemplo: por concurrencia)
    Task<bool> HandleAsync(ReserveSeatCommand command);
}