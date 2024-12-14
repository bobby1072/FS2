using fsCore.Common.Models;

namespace fsCore.Persistence.EntityFramework.Repository.Abstract
{
    public interface IActiveLiveMatchCatchRepository
    {
        Task<ICollection<LiveMatchCatch>?> Create(ICollection<LiveMatchCatch> entities);
        Task<ICollection<LiveMatchCatch>?> Delete(ICollection<LiveMatchCatch> entities);
        Task<ICollection<LiveMatchCatch>?> Update(ICollection<LiveMatchCatch> entities);
    }
}