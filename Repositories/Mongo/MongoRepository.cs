using DutyAssignment.Interfaces;

using MongoDB.Bson;
using MongoDB.Driver;

namespace DutyAssignment.Repositories.Mongo
{
    public abstract class MongoRepository<TEntity, TId> : IDisposable, IMongoRepository<TEntity, TId> where TEntity : IEntity<TId>
    {
        public IMongoClient Client { get; }
        public IMongoDatabase Database { get; }
        public IMongoCollection<TEntity> _collection { get; }
        public MongoRepository(string collectionName)
        {
            Client = new MongoClient(Configuration.Config.Config.MongoUrl);
            Database = Client.GetDatabase(Configuration.Config.Config.DatabaseName);
            _collection = Database.GetCollection<TEntity>(collectionName);
        }

        public async Task<IEnumerable<TEntity>> GetAsync()
        {
            return await _collection.AsQueryable().ToListAsync();
        }

        public async Task<TEntity> GetOneAsync(TId id)
        {
            return await _collection.Find(x => x.Id!.Equals(id)).FirstOrDefaultAsync();
        }
        public async Task<int> GetCountAsync()
        {
            return Convert.ToInt32(await _collection.CountDocumentsAsync(_ => true));
        }

        public async Task InsertManyAsync(IEnumerable<TEntity> entities)
        {
            await _collection.InsertManyAsync(entities);
        }

        public async Task CreateAsync(TEntity entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(TId id, TEntity entity)
        {
            await _collection.ReplaceOneAsync(new BsonDocument("_id", new ObjectId()), entity);
        }
        public async Task UpdateManyAsync(FilterDefinition<TEntity> filter, UpdateDefinition<TEntity> update)
        {
            await _collection.UpdateManyAsync(filter, update);
        }

        public async Task DeleteAsync(TId id)
        {
            await _collection.DeleteOneAsync(x => x.Id!.Equals(id));
        }
        public async Task BulkWriteAsync(IEnumerable<WriteModel<TEntity>> requests)
        {
            await _collection.BulkWriteAsync(requests);
        }

        public void Dispose()
        {
        }
    }
}