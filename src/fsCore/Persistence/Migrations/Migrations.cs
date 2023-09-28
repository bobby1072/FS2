using EvolveDb;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Persistence.Migrations
{
    public class DatabaseMigrations: IMigrator
    {
        private readonly ILogger<DatabaseMigrations> _logger;
        private readonly string _connectionString;
        public DatabaseMigrations(ILogger<DatabaseMigrations> logger, string connectionUrl)
        {
            _logger = logger;
            _connectionString = connectionUrl;
        }
        public async Task Migrate()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var evolve = new Evolve(connection, msg => _logger.LogInformation(msg))
            {
                EmbeddedResourceAssemblies = new[] { typeof(DatabaseMigrations).Assembly },
                EnableClusterMode = true,
                IsEraseDisabled= true,
                MetadataTableName = "migrations_changelog",
                OutOfOrder = true
            };
            evolve.Migrate();
        }
    }
}
