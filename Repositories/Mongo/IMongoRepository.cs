using DutyAssignment.Interfaces;

namespace DutyAssignment.Repositories.Mongo
{
    public interface IMongoRepository<TEntity, TId> where TEntity : IEntity<TId>
    {
        Task<IEnumerable<TEntity>> GetAsync();
        Task<TEntity> GetOneAsync(TId id);
        Task CreateAsync(TEntity entity);
        Task UpdateAsync(TId id, TEntity entity);
        Task DeleteAsync(TId id);
    }
}