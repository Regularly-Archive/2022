using DomainEvents.Domains;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DomainEvents.Infrastructure
{
    public class Repository<TEntity>: IRepository<TEntity> where TEntity : class, IEntity
    {
        private readonly DbContext _context;
        public Repository(DbContext context)
        {
            _context = context;
        }

        public void Delete(params TEntity[] entities)
        {
            _context.Set<TEntity>().RemoveRange(entities);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _context.Set<TEntity>().ToList();
        }

        public IEnumerable<TEntity> GetByQuery(Expression<Func<TEntity, bool>> exps)
        {
            return _context.Set<TEntity>().Where(exps).ToList();
        }

        public void Insert(params TEntity[] entities)
        {
            _context.Set<TEntity>().AddRange(entities);
        }

        public void Update(params TEntity[] entities)
        {
            _context.Set<TEntity>().UpdateRange(entities);
        }
    }

    public class Repository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>
    {
        private readonly ChinookContext _context;
        public Repository(ChinookContext context)
        {
            _context = context;
        }

        public void Delete(params TKey[] keys)
        {
            var entities = _context.Set<TEntity>().Where(x => keys.Contains(x.Id)).ToList();
            _context.Set<TEntity>().RemoveRange(entities);
        }

        public void Delete(params TEntity[] entities)
        {
            _context.Set<TEntity>().RemoveRange(entities);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _context.Set<TEntity>().ToList();
        }

        public TEntity GetByKey(TKey key)
        {
            return _context.Set<TEntity>().SingleOrDefault(x => x.Id.Equals(key));
        }

        public IEnumerable<TEntity> GetByQuery(Expression<Func<TEntity, bool>> exps)
        {
            return _context.Set<TEntity>().Where(exps).ToList();
        }

        public void Insert(params TEntity[] entities)
        {
            _context.Set<TEntity>().AddRange(entities);
        }

        public void Update(params TEntity[] entities)
        {
            _context.Set<TEntity>().UpdateRange(entities);
        }
    }
}
