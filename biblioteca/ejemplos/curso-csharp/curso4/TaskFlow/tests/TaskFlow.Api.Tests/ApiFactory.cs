using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Api.Tests;

/// <summary>
/// Arranca la API REAL en memoria (sin red ni puertos) contra una base de pruebas distinta a la de desarrollo.
/// </summary>
public sealed class ApiFactory : WebApplicationFactory<Program>
{
    public ApiFactory()
    {
        // Antes de usar la app: crear el esquema con las migraciones y dejar las tablas vacías.
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TaskFlowDbContext>();
        db.Database.Migrate();
        db.Database.ExecuteSqlRaw("TRUNCATE tasks, users RESTART IDENTITY");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");   // distinto de "Development": no carga los user-secrets de desarrollo
        builder.UseSetting("ConnectionStrings:TaskFlow", "Host=localhost;Database=taskflow_test;Username=taskflow;Password=taskflow_dev");
        builder.UseSetting("Jwt:Key", "clave-solo-para-pruebas-0123456789-abcdef");
    }
}
