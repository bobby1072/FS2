using System.Text.Json.Serialization;

namespace Common.Models;

public record UserInfoResponse
{
    [JsonPropertyName("email")]
    public required string Email { get; init; }

    [JsonPropertyName("email_verified")]
    public required string EmailVerified { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }
}