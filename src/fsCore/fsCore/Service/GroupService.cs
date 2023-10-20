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
        public GroupService(IGroupRepository repository,
        IGroupMemberRepository groupMemberRepo,
        IGroupPositionRepository groupPositionRepo) : base(repository)
        {
            _groupMemberRepo = groupMemberRepo;
            _groupPositionRepo = groupPositionRepo;
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
        public async Task<ICollection<GroupMember>> GetAllMembersForUserIncludingGroupAndUserAndPosition(User currentUser)
        {
            var allGroups = await _groupMemberRepo.GetManyGroupMemberForUserIncludingUserAndPositionAndGroup(currentUser.Email);
            return allGroups ?? throw new ApiException(ErrorConstants.NoGroupsFound, HttpStatusCode.NotFound);
        }
        public Task<ICollection<GroupMember>>? TryGetAllMembersForUserIncludingGroupAndUserAndPosition(User currentUser)
        {
            try
            {
                return GetAllMembersForUserIncludingGroupAndUserAndPosition(currentUser);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<ICollection<GroupMember>> GetAllMembersForGroup(Guid groupId)
        {
            var allMembers = await _groupMemberRepo.GetMany(groupId, "groupId".ToPascalCase());
            return allMembers ?? throw new ApiException(ErrorConstants.NoGroupMembersFound, HttpStatusCode.NotFound);
        }
        public async Task<ICollection<GroupMember>> GetAllMembersForGroupIncludingUser(Guid groupId)
        {
            var allMembers = await _groupMemberRepo.GetMany(groupId, "groupId".ToPascalCase());
            return allMembers ?? throw new ApiException(ErrorConstants.NoGroupMembersFound, HttpStatusCode.NotFound);
        }
        public async Task<ICollection<GroupMember>> GetAllMembersForGroupIncludingUserAndPosition(Guid groupId)
        {
            var allMembers = await _groupMemberRepo.GetManyGroupMembersIncludingUserAndPosition(groupId);
            return allMembers ?? throw new ApiException(ErrorConstants.NoGroupMembersFound, HttpStatusCode.NotFound);
        }
        public async Task<ICollection<GroupPosition>> GetAllPositionsForGroup(Guid groupId)
        {
            var allPositions = await _groupPositionRepo.GetMany(groupId, "groupId".ToPascalCase());
            return allPositions ?? throw new ApiException(ErrorConstants.NoGroupPositionsFound, HttpStatusCode.NotFound);
        }
        public async Task<GroupMember> UserJoinPublicGroup(GroupMember member)
        {
            var foundGroup = await _repo.GetOne(member.GroupId, "id".ToPascalCase()) ?? throw new ApiException(ErrorConstants.NoGroupsFound, HttpStatusCode.NotFound);
            var foundGroupMember = await _groupMemberRepo.GetOne(foundGroup.Id, "groupId".ToPascalCase());
            if (foundGroupMember is not null)
            {
                throw new ApiException(ErrorConstants.UserAlreadyInGroup, HttpStatusCode.BadRequest);
            }
            return (await _groupMemberRepo.Create(new List<GroupMember> { new(foundGroup.Id, member.UserEmail, member.PositionId) }))?.FirstOrDefault() ?? throw new ApiException(ErrorConstants.CouldntCreateGroupMember, HttpStatusCode.InternalServerError);
        }
        public async Task<GroupMember> UserLeavePublicGroup(User currentUser, Guid groupId)
        {
            var foundGroupMember = await _groupMemberRepo.GetGroupMemberIncludingUser(currentUser.Email, groupId) ?? throw new ApiException(ErrorConstants.NoGroupMembersFound, HttpStatusCode.BadRequest);
            return (await _groupMemberRepo.Delete(new List<GroupMember> { foundGroupMember }))?.FirstOrDefault() ?? throw new ApiException(ErrorConstants.CouldntDeleteGroupMember, HttpStatusCode.InternalServerError);
        }
        public async Task<GroupMember> UserChangePositionInGroup(GroupMember newMember)
        {
            var foundGroupMember = await _groupMemberRepo.GetGroupMemberIncludingUser(newMember.UserEmail, newMember.GroupId) ?? throw new ApiException(ErrorConstants.NoGroupMembersFound, HttpStatusCode.BadRequest);
            return (await _groupMemberRepo.Delete(new List<GroupMember> { foundGroupMember }))?.FirstOrDefault() ?? throw new ApiException(ErrorConstants.CouldntUpdateGroupMember, HttpStatusCode.InternalServerError);
        }
    }
}