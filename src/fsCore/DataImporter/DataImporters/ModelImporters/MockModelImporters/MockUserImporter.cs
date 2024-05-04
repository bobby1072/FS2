using Common.DbInterfaces.Repository;
using Common.Models;
using DataImporter.MockModelBuilders;
using Microsoft.Extensions.Logging;

namespace DataImporter.ModelImporters.MockModelImporters
{
    internal class MockUserImporter : IUserImporter
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<MockUserImporter> _logger;
        public MockUserImporter(IUserRepository userRepo, ILogger<MockUserImporter> logger)
        {
            _logger = logger;
            _userRepository = userRepo;
        }
        public async Task Import()
        {
            bool userSaved = false;
            int tryAmountCount = 0;
            while (!userSaved)
            {
                try
                {
                    var newUserArray = new User[(int)NumberOfMockModelToCreate.USERS];
                    for (int x = 0; x < newUserArray.Length; x++)
                    {
                        var tempUser = MockUserBuilder.Build();
                        newUserArray[x] = tempUser;
                    }
                    var createdUsers = await _userRepository.Create(newUserArray) ?? throw new InvalidOperationException("Failed to create groups");
                    userSaved = true;
                }
                catch (Exception e)
                {
                    tryAmountCount++;
                    _logger.LogError("Failed to create or save mock users: {0}", e);
                    if (tryAmountCount >= 5)
                    {
                        throw new InvalidOperationException("Cannot save mock users");
                    }
                }
            }
        }
    }
}