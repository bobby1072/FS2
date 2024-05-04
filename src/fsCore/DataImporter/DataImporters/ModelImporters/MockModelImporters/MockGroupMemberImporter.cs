using Common.DbInterfaces.Repository;
using Common.Models;
using DataImporter.MockModelBuilders;
using Microsoft.Extensions.Logging;

namespace DataImporter.ModelImporters.MockModelImporters
{
    internal class MockGroupMemberImporter : IGroupMemberImporter
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupPositionRepository _groupPositionRepository;
        private readonly IGroupMemberRepository _groupMemberRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<MockGroupMemberImporter> _logger;
        public MockGroupMemberImporter(IGroupRepository groupRepository, IGroupPositionRepository groupPositionRepository, IUserRepository userRepo, ILogger<MockGroupMemberImporter> logger, IGroupMemberRepository groupMemberRepo)
        {
            _groupMemberRepository = groupMemberRepo;
            _logger = logger;
            _groupRepository = groupRepository;
            _groupPositionRepository = groupPositionRepository;
            _userRepository = userRepo;
        }
        private static GroupMember AddGroupMembersToList(ICollection<GroupMember> groupMembers, ICollection<GroupPosition> allPositions, ICollection<User> allUsers)
        {
            var random = new Random();
            var foundUser = allUsers.ElementAt(random.Next(0, allUsers.Count));
            var foundPosition = allPositions.ElementAt(random.Next(0, allPositions.Count));
            var tempCreatedMember = MockGroupMemberBuilder.Build(foundPosition?.GroupId ?? throw new InvalidDataException("No groupId on position"), foundUser?.Id ?? throw new InvalidDataException("No id on group"), foundPosition?.Id ?? throw new InvalidDataException("No id on position"));
            if (groupMembers.FirstOrDefault(x => x?.GroupId == foundPosition.GroupId && x?.UserId == foundUser.Id) is not null)
            {
                return AddGroupMembersToList(groupMembers, allPositions, allUsers);
            }
            else
            {
                return tempCreatedMember;
            }
        }
        public async Task Import()
        {
            try
            {

                var allUser = _userRepository.GetAll();
                var allGroups = _groupRepository.GetAll();
                var allPositions = _groupPositionRepository.GetAll();
                await Task.WhenAll(allUser, allGroups, allPositions);
                var groupMembersToSave = new GroupMember[(int)NumberOfMockModelToCreate.MEMBERS];
                for (int x = 0; x < (int)NumberOfMockModelToCreate.MEMBERS; x++)
                {
                    var tempGroupMember = AddGroupMembersToList(groupMembersToSave, allPositions?.Result ?? throw new InvalidOperationException("Couldn't retrieve positions"), allUser?.Result ?? throw new InvalidOperationException("Couldn't retrieve users"));
                    groupMembersToSave[x] = tempGroupMember;
                }
                var createdGroupMembers = await _groupMemberRepository.Create(groupMembersToSave) ?? throw new InvalidOperationException("Failed to create groups");
            }
            catch (Exception e)
            {
                _logger.LogError("Failed to create or save mock groups: {0}", e);
            }
        }
    }
}