using DutyAssignment.DTOs;
using DutyAssignment.Interfaces;
using DutyAssignment.Models;
using MongoDB.Bson;

namespace DutyAssignment.Services
{
    public interface IAssignmentService
    {
        Task<IEnumerable<PersonalExcel>> SelectPersonalToBePaid(string dutyId, int numToSelect, bool reAssign);
        Task<IEnumerable<IDutyAssignments>> GetAssignments(IEnumerable<string> dutyIds, int numToSelect, bool reAssign);
        Task<IGetAssignedPersonalByDutyIdWithPaginationResult<object>> GetPaidAssignments(int pageNumber, int pageSize);
        Task<IGetAssignedPersonalByDutyIdWithPaginationResult<IPersonalExcel>> GetAssignedPersonalByDutyIdWithPagination(string dutyId, int page, int pageSize);
        Task<byte[]> DownloadPersonalReportForSpecificDuty(string dutyId);
    }
}