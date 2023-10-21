using System.Linq.Expressions;

namespace CollaborateMusicAPI.Repositories;

public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity> CreateAsync(TEntity entity);
    Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<TEntity> GetlAllAsync(Expression<Func<TEntity, bool>> predicate);  // I suspect there might be a typo here, perhaps you meant GetAllByPredicateAsync or something similar?
    Task<TEntity> UpdateAsync(Expression<Func<TEntity, bool>> predicate, TEntity entity);
    Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> predicate);
}

public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    public virtual async Task<TEntity> CreateAsync(TEntity entity)
    {
        return null!;
    }

    public virtual async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return null!;
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return null!;
    }

    public virtual async Task<TEntity> GetlAllAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return null!;
    }

    public virtual async Task<TEntity> UpdateAsync(Expression<Func<TEntity, bool>> predicate, TEntity entity)
    {
        return null!;
    }

    public virtual async Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return false;
    }
}
