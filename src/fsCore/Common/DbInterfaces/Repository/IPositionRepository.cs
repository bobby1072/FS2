using Common.Models;

namespace Common.Dbinterfaces.Repository
{
    public interface IPositionRepository
    {
        Task<ICollection<Position>?> GetAll();
        Task<ICollection<Position>?> Create(ICollection<Position> positionToCreate);
        Task<ICollection<Position>?> Update(ICollection<Position> positionToUpdate);
        Task<ICollection<Position>?> Delete(ICollection<Position> positionToDelete);
        Task<Position?> GetOne<T>(T field, string fieldName);
        Task<Position?> GetOne(Position position);
    }
}