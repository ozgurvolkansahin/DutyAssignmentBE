using DutyAssignment.Interfaces;
using DutyAssignment.Models;
using MongoDB.Bson;

namespace DutyAssignment.Repositories.Mongo.Duty
{
    public interface IAssignmentRepository : IMongoRepository<IAssignment, string>
    {
      Task<IAssignment> GeAssignmentByDutyId(string dutyId);
      Task<IEnumerable<PeopleCount>> GetOccurrencesOfSpecificValues(BsonArray specificValues);
      Task SetAssignmentPaid(string dutyId, IEnumerable<string> sicil);
      Task SetAssignmentUnPaid(string dutyId, IEnumerable<string> sicil, DateTime lastAssignmentDate);
    }
}