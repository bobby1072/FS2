using System.Net;
using System.Text.Json.Serialization;
using Common.Permissions;
using Common.Utils;
namespace Common.Models
{
    public class User : BaseModel
    {
        [JsonPropertyName("emailVerified")]
        public bool EmailVerified { get; set; }
        private string _email;
        [JsonPropertyName("email")]
        public string Email
        {
            get => _email;
            set
            {
                if (!value.ToLower().IsValidEmail())
                {
                    throw new ApiException(ErrorConstants.InvalidEmail, HttpStatusCode.UnprocessableEntity);
                }
                _email = value.ToLower();
            }
        }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("username")]
        public string? Username { get; set; }
        public User(string email, bool emailVerified, string? name, string? username = null)
        {
            EmailVerified = emailVerified;
            Email = email;
            Name = name;
            Username = username ?? CalculateDefaultUsername();
        }
        public string CalculateDefaultUsername()
        {
            return Email.Split('@').FirstOrDefault() ?? Email;
        }
    }
    public class UserWithGroupPermissionSet : User
    {
        [JsonPropertyName("permissions")]
        public PermissionSet<Group> GroupPermissions { get; set; } = new PermissionSet<Group>();
        public UserWithGroupPermissionSet(User user) : base(user.Email, user.EmailVerified, user.Name) { }
        public UserWithGroupPermissionSet(string email, bool emailVerified, string? name, GroupMember? member = null) : base(email, emailVerified, name)
        {
            if (member is not null)
            {
                BuildPermissions(member);
            }
        }
        public UserWithGroupPermissionSet(string email, bool emailVerified, string? name, ICollection<GroupMember>? member = null) : base(email, emailVerified, name)
        {
            if (member is not null)
            {
                BuildPermissions(member);
            }
        }
        [JsonConstructor]
        public UserWithGroupPermissionSet(string email, bool emailVerified, string? name) : base(email, emailVerified, name) { }
        public UserWithGroupPermissionSet BuildPermissions(ICollection<Group> groups)
        {
            foreach (var group in groups)
            {
                BuildPermissions(group);
            }
            return this;
        }

        public UserWithGroupPermissionSet BuildPermissions(Group group)
        {
            if (group.LeaderEmail == Email)
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
            if (member.Position is null)
            {
                throw new Exception();
            }
            GroupPermissions
                .AddCan(PermissionConstants.BelongsTo, member.Group);
            if (member.Group.LeaderEmail == Email)
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
            foreach (var member in groupMembers)
            {
                BuildPermissions(member);
            }
            return this;
        }
    }
}