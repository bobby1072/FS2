using Common.Misc;
using Common.Models;
using Common.Utils;
using DataImporter.MockModelBuilders;
using FluentValidation;
using Moq;
using Persistence.EntityFramework.Repository.Abstract;
using Services.Abstract;
using Services.Concrete;

namespace fsCore.Tests.ServiceTests
{
    public class GroupCatchServiceTests : TestBase
    {
        private readonly Mock<IWorldFishRepository> _worldFishRepository;
        private readonly Mock<IGroupService> _groupService;
        private readonly Mock<IGroupCatchRepository> _groupCatchRepository;
        private readonly Mock<IUserService> _userService;
        private readonly IValidator<GroupCatch> _catchValidator;
        private readonly IValidator<GroupCatchComment> _commentValidator;
        private readonly Mock<IGroupCatchCommentRepository> _commentRepo;
        private readonly IGroupCatchService _groupCatchService;
        public GroupCatchServiceTests()
        {
            _groupCatchRepository = new Mock<IGroupCatchRepository>();
            _worldFishRepository = new Mock<IWorldFishRepository>();
            _groupService = new Mock<IGroupService>();
            _userService = new Mock<IUserService>();
            _commentRepo = new Mock<IGroupCatchCommentRepository>();
            _catchValidator = new MockGroupCatchValidator();
            _commentValidator = new MockGroupCatchCommentValidator();
            _groupCatchService = new GroupCatchService(_groupCatchRepository.Object, _worldFishRepository.Object, _groupService.Object, _userService.Object, _commentRepo.Object, _commentValidator, _catchValidator);
        }
        internal class MockGroupCatchValidator : AbstractValidator<GroupCatch>, IValidator<GroupCatch> { }
        internal class MockGroupCatchCommentValidator : AbstractValidator<GroupCatchComment>, IValidator<GroupCatchComment> { }
        internal class Can_Manage_Catches_User_Class_Data : TheoryData<UserWithGroupPermissionSet, Group>
        {
            public Can_Manage_Catches_User_Class_Data()
            {
                var authorisedLeader = MockUserWithPermissionsBuilder.Build(MockUserBuilder.Build());
                var group = MockGroupBuilder.Build((Guid)authorisedLeader.Id!);
                authorisedLeader.BuildPermissions(new[] { group });
                Add(authorisedLeader, group);
                var positionToDeleteCatches = MockGroupPositionBuilder.Build((Guid)group.Id!, null, true, true, false, false, 1);
                var authorisedMemberUser = MockUserWithPermissionsBuilder.Build();
                var authorisedMember = MockGroupMemberBuilder.Build((Guid)group.Id, (Guid)authorisedMemberUser.Id!, (int)positionToDeleteCatches.Id!);
                authorisedMember.Group = group;
                authorisedMember.Position = positionToDeleteCatches;
                authorisedMemberUser.BuildPermissions(authorisedMember);
                Add(authorisedMemberUser, group);
            }
        }
        [Theory]
        [ClassData(typeof(Can_Manage_Catches_User_Class_Data))]
        public async Task Authorised_Member_Can_Save_And_Delete_Catches(UserWithGroupPermissionSet user, Group group)
        {
            var originalCatch = MockGroupCatchBuilder.Build(group.Id ?? Guid.Empty, user.Id ?? Guid.Empty);
            _groupCatchRepository.Setup(x => x.GetOne(originalCatch.Id ?? Guid.Empty)).ReturnsAsync(originalCatch);
            var newCatch = originalCatch.JsonClone();
            newCatch.Description = "New test description";
            newCatch.CaughtAt = DateTimeUtils.RandomPastDate()();
            _groupCatchRepository.Setup(x => x.Update(It.IsAny<ICollection<GroupCatch>>())).ReturnsAsync(new[] { newCatch });
            await _groupCatchService.SaveGroupCatch(newCatch, user);
            _groupCatchRepository.Verify(x => x.Update(It.IsAny<ICollection<GroupCatch>>()), Times.Once);
        }
    }
}