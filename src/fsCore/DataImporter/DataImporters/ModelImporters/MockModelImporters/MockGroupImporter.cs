using Common.DbInterfaces.Repository;

namespace DataImporter.ModelImporters
{
    public class MockGroupImporter : IGroupImporter
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IUserRepository _userRepository;
        public MockGroupImporter(IGroupRepository groupRepository, IUserRepository userRepo)
        {
            _groupRepository = groupRepository;
            _userRepository = userRepo;
        }
        public async Task Import()
        {

        }
    }
}