using System.Text.Json.Serialization;
using Common.Attributes;

namespace Common.Models
{
    public class LiveMatchParticipant : User
    {
        [JsonPropertyName("online")]
        public bool Online { get; set; }
        [LockedProperty]
        [JsonPropertyName("dbId")]
        public int? DbId { get; set; }
        public LiveMatchParticipant(string email, bool emailVerified, string? name = null, string? username = null, Guid? id = null, bool online = false, int? dbId = null) : base(email, emailVerified, name, username, id)
        {
            Online = online;
            DbId = dbId;
        }
        public static LiveMatchParticipant? FromUser(User? user, int? dbId = null) => user is null ? null : new(user.Email, user.EmailVerified, user.Name, user.Username, user.Id, false, dbId);
        public static LiveMatchParticipant? FromUser(User? user, bool online, int? dbId = null) => user is null ? null : new(user.Email, user.EmailVerified, user.Name, user.Username, user.Id, online, dbId);

    }
}