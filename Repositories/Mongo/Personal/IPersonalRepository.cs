using DutyAssignment.Interfaces;

namespace DutyAssignment.Repositories.Mongo.Duty
{
    public interface IPersonalRepository : IMongoRepository<IPersonalExcel, string>
    {
        Task InsertPersonalDataAsync(IEnumerable<IPersonalExcel> entities);
        Task<IEnumerable<IPersonalExcel>> GetPersonalById(IEnumerable<string> sicil);

    }
}