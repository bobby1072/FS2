using Common.Models;

namespace fsCore.Service
{
    public abstract class BaseService<TBase, IRepo> where TBase : BaseRuntime
    {
        protected readonly IRepo _repo;
        public BaseService(IRepo repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }
    }
}