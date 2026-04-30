using Domain.Entities;

namespace Application.Interfaces;

public interface ISeatRepository
{
    Task<IEnumerable<Seat>> GetBySectorIdAsync(int sectorId);
    Task<Seat?> GetByIdAsync(Guid id);

    // Nuevo método para validar existencia del sector
    Task<bool> SectorExistsAsync(int sectorId);

    void Update(Seat seat);
    void AddReservation(Reservation reservation);
    void AddAuditLog(Audit_Log log);
    Task<int> SaveChangesAsync();
}