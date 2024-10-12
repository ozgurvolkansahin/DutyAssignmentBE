using DutyAssignment.Interfaces;

namespace DutyAssignment.Repositories.Mongo.Duty
{
    public interface IAssignmentRepository : IMongoRepository<IAssignment, string>
    {
      Task<IAssignment> GeAssignmentByDutyId(string dutyId);
    }
}