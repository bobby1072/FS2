using System.Net;
using System.Text.Json.Serialization;
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
}