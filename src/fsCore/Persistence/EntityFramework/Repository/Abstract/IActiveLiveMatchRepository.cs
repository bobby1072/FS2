using Common.Models;

namespace Persistence.EntityFramework.Repository.Abstract
{
    public interface IActiveLiveMatchRepository
    {
        Task<ICollection<Guid>> GetForUser(Guid userId);
        Task<ICollection<LiveMatch>?> Create(ICollection<LiveMatch> entities);
        Task<ICollection<LiveMatch>?> Delete(ICollection<LiveMatch> entities);
        Task<ICollection<LiveMatch>?> Update(ICollection<LiveMatch> entities);
        Task<LiveMatch?> GetFullOneById(Guid id);
    }
}