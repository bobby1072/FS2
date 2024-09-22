using Common.Attributes;
using Common.Permissions;
using System.Text.Json.Serialization;
namespace Common.Models
{
    public class UserWithoutEmail : BaseModel
    {
        [JsonPropertyName("id")]
        [LockedProperty]
        public Guid? Id { get; set; }
        [LockedProperty]
        [JsonPropertyName("emailVerified")]
        public bool EmailVerified { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("username")]
        public string Username { get; set; }
    }
    public class User : UserWithoutEmail
    {
        public const string CacheKeyPrefix = $"{nameof(User)}_";
        private string _email;
        [LockedProperty]
        [SensitiveProperty]
        [JsonPropertyName("email")]
        public string Email
        {
            get => _email?.ToLower();
            set
            {
                _email = value;
            }
        }
        public User(string email, bool emailVerified, string? name = null, string? username = null, Guid? id = null)
        {
            Id = id;
            EmailVerified = emailVerified;
            Email = email;
            Name = name;
            Username = username ?? CalculateDefaultUsername();
        }
        [JsonConstructor]
        public User() { }
        public string CalculateDefaultUsername()
        {
            return Email.Split('@').FirstOrDefault() ?? Email;
        }
        public override bool Equals(object? obj)
        {
            if (obj is User parsedUser)
            {
                return parsedUser.Email == Email && parsedUser.Id == Id && parsedUser.Name == Name && parsedUser.Username == Username;
            }
            return false;
        }
    }
    public class UserWithGroupPermissionSet : User
    {
        public new const string CacheKeyPrefix = $"{nameof(UserWithGroupPermissionSet)}_";
        [JsonPropertyName("permissions")]
        public GroupPermissionSet GroupPermissions { get; set; } = new GroupPermissionSet();
        public UserWithGroupPermissionSet(User user) : base(user.Email, user.EmailVerified, user.Name, user.Username, user.Id) { }
        [JsonConstructor]
        public UserWithGroupPermissionSet(string email, bool emailVerified, string? name, string username, Guid? id) : base(email, emailVerified, name, username, id) { }
        public UserWithGroupPermissionSet BuildPermissions(ICollection<Group> groups)
        {
            var groupsAsArray = groups.ToArray();
            for (var i = 0; i < groupsAsArray.Length; i++)
            {
                var group = groupsAsArray[i];
                group.Emblem = null;
                BuildPermissions(group);
            }
            return this;
        }

        public UserWithGroupPermissionSet BuildPermissions(Group group)
        {
            if (group.LeaderId == Id)
            {
                GroupPermissions
                    .AddCan(PermissionConstants.BelongsTo, group)
                    .AddCan(PermissionConstants.Manage, group)
                    .AddCan(PermissionConstants.Read, group);
            }
            return this;
        }
        public UserWithGroupPermissionSet BuildPermissions(GroupMember member)
        {
            if (member.Group is null)
            {
                throw new Exception();
            }
            member.Group.Emblem = null;
            if (member.Position is null)
            {
                throw new Exception();
            }
            GroupPermissions
                .AddCan(PermissionConstants.BelongsTo, member.Group);
            if (member.Group.LeaderId == Id)
            {
                GroupPermissions
                    .AddCan(PermissionConstants.Manage, member.Group)
                    .AddCan(PermissionConstants.Read, member.Group);
                return this;
            }
            if (member.Position.CanManageGroup)
            {
                GroupPermissions
                    .AddCan(PermissionConstants.Read, member.Group)
                    .AddCan(PermissionConstants.Manage, member.Group);
                return this;
            }
            if (member.Position.CanManageCatches)
            {
                GroupPermissions.AddCan(PermissionConstants.Manage, member.Group, nameof(GroupCatch));
            }
            if (member.Position.CanReadCatches)
            {
                GroupPermissions.AddCan(PermissionConstants.Read, member.Group, nameof(GroupCatch));
            }
            if (member.Position.CanManageMembers)
            {
                GroupPermissions.AddCan(PermissionConstants.Manage, member.Group, nameof(GroupMember));
            }
            if (member.Position.CanReadMembers)
            {
                GroupPermissions.AddCan(PermissionConstants.Read, member.Group, nameof(GroupMember));
            }
            return this;
        }
        public UserWithGroupPermissionSet BuildPermissions(ICollection<GroupMember> groupMembers)
        {
            var groupMembersArray = groupMembers.ToArray();
            for (var i = 0; i < groupMembersArray.Length; i++)
            {
                var member = groupMembersArray[i];
                BuildPermissions(member);
            }
            return this;
        }
    }
    public class GroupPermissionSet : PermissionSet<Group>
    {
        public Group? FindGroupAssociatedWithUser(Guid groupId)
        {
            return Abilities.FirstOrDefault(x => x.Subject.Id == groupId)?.Subject;
        }
        public bool Can(string action, Guid groupId)
        {
            return Abilities.Any(x => x.Action == action && x.Subject.Id == groupId && x.Fields is null);
        }
        public bool Can(string action, Guid groupId, string field)
        {
            return Abilities.Any(x => x.Action == action && x.Subject.Id == groupId && (x.Fields is null || x.Fields?.Contains(field) == true));
        }

    }
}