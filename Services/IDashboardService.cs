using DutyAssignment.DTOs;
namespace DutyAssignment.Services
{
    public interface IDashboardService
    {
        Task<IDashboardDTO> GetDashboardData(int pageNumber, int pageSize);
    }
}