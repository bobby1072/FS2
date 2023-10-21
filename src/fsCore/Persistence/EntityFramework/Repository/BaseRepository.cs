using System.Linq.Expressions;
using Common;
using Common.Models;
using Common.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
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
        private IQueryable<TEnt> _addRelationsToQuery(DbSet<TEnt> set, ICollection<string>? relationships = null)
        {
            IQueryable<TEnt>? newQuery = null;
            var foundRelationProperties = typeof(TEnt).GetProperties().Where(x => relationships?.Contains(x.Name) == true);
            int index = 0;
            foreach (var relation in foundRelationProperties)
            {
                if (index == 0)
                {
                    newQuery = set.Include(relation.Name);
                    index++;
                    continue;
                }
                newQuery = newQuery?.Include(relation.Name);
                index++;
            }
            return newQuery is not null ? newQuery : set;
        }
        public virtual async Task<ICollection<TBase>?> GetAll(ICollection<string>? relationships = null)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var foundAllQuerySet = dbContext.Set<TEnt>();
            var runtimeObj = await _addRelationsToQuery(foundAllQuerySet, relationships).ToArrayAsync();
            return runtimeObj?.OfType<TBase>().ToList();
        }
        public virtual async Task<TBase?> GetOne(TBase baseUser, ICollection<string>? relationships = null)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var foundOneQuerySet = dbContext.Set<TEnt>();
            var runtimeObj = await _addRelationsToQuery(foundOneQuerySet, relationships).FirstOrDefaultAsync(x => x.ToRuntime().Equals(baseUser));
            if (runtimeObj is TBase correctOBj)
            {
                return correctOBj;
            }
            return null;
        }
        public virtual async Task<TBase?> GetOne<TField>(TField field, string fieldName, ICollection<string>? relationships = null)
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
                var set = dbContext.Set<TEnt>();
                var foundOne = await _addRelationsToQuery(set, relationships).FirstOrDefaultAsync(x => EF.Property<TField>(x, fieldName.ToPascalCase()).Equals(field));
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
        public virtual async Task<ICollection<TBase>?> GetMany<TField>(TField field, string fieldName, ICollection<string>? relationships = null)
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
                var set = dbContext.Set<TEnt>();
                var foundMany = await _addRelationsToQuery(set, relationships).Where(x => EF.Property<TField>(x, fieldName.ToPascalCase()).Equals(field)).ToArrayAsync();
                var runtimeArray = foundMany?.Select(x => x.ToRuntime());
                return runtimeArray?.OfType<TBase>().ToList();
            }
            else
            {
                throw new Exception(ErrorConstants.FieldNotFound);
            }
        }
        public virtual async Task<ICollection<TBase>?> GetMany(TBase baseObj, ICollection<string>? relationships = null)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var set = dbContext.Set<TEnt>();
            var foundMany = await _addRelationsToQuery(set, relationships).Where(x => x.ToRuntime().Equals(baseObj)).ToArrayAsync();
            var runtimeArray = foundMany?.Select(x => x.ToRuntime());
            return runtimeArray?.OfType<TBase>().ToList();
        }
        public virtual async Task<ICollection<TBase>?> _getSomeLike<TField>(TField field, string fieldName, ICollection<string>? relationships = null)
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
                var set = dbContext.Set<TEnt>();
                var similarItems = await _addRelationsToQuery(set, relationships)
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