using System.Text.Json.Serialization;
using Common.Models;
using Common.Permissions;
namespace fsCore.Controllers.ControllerModels
{
    public class RawUserPermission : User
    {
        public RawUserPermission(User user) : base(user.Email, user.EmailVerified, user.Name, user.Username, user.Id) { }
        [JsonConstructor]
        public RawUserPermission(string email, bool emailVerified, string? name, string username, Guid? id) : base(email, emailVerified, name, username, id) { }
        [JsonPropertyName("groupPermissions")]
        public PermissionSet<Guid> GroupPermissions { get; set; } = new PermissionSet<Guid>();
        public static RawUserPermission FromUserWithPermissions(UserWithGroupPermissionSet user)
        {
            var rawUser = new RawUserPermission(user);
            rawUser.GroupPermissions.Abilities = user.GroupPermissions.Abilities.Select(x => new Permission<Guid> { Action = x.Action, Fields = x.Fields, Subject = x.Subject.Id ?? throw new Exception() }).ToArray();
            return rawUser;
        }
    }
}