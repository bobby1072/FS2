using Common;
using Common.DbInterfaces.Repository;
using Common.Models;
using DataImporter.MockModelBuilders;
using FluentValidation;
using fsCore.Service;
using fsCore.Service.Interfaces;
using Moq;

namespace fsCore.test.ServiceTests
{
    public class GroupServiceTests
    {
        private readonly Mock<IGroupRepository> _mockGroupRepository;
        private readonly Mock<IGroupMemberRepository> _mockGroupMemberRepository;
        private readonly Mock<IGroupPositionRepository> _mockGroupPositionRepository;
        private readonly IValidator<Group> _mockGroupValidator;
        private readonly IValidator<GroupPosition> _mockGroupPositionValidator;
        private readonly IGroupService _groupService;
        public GroupServiceTests()
        {
            _mockGroupMemberRepository = new Mock<IGroupMemberRepository>();
            _mockGroupPositionRepository = new Mock<IGroupPositionRepository>();
            _mockGroupRepository = new Mock<IGroupRepository>();
            _mockGroupValidator = new MockGroupValidator();
            _mockGroupPositionValidator = new MockGroupPositionValidator();
            _groupService = new GroupService(_mockGroupRepository.Object, _mockGroupMemberRepository.Object, _mockGroupPositionRepository.Object, _mockGroupValidator, _mockGroupPositionValidator);
        }
        internal class MockGroupPositionValidator : AbstractValidator<GroupPosition>, IValidator<GroupPosition>
        {
            public MockGroupPositionValidator() { }
        }
        internal class MockGroupValidator : AbstractValidator<Group>, IValidator<Group>
        {
            public MockGroupValidator() { }
        }
        internal class GroupLeaderOrCanManageGroupMemberCanEditGroupClassData : TheoryData<UserWithGroupPermissionSet, Group>
        {
            public GroupLeaderOrCanManageGroupMemberCanEditGroupClassData()
            {
                var leaderUser = MockUserWithPermissionsBuilder.Build(null);
                var group = MockGroupBuilder.Build(leaderUser.Id ?? Guid.Empty);
                leaderUser.BuildPermissions(group);
                var position = MockGroupPositionBuilder.Build(group.Id ?? Guid.Empty, true, null, null, null, null, 1);
                var memberUser = MockUserWithPermissionsBuilder.Build(null);
                var member = MockGroupMemberBuilder.Build(group.Id ?? Guid.Empty, memberUser.Id ?? Guid.Empty, position.Id ?? 1);
                member.Position = position;
                member.Group = group;
                memberUser.BuildPermissions(member);
                Add(leaderUser, group);
                Add(memberUser, group);
            }
        }
        [Theory]
        [ClassData(typeof(GroupLeaderOrCanManageGroupMemberCanEditGroupClassData))]
        public async Task GroupLeaderOrCanManageGroupMemberCanEditGroupAndSaveAndDeletePositions(UserWithGroupPermissionSet user, Group group)
        {
            _mockGroupRepository.Setup(x => x.GetGroupWithoutEmblem(group.Id ?? Guid.Empty, It.IsAny<ICollection<string>>())).ReturnsAsync(group);
            var editedGroup = group.JsonClone();
            editedGroup.Name = "New name for test";
            _mockGroupRepository.Setup(x => x.Update(new[] { editedGroup })).ReturnsAsync(new[] { editedGroup });
            await _groupService.SaveGroup(editedGroup, user);
            _mockGroupRepository.Verify(x => x.Update(new[] { editedGroup }), Times.Once);
            var newPosition = MockGroupPositionBuilder.Build(group.Id ?? Guid.Empty, null, null, null, null, null, null);
            newPosition.Id = null;
            _mockGroupPositionRepository.Setup(x => x.Create(It.IsAny<ICollection<GroupPosition>>())).ReturnsAsync(new[] { newPosition });
            await _groupService.SavePosition(newPosition, user);
            _mockGroupPositionRepository.Verify(x => x.Create(It.IsAny<ICollection<GroupPosition>>()), Times.Once);
        }
        internal class NonAuthorisedUserCantEditOrDeleteGroupClassData : TheoryData<UserWithGroupPermissionSet, Group>
        {
            public NonAuthorisedUserCantEditOrDeleteGroupClassData()
            {
                var nonGroupUser = MockUserWithPermissionsBuilder.Build(null);
                var group = MockGroupBuilder.Build(Guid.NewGuid());
                var position = MockGroupPositionBuilder.Build(group.Id ?? Guid.Empty, false, null, null, null, null, 1);
                var memberUserWithoutPermission = MockUserWithPermissionsBuilder.Build(null);
                var member = MockGroupMemberBuilder.Build(group.Id ?? Guid.Empty, memberUserWithoutPermission.Id ?? Guid.Empty, position.Id ?? 1);
                member.Position = position;
                member.Group = group;
                memberUserWithoutPermission.BuildPermissions(member);
                Add(nonGroupUser, group);
                Add(memberUserWithoutPermission, group);
            }
        }
        [Theory]
        [ClassData(typeof(NonAuthorisedUserCantEditOrDeleteGroupClassData))]
        public async Task NonAuthorisedUserCantEditOrDeleteGroup(UserWithGroupPermissionSet currentUser, Group group)
        {
            _mockGroupRepository.Setup(x => x.GetGroupWithoutEmblem(group.Id ?? Guid.Empty, It.IsAny<ICollection<string>>())).ReturnsAsync(group);
            var editedGroup = group.JsonClone();
            editedGroup.Name = "New name for test";
            var foundException = await Assert.ThrowsAsync<ApiException>(() => _groupService.SaveGroup(editedGroup, currentUser));
            Assert.Equal(ErrorConstants.DontHavePermission, foundException.Message);
            var foundDeleteException = await Assert.ThrowsAsync<ApiException>(() => _groupService.DeleteGroup(group.Id ?? Guid.Empty, currentUser));
            Assert.Equal(ErrorConstants.DontHavePermission, foundDeleteException.Message);
        }

    }
}