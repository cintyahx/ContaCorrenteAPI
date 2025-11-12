namespace Miotto.BankMore.Conta.Domain.Interfaces
{
    public interface IRepository<TEntity> where TEntity : IEntity
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity?> GetAsync(Guid id);
        Task<TEntity> CreateAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
        Task DeleteAsync(Guid id);
    }
}
