using DutyAssignment.Interfaces;
using DutyAssignment.Models;

namespace DutyAssignment.Services
{
    public interface IDutyService
    {
        Task<IEnumerable<IDuty>> GetDuties();
        Task<IEnumerable<IDuty>> ProcessDutyExcelFilesAsync();
        Task<object> InsertDuties();
    }
}