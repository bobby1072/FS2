using DataImporter.DataImporters;
using DataImporter.DataImporters.ModelImporters.MockModelImporters;
using DataImporter.ModelImporters;
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
                    .AddTransient<IUserImporter, MockUserImporter>()
                    .AddTransient<IDataImporter, MockDataImporter>();
            }
            return services;
        }
    }
}