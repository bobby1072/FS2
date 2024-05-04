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
            var allUsers = await _userRepository.GetAll() ?? throw new InvalidOperationException("Cannot get all users");
            var listOfGroupsToCreate = new Group[(int)NumberOfMockModelToCreate.GROUPS];
            bool userSaved = false;
            int tryAmountCount = 0;
            while (!userSaved)
            {
                try
                {

                    for (int x = 0; x < (int)NumberOfMockModelToCreate.GROUPS - 1; x += 2)
                    {
                        var foundUser = allUsers.ElementAt(x == 0 ? 0 : x / 2);
                        var tempGroup = MockGroupBuilder.Build(foundUser.Id ?? throw new InvalidDataException("No id on user"));
                        listOfGroupsToCreate[x] = tempGroup;
                        var tempGroup2 = MockGroupBuilder.Build(foundUser.Id ?? throw new InvalidDataException("No id on user"));
                        listOfGroupsToCreate[x + 1] = tempGroup2;
                    }
                    var createdGroups = await _groupRepository.Create(listOfGroupsToCreate) ?? throw new InvalidOperationException("Failed to create groups");
                    userSaved = true;
                }
                catch (Exception e)
                {
                    tryAmountCount++;
                    _logger.LogError("Failed to create or save groups users: {0}", e);

                    if (tryAmountCount >= 5)
                    {
                        throw new InvalidOperationException("Cannot save mock groups");
                    }
                }
            }
        }
    }
}