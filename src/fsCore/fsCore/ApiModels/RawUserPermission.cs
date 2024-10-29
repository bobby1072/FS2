using Common.Models;
using Common.Permissions;
using System.Text.Json.Serialization;
namespace fsCore.ApiModels
{
    public class RawUserPermission : User
    {
        public RawUserPermission(User user) : base(user.Email, user.EmailVerified, user.Name, user.Username, user.Id) { }
        [JsonConstructor]
        public RawUserPermission(string email, bool emailVerified, string? name, string username, Guid? id) : base(email, emailVerified, name, username, id) { }
        [JsonPropertyName("groupPermissions")]
        public PermissionSet<Guid> GroupPermissions { get; set; } = new PermissionSet<Guid>();
    }
}