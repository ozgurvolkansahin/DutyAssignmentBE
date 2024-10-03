using DutyAssignment.Models;

namespace DutyAssignment.Services
{
    public interface IDutyService
    {
        Task<List<Duty>> GetAsync();
        Task<Duty> GetAsync(string id);
        Task<Duty> CreateAsync(Duty duty);
        Task UpdateAsync(string id, Duty dutyIn);
        Task RemoveAsync(Duty dutyIn);
        Task RemoveAsync(string id);
    }
}