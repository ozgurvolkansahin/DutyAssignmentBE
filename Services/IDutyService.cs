using DutyAssignment.Interfaces;
using DutyAssignment.Models;

namespace DutyAssignment.Services
{
    public interface IDutyService
    {
        Task<IEnumerable<IDuty>> GetDuties();
        IEnumerable<IDuty> ProcessDutyExcelFiles();
         Task<object> InsertDuties();
    }
}