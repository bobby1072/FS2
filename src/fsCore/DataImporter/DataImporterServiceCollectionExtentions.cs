using DataImporter.ModelImporters;
using DataImporter.ModelImporters.MockModelImporters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataImporter
{
    public static class DataImporterServiceCollectionExtensions
    {
        public static IServiceCollection AddDataImporter(this IServiceCollection services, IConfiguration configuration)
        {
            var environment = configuration["EnvironmentName"] ?? throw new InvalidOperationException("Missing env var");
            var useJsonFileImport = bool.Parse(configuration["DataImporter:UseJsonFile"] ?? throw new InvalidOperationException("Missing env var"));
            if (environment == "Development" && !useJsonFileImport)
            {
                services
                    .AddScoped<IUserImporter, MockUserImporter>()
                    .AddScoped<IGroupImporter, MockGroupImporter>()
                    .AddScoped<IGroupPositionImporter, MockGroupPositionImporter>()
                    .AddScoped<IGroupMemberImporter, MockGroupMemberImporter>()
                    .AddScoped<IDataImporter, DataImporter>();
            }
            else if (!useJsonFileImport)
            {
                services
                    .AddScoped<IDataImporter, ProductionDataImporter>();
            }
            return services;
        }
    }
}