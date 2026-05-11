using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Seats.Commands.PayReservation;

public class PayReservationCommand
{
    public Guid ReservationId { get; set; }
    public int UserId { get; set; }
}
