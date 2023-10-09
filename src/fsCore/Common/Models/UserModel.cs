using System.Net;
using System.Text.Json.Serialization;
using Common.Utils;
namespace Common.Models
{
    public class User : BaseModel
    {
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
        public User(string email, string? name)
        {
            Email = email;
            Name = name;
        }
    }
}