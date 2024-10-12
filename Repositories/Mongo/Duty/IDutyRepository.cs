using DutyAssignment.Interfaces;

namespace DutyAssignment.Repositories.Mongo.Duty
{
    public interface IDutyRepository : IMongoRepository<IDuty, string>
    {
      Task<IEnumerable<IDuty>> GetDutiesAsync();
      Task<IEnumerable<IDuty>> GetDutiesById(IEnumerable<string> ids);
    }
}