using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs;

// DTO de salida para la creación de una reserva
public class ReservationResponse
{
    public Guid ReservationId { get; set; }
    public string Message { get; set; } = string.Empty;
}
