using Microsoft.EntityFrameworkCore;
using Miotto.BankMore.Conta.Domain.Entities;
using Miotto.BankMore.Conta.Domain.Interfaces;

namespace Miotto.BankMore.Conta.Infra.Repositories;
public abstract class RepositoryBase<TEntity> : IRepository<TEntity> where TEntity : BaseEntity, IEntity, new()
{
    private static readonly IEnumerable<EntityState> EntityMaintainedStatus = [EntityState.Modified, EntityState.Added, EntityState.Deleted];

    private readonly BankMoreContext _bankMoreContext;
    protected DbSet<TEntity> Set { get; private set; }

    protected RepositoryBase(BankMoreContext dbContext)
    {
        _bankMoreContext = dbContext;
        Set = dbContext.Set<TEntity>();
    }

    public async Task<TEntity> CreateAsync(TEntity entity)
    {
        entity.CreateAt = DateTime.Now;

        var entry = await Set.AddAsync(entity);

        return entry.Entity;
    }

    public Task DeleteAsync(TEntity entity)
    {
        entity.IsActive = false;
        UpdateAsync(entity);

        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id)
    {
        var entity = GetAsync(id);
        DeleteAsync(entity.Result);

        return Task.CompletedTask;
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await Set.ToListAsync();
    }

    public async Task<TEntity?> GetAsync(Guid id)
    {
        if (id == Guid.Empty)
            return default;

        var entry = await Set.FindAsync(id);
        return entry;
    }

    public Task UpdateAsync(TEntity entity)
    {
        entity.UpdateAt = DateTime.Now;

        var entry = _bankMoreContext.Entry(entity);

        if (!EntityMaintainedStatus.Contains(entry.State))
        {
            entry.State = EntityState.Modified;
        }

        return Task.CompletedTask;
    }
}