using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Common;
using Npgsql;
using Microsoft.Extensions.Logging;
using Persistence.Migrations;

namespace Persistence
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSqlPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception(ErrorConstants.MissingEnvVars);
            }
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString);
            
            services
                .AddSingleton<IMigrator, DatabaseMigrations>(sp => new DatabaseMigrations(sp.GetRequiredService<ILoggerFactory>().CreateLogger<DatabaseMigrations>(), connectionString));

            services
                .AddHostedService<DatabaseMigratorHostedService>()
                .AddSingleton<DatabaseMigratorHealthCheck>()
                .AddHealthChecks()
                .AddCheck<DatabaseMigratorHealthCheck>(DatabaseMigratorHealthCheck.Name, tags: new[] { HealthCheckConstants.ReadyTag });

            return services;
        }
    }
}