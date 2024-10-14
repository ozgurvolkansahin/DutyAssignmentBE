using DutyAssignment.Interfaces;
using DutyAssignment.Models;
using MongoDB.Bson;

namespace DutyAssignment.Services
{
    public interface IDutyService
    {
        Task<IEnumerable<IDuty>> GetDuties();
        Task<IEnumerable<IDuty>> ProcessDutyExcelFilesAsync();
        Task<object> InsertDuties();
        Task<IEnumerable<PeopleCount>> GetOccurrencesOfSpecificValues(string[] specificValues);
    }
}