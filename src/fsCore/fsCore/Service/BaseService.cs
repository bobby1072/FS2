using Common.Models;

namespace fsCore.Service
{
    internal abstract class BaseService<TBase, IRepo> where TBase : BaseModel
    {
        protected readonly IRepo _repo;
        public BaseService(IRepo repo)
        {
            _repo = repo;
        }
        protected virtual ICollection<string>? _produceRelationsList(IDictionary<string, bool> bools)
        {
            var relations = new List<string>();
            foreach (var boolIndividual in bools)
            {
                if (boolIndividual.Value)
                {
                    relations.Add(boolIndividual.Key);
                }
            }
            return relations.Count > 0 ? relations : null;
        }
    }
}