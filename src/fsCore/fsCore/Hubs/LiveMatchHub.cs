using Microsoft.AspNetCore.Authorization;
using fsCore.Attributes;
using Services.Abstract;
namespace fsCore.Hubs
{
    [Authorize]
    [RequiredUser]
    [RequiredUserWithGroupPermissions]
    public class LiveMatchHub : BaseHub
    {
        public LiveMatchHub(ICachingService cachingService) : base(cachingService) { }
    }
}