using Application.Interfaces;
using Application.UseCases.Events.Queries.GetAllEvents;
using Application.UseCases.Seats.Commands.PayReservation;
using Application.UseCases.Seats.Commands.ReserveSeat;
using Application.UseCases.Seats.Queries.GetSeatsBySectorId;
using Application.UseCases.Sectors.Queries.GetSectorsByEventId;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Presentation.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// CORS abierto para permitir que el frontend estático consuma la API. En producción debería restringirse al dominio del frontend.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Repositorios registrados como Scoped para que cada request tenga su propia instancia de contexto.
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<ISeatRepository, SeatRepository>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();

// Handlers de casos de uso siguiendo el patrón Command/Query para separar lectura de escritura.
builder.Services.AddScoped<IGetAllEventsHandler, GetAllEventsHandler>();
builder.Services.AddScoped<IGetSectorsByEventIdHandler, GetSectorsByEventIdHandler>();
builder.Services.AddScoped<IGetSeatsBySectorIdHandler, GetSeatsBySectorIdHandler>();
builder.Services.AddScoped<IReserveSeatHandler, ReserveSeatHandler>();
builder.Services.AddScoped<IPayReservationHandler, PayReservationHandler>();

// Job registrado como Hosted Service para liberar reservas vencidas en segundo plano sin intervención del usuario.
builder.Services.AddHostedService<Infrastructure.BackgroundJobs.ReservationExpiryJob>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "TicketSystem API",
        Version = "v1",
        Description = "API REST para la plataforma de venta de entradas. Gestiona eventos, sectores, butacas y reservas con control de concurrencia."
    });

    // Los comentarios XML permiten que Swagger muestre las descripciones definidas en los /// <summary> de los controllers.
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// El middleware de excepciones debe ir primero para capturar cualquier error que ocurra en los middlewares siguientes.
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();

// CORS debe ir antes de Authorization para que las preflight requests no sean rechazadas antes de llegar al pipeline de autenticación.
app.UseCors();

app.UseAuthorization();
app.MapControllers();

app.Run();