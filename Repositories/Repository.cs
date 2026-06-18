using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Zariya.Data;
using Zariya.Interfaces;

namespace Zariya.Repositories;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly ZariyaDbContext Context;
    protected readonly DbSet<TEntity> Set;

    public Repository(ZariyaDbContext context)
    {
        Context = context;
        Set = context.Set<TEntity>();
    }

    public IQueryable<TEntity> Query() => Set.AsQueryable();

    public async Task<TEntity?> GetByIdAsync(int id) => await Set.FindAsync(id);

    public async Task<List<TEntity>> ListAsync(Expression<Func<TEntity, bool>>? predicate = null)
    {
        var query = Set.AsQueryable();
        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        return await query.ToListAsync();
    }

    public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate) => Set.AnyAsync(predicate);

    public async Task AddAsync(TEntity entity) => await Set.AddAsync(entity);

    public void Update(TEntity entity) => Set.Update(entity);

    public void Remove(TEntity entity) => Set.Remove(entity);

    public void RemoveRange(IEnumerable<TEntity> entities) => Set.RemoveRange(entities);

    public Task<int> SaveChangesAsync() => Context.SaveChangesAsync();
}
