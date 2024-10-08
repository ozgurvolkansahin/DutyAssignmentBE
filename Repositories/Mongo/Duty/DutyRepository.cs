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
        public async Task<IEnumerable<IDuty>> GetDutiesByDateAsync()
        {
            var obj = await GetAsync();
            return obj;
        }
    }

}