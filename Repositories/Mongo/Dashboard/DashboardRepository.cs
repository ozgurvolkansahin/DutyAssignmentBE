using DutyAssignment.Interfaces;
using DutyAssignment.Models;
using MongoDB.Driver;

namespace DutyAssignment.Repositories.Mongo.Duty
{
    public class DashboardRepository : MongoRepository<IDashboard, string>, IDashboardRepository
    {
        public DashboardRepository() : base("environment")
        {

        }
        public async Task<IDashboard> GetDashboardData()
        {
            return await _collection.Find(x => x.Name.Equals("SystemInfo")).FirstOrDefaultAsync();
        }
    }
}