using Common.DbInterfaces.Repository;
using Common.Models;
using DataImporter.MockModelBuilders;
using Microsoft.Extensions.Logging;

namespace DataImporter.ModelImporters.MockModelImporters
{
    internal class MockGroupImporter : IGroupImporter
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<MockGroupImporter> _logger;
        public MockGroupImporter(IGroupRepository groupRepository, IUserRepository userRepo, ILogger<MockGroupImporter> logger)
        {
            _logger = logger;
            _groupRepository = groupRepository;
            _userRepository = userRepo;
        }
        public async Task Import()
        {
            try
            {
                var allUsers = await _userRepository.GetAll() ?? throw new InvalidOperationException("Cannot get all users");
                var listOfGroupsToCreate = new List<Group>();
                for (int x = 0; x < allUsers.Count; x++)
                {
                    var currentGroupList = new Group[(int)NumberOfMockModelToCreate.GROUPSPERUSERS];
                    var foundUser = allUsers.ElementAt(x);
                    for (int deepX = 0; deepX < (int)NumberOfMockModelToCreate.GROUPSPERUSERS; deepX++)
                    {
                        currentGroupList[deepX] = MockGroupBuilder.Build(foundUser?.Id ?? throw new InvalidOperationException("Cannot get user id"));
                    }
                    listOfGroupsToCreate.AddRange(currentGroupList);
                }
                var createdGroups = await _groupRepository.Create(listOfGroupsToCreate) ?? throw new InvalidOperationException("Failed to create groups");
            }
            catch (Exception e)
            {
                _logger.LogError("Failed to create or save mock groups: {0}", e);
            }
        }
    }
}