using DutyAssignment.DTOs;
using DutyAssignment.Interfaces;
using MongoDB.Driver;

namespace DutyAssignment.Repositories.Mongo.Duty
{
    public interface IDutyRepository : IMongoRepository<IDuty, string>
    {
      Task<IEnumerable<IDuty>> GetDutiesAsync();
      Task<IEnumerable<IDuty>> GetDutiesById(IEnumerable<string> ids);
      Task<IEnumerable<IDuty>> GetDutiesByIdWithPagination(IEnumerable<string> dutyIds, int page, int pageSize);
      Task<IEnumerable<IDuty>> GetDutiesByIdWithPaginationAndFilter(FilterAssignments filterAssignments);
      Task<DeleteResult> DeleteDuty(string dutyId);
    }
}