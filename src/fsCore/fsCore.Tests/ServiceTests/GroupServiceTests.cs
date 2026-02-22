using FluentValidation;
using fsCore.Common.Misc;
using fsCore.Common.Models;
using fsCore.DataImporter.MockModelBuilders;
using fsCore.Persistence.EntityFramework.Repository.Abstract;
using fsCore.Services.Abstract;
using fsCore.Services.Concrete;
using Moq;

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
            _groupService = new GroupService(
                _mockGroupRepository.Object,
                _mockGroupMemberRepository.Object,
                _mockGroupPositionRepository.Object,
                _mockGroupValidator,
                _mockGroupPositionValidator
            );
        }

        private class MockGroupPositionValidator
            : AbstractValidator<GroupPosition>,
                IValidator<GroupPosition> { }

        private class MockGroupValidator : AbstractValidator<Group>, IValidator<Group> { }

        private class Group_Leader_Or_Can_Manage_Group_Member_Class_Data
            : TheoryData<UserWithGroupPermissionSet, Group>
        {
            public Group_Leader_Or_Can_Manage_Group_Member_Class_Data()
            {
                var leaderUser = MockUserWithPermissionsBuilder.Build(null);
                var group = MockGroupBuilder.Build(leaderUser.Id ?? Guid.Empty);
                leaderUser.BuildPermissions(group);
                var position = MockGroupPositionBuilder.Build(
                    (Guid)group.Id!,
                    true,
                    null,
                    null,
                    null,
                    null,
                    1
                );
                var memberUser = MockUserWithPermissionsBuilder.Build(null);
                var member = MockGroupMemberBuilder.Build(
                    (Guid)group.Id!,
                    memberUser.Id ?? Guid.Empty,
                    position.Id ?? 1
                );
                member.Position = position;
                member.Group = group;
                memberUser.BuildPermissions(member);
                Add(leaderUser, group);
                Add(memberUser, group);
            }
        }

        private class Non_Member_And_No_Permissions_GroupMember_Class_Data
            : TheoryData<UserWithGroupPermissionSet, Group>
        {
            public Non_Member_And_No_Permissions_GroupMember_Class_Data()
            {
                var nonGroupUser = MockUserWithPermissionsBuilder.Build(null);
                var group = MockGroupBuilder.Build(Guid.NewGuid());
                var position = MockGroupPositionBuilder.Build(
                    (Guid)group.Id!,
                    false,
                    false,
                    false,
                    false,
                    false,
                    1
                );
                var memberUserWithoutPermission = MockUserWithPermissionsBuilder.Build(null);
                var member = MockGroupMemberBuilder.Build(
                    (Guid)group.Id!,
                    memberUserWithoutPermission.Id ?? Guid.Empty,
                    position.Id ?? 1
                );
                member.Position = position;
                member.Group = group;
                memberUserWithoutPermission.BuildPermissions(member);
                Add(nonGroupUser, group);
                Add(memberUserWithoutPermission, group);
            }
        }

        [Theory]
        [ClassData(typeof(Group_Leader_Or_Can_Manage_Group_Member_Class_Data))]
        public async Task Group_Leader_Or_Can_Manage_Group_Member_Can_Edit_Group_And_Save_And_Delete_Positions(
            UserWithGroupPermissionSet user,
            Group group
        )
        {
            _mockGroupRepository
                .Setup(x =>
                    x.GetGroupWithoutEmblem((Guid)group.Id!, It.IsAny<ICollection<string>>())
                )
                .ReturnsAsync(group);
            var editedGroup = group.JsonClone();
            editedGroup.Name = "New name for test";
            _mockGroupRepository
                .Setup(x => x.Update(new[] { editedGroup }))
                .ReturnsAsync(new[] { editedGroup });
            await _groupService.SaveGroup(editedGroup, user);
            _mockGroupRepository.Verify(x => x.Update(new[] { editedGroup }), Times.Once);
            var newPosition = MockGroupPositionBuilder.Build(
                (Guid)group.Id!,
                null,
                null,
                null,
                null,
                null,
                null
            );
            newPosition.Id = null;
            _mockGroupPositionRepository
                .Setup(x => x.Create(It.IsAny<ICollection<GroupPosition>>()))
                .ReturnsAsync(new[] { newPosition });
            await _groupService.SavePosition(newPosition, user);
            _mockGroupPositionRepository.Verify(
                x => x.Create(It.IsAny<ICollection<GroupPosition>>()),
                Times.Once
            );
        }

        [Theory]
        [ClassData(typeof(Non_Member_And_No_Permissions_GroupMember_Class_Data))]
        public async Task Non_Authorised_User_Cant_Edit_Or_Delete_Group(
            UserWithGroupPermissionSet currentUser,
            Group group
        )
        {
            _mockGroupRepository
                .Setup(x =>
                    x.GetGroupWithoutEmblem((Guid)group.Id!, It.IsAny<ICollection<string>>())
                )
                .ReturnsAsync(group);
            var editedGroup = group.JsonClone();
            editedGroup.Name = "New name for test";
            var foundSaveException = await Assert.ThrowsAsync<ApiException>(
                () => _groupService.SaveGroup(editedGroup, currentUser)
            );
            Assert.Equal(ErrorConstants.DontHavePermission, foundSaveException.Message);
            var foundDeleteException = await Assert.ThrowsAsync<ApiException>(
                () => _groupService.DeleteGroup((Guid)group.Id!, currentUser)
            );
            Assert.Equal(ErrorConstants.DontHavePermission, foundDeleteException.Message);
        }

        [Theory]
        [ClassData(typeof(Non_Member_And_No_Permissions_GroupMember_Class_Data))]
        public async Task Non_Authorised_User_Cant_Edit_Or_Delete_Members(
            UserWithGroupPermissionSet currentUser,
            Group group
        )
        {
            var position = MockGroupPositionBuilder.Build(
                (Guid)group.Id!,
                null,
                null,
                null,
                null,
                null,
                2
            );
            var newMember = MockGroupMemberBuilder.Build(
                (Guid)group.Id!,
                Guid.NewGuid(),
                position.Id ?? 1
            );
            var foundSaveGroupMemberException = await Assert.ThrowsAsync<ApiException>(
                () => _groupService.SaveGroupMember(newMember, currentUser)
            );
            Assert.Equal(ErrorConstants.DontHavePermission, foundSaveGroupMemberException.Message);
            _mockGroupMemberRepository
                .Setup(x => x.GetOne(newMember.Id ?? 2, "Id", It.IsAny<ICollection<string>>()))
                .ReturnsAsync(newMember);
            var foundDeleteGroupMemberException = await Assert.ThrowsAsync<ApiException>(
                () => _groupService.DeleteGroupMember(newMember.Id ?? 2, currentUser)
            );
            Assert.Equal(
                ErrorConstants.DontHavePermission,
                foundDeleteGroupMemberException.Message
            );
        }
    }
}
