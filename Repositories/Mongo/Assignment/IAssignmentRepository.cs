using DutyAssignment.DTOs;
using DutyAssignment.Interfaces;
using DutyAssignment.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DutyAssignment.Repositories.Mongo.Duty
{
  public interface IAssignmentRepository : IMongoRepository<IAssignment, string>
  {
    Task<IAssignment> GeAssignmentByDutyId(string dutyId);
    Task<IAssignment> GetAssignmentByDutyIdAndType(string dutyId, int type);
    Task<IGetAssignedPersonalByDutyIdWithPaginationResult<object>> GetPaidAssignments(int pageNumber, int pageSize);
    Task<IEnumerable<PeopleCount>> GetOccurrencesOfSpecificValues(BsonArray specificValues);
    Task SetAssignmentPaid(string dutyId, IEnumerable<string> sicil, int type);
    Task SetAssignmentUnPaid(string dutyId, IEnumerable<string> sicil, DateTime lastAssignmentDate, int type);
    Task<IEnumerable<IAssignmentLookupDuty>> SortAssignmentsByDateAndGetByPage(int page, int pageSize);
    Task<IEnumerable<IAssignmentLookupDuty>> SortAssignmentsByDateTypeAndGetByPage(int page, int pageSize, int type);
    Task<int> GetWaitingAssignmentsCount();
    Task<int> GetWaitingAssignmentsByTypeCount(int type);
    Task<IGetAssignedPersonalByDutyIdWithPaginationResult<IPersonalExcel>> GetAssignedPersonalByDutyIdWithPagination(string dutyId, int page, int pageSize);
    Task<IGetAssignedPersonalByDutyIdWithPaginationResult<IPersonalExcel>> GetAssignedPersonalByDutyIdAndTypeWithPagination(string dutyId, int page, int pageSize, int type);
    Task<FilterAssignmentsByFilter> FilterAssignments(FilterAssignments filterAssignments);
    Task<UpdateResult> ResetAssignment(string dutyId, int type);
    Task<DeleteResult> DeleteAssignment(string dutyId);
  }
}