using Application.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.SignalWire;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

/// <summary>
/// Extension methods for registering Infrastructure services
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Register database connection factory
        services.AddSingleton<DatabaseConnectionFactory>();

        // Register repositories
        services.AddScoped<ICallLogRepository, CallLogRepository>();
        services.AddSingleton<ICallSessionRepository, CallSessionRepository>();

        // Register SignalWire services
        services.AddSingleton<ISWMLGenerator, SWMLGenerator>();

        // Register memory cache
        services.AddMemoryCache();

        return services;
    }
}
