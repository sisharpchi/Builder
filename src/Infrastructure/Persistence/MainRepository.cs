using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;
using Core.Persistence;
using Infrastructure.Data;

namespace Infrastructure.Persistence;

internal class MainRepository : GenericRepository<MainDbContext>, IMainRepository
{
    public MainRepository(
        MainDbContext context,
        UnitOfWork<MainDbContext> unitOfWork) : base(context, unitOfWork)
    {
    }

    async Task<TEntity?> IMainRepository.GetAsync<TEntity>(object? id)
            where TEntity : class
    {
        var entity = await Context.Set<TEntity>().FindAsync(id);
        return entity;
    }

    Task<TEntity?> IMainRepository.GetAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
        where TEntity : class
    {
        return Context.Set<TEntity>().FirstOrDefaultAsync(predicate);
    }

    Task<List<TEntity>> IMainRepository.GetListAsync<TEntity>(Expression<Func<TEntity, bool>>? predicate)
         where TEntity : class
    {
        return predicate == default ? Context.Set<TEntity>().ToListAsync()
            : Context.Set<TEntity>().Where(predicate).ToListAsync();
    }

    Task<Dictionary<TKey, TEntity>> IMainRepository.GetDictionaryAsync<TKey, TEntity>(
        Func<TEntity, TKey> keySelector,
        Expression<Func<TEntity, bool>>? predicate)
        where TEntity : class
    {
        return predicate == default ? Context.Set<TEntity>().ToDictionaryAsync(keySelector)
            : Context.Set<TEntity>().Where(predicate).ToDictionaryAsync(keySelector);
    }

    IQueryable<TEntity> IMainRepository.Query<TEntity>(Expression<Func<TEntity, bool>>? predicate)
         where TEntity : class
    {
        return predicate == default
            ? Context.Set<TEntity>()
            : Context.Set<TEntity>().Where(predicate);
    }

    async Task IMainRepository.AddAsync<TEntity>(TEntity entity)
        where TEntity : class
    {
        await Context.Set<TEntity>().AddAsync(entity);
    }

    void IMainRepository.Update<TEntity>(TEntity entity)
        where TEntity : class
    {
        Context.Set<TEntity>().Attach(entity);
        Context.Entry(entity).State = EntityState.Modified;
    }

    void IMainRepository.Delete<TEntity>(TEntity? entity)
        where TEntity : class
    {
        if (entity == null)
            return;

        Context.Set<TEntity>().Remove(entity);
    }

    IExecutionStrategy IMainRepository.CreateExecutionStrategy()
    {
        return Context.Database.CreateExecutionStrategy();
    }

    Task<IDbContextTransaction> IMainRepository.BeginTransactionAsync()
    {
        return Context.Database.BeginTransactionAsync();
    }

    public Task AddRangeAsync<TEntity>(IEnumerable<TEntity> entities)
        where TEntity : class
    {
        return Context.Set<TEntity>().AddRangeAsync(entities);
    }

    DbSet<TEntity> IMainRepository.Set<TEntity>()
    {
        return Context.Set<TEntity>();
    }

    void IMainRepository.DeleteRange<TEntity>(IEnumerable<TEntity> entities)
    {
        Context.Set<TEntity>().RemoveRange(entities);
    }
}