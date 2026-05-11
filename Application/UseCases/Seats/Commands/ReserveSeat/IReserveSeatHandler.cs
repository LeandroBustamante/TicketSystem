namespace Application.UseCases.Seats.Commands.ReserveSeat;

public interface IReserveSeatHandler
{
    // Devuelve true si la reserva fue exitosa o false si falló (Ejemplo: por concurrencia)
    //Task<bool> HandleAsync(ReserveSeatCommand command);

    // Devuelve el ID de la reserva si fue exitosa, null si falló (ej: por concurrencia)
    Task<Guid?> HandleAsync(ReserveSeatCommand command);
}