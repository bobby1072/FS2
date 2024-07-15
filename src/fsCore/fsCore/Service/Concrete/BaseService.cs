using Common.Models;

namespace fsCore.Service.Concrete
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