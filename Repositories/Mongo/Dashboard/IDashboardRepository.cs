using DutyAssignment.Interfaces;

namespace DutyAssignment.Repositories.Mongo.Duty
{
    public interface IDashboardRepository : IMongoRepository<IDashboard, string>
    {
      Task<IDashboard> GetDashboardData();
    }
}