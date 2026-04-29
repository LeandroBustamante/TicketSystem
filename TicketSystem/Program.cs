// 1. Agregamos los usings necesarios para encontrar el contexto y EF Core
using Application.Interfaces;
using Application.UseCases.Events.Queries.GetAllEvents;
using Application.UseCases.Seats.Commands.ReserveSeat;
using Application.UseCases.Seats.Queries.GetSeatsBySectorId;
using Application.UseCases.Sectors.Queries.GetSectorsByEventId;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


// 2. Registramos el DbContext en el contenedor de dependencias (Dependency Injection)
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


// CONFIGURACIÓN DE CORS 
// PERMITE QUE EL FRONTEND INDEPENDIENTE SE CONECTE A ESTA API
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


// REGISTRO DE REPOSITORIOS 
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<ISeatRepository, SeatRepository>();


// REGISTRO DE HANDLERS (CASOS DE USO) 
builder.Services.AddScoped<IGetAllEventsHandler, GetAllEventsHandler>();
builder.Services.AddScoped<IGetSectorsByEventIdHandler, GetSectorsByEventIdHandler>();
builder.Services.AddScoped<IGetSeatsBySectorIdHandler, GetSeatsBySectorIdHandler>();
builder.Services.AddScoped<IReserveSeatHandler, ReserveSeatHandler>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ACTIVACIÓN DE CORS (NOTA: DEBE IR DESPUÉS DE HTTPS REDIRECTION Y ANTES DE AUTHORIZATION)
app.UseCors();

app.UseAuthorization();
app.MapControllers();

app.Run();