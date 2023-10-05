using Common;
using Common.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.EntityFramework.Entity;

namespace Persistence.EntityFramework.Repository
{
    internal abstract class BaseRepository<TEnt, TBase> where TEnt : BaseEntity where TBase : BaseRuntime
    {
        protected readonly IDbContextFactory<FsContext> _dbContextFactory;
        public BaseRepository(IDbContextFactory<FsContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
        }
        public async Task<ICollection<TBase>> GetAll()
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var foundAllEnts = await dbContext.Set<TEnt>().ToArrayAsync();
            var runtimeArray = foundAllEnts?.Select(x => x.ToRuntime()).ToArray();
            if (runtimeArray is ICollection<TBase> correctArray)
            {
                return correctArray;
            }
            return new List<TBase>();
        }
        public async Task<TBase?> GetOne<TField>(TField field, string fieldName)
        {
            var myProps = typeof(TEnt).GetProperties();
            var foundDetail = myProps.FirstOrDefault(x =>
            {
                var xType = x.GetType();
                return x.Name == fieldName && typeof(TField) == x.PropertyType;
            });
            if (foundDetail != null)
            {
                await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
                var foundOne = await dbContext.Set<TEnt>().FirstOrDefaultAsync(x => EF.Property<TField>(x, fieldName).Equals(field));
                var runtimeObj = foundOne?.ToRuntime();
                if (runtimeObj is TBase correctOBj)
                {
                    return correctOBj;
                }
                return null;
            }
            else
            {
                throw new Exception(ErrorConstants.FieldNotFound);
            }
        }
        public async Task<ICollection<TBase>?> _create(ICollection<TEnt> entObj)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var set = dbContext.Set<TEnt>();
            await set.AddRangeAsync(entObj);
            await dbContext.SaveChangesAsync();
            var createdRuntime = set.Local.FirstOrDefault()?.ToRuntime();
            if (createdRuntime is ICollection<TBase> correctRuntime)
            {
                return correctRuntime;
            }
            return null;

        }
        public async Task<ICollection<TBase>?> _delete(ICollection<TEnt> entObj)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var set = dbContext.Set<TEnt>();
            set.RemoveRange(entObj);
            await dbContext.SaveChangesAsync();
            var createdRuntime = set.Local.FirstOrDefault()?.ToRuntime();
            if (createdRuntime is ICollection<TBase> correctRuntime)
            {
                return correctRuntime;
            }
            return null;

        }
        public async Task<ICollection<TBase>?> _update(ICollection<TEnt> entObj)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var set = dbContext.Set<TEnt>();
            set.UpdateRange(entObj);
            await dbContext.SaveChangesAsync();
            var createdRuntime = set.Local.FirstOrDefault()?.ToRuntime();
            if (createdRuntime is ICollection<TBase> correctRuntime)
            {
                return correctRuntime;
            }
            return null;

        }
    }
}