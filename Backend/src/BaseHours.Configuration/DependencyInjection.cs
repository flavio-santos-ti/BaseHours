using BaseHours.Application.Interfaces;
using BaseHours.Application.Services;
using BaseHours.Domain.Interfaces;
using BaseHours.Infrastructure.Persistence;
using BaseHours.Infrastructure.Persistence.Repositories;
using FDS.DbLogger.PostgreSQL.Published;
using FDS.RequestTracking.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BaseHours.Configuration
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Registers the application and infrastructure layers, ensuring proper separation of concerns.
        /// </summary>
        public static IServiceCollection AddApplicationConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddInfrastructureLayer(configuration)
                .AddApplicationLayer()
                .AddRequestTracking();
        }

        private static IServiceCollection AddApplicationLayer(this IServiceCollection services)
        {
            services.AddScoped<IClientService>(provider =>
            {
                var clientRepository = provider.GetRequiredService<IClientRepository>();
                var auditLogServiceFactory = provider.GetRequiredService<Func<string, IAuditLogService>>();
                var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>(); // ⬅ Adicionando

                return new ClientService(clientRepository, auditLogServiceFactory("Client"), httpContextAccessor);
            });

            return services;
        }

        private static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
        {
            // Configuration for timestamp compatibility in PostgreSQL
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            // Registers ApplicationDbContext (MAIN DATABASE)
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // **Passando apenas a connection string para a Biblioteca**
            var auditLogConnectionString = configuration.GetConnectionString("AuditLogConnection")
                                           ?? "Host=localhost;Database=AuditLogDb;Username=defaultUser;Password=defaultPass";

            services.AddDbLogger(auditLogConnectionString);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Registers repositories
            services.AddScoped<IClientRepository, ClientRepository>();

            return services;
        }
    }
}
