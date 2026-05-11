using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.UseCases.Seats.Commands.PayReservation;

namespace Application.Interfaces;

public interface IPayReservationHandler
{
    Task<PayReservationResult> HandleAsync(PayReservationCommand command);
}
