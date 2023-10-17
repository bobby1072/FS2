using System.Linq.Expressions;
using Common;
using Common.Models;
using Common.Utils;
using Microsoft.EntityFrameworkCore;
using Persistence.EntityFramework.Entity;

namespace Persistence.EntityFramework.Repository
{
    internal abstract class BaseRepository<TEnt, TBase> where TEnt : BaseEntity<TBase> where TBase : BaseModel
    {
        protected abstract TEnt _runtimeToEntity(TBase runtimeObj);
        protected readonly IDbContextFactory<FsContext> _dbContextFactory;
        public BaseRepository(IDbContextFactory<FsContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
        }
        public virtual async Task<ICollection<TBase>?> GetAll()
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var foundAllEnts = await dbContext.Set<TEnt>().ToArrayAsync();
            var runtimeArray = foundAllEnts?.Select(x => x.ToRuntime());
            return runtimeArray?.OfType<TBase>().ToList();
        }
        public virtual async Task<TBase?> GetOne(TBase baseUser)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var foundOne = await dbContext.Set<TEnt>().FirstOrDefaultAsync(x => x.ToRuntime().Equals(baseUser));
            var runtimeObj = foundOne?.ToRuntime();
            if (runtimeObj is TBase correctOBj)
            {
                return correctOBj;
            }
            return null;
        }
        public virtual async Task<TBase?> GetOne<TField>(TField field, string fieldName)
        {
            var myProps = typeof(TEnt).GetProperties();
            var foundDetail = myProps.FirstOrDefault(x =>
            {
                var xType = x.GetType();
                return x.Name == fieldName.ToPascalCase() && typeof(TField) == x.PropertyType;
            });
            if (foundDetail is not null)
            {
                await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
                var foundOne = await dbContext.Set<TEnt>().FirstOrDefaultAsync(x => EF.Property<TField>(x, fieldName.ToPascalCase()).Equals(field));
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
        public virtual async Task<ICollection<TBase>?> GetMany<TField>(TField field, string fieldName)
        {
            var myProps = typeof(TEnt).GetProperties();
            var foundDetail = myProps.FirstOrDefault(x =>
            {
                var xType = x.GetType();
                return x.Name == fieldName.ToPascalCase() && typeof(TField) == x.PropertyType;
            });
            if (foundDetail is not null)
            {
                await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
                var foundMany = await dbContext.Set<TEnt>().Where(x => EF.Property<TField>(x, fieldName.ToPascalCase()).Equals(field)).ToArrayAsync();
                var runtimeArray = foundMany?.Select(x => x.ToRuntime());
                return runtimeArray?.OfType<TBase>().ToList();
            }
            else
            {
                throw new Exception(ErrorConstants.FieldNotFound);
            }
        }
        public virtual async Task<ICollection<TBase>?> GetMany(TBase baseObj)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var foundMany = await dbContext.Set<TEnt>().Where(x => x.ToRuntime().Equals(baseObj)).ToArrayAsync();
            var runtimeArray = foundMany?.Select(x => x.ToRuntime());
            return runtimeArray?.OfType<TBase>().ToList();
        }
        public virtual async Task<ICollection<TBase>?> _getSomeLike<TField>(TField field, string fieldName)
        {
            var myProps = typeof(TEnt).GetProperties();
            var foundDetail = myProps.FirstOrDefault(x =>
            {
                var xType = x.GetType();
                return x.Name == fieldName.ToPascalCase() && typeof(TField) == x.PropertyType;
            });
            if (foundDetail is not null)
            {
                await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
                Expression<Func<TEnt, bool>> likePredicate = x =>
                           EF.Property<TField>(x, fieldName.ToPascalCase()).ToString().Contains(field.ToString());

                var similarItems = await dbContext.Set<TEnt>()
                    .Where(likePredicate)
                    .ToListAsync();
                var runtimeObj = similarItems?.Select(x => x.ToRuntime());
                return runtimeObj?.OfType<TBase>().ToList();
            }
            else
            {
                throw new Exception(ErrorConstants.FieldNotFound);
            }
        }
        public virtual async Task<ICollection<TBase>?> Create(ICollection<TBase> entObj)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var set = dbContext.Set<TEnt>();
            await set.AddRangeAsync(entObj.Select(x => _runtimeToEntity(x)));
            await dbContext.SaveChangesAsync();
            var runtimeObjs = set.Local.Select(x => x.ToRuntime()).ToArray();
            return runtimeObjs?.Length > 0 ? runtimeObjs.OfType<TBase>().ToList() : null;

        }
        public virtual async Task<ICollection<TBase>?> Delete(ICollection<TBase> entObj)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var set = dbContext.Set<TEnt>();
            set.RemoveRange(entObj.Select(x => _runtimeToEntity(x)));
            await dbContext.SaveChangesAsync();
            var runtimeObjs = set.Local.Select(x => x.ToRuntime()).ToArray();
            return runtimeObjs?.Length > 0 ? runtimeObjs.OfType<TBase>().ToList() : null;


        }
        public virtual async Task<ICollection<TBase>?> Update(ICollection<TBase> entObj)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var set = dbContext.Set<TEnt>();
            set.UpdateRange(entObj.Select(x => _runtimeToEntity(x)));
            await dbContext.SaveChangesAsync();
            var runtimeObjs = set.Local.Select(x => x.ToRuntime()).ToArray();
            return runtimeObjs?.Length > 0 ? runtimeObjs.OfType<TBase>().ToList() : null;

        }
    }
}