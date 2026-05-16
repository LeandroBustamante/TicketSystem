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

        // MAPEO DE TABLAS A MAYÚSCULAS
        modelBuilder.Entity<Event>().ToTable("EVENT");
        modelBuilder.Entity<Sector>().ToTable("SECTOR");
        modelBuilder.Entity<Seat>().ToTable("SEAT");
        modelBuilder.Entity<User>().ToTable("USER");
        modelBuilder.Entity<Reservation>().ToTable("RESERVATION");
        modelBuilder.Entity<Audit_Log>().ToTable("AUDIT_LOG");

        // CONFIGURACIÓN DE SEAT (BUTACA) 
        modelBuilder.Entity<Seat>(entity =>
        {
            // El campo Version se usa para Optimistic Locking.
            // IsConcurrencyToken indica a EF Core que debe incluirlo en el WHERE al actualizar,
            // garantizando que dos usuarios no pisen el mismo registro al mismo tiempo.
            entity.Property(s => s.Version).IsConcurrencyToken();

            // RELACIÓN: UN SECTOR TIENE MUCHAS BUTACAS 
            entity.HasOne(s => s.Sector)
                  .WithMany(sector => sector.Seats)
                  .HasForeignKey(s => s.SectorId);
        });

        // CONFIGURACIÓN DE SECTOR
        modelBuilder.Entity<Sector>(entity =>
        {
            // PRECISIÓN PARA EL PRECIO DECIMAL 
            entity.Property(s => s.Price).HasPrecision(18, 2);

            // RELACIÓN: UN EVENTO TIENE MUCHOS SECTORES
            entity.HasOne(s => s.Event)
                  .WithMany(e => e.Sectors)
                  .HasForeignKey(s => s.EventId);
        });

        // CONFIGURACIÓN DE RESERVATION 
        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasOne(r => r.User).WithMany(u => u.Reservations).HasForeignKey(r => r.UserId);
            entity.HasOne(r => r.Seat).WithMany().HasForeignKey(r => r.SeatId);
        });

        // CONFIGURACIÓN DE AUDIT_LOG 
        modelBuilder.Entity<Audit_Log>(entity =>
        {
            // UserId es opcional cuando la acción la ejecuta el sistema (ej: Background Job)
            entity.HasOne(a => a.User)
                  .WithMany(u => u.Audit_Logs)
                  .HasForeignKey(a => a.UserId)
                  .IsRequired(false);
        });

        // ÍNDICES PARA MEJORAR PERFORMANCE EN CONSULTAS FRECUENTES

        // Índice en Seat por SectorId — se consulta frecuentemente para renderizar el mapa
        modelBuilder.Entity<Seat>()
            .HasIndex(s => s.SectorId)
            .HasDatabaseName("IX_Seat_SectorId");

        // Índice en Seat por Status — se filtra por estado en queries de disponibilidad
        modelBuilder.Entity<Seat>()
            .HasIndex(s => s.Status)
            .HasDatabaseName("IX_Seat_Status");

        // Índice en Reservation por SeatId — se consulta para verificar reservas activas
        modelBuilder.Entity<Reservation>()
            .HasIndex(r => r.SeatId)
            .HasDatabaseName("IX_Reservation_SeatId");

        // Índice en Reservation por Status y ExpiresAt — el background job lo usa para buscar reservas vencidas
        modelBuilder.Entity<Reservation>()
            .HasIndex(r => new { r.Status, r.ExpiresAt })
            .HasDatabaseName("IX_Reservation_Status_ExpiresAt");

        // Índice en Audit_Log por EntityId — se consulta para ver el historial de una entidad
        modelBuilder.Entity<Audit_Log>()
            .HasIndex(a => a.EntityId)
            .HasDatabaseName("IX_AuditLog_EntityId");

        // Índice en Sector por EventId — se consulta para listar sectores de un evento
        modelBuilder.Entity<Sector>()
            .HasIndex(s => s.EventId)
            .HasDatabaseName("IX_Sector_EventId");

        // PRECARGA DE DATOS (SEEDING) 
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

        // SECTOR A — 50 butacas fila A
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

        // SECTOR B — 50 butacas fila B
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