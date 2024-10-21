using DutyAssignment.DTOs;
using DutyAssignment.Interfaces;

namespace DutyAssignment.Repositories.Mongo.Duty
{
    public interface IPersonalRepository : IMongoRepository<IPersonalExcel, string>
    {
        Task InsertPersonalDataAsync(IEnumerable<IPersonalExcel> entities);
        Task<IEnumerable<IPersonalExcel>> GetPersonalById(IEnumerable<string> sicil);
        Task PushDutyIdToDutyArray(string dutyId, List<string> sicil);
        Task PushDutyIdToPaidDutyArray(string dutyId, List<string> sicil);
        Task RemoveDutyIdFromPaidDutyArray(string dutyId);
        Task<int> GetTotalPaymentsAsync();
        Task<int> GetTotalAssignedPersonal();
        Task<IGetAssignedPersonalByDutyIdWithPaginationResult<IPersonalExcel>> GetPersonalWithPagination(int page, int pageSize);
        Task<FilterPersonnelWithTotalCount> FilterPersonnel(FilterPersonnel filterPersonnel);
    }
}