using System.Net;
using System.Text.Json.Serialization;
using Common.GroupPermissions;
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
        [JsonConstructor]
        public User(string email, bool emailVerified, string? name)
        {
            EmailVerified = emailVerified;
            Email = email;
            Name = name;
        }
    }
    public class UserWithGroupPermissionSet : User
    {
        public readonly PermissionSet Permissions;
        public UserWithGroupPermissionSet(string email, bool emailVerified, string? name) : base(email, emailVerified, name)
        {
            Permissions = PermissionSet.CreateSet();
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
            if (member.Group.LeaderEmail == Email)
            {
                Permissions
                    .AddCan(PermissionConstants.Manage, member.Group);
                return this;
            }
            if (member.Position.CanManageGroup)
            {
                Permissions.AddCan(PermissionConstants.Manage, member.Group);
                return this;
            }
            if (member.Position.CanManageCatches)
            {
                Permissions.AddCan(PermissionConstants.Manage, member.Group, "Catches");
            }
            if (member.Position.CanReadCatches)
            {
                Permissions.AddCan(PermissionConstants.Read, member.Group, "Catches");
            }
            if (member.Position.CanManageMembers)
            {
                Permissions.AddCan(PermissionConstants.Manage, member.Group, "Members");
            }
            if (member.Position.CanReadMembers)
            {
                Permissions.AddCan(PermissionConstants.Read, member.Group, "Members");
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