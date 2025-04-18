using fsCore.Common.Models;
using fsCore.DataImporter.DataImporters.ModelImporters.Abstract;
using fsCore.DataImporter.MockModelBuilders;
using fsCore.Persistence.EntityFramework.Repository.Abstract;
using Microsoft.Extensions.Logging;

namespace fsCore.DataImporter.DataImporters.ModelImporters.Concrete.Mock
{
    internal class MockGroupPositionImporter : IGroupPositionImporter
    {
        private readonly IGroupRepository _groupRepository;
        private readonly ILogger<MockGroupPositionImporter> _logger;
        private readonly IGroupPositionRepository _groupPositionRepository;

        public MockGroupPositionImporter(
            IGroupRepository groupRepository,
            ILogger<MockGroupPositionImporter> logger,
            IGroupPositionRepository groupPositionRepository
        )
        {
            _logger = logger;
            _groupRepository = groupRepository;
            _groupPositionRepository = groupPositionRepository;
        }

        private static GroupPosition CreateUniqueGroupIdNameGroupPosition(
            ICollection<GroupPosition> groupPositions,
            Guid groupId
        )
        {
            var tempGroupPosition = MockGroupPositionBuilder.Build(
                groupId,
                null,
                null,
                null,
                null,
                null,
                null
            );
            if (
                groupPositions.FirstOrDefault(x =>
                    x?.GroupId == tempGroupPosition.GroupId && x?.Name == tempGroupPosition.Name
                )
                is not null
            )
            {
                return CreateUniqueGroupIdNameGroupPosition(groupPositions, groupId);
            }
            else
            {
                return tempGroupPosition;
            }
        }

        public async Task Import()
        {
            int tryCount = 0;
            while (tryCount < 3)
            {
                try
                {
                    var allGroups =
                        await _groupRepository.GetAll()
                        ?? throw new InvalidOperationException("Cannot get all groups");
                    var listOfPositionsToCreate = new List<GroupPosition>();
                    for (int x = 0; x < allGroups.Count; x++)
                    {
                        var currentPositionsList = new GroupPosition[
                            (int)NumberOfMockModelToCreate.PositionPerGroup
                        ];
                        var tempGroup = allGroups.ElementAt(x);
                        for (int deepX = 0; deepX < currentPositionsList.Length; deepX++)
                        {
                            var tempPositionList = new List<GroupPosition>(listOfPositionsToCreate);
                            tempPositionList.AddRange(currentPositionsList);
                            currentPositionsList[deepX] = CreateUniqueGroupIdNameGroupPosition(
                                tempPositionList,
                                tempGroup?.Id
                                    ?? throw new InvalidOperationException("Cannot get group id")
                            );
                        }
                        listOfPositionsToCreate.AddRange(currentPositionsList);
                    }
                    var createdGroupPositions =
                        await _groupPositionRepository.Create(listOfPositionsToCreate)
                        ?? throw new InvalidOperationException("Failed to create groups");
                    return;
                }
                catch (Exception e)
                {
                    tryCount++;
                    _logger.LogError("Failed to create or save groups users: {0}", e);
                }
                throw new InvalidOperationException("Failed to create mock users");
            }
        }
    }
}
