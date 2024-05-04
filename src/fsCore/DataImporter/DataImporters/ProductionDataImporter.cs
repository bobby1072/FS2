namespace DataImporter.ModelImporters
{
    internal class ProductionDataImporter : IDataImporter
    {
        public Task Import()
        {
            return Task.CompletedTask;
        }
    }
}