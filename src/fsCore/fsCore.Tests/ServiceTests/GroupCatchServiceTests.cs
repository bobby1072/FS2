using FluentValidation;
using fsCore.Common.Misc;
using fsCore.Common.Models;
using fsCore.Common.Utils;
using fsCore.DataImporter.MockModelBuilders;
using fsCore.Persistence.EntityFramework.Repository.Abstract;
using fsCore.Services.Abstract;
using fsCore.Services.Concrete;
using Moq;

namespace fsCore.Tests.ServiceTests
{
    public class GroupCatchServiceTests : TestBase
    {
        private readonly Mock<IWorldFishRepository> _worldFishRepository = new();
        private readonly Mock<IGroupService> _groupService = new();
        private readonly Mock<IGroupCatchRepository> _groupCatchRepository = new();
        private readonly Mock<IUserService> _userService = new();
        private readonly IValidator<GroupCatch> _catchValidator = new MockGroupCatchValidator();
        private readonly IValidator<GroupCatchComment> _commentValidator =
            new MockGroupCatchCommentValidator();
        private readonly Mock<IGroupCatchCommentRepository> _commentRepo = new();
        private readonly GroupCatchService _groupCatchService;

        public GroupCatchServiceTests()
        {
            _groupCatchService = new GroupCatchService(
                _groupCatchRepository.Object,
                _worldFishRepository.Object,
                _groupService.Object,
                _userService.Object,
                _commentRepo.Object,
                _commentValidator,
                _catchValidator
            );
        }

        private class MockGroupCatchValidator
            : AbstractValidator<GroupCatch>,
                IValidator<GroupCatch> { }

        private class MockGroupCatchCommentValidator
            : AbstractValidator<GroupCatchComment>,
                IValidator<GroupCatchComment> { }

        private class Can_Manage_Catches_User_Class_Data
            : TheoryData<UserWithGroupPermissionSet, Group>
        {
            public Can_Manage_Catches_User_Class_Data()
            {
                var authorisedLeader = MockUserWithPermissionsBuilder.Build(
                    MockUserBuilder.Build()
                );
                var group = MockGroupBuilder.Build((Guid)authorisedLeader.Id!);
                authorisedLeader.BuildPermissions(new[] { group });
                Add(authorisedLeader, group);
                var positionToDeleteCatches = MockGroupPositionBuilder.Build(
                    (Guid)group.Id!,
                    null,
                    true,
                    true,
                    false,
                    false,
                    1
                );
                var authorisedMemberUser = MockUserWithPermissionsBuilder.Build();
                var authorisedMember = MockGroupMemberBuilder.Build(
                    (Guid)group.Id,
                    (Guid)authorisedMemberUser.Id!,
                    (int)positionToDeleteCatches.Id!
                );
                authorisedMember.Group = group;
                authorisedMember.Position = positionToDeleteCatches;
                authorisedMemberUser.BuildPermissions(authorisedMember);
                Add(authorisedMemberUser, group);
            }
        }

        [Theory]
        [ClassData(typeof(Can_Manage_Catches_User_Class_Data))]
        public async Task Authorised_Member_Can_Save_And_Delete_Catches(
            UserWithGroupPermissionSet user,
            Group group
        )
        {
            var originalCatch = MockGroupCatchBuilder.Build(
                group.Id ?? Guid.Empty,
                user.Id ?? Guid.Empty
            );
            _groupCatchRepository
                .Setup(x => x.GetOne(originalCatch.Id ?? Guid.Empty))
                .ReturnsAsync(originalCatch);
            var newCatch = originalCatch.JsonClone();
            newCatch.Description = "New test description";
            newCatch.CaughtAt = DateTimeUtils.RandomPastDate().Invoke();
            _groupCatchRepository
                .Setup(x => x.Update(It.IsAny<ICollection<GroupCatch>>()))
                .ReturnsAsync(new[] { newCatch });
            await _groupCatchService.SaveGroupCatch(newCatch, user);
            _groupCatchRepository.Verify(
                x => x.Update(It.IsAny<ICollection<GroupCatch>>()),
                Times.Once
            );
        }
    }
}
