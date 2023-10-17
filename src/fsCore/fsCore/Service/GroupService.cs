using System.Net;
using Common;
using Common.Dbinterfaces.Repository;
using Common.Models;
using Common.Utils;
using fsCore.Service.Interfaces;

namespace fsCore.Service
{
    internal class GroupService : BaseService<Group, IGroupRepository>, IGroupService
    {
        private readonly IGroupMemberRepository _groupMemberRepo;
        private readonly IGroupPositionRepository _groupPositionRepo;
        private readonly IUserService _userService;
        public GroupService(IGroupRepository repository,
        IGroupMemberRepository groupMemberRepo,
        IGroupPositionRepository groupPositionRepo,
        IUserService userService) : base(repository)
        {
            _groupMemberRepo = groupMemberRepo;
            _groupPositionRepo = groupPositionRepo;
            _userService = userService;
        }
        public async Task<ICollection<Group>> GetAllListedGroups()
        {
            var allGroups = await _repo.GetMany(true, "listed".ToPascalCase());
            return allGroups ?? throw new ApiException(ErrorConstants.NoGroupsFound, HttpStatusCode.NotFound);
        }
        public async Task<bool> IsUserInGroup(User currentUser, Guid groupId)
        {
            var foundGroupMember = await _groupMemberRepo.GetGroupMemberIncludingUser(currentUser.Email, groupId);
            return foundGroupMember != null && foundGroupMember.User?.Equals(currentUser) == true;
        }
        public async Task<bool> IsUserLeader(User currentUser, Guid groupId)
        {
            var foundGroupMember = await _groupMemberRepo.GetGroupMemberIncludingUserAndGroup(currentUser.Email, groupId);
            return foundGroupMember != null && foundGroupMember.User?.Equals(currentUser) == true && foundGroupMember.UserEmail == foundGroupMember.Group?.LeaderEmail;
        }
        public async Task<GroupMember> UserMembershipIncludingPosition(User currentUser, Guid groupId)
        {
            var foundGroupMember = await _groupMemberRepo.GetGroupMemberIncludingPosition(currentUser.Email, groupId);
            return foundGroupMember ?? throw new ApiException(ErrorConstants.GroupMemberDetailsNotFound, HttpStatusCode.NotFound);
        }
        public async Task<ICollection<Group>> GetAllMembersForUserIncludingGroup(User currentUser)
        {
            var allGroups = await _groupMemberRepo.GetAllGroupMembersForUserIncludingUserAndGroup(currentUser.Email);
            return allGroups?.Select(x => ) ?? throw new ApiException(ErrorConstants.NoGroupsFound, HttpStatusCode.NotFound);
        }
        public async Task<ICollection<GroupMember>> GetAllMembersForGroup(Guid groupId)
        {
            var allMembers = await _groupMemberRepo.GetMany(groupId, "groupId".ToPascalCase());
            return allMembers ?? throw new ApiException(ErrorConstants.NoGroupMembersFound, HttpStatusCode.NotFound);
        }
        public async Task<ICollection<GroupMember>> GetAllMembersForGroupIncludingUser(Guid groupId)
        {
            var allMembers = await _groupMemberRepo.GetManyGroupMembersIncludingUser(groupId);
            return allMembers ?? throw new ApiException(ErrorConstants.NoGroupMembersFound, HttpStatusCode.NotFound);
        }
        public async Task<ICollection<GroupMember>> GetAllMembersForGroupIncludingUserAndPosition(Guid groupId)
        {
            var allMembers = await _groupMemberRepo.GetManyGroupMemberIncludingUserAndPosition(groupId);
            return allMembers ?? throw new ApiException(ErrorConstants.NoGroupMembersFound, HttpStatusCode.NotFound);
        }
        public async Task<ICollection<GroupPosition>> GetAllPositionsForGroup(Guid groupId)
        {
            var allPositions = await _groupPositionRepo.GetMany(groupId, "groupId".ToPascalCase());
            return allPositions ?? throw new ApiException(ErrorConstants.NoGroupPositionsFound, HttpStatusCode.NotFound);
        }
        public async Task<GroupMember> UserJoinPublicGroup(User currentUser, Guid groupId)
        {
            var foundGroupMemberTask = _groupMemberRepo.;
            if (foundGroupMemberTask is not null)
            {
                throw new ApiException(ErrorConstants.UserAlreadyInGroup, HttpStatusCode.NotFound);
            }
            var foundGroup = await _repo.GetOne(groupId, "id".ToPascalCase());
            var userAddedToGroup = await _groupMemberRepo.Create(new List<GroupMember> { new GroupMember(groupId, currentUser.) });
        }

    }
}