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
            int tryCount = 0;
            while (tryCount < 3)
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
                }
                catch (Exception e)
                {
                    tryCount++;
                    _logger.LogError("Failed to create or save mock users: {0}", e);
                }
            }
        }
    }
}