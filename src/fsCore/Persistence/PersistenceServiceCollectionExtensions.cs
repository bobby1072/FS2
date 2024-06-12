using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Common;
using Npgsql;
using Microsoft.Extensions.Logging;
using Persistence.Migrations;
using Persistence.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Common.DbInterfaces.Repository;
using Persistence.EntityFramework.Repository;
using Common.DbInterfaces.ErrorHandlers;

namespace Persistence
{
    public static class PersistenceServiceCollectionExtensions
    {
        public static IServiceCollection AddSqlPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var migrationStartVersion = configuration["Migration:StartVersion"];
            if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(migrationStartVersion))
            {
                throw new Exception(ErrorConstants.MissingEnvVars);
            }
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString);

            services
                .AddSingleton<IMigrator, DatabaseMigrations>(sp => new DatabaseMigrations(sp.GetRequiredService<ILoggerFactory>().CreateLogger<DatabaseMigrations>(), connectionString, migrationStartVersion));

            services
                .AddSingleton<INpgExceptionHandler, NpgExceptionHandler>()
                .AddSingleton<IWorldFishRepository, WorldFishRepository>()
                .AddSingleton<IUserRepository, UserRepository>()
                .AddSingleton<IGroupRepository, GroupRepository>()
                .AddSingleton<IGroupPositionRepository, GroupPositionRepository>()
                .AddSingleton<IGroupMemberRepository, GroupMemberRepository>()
                .AddSingleton<IGroupCatchRepository, GroupCatchRepository>()
                .AddSingleton<IGroupCatchCommentRepository, GroupCatchCommentRepository>();

            services
                .AddHostedService<DatabaseMigratorHostedService>()
                .AddSingleton<DatabaseMigratorHealthCheck>()
                .AddHealthChecks()
                .AddCheck<DatabaseMigratorHealthCheck>(DatabaseMigratorHealthCheck.Name, tags: new[] { HealthCheckConstants.ReadyTag });

            services
                .AddPooledDbContextFactory<FsContext>(
                    options =>
                    options
                        .UseSnakeCaseNamingConvention()
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

            return services;
        }
    }
}