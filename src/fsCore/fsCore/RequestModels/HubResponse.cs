using System.Text.Json.Serialization;

namespace fsCore.RequestModels
{
    public record HubResponse
    {
        [JsonPropertyName("isSuccess")]
        public bool IsSuccess { get; init; }
        [JsonPropertyName("status")]
        public int Status { get; init; }
        [JsonPropertyName("message")]
        public object? Data { get; init; }
        [JsonPropertyName("errorMessage")]
        public string? ErrorMessage { get; init; }
        public HubResponse(int status, object data)
        {
            IsSuccess = true;
            Status = status;
            Data = data;
        }
        public HubResponse(int status, string errorMessage)
        {
            IsSuccess = false;
            Status = status;
            ErrorMessage = errorMessage;
        }
    }
}