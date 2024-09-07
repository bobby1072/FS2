using Common.Misc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using Persistence.EntityFramework;
using Persistence.EntityFramework.Repository.Abstract;
using Persistence.EntityFramework.Repository.Concrete;
using Persistence.Migration;

namespace Persistence
{
    public static class PersistenceServiceCollectionExtensions
    {
        public static IServiceCollection AddSqlPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var migrationStartVersion = configuration.GetSection("Migration").GetSection("StartVersion")?.Value;
            if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(migrationStartVersion))
            {
                throw new Exception(ErrorConstants.MissingEnvVars);
            }
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString);

            services
                .AddSingleton<IMigrator, DatabaseMigrations>(sp => new DatabaseMigrations(sp.GetRequiredService<ILoggerFactory>().CreateLogger<DatabaseMigrations>(), connectionString, migrationStartVersion));

            services
                .AddSingleton<IWorldFishRepository, WorldFishRepository>()
                .AddSingleton<IUserRepository, UserRepository>()
                .AddSingleton<IGroupRepository, GroupRepository>()
                .AddSingleton<IGroupPositionRepository, GroupPositionRepository>()
                .AddSingleton<IGroupMemberRepository, GroupMemberRepository>()
                .AddSingleton<IGroupCatchRepository, GroupCatchRepository>()
                .AddSingleton<IGroupCatchCommentRepository, GroupCatchCommentRepository>()
                .AddSingleton<IActiveLiveMatchCatchRepository, ActiveLiveMatchCatchRepository>()
                .AddSingleton<IActiveLiveMatchRepository, ActiveLiveMatchRepository>()
                .AddSingleton<IActiveLiveMatchParticipantRepository, ActiveLiveMatchParticipantRepository>();

            services
                .AddHostedService<DatabaseMigratorHostedService>()
                .AddSingleton<DatabaseMigratorHealthCheck>()
                .AddHealthChecks()
                .AddCheck<DatabaseMigratorHealthCheck>(DatabaseMigratorHealthCheck.Name, tags: [HealthCheckConstants.ReadyTag]);

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