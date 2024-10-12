using DutyAssignment.Interfaces;
using DutyAssignment.Models;
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
    }
}