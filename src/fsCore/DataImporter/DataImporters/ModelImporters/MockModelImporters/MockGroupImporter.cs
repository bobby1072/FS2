using Common.DbInterfaces.Repository;

namespace DataImporter.ModelImporters
{
    public class MockGroupImporter : IGroupImporter
    {
        private IGroupRepository _groupRepository;
        public MockGroupImporter(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }
    }
}