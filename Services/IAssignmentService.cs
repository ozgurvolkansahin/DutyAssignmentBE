using DutyAssignment.DTOs;
using DutyAssignment.Interfaces;
using DutyAssignment.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DutyAssignment.Services
{
    public interface IAssignmentService
    {
        Task<IEnumerable<PersonalExcel>> SelectPersonalToBePaid(string dutyId, int numToSelect, bool reAssign, int type);
        Task<IEnumerable<IDutyAssignments>> GetAssignments(IEnumerable<string> dutyIds, int numToSelect, bool reAssign, int type);
        Task<IGetAssignedPersonalByDutyIdWithPaginationResult<object>> GetPaidAssignments(int pageNumber, int pageSize);
        Task<IGetAssignedPersonalByDutyIdWithPaginationResult<IPersonalExcel>> GetAssignedPersonalByDutyIdWithPagination(string dutyId, int page, int pageSize);
        Task<byte[]> DownloadPersonalReportForSpecificDuty(string dutyId, int type);
        Task<FilterAssignmentsByFilter> FilterAssignments(FilterAssignments filterAssignments);
        Task<UpdateResult> ResetAssignment(string dutyId);
        Task<string> ProcessPaidDuties();
    }
}