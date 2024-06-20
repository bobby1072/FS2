using Common.Models;

namespace fsCore.Service
{
    public abstract class BaseService<TBase, IRepo> where TBase : BaseModel
    {
        protected readonly IRepo _repo;
        public BaseService(IRepo repo)
        {
            _repo = repo;
        }
    }
}