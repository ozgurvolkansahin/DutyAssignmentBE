using DutyAssignment.DTOs;
namespace DutyAssignment.Services
{
    public interface IDashboardService
    {
        Task<IDashboardDTO> GetDashboardData();
        Task<IBranchDashboardDTO> GetBranchDashboardData(int pageNumber, int pageSize, int type);
    }
}