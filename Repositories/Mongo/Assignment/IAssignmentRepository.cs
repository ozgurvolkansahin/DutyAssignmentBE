using DutyAssignment.Interfaces;
using DutyAssignment.Models;
using MongoDB.Bson;

namespace DutyAssignment.Repositories.Mongo.Duty
{
    public interface IAssignmentRepository : IMongoRepository<IAssignment, string>
    {
      Task<IAssignment> GeAssignmentByDutyId(string dutyId);
      Task<IEnumerable<PeopleCount>> GetOccurrencesOfSpecificValues(string[] specificValues);
    }
}