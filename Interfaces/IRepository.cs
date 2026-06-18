using System.Linq.Expressions;

namespace Zariya.Interfaces;

public interface IRepository<TEntity> where TEntity : class
{
    IQueryable<TEntity> Query();
    Task<TEntity?> GetByIdAsync(int id);
    Task<List<TEntity>> ListAsync(Expression<Func<TEntity, bool>>? predicate = null);
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);
    Task AddAsync(TEntity entity);
    void Update(TEntity entity);
    void Remove(TEntity entity);
    void RemoveRange(IEnumerable<TEntity> entities);
    Task<int> SaveChangesAsync();
}
