using Common.DbInterfaces.Repository;
using Common.Models;
using Common.Models.Validators;
using DataImporter.MockModelBuilders;
using FluentValidation;
using fsCore.Service;
using fsCore.Service.Interfaces;
using Moq;

namespace fsCore.Tests.ServiceTests
{
    public class GroupServiceTests
    {
        private readonly Mock<IGroupRepository> _mockGroupRepository;
        private readonly Mock<IGroupMemberRepository> _mockGroupMemberRepository;
        private readonly Mock<IGroupPositionRepository> _mockGroupPositionRepository;
        private readonly Mock<GroupValidator> _mockGroupValidator;
        private readonly Mock<GroupPositionValidator> _mockGroupPositionValidator;
        private readonly IGroupService _groupService;
        public GroupServiceTests()
        {
            _mockGroupMemberRepository = new Mock<IGroupMemberRepository>();
            _mockGroupPositionRepository = new Mock<IGroupPositionRepository>();
            _mockGroupRepository = new Mock<IGroupRepository>();
            _mockGroupValidator = new Mock<GroupValidator>();
            _mockGroupPositionValidator = new Mock<GroupPositionValidator>();
            _mockGroupValidator.Setup(x => x.ValidateAndThrowAsync(It.IsAny<Group>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _mockGroupPositionValidator.Setup(x => x.ValidateAndThrowAsync(It.IsAny<GroupPosition>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _groupService = new GroupService(_mockGroupRepository.Object, _mockGroupMemberRepository.Object, _mockGroupPositionRepository.Object, _mockGroupValidator.Object, _mockGroupPositionValidator.Object);
        }
        public class GroupLeaderOrCanManageGroupMemberCanEditGroupClassData : TheoryData<UserWithGroupPermissionSet, Group>
        {
            public GroupLeaderOrCanManageGroupMemberCanEditGroupClassData()
            {
                var leaderUser = MockUserWithPerrmissionsBuilder.Build(null);
                var group = MockGroupBuilder.Build(leaderUser.Id ?? Guid.Empty);
                leaderUser.BuildPermissions(group);
                var position = MockGroupPositionBuilder.Build(group.Id ?? Guid.Empty, true, null, null, null, null, 1);
                var memberUser = MockUserWithPerrmissionsBuilder.Build(null);
                var member = MockGroupMemberBuilder.Build(group.Id ?? Guid.Empty, memberUser.Id ?? Guid.Empty, position.Id ?? 1);
                memberUser.BuildPermissions(member);
                Add(leaderUser, group);
                Add(memberUser, group);
            }
        }
        [Theory]
        [ClassData(typeof(GroupLeaderOrCanManageGroupMemberCanEditGroupClassData))]
        public async void GroupLeaderOrCanManageGroupMemberCanEditGroup(UserWithGroupPermissionSet user, Group group)
        {

        }
    }
}