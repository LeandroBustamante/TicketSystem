using Domain.Entities;

namespace Application.Interfaces;

public interface ISeatRepository
{
    Task<IEnumerable<Seat>> GetBySectorIdAsync(int sectorId);
    Task<Seat?> GetByIdAsync(Guid id);

    // Metodos para preparar los cambios en memoria
    void Update(Seat seat);
    void AddReservation(Reservation reservation);
    void AddAuditLog(Audit_Log log);

    // Persiste toda la unidad de trabajo en una sola transacción
    Task<int> SaveChangesAsync();
}