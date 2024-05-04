using Common.DbInterfaces.Repository;
using Common.Models;
using DataImporter.MockModelBuilders;
using Microsoft.Extensions.Logging;

namespace DataImporter.ModelImporters.MockModelImporters
{
    internal class MockGroupPositionImporter : IGroupPositionImporter
    {
        private readonly IGroupRepository _groupRepository;
        private readonly ILogger<MockGroupPositionImporter> _logger;
        private readonly IGroupPositionRepository _groupPositionRepository;
        public MockGroupPositionImporter(IGroupRepository groupRepository, ILogger<MockGroupPositionImporter> logger, IGroupPositionRepository groupPositionRepository)
        {
            _logger = logger;
            _groupRepository = groupRepository;
            _groupPositionRepository = groupPositionRepository;
        }
        public async Task Import()
        {
            try
            {
                var allGroups = await _groupRepository.GetAll() ?? throw new InvalidOperationException("Cannot get all groups");
                var listOfPositionsToCreate = new List<GroupPosition>();
                for (int x = 0; x < allGroups.Count; x++)
                {
                    var currentPositionsList = new GroupPosition[(int)NumberOfMockModelToCreate.POSITIONSPERGROUP];
                    var tempGroup = allGroups.ElementAt(x);
                    for (int deepX = 0; deepX < (int)NumberOfMockModelToCreate.POSITIONSPERGROUP; deepX++)
                    {
                        currentPositionsList[deepX] = MockGroupPositionBuilder.Build(tempGroup?.Id ?? throw new InvalidOperationException("Cannot get group id"));
                    }
                    listOfPositionsToCreate.AddRange(currentPositionsList);
                }
                var createdGroupPositions = await _groupPositionRepository.Create(listOfPositionsToCreate) ?? throw new InvalidOperationException("Failed to create groups");
            }
            catch (Exception e)
            {
                _logger.LogError("Failed to create or save groups users: {0}", e);
            }
        }
    }
}