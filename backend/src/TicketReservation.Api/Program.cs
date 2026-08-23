using Microsoft.EntityFrameworkCore;
using TicketReservation.Api.Endpoints;
using TicketReservation.Api.Middleware;
using TicketReservation.Application;
using TicketReservation.Infrastructure;
using TicketReservation.Infrastructure.Persistence;
using TicketReservation.Infrastructure.Persistence.Seed;

var builder = WebApplication.CreateBuilder(args);

// 1. Obtener Cadena de Conexión
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no está configurada.");

// 2. Obtener Orígenes de CORS desde appsettings.json
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? new[] { "http://localhost:5173" };

// 3. Registrar Servicios de la Aplicación e Infraestructura
builder.Services.AddApplication();
builder.Services.AddInfrastructure(connectionString);

// 4. Configurar Middleware de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials(); // Permitir credenciales/cookies si el frontend las requiere
        }
    });
});

// 5. Configuración de Manejo de Excepciones y Swagger
builder.Services.AddExceptionHandler<DomainExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 6. Pipeline de Middleware HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
        c.RoutePrefix = string.Empty; // Hace que Swagger cargue directamente en la raíz (/)
    });
}

app.UseExceptionHandler();

// Aplicar CORS antes de mapear los endpoints
app.UseCors("Frontend");

// 7. Mapeo de Endpoints
app.MapEventEndpoints();
app.MapReservationEndpoints();

// 8. Migraciones y Seed Data
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
    await DbSeeder.SeedAsync(dbContext);
}

app.Run();