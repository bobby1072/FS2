using fsCore.Common.Models;
using fsCore.DataImporter.DataImporters.ModelImporters.Abstract;
using fsCore.DataImporter.MockModelBuilders;
using fsCore.Persistence.EntityFramework.Repository.Abstract;
using Microsoft.Extensions.Logging;

namespace fsCore.DataImporter.DataImporters.ModelImporters.Concrete.Mock
{
    internal class MockGroupMemberImporter : IGroupMemberImporter
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupPositionRepository _groupPositionRepository;
        private readonly IGroupMemberRepository _groupMemberRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<MockGroupMemberImporter> _logger;

        public MockGroupMemberImporter(
            IGroupRepository groupRepository,
            IGroupPositionRepository groupPositionRepository,
            IUserRepository userRepo,
            ILogger<MockGroupMemberImporter> logger,
            IGroupMemberRepository groupMemberRepo
        )
        {
            _groupMemberRepository = groupMemberRepo;
            _logger = logger;
            _groupRepository = groupRepository;
            _groupPositionRepository = groupPositionRepository;
            _userRepository = userRepo;
        }

        private static GroupMember CreateUniqueGroupIdUserIdGroupMember(
            ICollection<GroupMember> groupMembers,
            ICollection<GroupPosition> allPositions,
            ICollection<User> allUsers
        )
        {
            var random = new Random();
            var foundUser = allUsers.ElementAt(random.Next(0, allUsers.Count));
            var foundPosition = allPositions.ElementAt(random.Next(0, allPositions.Count));
            var tempCreatedMember = MockGroupMemberBuilder.Build(
                foundPosition?.GroupId ?? throw new InvalidDataException("No groupId on position"),
                foundUser?.Id ?? throw new InvalidDataException("No id on group"),
                foundPosition?.Id ?? throw new InvalidDataException("No id on position")
            );
            if (
                groupMembers.FirstOrDefault(x =>
                    x?.GroupId == foundPosition.GroupId && x?.UserId == foundUser.Id
                )
                is not null
            )
            {
                return CreateUniqueGroupIdUserIdGroupMember(groupMembers, allPositions, allUsers);
            }
            else
            {
                return tempCreatedMember;
            }
        }

        public async Task Import()
        {
            int tryCount = 0;
            while (tryCount < 3)
            {
                try
                {
                    var allUser = _userRepository.GetAll();
                    var allGroups = _groupRepository.GetAll();
                    var allPositions = _groupPositionRepository.GetAll();
                    await Task.WhenAll(allUser, allGroups, allPositions);
                    var groupMembersToSave = new List<GroupMember>();
                    for (int x = 0; x < allPositions.Result?.Count; x++)
                    {
                        var tempGroupMemberArray = new GroupMember[
                            (int)NumberOfMockModelToCreate.MembersPerPosition
                        ];
                        for (int deepX = 0; deepX < tempGroupMemberArray.Length; deepX++)
                        {
                            var tempCheckArray = new List<GroupMember>(groupMembersToSave);
                            tempCheckArray.AddRange(tempGroupMemberArray);
                            tempGroupMemberArray[deepX] = CreateUniqueGroupIdUserIdGroupMember(
                                tempCheckArray,
                                allPositions?.Result
                                    ?? throw new InvalidOperationException(
                                        "Couldn't retrieve positions"
                                    ),
                                allUser?.Result
                                    ?? throw new InvalidOperationException(
                                        "Couldn't retrieve users"
                                    )
                            );
                        }
                        groupMembersToSave.AddRange(tempGroupMemberArray);
                    }
                    var createdGroupMembers =
                        await _groupMemberRepository.Create(groupMembersToSave)
                        ?? throw new InvalidOperationException("Failed to create groups");
                    return;
                }
                catch (Exception e)
                {
                    tryCount++;
                    _logger.LogError("Failed to create or save mock groups: {0}", e);
                }
            }
            throw new InvalidOperationException("Failed to create mock users");
        }
    }
}
