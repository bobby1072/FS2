using Common.Models;

namespace fsCore.Services.Concrete
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