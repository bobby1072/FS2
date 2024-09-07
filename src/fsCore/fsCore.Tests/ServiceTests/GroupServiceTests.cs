using Common.Misc;
using Common.Models;
using DataImporter.MockModelBuilders;
using FluentValidation;
using Moq;
using Persistence.EntityFramework.Repository.Abstract;
using Services.Abstract;
using Services.Concrete;
namespace fsCore.Tests.ServiceTests
{
    public class GroupServiceTests : TestBase
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
        internal class MockGroupPositionValidator : AbstractValidator<GroupPosition>, IValidator<GroupPosition> { }
        internal class MockGroupValidator : AbstractValidator<Group>, IValidator<Group> { }
        internal class Group_Leader_Or_Can_Manage_Group_Member_Class_Data : TheoryData<UserWithGroupPermissionSet, Group>
        {
            public Group_Leader_Or_Can_Manage_Group_Member_Class_Data()
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
        internal class Non_Member_And_No_Permissions_GroupMember_Class_Data : TheoryData<UserWithGroupPermissionSet, Group>
        {
            public Non_Member_And_No_Permissions_GroupMember_Class_Data()
            {
                var nonGroupUser = MockUserWithPermissionsBuilder.Build(null);
                var group = MockGroupBuilder.Build(Guid.NewGuid());
                var position = MockGroupPositionBuilder.Build(group.Id ?? Guid.Empty, false, false, false, false, false, 1);
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
        [ClassData(typeof(Group_Leader_Or_Can_Manage_Group_Member_Class_Data))]
        public async Task Group_Leader_Or_Can_Manage_Group_Member_Can_Edit_Group_And_Save_And_Delete_Positions(UserWithGroupPermissionSet user, Group group)
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
        [Theory]
        [ClassData(typeof(Non_Member_And_No_Permissions_GroupMember_Class_Data))]
        public async Task Non_Authorised_User_Cant_Edit_Or_Delete_Group(UserWithGroupPermissionSet currentUser, Group group)
        {
            _mockGroupRepository.Setup(x => x.GetGroupWithoutEmblem(group.Id ?? Guid.Empty, It.IsAny<ICollection<string>>())).ReturnsAsync(group);
            var editedGroup = group.JsonClone();
            editedGroup.Name = "New name for test";
            var foundSaveException = await Assert.ThrowsAsync<ApiException>(() => _groupService.SaveGroup(editedGroup, currentUser));
            Assert.Equal(ErrorConstants.DontHavePermission, foundSaveException.Message);
            var foundDeleteException = await Assert.ThrowsAsync<ApiException>(() => _groupService.DeleteGroup(group.Id ?? Guid.Empty, currentUser));
            Assert.Equal(ErrorConstants.DontHavePermission, foundDeleteException.Message);
        }
        [Theory]
        [ClassData(typeof(Non_Member_And_No_Permissions_GroupMember_Class_Data))]
        public async Task Non_Authorised_User_Cant_Edit_Or_Delete_Members(UserWithGroupPermissionSet currentUser, Group group)
        {
            var position = MockGroupPositionBuilder.Build(group.Id ?? Guid.Empty, null, null, null, null, null, 2);
            var newMember = MockGroupMemberBuilder.Build(group.Id ?? Guid.Empty, Guid.NewGuid(), position.Id ?? 1);
            var foundSaveGroupMemberException = await Assert.ThrowsAsync<ApiException>(() => _groupService.SaveGroupMember(newMember, currentUser));
            Assert.Equal(ErrorConstants.DontHavePermission, foundSaveGroupMemberException.Message);
            _mockGroupMemberRepository.Setup(x => x.GetOne(newMember.Id ?? 2, "Id", It.IsAny<ICollection<string>>())).ReturnsAsync(newMember);
            var foundDeleteGroupMemberException = await Assert.ThrowsAsync<ApiException>(() => _groupService.DeleteGroupMember(newMember.Id ?? 2, currentUser));
            Assert.Equal(ErrorConstants.DontHavePermission, foundDeleteGroupMemberException.Message);
        }
    }
}