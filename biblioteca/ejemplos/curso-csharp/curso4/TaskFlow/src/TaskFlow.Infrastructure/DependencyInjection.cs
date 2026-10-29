using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Application;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Infrastructure;

public static class DependencyInjection
{
    /// <summary>Cada capa expone un método que registra sus propios servicios; Program.cs solo los junta.</summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        string connection = config.GetConnectionString("TaskFlow")
            ?? throw new InvalidOperationException("Falta ConnectionStrings:TaskFlow (usa user-secrets o una variable de entorno).");

        services.AddDbContext<TaskFlowDbContext>(o => o.UseNpgsql(connection));
        services.AddScoped<ITaskRepository, EfTaskRepository>();   // Scoped: igual vida que el DbContext
        return services;
    }
}
