using System.Text.Json.Serialization;
using Common.Permissions;
using Common.Attributes;
using Common.Models.Validators;
using FluentValidation;
namespace Common.Models
{
    public class User : BaseModel
    {
        private static readonly UserValidator _validator = new();
        [JsonPropertyName("id")]
        [LockedProperty]
        public Guid? Id { get; set; }
        [LockedProperty]
        [JsonPropertyName("emailVerified")]
        public bool EmailVerified { get; set; }
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
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("username")]
        public string Username { get; set; }
        public User(string email, bool emailVerified, string? name = null, string? username = null, Guid? id = null)
        {
            Id = id;
            EmailVerified = emailVerified;
            Email = email;
            Name = name;
            Username = username ?? CalculateDefaultUsername();
            _validator.ValidateAndThrow(this);
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
        public UserWithGroupPermissionSet(User user) : base(user.Email, user.EmailVerified, user.Name, user.Username, user.Id) { }
        [JsonConstructor]
        public UserWithGroupPermissionSet(string email, bool emailVerified, string? name, string username, Guid? id) : base(email, emailVerified, name, username, id) { }
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
            if (member.Position is null)
            {
                throw new Exception();
            }
            GroupPermissions
                .AddCan(PermissionConstants.BelongsTo, member.Group);
            if (member.Group.Id == Id)
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