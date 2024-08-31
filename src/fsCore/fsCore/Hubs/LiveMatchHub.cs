using Microsoft.AspNetCore.Authorization;
using Services.Abstract;
using fsCore.Attributes;
namespace fsCore.Hubs
{
    [Authorize]
    [RequiredUser]
    public class LiveMatchHub : BaseHub
    {
        public const string UpdateMatchMessage = "UpdateMatch";
        public const string CreateMatchMessage = "CreateMatch";
        public LiveMatchHub(ICachingService cachingService) : base(cachingService) { }
    }
}