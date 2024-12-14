using DataImporter.DataImporters.ModelImporters.Abstract;
using DataImporter.MockModelBuilders;
using fsCore.Common.Models;
using fsCore.Persistence.EntityFramework.Repository.Abstract;
using Microsoft.Extensions.Logging;

namespace DataImporter.DataImporters.ModelImporters.Concrete.Mock
{
    internal class MockGroupCatchImporter : IGroupCatchImporter
    {
        private readonly IGroupCatchRepository _groupCatchRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupMemberRepository _groupMemberRepository;
        private readonly ILogger<MockGroupCatchImporter> _logger;

        public MockGroupCatchImporter(
            IGroupCatchRepository groupCatchRepository,
            IGroupRepository groupRepository,
            IGroupMemberRepository groupMemberRepository,
            ILogger<MockGroupCatchImporter> logger
        )
        {
            _logger = logger;
            _groupCatchRepository = groupCatchRepository;
            _groupRepository = groupRepository;
            _groupMemberRepository = groupMemberRepository;
        }

        private static (decimal, decimal) GetCloseByLatLng(decimal lat, decimal lng)
        {
            var random = new Random();
            var latOffset =
                lat
                + (decimal)(
                    random.Next(0, 2) == 1
                        ? -(1.0 + random.NextDouble() * (2.0 - 1.0))
                        : 1.0 + random.NextDouble() * (2.0 - 1.0)
                );
            var lngOffset =
                lng
                + (decimal)(
                    random.Next(0, 2) == 1
                        ? -(1.0 + random.NextDouble() * (4.0 - 2.0))
                        : 2.0 + random.NextDouble() * (4.0 - 2.0)
                );
            if (latOffset < -90 || latOffset > 90 || lngOffset < -180 || lngOffset > 180)
            {
                return GetCloseByLatLng(random.Next(-90, 90), random.Next(-180, 180));
            }
            return (latOffset, lngOffset);
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
                        var currentGroupCatchList = new GroupCatch[
                            random.Next(0, (int)NumberOfMockModelToCreate.MaxCatchesPerGroup)
                        ];
                        var randomGroupMemberList = allMembers
                            .Result?.Where(x => x.GroupId == allGroups.Result.ElementAt(i)?.Id)
                            .ToArray();
                        if (randomGroupMemberList?.Length < 1)
                        {
                            continue;
                        }
                        var randomGroupMember = randomGroupMemberList?.ElementAt(
                            random.Next(0, randomGroupMemberList.Length)
                        );
                        for (int deepI = 0; deepI < currentGroupCatchList.Length; deepI++)
                        {
                            var tempGroupCatch = MockGroupCatchBuilder.Build(
                                randomGroupMember?.GroupId
                                    ?? throw new InvalidDataException("No groupId on member"),
                                randomGroupMember?.UserId
                                    ?? throw new InvalidDataException("No userId on member")
                            );
                            if (deepI != 0)
                            {
                                var (lat, lng) = GetCloseByLatLng(
                                    currentGroupCatchList[deepI - 1].Latitude,
                                    currentGroupCatchList[deepI - 1].Longitude
                                );
                                tempGroupCatch.Latitude = lat;
                                tempGroupCatch.Longitude = lng;
                            }
                            currentGroupCatchList[deepI] = tempGroupCatch;
                        }
                        allGroupCatches.AddRange(currentGroupCatchList);
                    }
                    var createdGroupCatches =
                        await _groupCatchRepository.Create(allGroupCatches)
                        ?? throw new InvalidOperationException("Failed to create group catches");
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
