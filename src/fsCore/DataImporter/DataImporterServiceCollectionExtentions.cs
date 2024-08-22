using DataImporter.DataImporters;
using DataImporter.DataImporters.ModelImporters.Abstract;
using DataImporter.DataImporters.ModelImporters.Concrete.Mock;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DataImporter
{
    public static class DataImporterServiceCollectionExtensions
    {
        public static IServiceCollection AddDataImporter(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            var isDev = environment.IsDevelopment();
            var useJsonFileImport = bool.Parse(configuration.GetSection("DataImporter").GetSection("UseJsonFile")?.Value ?? "false");
            if (isDev && !useJsonFileImport)
            {
                services
                    .AddScoped<IUserImporter, MockUserImporter>()
                    .AddScoped<IGroupImporter, MockGroupImporter>()
                    .AddScoped<IGroupPositionImporter, MockGroupPositionImporter>()
                    .AddScoped<IGroupMemberImporter, MockGroupMemberImporter>()
                    .AddScoped<IGroupCatchImporter, MockGroupCatchImporter>()
                    .AddScoped<IDataImporter, GenericDataImporter>();
            }
            else if (!useJsonFileImport)
            {
                services
                    .AddScoped<IDataImporter, ProductionDataImporter>();
            }
            else
            {
                services
                    .AddScoped<IDataImporter, JsonDataImporter>();
            }
            return services;
        }
    }
}