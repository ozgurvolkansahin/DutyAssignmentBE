using DutyAssignment.DTOs;
using DutyAssignment.Interfaces;
using MongoDB.Driver;

namespace DutyAssignment.Repositories.Mongo.Duty
{
    public interface IPersonalRepository : IMongoRepository<IPersonalExcel, string>
    {
        Task InsertPersonalDataAsync(IEnumerable<IPersonalExcel> entities, int type = 1);
        Task<IEnumerable<IPersonalExcel>> GetPersonalById(IEnumerable<string> sicil);
        Task<IEnumerable<IPersonalExcel>> GetPersonalByIdAndType(IEnumerable<string> sicil, int type);
        Task PushDutyIdToDutyArray(string dutyId, List<string> sicil, int type);
        Task PullDutyIdFromDutyArray(string dutyId, List<string> sicil, int type);
        Task PushDutyIdToPaidDutyArray(string dutyId, List<string> sicil, int type);
        Task RemoveDutyIdFromPaidDutyArray(string dutyId, int type);
        Task<int> GetCountByTypeAsync(int type);
        Task<int> GetTotalPaymentsAsync();
        Task<int> GetTotalPaymentsByTypeAsync(int type);
        Task<int> GetTotalAssignedPersonal();
        Task<int> GetTotalAssignedPersonalByType(int type);
        Task<int> GetTotalPaymentsDone();
        Task<int> GetTotalPaymentsDoneByType(int type);
        Task<IGetAssignedPersonalByDutyIdWithPaginationResult<IPersonalExcel>> GetPersonalWithPagination(int page, int pageSize, int type, string order, string orderBy);
        Task<FilterPersonnelWithTotalCount> FilterPersonnel(FilterPersonnel filterPersonnel);
        Task<IEnumerable<IPersonalExcel>> GetAllPersonnelWithType(int type);
        Task<UpdateResult> ResetAssignment(string dutyId, int type);
        Task<DeleteResult> DeleteManyPersonnel(List<string> sicil);
    }
}