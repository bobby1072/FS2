using Common.Models;
using DataImporter.MockModelBuilders;
using Microsoft.Extensions.Logging;
using Persistence.EntityFramework.Abstract.Repository;
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
        public User CreateUniqueEmailUsernameUser(ICollection<User> users)
        {
            var tempUser = MockUserBuilder.Build();
            if (users.FirstOrDefault(x => x?.Email == tempUser.Email || x?.Username == tempUser.Username) is not null)
            {
                return CreateUniqueEmailUsernameUser(users);
            }
            else
            {
                return tempUser;
            }
        }
        public async Task Import()
        {
            int tryCount = 0;
            while (tryCount < 3)
            {

                try
                {
                    var newUserArray = new User[(int)NumberOfMockModelToCreate.Users];
                    for (int x = 0; x < newUserArray.Length; x++)
                    {
                        var tempUser = CreateUniqueEmailUsernameUser(newUserArray);
                        newUserArray[x] = tempUser;
                    }
                    var createdUsers = await _userRepository.Create(newUserArray) ?? throw new InvalidOperationException("Failed to create groups");
                    return;
                }
                catch (Exception e)
                {
                    tryCount++;
                    _logger.LogError("Failed to create or save mock users: {0}", e);
                }
            }
            throw new InvalidOperationException("Failed to create mock users");
        }
    }
}