using Common.DbInterfaces.Repository;
using DataImporter.ModelImporters;

namespace DataImporter.DataImporters
{
    internal class MockDataImporter : IDataImporter
    {
        private readonly IUserImporter _userImporter;
        public MockDataImporter(IUserImporter userImporter)
        {
            _userImporter = userImporter;
        }
        public async Task Import()
        {
            await _userImporter.Import();
        }
    }
}