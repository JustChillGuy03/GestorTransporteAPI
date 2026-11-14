using GestorTransporteAPI.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Controladores de la API
builder.Services.AddControllers();

// Documento OpenAPI (disponible en /openapi/v1.json)
builder.Services.AddOpenApi();

// Contexto de base de datos: SQL Server con la cadena "DefaultConnection" de appsettings.json
builder.Services.AddDbContext<TransporteContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Interfaz gráfica para probar los endpoints en /swagger
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "GestorTransporteAPI v1");
    });

    // Aplica las migraciones pendientes al iniciar (crea la base de datos si no existe)
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<TransporteContext>();
    context.Database.Migrate();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
