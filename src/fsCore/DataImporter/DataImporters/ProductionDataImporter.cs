namespace DataImporter.ModelImporters
{
    internal class ProductionDataImporter : IDataImporter
    {
        public ProductionDataImporter() { }
        public Task Import()
        {
            return Task.CompletedTask;
        }
    }
}