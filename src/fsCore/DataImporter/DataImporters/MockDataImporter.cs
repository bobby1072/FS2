using Common.Dbinterfaces.Repository;

namespace DataImporter.DataImporters
{
    public class MockDataImporter : IDataImporter
    {
        private readonly IUserRepository _userRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupMemberRepository _groupMemberRepository;
        private readonly IGroupPositionRepository _groupPositionRepository;
        private readonly IGroupCatchRepository _groupCatchRepo;
        public MockDataImporter(IUserRepository userRepository, IGroupRepository groupRepository, IGroupMemberRepository groupMemberRepository, IGroupPositionRepository groupPositionRepository, IGroupCatchRepository groupCatchRepo)
        {
            _userRepository = userRepository;
            _groupRepository = groupRepository;
            _groupMemberRepository = groupMemberRepository;
            _groupPositionRepository = groupPositionRepository;
            _groupCatchRepo = groupCatchRepo;
        }
        public async Task Import()
        {

        }
    }
}