using Common.DbInterfaces.Repository;
using Common.Models;
using DataImporter.MockModelBuilders;
using Microsoft.Extensions.Logging;

namespace DataImporter.ModelImporters.MockModelImporters
{
    internal class MockGroupCatchImporter : IGroupCatchImporter
    {
        private readonly IGroupCatchRepository _groupCatchRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupMemberRepository _groupMemberRepository;
        private readonly ILogger<MockGroupCatchImporter> _logger;
        public MockGroupCatchImporter(IGroupCatchRepository groupCatchRepository, IGroupRepository groupRepository, IGroupMemberRepository groupMemberRepository, ILogger<MockGroupCatchImporter> logger)
        {
            _logger = logger;
            _groupCatchRepository = groupCatchRepository;
            _groupRepository = groupRepository;
            _groupMemberRepository = groupMemberRepository;
        }

        public async Task Import()
        {
            int tryCount = 0;
            while (tryCount < 3)
            {
                try
                {
                    var allMembers = _groupMemberRepository.GetAll();
                    var allGroups = _groupRepository.GetAll();
                    await Task.WhenAll(allMembers, allGroups);
                    var allGroupCatches = new List<GroupCatch>();
                    for (int i = 0; i < allGroups.Result?.Count; i++)
                    {
                        var random = new Random();
                        var currentGroupCatchList = new GroupCatch[random.Next(0, (int)NumberOfMockModelToCreate.MAXCATCHESPERGROUP)];
                        var randomGroupMemberList = allMembers.Result?.Where(x => x.GroupId == allGroups.Result.ElementAt(i)?.Id).ToArray();
                        var randomGroupMember = randomGroupMemberList?.ElementAt(random.Next(0, randomGroupMemberList.Length));
                        for (int deepI = 0; deepI < currentGroupCatchList.Length; deepI++)
                        {
                            currentGroupCatchList[deepI] = MockGroupCatchBuilder.Build(randomGroupMember?.GroupId ?? throw new InvalidDataException("No groupId on member"), randomGroupMember?.UserId ?? throw new InvalidDataException("No userId on member"));
                        }
                        allGroupCatches.AddRange(currentGroupCatchList);
                    }
                    var createdGroupCatches = await _groupCatchRepository.Create(allGroupCatches) ?? throw new InvalidOperationException("Failed to create group catches");
                    return;
                }
                catch (Exception e)
                {
                    tryCount++;
                    _logger.LogError("Failed to create or save mock groups: {0}", e);
                }
            }
        }

    }
}