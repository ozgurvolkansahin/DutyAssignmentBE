using DutyAssignment.Interfaces;
using MongoDB.Driver;

namespace DutyAssignment.Repositories.Mongo
{
    public interface IMongoRepository<TEntity, TId> where TEntity : IEntity<TId>
    {
        Task<IEnumerable<TEntity>> GetAsync();
        Task<TEntity> GetOneAsync(TId id);
        Task<int> GetCountAsync();
        Task CreateAsync(TEntity entity);
        Task InsertManyAsync(IEnumerable<TEntity> entities);
        Task UpdateAsync(TId id, TEntity entity);
        Task UpdateManyAsync(FilterDefinition<TEntity> filter, UpdateDefinition<TEntity> update);
        Task DeleteAsync(TId id);
    }
}