using Microsoft.AspNetCore.Authorization;
using Services.Abstract;

namespace fsCore.Hubs
{
    [Authorize]
    public class LiveMatchHub : BaseHub
    {
        public const string UpdateMatchMessage = "UpdateMatch";
        public const string CreateMatchMessage = "CreateMatch";
        public LiveMatchHub(ICachingService cachingService) : base(cachingService) { }
    }
}