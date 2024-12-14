using fsCore.Common.Models;

namespace fsCore.Persistence.EntityFramework.Repository.Abstract
{
    public interface IActiveLiveMatchRepository
    {
        Task<ICollection<LiveMatch>?> Create(ICollection<LiveMatch> entities);
        Task<ICollection<LiveMatch>?> Delete(ICollection<LiveMatch> entities);
        Task<ICollection<LiveMatch>?> Update(ICollection<LiveMatch> entities);
        Task<LiveMatch?> GetFullOneById(Guid id);
        Task<ICollection<LiveMatch>?> GetFullOneById(ICollection<Guid> ids);
    }
}