using fsCore.Common.Models;
using DataImporter.DataImporters.ModelImporters.Abstract;
using DataImporter.MockModelBuilders;
using Microsoft.Extensions.Logging;
using Persistence.EntityFramework.Repository.Abstract;

namespace DataImporter.DataImporters.ModelImporters.Concrete.Mock
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
        private static Group CreateUniqueGroupNameGroup(ICollection<Group> groups, Guid leaderId)
        {
            var tempGroup = MockGroupBuilder.Build(leaderId);
            if (groups.FirstOrDefault(x => x?.Name == tempGroup.Name) is not null)
            {
                return CreateUniqueGroupNameGroup(groups, leaderId);
            }
            else
            {
                return tempGroup;
            }
        }
        public async Task Import()
        {
            int tryCount = 0;
            while (tryCount < 3)
            {

                try
                {
                    var allUsers = await _userRepository.GetAll() ?? throw new InvalidOperationException("Cannot get all users");
                    var listOfGroupsToCreate = new List<Group>();
                    for (int x = 0; x < allUsers.Count; x++)
                    {
                        var currentGroupList = new Group[(int)NumberOfMockModelToCreate.GroupsPerUser];
                        var foundUser = allUsers.ElementAt(x);
                        for (int deepX = 0; deepX < currentGroupList.Length; deepX++)
                        {
                            var tempCheckList = new List<Group>(listOfGroupsToCreate);
                            tempCheckList.AddRange(currentGroupList);
                            currentGroupList[deepX] = CreateUniqueGroupNameGroup(tempCheckList, foundUser?.Id ?? throw new InvalidOperationException("Cannot get user id"));
                        }
                        listOfGroupsToCreate.AddRange(currentGroupList);
                    }
                    var createdGroups = await _groupRepository.Create(listOfGroupsToCreate) ?? throw new InvalidOperationException("Failed to create groups");
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