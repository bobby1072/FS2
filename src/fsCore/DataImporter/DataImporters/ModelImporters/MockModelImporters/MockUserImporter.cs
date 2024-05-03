using Common.DbInterfaces.Repository;
using Common.Models;
using DataImporter.MockModelBuilders;
using DataImporter.ModelImporters;

namespace DataImporter.DataImporters.ModelImporters.MockModelImporters
{
    internal class MockUserImporter : IUserImporter
    {
        private readonly IUserRepository _userRepository;
        public MockUserImporter(IUserRepository userRepo)
        {
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
                    var newUserArray = new User[10];
                    for (int x = 0; x < newUserArray.Length; x++)
                    {
                        newUserArray[x] = MockUserBuilder.Build();
                    }
                    var submittedUsers = await _userRepository.Create(newUserArray);
                    userSaved = true;
                }
                catch (Exception)
                {
                    tryAmountCount++;
                    if (tryAmountCount >= 5)
                    {
                        throw new InvalidOperationException("Cannot save mock users");
                    }
                }
            }
        }
    }
}