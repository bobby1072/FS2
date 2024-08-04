using Common.Models;

namespace Services.Abstract
{
    public abstract class BaseService<TBase, IRepo> where TBase : BaseModel
    {
        protected readonly IRepo _repo;
        protected BaseService(IRepo repo)
        {
            _repo = repo;
        }
    }
}