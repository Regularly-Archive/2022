using System.Linq.Expressions;

namespace DomainEvents.Infrastructure
{
    public interface IRepository<TEntity> where TEntity : class, IEntity
    {
        void Insert(params TEntity[] entities);

        void Update(params TEntity[] entities);

        void Delete(params TEntity[] entities);

        IEnumerable<TEntity> GetByQuery(Expression<Func<TEntity, bool>> exps);

        IEnumerable<TEntity> GetAll();
    }

    public interface IRepository<TEntity,TKey> where TEntity:class, IEntity<TKey>
    {
        TEntity GetByKey(TKey key);

        void Insert(params TEntity[] entities);

        void Update(params TEntity[] entities);

        void Delete(params TKey[] keys);

        void Delete(params TEntity[] entities);

        IEnumerable<TEntity> GetByQuery(Expression<Func<TEntity, bool>> exps);

        IEnumerable<TEntity> GetAll();
    }
}
