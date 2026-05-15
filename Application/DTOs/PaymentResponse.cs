using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs;

// DTO de salida para la confirmación de un pago
public class PaymentResponse
{
    public string Message { get; set; } = string.Empty;
}
