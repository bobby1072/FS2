using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
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
        private readonly Type _entType = typeof(TEnt);
        public readonly IDbContextFactory<FsContext> DbContextFactory;
        public BaseRepository(IDbContextFactory<FsContext> dbContextFactory)
        {
            DbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
        }
        public virtual async Task<int> GetCount()
        {
            await using var dbContext = await DbContextFactory.CreateDbContextAsync();
            var foundOneQuerySet = dbContext.Set<TEnt>();
            return await foundOneQuerySet.CountAsync();
        }
        protected virtual IQueryable<TEnt> _addRelationsToQuery(IQueryable<TEnt> set, ICollection<string>? relationships = null)
        {
            if (relationships is null) return set;
            var newQuery = set;
            //Needs investigation
            var foundRelationProperties = _entType.GetProperties().Where(x => relationships?.Contains(x.Name.Replace("Entity", "")) == true).ToArray();
            for (var i = 0; i < foundRelationProperties.Length; i++)
            {
                var relation = foundRelationProperties[i];
                newQuery = newQuery.Include(relation.Name);
            }
            return newQuery;
        }
        public virtual async Task<ICollection<TBase>?> GetAll(params string[] relationships)
        {
            await using var dbContext = await DbContextFactory.CreateDbContextAsync();
            var foundAllQuerySet = dbContext.Set<TEnt>();
            var runtimeObj = await _addRelationsToQuery(foundAllQuerySet.AsQueryable(), relationships).ToArrayAsync();
            return runtimeObj?.Select(x => x.ToRuntime()).OfType<TBase>().ToArray();
        }
        public virtual async Task<TBase?> GetOne(TBase baseUser, ICollection<string>? relationships = null)
        {
            await using var dbContext = await DbContextFactory.CreateDbContextAsync();
            var foundOneQuerySet = dbContext.Set<TEnt>();
            var primaryKey = _runtimeToEntity(baseUser).GetType().GetProperties().FirstOrDefault(x => x.GetCustomAttribute<KeyAttribute>() is not null) ?? throw new Exception();
            var baseValuePrimaryKey = primaryKey.GetValue(_runtimeToEntity(baseUser));
            var runtimeObj = await GetOne(baseValuePrimaryKey, primaryKey.Name, relationships);
            if (runtimeObj is TBase correctOBj)
            {
                return correctOBj;
            }
            return null;
        }

        public virtual async Task<TBase?> GetOne<TField>(TField field, string fieldName, ICollection<string>? relationships = null)
        {
            var myProps = _entType.GetProperties();
            var foundDetail = myProps.FirstOrDefault(x => x.Name == fieldName.ToPascalCase() && typeof(TField).IsAssignableFrom(x.PropertyType));
            if (foundDetail is not null)
            {
                await using var dbContext = await DbContextFactory.CreateDbContextAsync();
                var set = dbContext.Set<TEnt>();
                var foundOne = await _addRelationsToQuery(set.AsQueryable(), relationships).FirstOrDefaultAsync(x => EF.Property<TField>(x, fieldName.ToPascalCase()).Equals(field));
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
            var myProps = _entType.GetProperties();
            var foundDetail = myProps.FirstOrDefault(x => x.Name == fieldName.ToPascalCase() && typeof(TField).IsAssignableFrom(x.PropertyType));
            if (foundDetail is not null)
            {
                await using var dbContext = await DbContextFactory.CreateDbContextAsync();
                var set = dbContext.Set<TEnt>();
                var foundMany = await _addRelationsToQuery(set, relationships).Where(x => EF.Property<TField>(x, fieldName.ToPascalCase()).Equals(field)).ToArrayAsync();
                var runtimeArray = foundMany?.Select(x => x.ToRuntime());
                return runtimeArray?.OfType<TBase>().ToArray();
            }
            else
            {
                throw new Exception(ErrorConstants.FieldNotFound);
            }
        }
        public virtual async Task<ICollection<TBase>?> GetMany(TBase baseObj, ICollection<string>? relationships = null)
        {
            await using var dbContext = await DbContextFactory.CreateDbContextAsync();
            var set = dbContext.Set<TEnt>();
            var foundMany = await _addRelationsToQuery(set, relationships).Where(x => x.ToRuntime().Equals(baseObj)).ToArrayAsync();
            var runtimeArray = foundMany?.Select(x => x.ToRuntime());
            return runtimeArray?.OfType<TBase>().ToArray();
        }
        public virtual async Task<ICollection<TBase>?> _getSomeLike<TField>(TField field, string fieldName, ICollection<string>? relationships = null)
        {
            var myProps = _entType.GetProperties();
            var foundDetail = myProps.FirstOrDefault(x => x.Name == fieldName.ToPascalCase() && typeof(TField).IsAssignableFrom(x.PropertyType));
            if (foundDetail is not null)
            {
                await using var dbContext = await DbContextFactory.CreateDbContextAsync();
                Expression<Func<TEnt, bool>> likePredicate = x =>
                           EF.Property<TField>(x, fieldName.ToPascalCase()).ToString().Contains(field.ToString());
                var set = dbContext.Set<TEnt>();
                var similarItems = await _addRelationsToQuery(set, relationships)
                    .Where(likePredicate)
                    .ToArrayAsync();
                var runtimeObj = similarItems?.Select(x => x.ToRuntime());
                return runtimeObj?.OfType<TBase>().ToArray();
            }
            else
            {
                throw new Exception(ErrorConstants.FieldNotFound);
            }
        }
        public virtual async Task<ICollection<TBase>?> Create(ICollection<TBase> entObj)
        {
            await using var dbContext = await DbContextFactory.CreateDbContextAsync();
            var set = dbContext.Set<TEnt>();
            await set.AddRangeAsync(entObj.Select(x => _runtimeToEntity(x)));
            await dbContext.SaveChangesAsync();
            var runtimeObjs = set.Local.Select(x => x.ToRuntime());
            return runtimeObjs?.Count() > 0 ? runtimeObjs.OfType<TBase>().ToArray() : null;

        }
        public virtual async Task<ICollection<TBase>?> Delete(ICollection<TBase> entObj)
        {
            await using var dbContext = await DbContextFactory.CreateDbContextAsync();
            var set = dbContext.Set<TEnt>();
            set.RemoveRange(entObj.Select(x => _runtimeToEntity(x)));
            await dbContext.SaveChangesAsync();
            return entObj;
        }
        public virtual async Task<ICollection<TBase>?> Update(ICollection<TBase> entObj)
        {
            await using var dbContext = await DbContextFactory.CreateDbContextAsync();
            var set = dbContext.Set<TEnt>();
            set.UpdateRange(entObj.Select(x => _runtimeToEntity(x)));
            await dbContext.SaveChangesAsync();
            var runtimeObjs = set.Local.Select(x => x.ToRuntime());
            return runtimeObjs?.Count() > 0 ? runtimeObjs.OfType<TBase>().ToArray() : null;

        }
        public virtual async Task<ICollection<TBase>?> GetMany(int startIndex, int count, string fieldNameToOrderBy, ICollection<string>? relations = null)
        {
            await using var dbContext = await DbContextFactory.CreateDbContextAsync();
            var set = dbContext.Set<TEnt>();
            var runtimeArray = (await _addRelationsToQuery(set, relations)
                .OrderBy(x => EF.Property<object>(x, fieldNameToOrderBy.ToPascalCase()))
                .Skip(startIndex)
                .Take(count)
            .ToArrayAsync()).Select(x => x.ToRuntime());
            return runtimeArray?.Count() > 0 ? runtimeArray?.OfType<TBase>().ToArray() : null;
        }
    }
}