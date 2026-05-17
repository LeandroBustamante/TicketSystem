using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Event> Events { get; set; }
    public DbSet<Sector> Sectors { get; set; }
    public DbSet<Seat> Seats { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Audit_Log> Audit_Logs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Los nombres de tabla en mayúsculas siguen la convención del proyecto para distinguirlos visualmente en SQL Server.
        modelBuilder.Entity<Event>().ToTable("EVENT");
        modelBuilder.Entity<Sector>().ToTable("SECTOR");
        modelBuilder.Entity<Seat>().ToTable("SEAT");
        modelBuilder.Entity<User>().ToTable("USER");
        modelBuilder.Entity<Reservation>().ToTable("RESERVATION");
        modelBuilder.Entity<Audit_Log>().ToTable("AUDIT_LOG");

        modelBuilder.Entity<Seat>(entity =>
        {
            // IsConcurrencyToken indica a EF Core que debe incluir Version en el WHERE al actualizar, garantizando que dos usuarios no pisen el mismo registro al mismo tiempo.
            entity.Property(s => s.Version).IsConcurrencyToken();

            entity.HasOne(s => s.Sector)
                  .WithMany(sector => sector.Seats)
                  .HasForeignKey(s => s.SectorId);
        });

        modelBuilder.Entity<Sector>(entity =>
        {
            // HasPrecision evita errores de redondeo en precios al mapear decimal a SQL Server.
            entity.Property(s => s.Price).HasPrecision(18, 2);

            entity.HasOne(s => s.Event)
                  .WithMany(e => e.Sectors)
                  .HasForeignKey(s => s.EventId);
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasOne(r => r.User).WithMany(u => u.Reservations).HasForeignKey(r => r.UserId);
            entity.HasOne(r => r.Seat).WithMany().HasForeignKey(r => r.SeatId);
        });

        modelBuilder.Entity<Audit_Log>(entity =>
        {
            // UserId es opcional porque el Background Job registra acciones del sistema sin usuario asociado.
            entity.HasOne(a => a.User)
                  .WithMany(u => u.Audit_Logs)
                  .HasForeignKey(a => a.UserId)
                  .IsRequired(false);
        });

        // Índice en Seat por SectorId — se consulta frecuentemente para renderizar el mapa de asientos.
        modelBuilder.Entity<Seat>()
            .HasIndex(s => s.SectorId)
            .HasDatabaseName("IX_Seat_SectorId");

        // Índice en Seat por Status — se filtra por estado en queries de disponibilidad.
        modelBuilder.Entity<Seat>()
            .HasIndex(s => s.Status)
            .HasDatabaseName("IX_Seat_Status");

        // Índice en Reservation por SeatId — se consulta para verificar reservas activas sobre una butaca.
        modelBuilder.Entity<Reservation>()
            .HasIndex(r => r.SeatId)
            .HasDatabaseName("IX_Reservation_SeatId");

        // Índice compuesto en Reservation — el Background Job lo usa para buscar reservas vencidas eficientemente.
        modelBuilder.Entity<Reservation>()
            .HasIndex(r => new { r.Status, r.ExpiresAt })
            .HasDatabaseName("IX_Reservation_Status_ExpiresAt");

        // Índice en Audit_Log por EntityId — permite consultar el historial de auditoría de una entidad específica.
        modelBuilder.Entity<Audit_Log>()
            .HasIndex(a => a.EntityId)
            .HasDatabaseName("IX_AuditLog_EntityId");

        // Índice en Sector por EventId — optimiza la query que lista sectores al seleccionar un evento.
        modelBuilder.Entity<Sector>()
            .HasIndex(s => s.EventId)
            .HasDatabaseName("IX_Sector_EventId");

        // Datos de precarga fijos para garantizar que la aplicación arranque con datos válidos sin intervención manual.
        var userId = 1;
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = userId,
            Name = "Usuario Test",
            Email = "test@example.com",
            PasswordHash = "hashed_password"
        });

        var eventId = 1;
        modelBuilder.Entity<Event>().HasData(new Event
        {
            Id = eventId,
            Name = "Concierto de Rock",
            EventDate = new DateTime(2026, 05, 20),
            Venue = "Estadio Unaj",
            Status = "Active"
        });

        var sectorAId = 1;
        var sectorBId = 2;
        modelBuilder.Entity<Sector>().HasData(
            new Sector { Id = sectorAId, EventId = eventId, Name = "Platea Alta", Price = 5000, Capacity = 50 },
            new Sector { Id = sectorBId, EventId = eventId, Name = "Campo General", Price = 3000, Capacity = 50 }
        );

        var seats = new List<Seat>();

        // GUIDs fijos para que la migración sea idempotente y no genere IDs distintos en cada compilación.
        for (int i = 1; i <= 50; i++)
        {
            seats.Add(new Seat
            {
                Id = new Guid($"00000000-0000-0000-0000-0000000000{i:D2}"),
                SectorId = sectorAId,
                RowIdentifier = "A",
                SeatNumber = i,
                Status = "Available",
                Version = 1
            });
        }

        for (int i = 1; i <= 50; i++)
        {
            seats.Add(new Seat
            {
                Id = new Guid($"00000000-0000-0000-0000-0000000001{i:D2}"),
                SectorId = sectorBId,
                RowIdentifier = "B",
                SeatNumber = i,
                Status = "Available",
                Version = 1
            });
        }

        modelBuilder.Entity<Seat>().HasData(seats);
    }
}