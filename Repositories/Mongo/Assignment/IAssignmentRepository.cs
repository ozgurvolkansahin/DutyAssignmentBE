using DutyAssignment.DTOs;
using DutyAssignment.Interfaces;
using DutyAssignment.Models;
using MongoDB.Bson;

namespace DutyAssignment.Repositories.Mongo.Duty
{
    public interface IAssignmentRepository : IMongoRepository<IAssignment, string>
    {
      Task<IAssignment> GeAssignmentByDutyId(string dutyId);
      Task<IGetAssignedPersonalByDutyIdWithPaginationResult<object>> GetPaidAssignments(int pageNumber, int pageSize);
      Task<IEnumerable<PeopleCount>> GetOccurrencesOfSpecificValues(BsonArray specificValues);
      Task SetAssignmentPaid(string dutyId, IEnumerable<string> sicil);
      Task SetAssignmentUnPaid(string dutyId, IEnumerable<string> sicil, DateTime lastAssignmentDate);
      Task<IEnumerable<IAssignmentLookupDuty>> SortAssignmentsByDateAndGetByPage(int page, int pageSize);
      Task<int> GetWaitingAssignmentsCount();
      Task<IGetAssignedPersonalByDutyIdWithPaginationResult<IPersonalExcel>> GetAssignedPersonalByDutyIdWithPagination(string dutyId, int page, int pageSize);
    }
}