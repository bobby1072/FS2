namespace fsCore.Common.Misc.Abstract
{
    public interface ILiveMatchHubContextServiceProvider
    {
        Task UpdateMatchForClients(Guid matchId);
    }
}