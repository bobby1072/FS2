using System.Net;
using System.Text.Json.Serialization;
using Common.Misc;
using Common.Models;

namespace fsCore.RequestModels
{
    internal record HubResponse<TData>
    {
        [JsonPropertyName("isSuccess")]
        public bool IsSuccess { get; init; }
        [JsonPropertyName("status")]
        public int Status { get; init; }
        [JsonPropertyName("data")]
        public TData? Data { get; init; }
        public HubResponse(int status, TData? data, bool isSuccess = true)
        {
            IsSuccess = isSuccess;
            Status = status;
            Data = data;
        }
    }
    internal record HubResponse
    {
        [JsonPropertyName("isSuccess")]
        public bool IsSuccess { get; init; }
        [JsonPropertyName("status")]
        public int Status { get; init; }
        [JsonPropertyName("errorMessage")]
        public string? ErrorMessage { get; init; }
        public HubResponse(int status, string errorMessage, bool isSuccess = false)
        {
            IsSuccess = isSuccess;
            Status = status;
            ErrorMessage = errorMessage;
        }
    }
    internal static class HubResponseBuilder
    {
        public static HubResponse<LiveMatch> FromLiveMatch(LiveMatch liveMatch) => new((int)HttpStatusCode.OK, liveMatch);
        public static HubResponse<ICollection<LiveMatch>> FromLiveMatch(ICollection<LiveMatch> liveMatch) => new((int)HttpStatusCode.OK, liveMatch);
        public static HubResponse FromError(ApiException exception) => new((int)exception.StatusCode, exception.Message);
        public static HubResponse FromError(Exception exception) => new((int)HttpStatusCode.InternalServerError, exception.Message);
    }
}