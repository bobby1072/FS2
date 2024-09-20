namespace Common.Models
{
    public class LiveMatchParticipant : User
    {
        public bool Online { get; set; }
        public LiveMatchParticipant(string email, bool emailVerified, string? name = null, string? username = null, Guid? id = null, bool online = false) : base(email, emailVerified, name, username, id)
        {
            Online = online;
        }
        public static LiveMatchParticipant? FromUser(User? user) => user is null ? null : new(user.Email, user.EmailVerified, user.Name, user.Username, user.Id, false);
        public static LiveMatchParticipant? FromUser(User? user, bool online) => user is null ? null : new(user.Email, user.EmailVerified, user.Name, user.Username, user.Id, online);

    }
}