using DutyAssignment.Interfaces;
using DutyAssignment.Models;
using MongoDB.Bson;

namespace DutyAssignment.Services
{
    public interface IAssignmentService
    {
        Task<IEnumerable<PersonalExcel>> SelectPersonalToBePaid(string dutyId, int numToSelect, bool reAssign);
        Task<IEnumerable<IDutyAssignments>> GetAssignments(IEnumerable<string> dutyIds, int numToSelect, bool reAssign);
    }
}