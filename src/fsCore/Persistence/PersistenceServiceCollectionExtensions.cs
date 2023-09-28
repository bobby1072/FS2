using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Common;
using Npgsql;
using Microsoft.Extensions.Logging;
using Persistence.Migrations;
using Persistence.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public static class PersistenceServiceCollectionExtensions
    {
        public static IServiceCollection AddSqlPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var migrationStartVersion = configuration["Migration:StartVersion"];
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception(ErrorConstants.MissingEnvVars);
            }
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString);

            services
                .AddSingleton<IMigrator, DatabaseMigrations>(sp => new DatabaseMigrations(sp.GetRequiredService<ILoggerFactory>().CreateLogger<DatabaseMigrations>(), connectionString, migrationStartVersion));

            services
                .AddHostedService<DatabaseMigratorHostedService>()
                .AddSingleton<DatabaseMigratorHealthCheck>()
                .AddHealthChecks()
                .AddCheck<DatabaseMigratorHealthCheck>(DatabaseMigratorHealthCheck.Name, tags: new[] { HealthCheckConstants.ReadyTag });

            services
                .AddPooledDbContextFactory<FsContext>(
                    options =>
                    options
                        .UseCamelCaseNamingConvention()
                        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                        .UseNpgsql(
                            connectionStringBuilder.ConnectionString,
                            options =>
                            {
                                options.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
                            }
                        )
                )
                .AddHealthChecks();

            services
                .AddScoped(sp => sp.GetRequiredService<IDbContextFactory<FsContext>>().CreateDbContext());

            return services;
        }
    }
}