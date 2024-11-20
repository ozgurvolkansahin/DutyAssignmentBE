using DutyAssignment.DTOs;
using DutyAssignment.Interfaces;
using MongoDB.Driver;

namespace DutyAssignment.Repositories.Mongo.Duty
{
    public class DutyRepository : MongoRepository<IDuty, string>, IDutyRepository
    {
        public DutyRepository() : base("duty")
        {
        }
        public async Task<IEnumerable<IDuty>> GetDutiesAsync()
        {
            return await GetAsync();
        }
        public async Task<IEnumerable<IDuty>> GetDutiesById(IEnumerable<string> ids)
        {
            var filter = Builders<IDuty>.Filter.In(x => x.DutyId, ids);
            return await _collection.Find(filter).ToListAsync();
        }
        public async Task<IEnumerable<IDuty>> GetDutiesByIdAndType(IEnumerable<string> ids, int type)
        {
            var filter = Builders<IDuty>.Filter.In(x => x.DutyId, ids) & Builders<IDuty>.Filter.Eq(x => x.Type, type);
            return await _collection.Find(filter).ToListAsync();
        }
        public async Task<IEnumerable<IDuty>> GetDutiesByIdWithPagination(IEnumerable<string> dutyIds, int page, int pageSize)
        {
            var filter = Builders<IDuty>.Filter.In(d => d.DutyId, dutyIds);
            var skip = (page - 1) * pageSize;
            var duties = await _collection.Find(filter)
                                        .Skip(skip)
                                        .Limit(pageSize)
                                        .ToListAsync();
            return duties;
        }
        public async Task<IEnumerable<IDuty>> GetDutiesByIdAndTypeWithPagination(IEnumerable<string> dutyIds, int page, int pageSize, int type)
        {
            var filter = Builders<IDuty>.Filter.In(d => d.DutyId, dutyIds);
            filter = filter & Builders<IDuty>.Filter.Eq(x => x.Type, type);
            var skip = (page - 1) * pageSize;
            var duties = await _collection.Find(filter)
                                        .Skip(skip)
                                        .Limit(pageSize)
                                        .ToListAsync();
            return duties;
        }
        public async Task<IEnumerable<IDuty>> GetDutiesByIdWithPaginationAndFilter(FilterAssignments filterAssignments)
        {
            // search with regex
            var filter = Builders<IDuty>.Filter.Regex(x => x.DutyId, new MongoDB.Bson.BsonRegularExpression(filterAssignments.dutyId)) &
                Builders<IDuty>.Filter.Regex(x => x.Description, new MongoDB.Bson.BsonRegularExpression(filterAssignments.dutyDescription));
            var skip = (filterAssignments.page - 1) * filterAssignments.pageSize;
            var duties = await _collection.Find(filter)
                                        .Skip(skip)
                                        .Limit(filterAssignments.pageSize)
                                        .ToListAsync();
            return duties;

        }
        public async Task<DeleteResult> DeleteDuty(string dutyId)
        {
            var filter = Builders<IDuty>.Filter.Eq(x => x.DutyId, dutyId);
            return await _collection.DeleteOneAsync(filter);
        }
        public async Task<int> GetDutiesCountByType(int type)
        {
            var filter = Builders<IDuty>.Filter.Eq(x => x.Type, type);
            return (int)await _collection.CountDocumentsAsync(filter);
        }
    }
}