using DutyAssignment.Interfaces;
using DutyAssignment.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DutyAssignment.Services
{
    public interface IDutyService
    {
        Task<IEnumerable<IDuty>> GetDuties();
        Task<IEnumerable<IDuty>> GetDutiesByIdListWithPagination(string sicil, int page, int pageSize, bool isPaidDuties);
        Task<IEnumerable<IDuty>> ProcessDutyExcelFilesAsync(int type);
        Task<object> InsertDuties(int type);
        Task<IEnumerable<PeopleCount>> GetOccurrencesOfSpecificValues(BsonArray specificValues);
        Task<DeleteResult> DeleteDuty(string dutyId);
    }
}