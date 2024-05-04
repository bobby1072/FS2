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
                var allGroups = await _groupRepository.GetAll();
                var listOfPositionsToCreate = new GroupPosition[(int)NumberOfMockModelToCreate.POSITIONS];
                for (int x = 0; x < (int)NumberOfMockModelToCreate.POSITIONS; x += 2)
                {
                    var tempGroup = allGroups?.ElementAt(x == 0 ? 0 : x / 2);
                    listOfPositionsToCreate[x] = MockGroupPositionBuilder.Build(tempGroup?.Id ?? throw new InvalidDataException("No id on group"));
                    listOfPositionsToCreate[x + 1] = MockGroupPositionBuilder.Build(tempGroup?.Id ?? throw new InvalidDataException("No id on group"));
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