using System.Text.Json.Serialization;

namespace fsCore.RequestModels
{
    public record HubResponse<TData>
    {
        [JsonPropertyName("isSuccess")]
        public bool IsSuccess { get; init; }
        [JsonPropertyName("status")]
        public int Status { get; init; }
        [JsonPropertyName("data")]
        public TData? Data { get; init; }
        [JsonPropertyName("errorMessage")]
        public string? ErrorMessage { get; init; }
        public HubResponse(int status, TData? data, bool isSuccess = true)
        {
            IsSuccess = isSuccess;
            Status = status;
            Data = data;
        }
        public HubResponse(int status, string errorMessage, bool isSuccess = false)
        {
            IsSuccess = isSuccess;
            Status = status;
            ErrorMessage = errorMessage;
        }
    }
}