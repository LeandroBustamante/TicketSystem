using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Seats.Commands.ReserveSeat;

public class ReserveSeatResult
{
    public bool Success { get; set; }
    public Guid? ReservationId { get; set; }
    public string ErrorCode { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
