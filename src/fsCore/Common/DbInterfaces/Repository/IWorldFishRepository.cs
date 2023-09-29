using Common.Models;

namespace Common.Dbinterfaces.Repository
{
    public interface IWorldFishRepository
    {
        Task<ICollection<WorldFish>> GetAll();
        Task Create(WorldFish fishToCreate);
        Task<WorldFish> Create(WorldFish fishToCreate, bool returnFish);
        Task<WorldFish?> GetOne(WorldFish fishToGet);
        Task<WorldFish?> GetOne(string fishTaxocode);
        Task Update(WorldFish fishToUpdate);
        Task<WorldFish> Update(WorldFish fishToUpdate, bool returnFish);
        Task Delete(WorldFish fishToDelete);
        Task<WorldFish> Delete(WorldFish fishToDelete, bool returnFish);
    }
}