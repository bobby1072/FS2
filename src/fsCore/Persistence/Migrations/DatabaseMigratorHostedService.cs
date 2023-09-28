﻿using Microsoft.Extensions.Hosting;

namespace Persistence.Migrations
{
    public class DatabaseMigratorHostedService: IHostedService
    {
        private readonly IEnumerable<IMigrator> _databaseMigrators;
        private readonly DatabaseMigratorHealthCheck _databaseMigratorHealthCheck;
        public DatabaseMigratorHostedService(IEnumerable<IMigrator>? databaseMigrators, DatabaseMigratorHealthCheck databaseMigratorHealthCheck)
        {
            _databaseMigrators = databaseMigrators ?? new List<IMigrator>();
            _databaseMigratorHealthCheck = databaseMigratorHealthCheck;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var migrator in _databaseMigrators)
            {
                await migrator.Migrate();
            }
            _databaseMigratorHealthCheck.MigrationCompleted = true;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
