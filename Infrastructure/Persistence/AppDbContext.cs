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
            // EL CAMPO VERSION SE USA PARA OPTIMISTIC LOCKING 
            // USAMOS ISCONCURRENCYTOKEN PORQUE ES UN INT Y NO UN ROWVERSION DE SQL SERVER
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
            // USERID ES OPCIONAL SI LA ACCIÓN ES DEL SISTEMA 
            entity.HasOne(a => a.User)
                  .WithMany(u => u.Audit_Logs)
                  .HasForeignKey(a => a.UserId)
                  .IsRequired(false);
        });



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
            EventDate = new DateTime(2026, 05, 20), // USAR FECHA FIJA PARA EVITAR CAMBIOS EN MIGRACIONES
            Venue = "Estadio Unaj",
            Status = "Active"
        });

        var sectorAId = 1;
        var sectorBId = 2;
        modelBuilder.Entity<Sector>().HasData(
            new Sector { Id = sectorAId, EventId = eventId, Name = "Platea Alta", Price = 5000, Capacity = 50 },
            new Sector { Id = sectorBId, EventId = eventId, Name = "Campo General", Price = 3000, Capacity = 50 }
        );

        // SEEDING DE BUTACAS: USAR IDS FIJOS PARA QUE LA MIGRACIÓN NO FALLE SIEMPRE
        var seats = new List<Seat>();

        // SECTOR A
        for (int i = 1; i <= 50; i++)
        {
            seats.Add(new Seat
            {
                // GENERAR GUID ESTÁTICO BASADO EN EL ÍNDICE PARA EVITAR NUEVOS IDS CADA VEZ QUE SE COMPILA
                Id = new Guid($"00000000-0000-0000-0000-0000000000{i:D2}"),
                SectorId = sectorAId,
                RowIdentifier = "A",
                SeatNumber = i,
                Status = "Available",
                Version = 1
            });
        }

        // SECTOR B
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