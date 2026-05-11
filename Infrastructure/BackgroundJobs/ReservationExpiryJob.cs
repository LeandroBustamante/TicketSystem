using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.BackgroundJobs;

public class ReservationExpiryJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ReservationExpiryJob> _logger;

    // Se ejecuta cada 1 minuto
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(1);

    public ReservationExpiryJob(IServiceScopeFactory scopeFactory, ILogger<ReservationExpiryJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ReservationExpiryJob iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessExpiredReservationsAsync();
            await Task.Delay(_interval, stoppingToken);
        }
    }

    private async Task ProcessExpiredReservationsAsync()
    {
        // Creamos un scope porque AppDbContext es Scoped y el job es Singleton
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var now = DateTime.Now;

        // Buscamos reservas pendientes que ya vencieron
        var expiredReservations = await context.Reservations
            .Where(r => r.Status == "Pending" && r.ExpiresAt < now)
            .Include(r => r.Seat)
            .ToListAsync();

        if (!expiredReservations.Any())
            return;

        foreach (var reservation in expiredReservations)
        {
            // Guardamos si el Seat es null para no procesar reservas huérfanas
            if (reservation.Seat == null)
            {
                _logger.LogWarning($"Reserva {reservation.Id} no tiene butaca asociada. Se omite.");
                continue;
            }

            // Marcamos la reserva como expirada
            reservation.Status = "Expired";

            // Liberamos la butaca
            reservation.Seat.Status = "Available";
            reservation.Seat.Version++;

            // Registramos en AuditLog
            context.Audit_Logs.Add(new Domain.Entities.Audit_Log
            {
                Id = Guid.NewGuid(),
                UserId = null, // Es un proceso del sistema
                Action = "EXPIRED",
                EntityType = "Reservation",
                EntityId = reservation.Id.ToString(),
                Details = $"RESERVA EXPIRADA: BUTACA {reservation.Seat.SeatNumber} LIBERADA AUTOMÁTICAMENTE",
                CreatedAt = DateTime.Now
            });

            _logger.LogInformation($"Reserva {reservation.Id} expirada. Butaca {reservation.Seat.SeatNumber} liberada.");
        }

        await context.SaveChangesAsync();
    }
}