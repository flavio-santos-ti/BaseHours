using BaseHours.Application.Interfaces;
using BaseHours.Application.Services;
using BaseHours.Domain.Interfaces;
using BaseHours.Infrastructure.Persistence;
using BaseHours.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BaseHours.Configuration;

public static class DependencyInjection
{
    /// <summary>
    /// Registers the application and infrastructure layers, ensuring proper separation of concerns.
    /// </summary>
    public static IServiceCollection AddApplicationConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddApplicationLayer()
            .AddInfrastructureLayer(configuration);
    }

    private static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddScoped<IClientService, ClientService>(); // Registers ClientService
        return services;
    }

    private static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
    {
        // Configuration for timestamp compatibility in PostgreSQL
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        // Registers ApplicationDbContext and database connection
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Registers repositories
        services.AddScoped<IClientRepository, ClientRepository>();

        return services;
    }
}
