using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces;

public interface IReservationRepository
{
    Task<Domain.Entities.Reservation?> GetByIdWithSeatAsync(Guid reservationId);
    void UpdateReservation(Domain.Entities.Reservation reservation);
    void UpdateSeat(Domain.Entities.Seat seat);
    void AddAuditLog(Domain.Entities.Audit_Log log);
    Task SaveChangesAsync();
}
