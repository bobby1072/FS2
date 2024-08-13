using Common.Models;

namespace fsCore.Services.Abstract
{
    public interface ILiveMatchService
    {
        Task<LiveMatch> SaveMatch(LiveMatch match, UserWithGroupPermissionSet currentUser);

    }
}