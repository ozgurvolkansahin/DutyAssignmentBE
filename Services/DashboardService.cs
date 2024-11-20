using DutyAssignment.DTOs;
using DutyAssignment.Enum;
using DutyAssignment.Interfaces;
using DutyAssignment.Models;
using DutyAssignment.Repositories.Mongo.Duty;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DutyAssignment.Services;

public class DashboardService : IDashboardService
{
    private readonly IDutyRepository _dutyRepository;
    private readonly IPersonalRepository _personalRepository;
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IDashboardRepository _dashboardRepository;

    public DashboardService(IDutyRepository dutyRepository, IPersonalRepository personalRepository, IAssignmentRepository assignmentRepository, IDashboardRepository dashboardRepository)
    {
        _dutyRepository = dutyRepository;
        _personalRepository = personalRepository;
        _assignmentRepository = assignmentRepository;
        _dashboardRepository = dashboardRepository;
    }

    // var assignmentsLookup = await _assignmentRepository.SortAssignmentsByDateAndGetByPage(pageNumber, pageSize);

    public async Task<IDashboardDTO> GetDashboardData()
    {
        var branchesInfo = new List<IBranchInfo>
        {
            new BranchInfo("Kadro", await GetBranchData((int)PersonnelTypeEnum.KADRO)),
            new BranchInfo("Şube", await GetBranchData((int)PersonnelTypeEnum.SUBE)),
            new BranchInfo("Çevik", await GetBranchData((int)PersonnelTypeEnum.CEVIK))
        };

        var dashboardData = await _dashboardRepository.GetDashboardData();
        var dashboardInfo = new DashboardDTO(
            dashboardData,
            branchesInfo
        );
        return dashboardInfo;
    }

    private async Task<IBranchData> GetBranchData(int type)
    {
        var totalDuties = await _dutyRepository.GetDutiesCountByType(type);
        var totalAssignments = await _personalRepository.GetCountByTypeAsync(type);
        var totalPayments = await _personalRepository.GetTotalPaymentsByTypeAsync(type);
        var totalAssignedPersonal = await _personalRepository.GetTotalAssignedPersonalByType(type);
        var totalPaymentsDone = await _personalRepository.GetTotalPaymentsDoneByType(type);
        // var waitingAssignmentsCount = await _assignmentRepository.GetWaitingAssignmentsCount();
        return new BranchData(totalDuties, totalAssignments, totalPayments, totalAssignedPersonal, totalPaymentsDone);
    }
    public async Task<IBranchDashboardDTO> GetBranchDashboardData(int pageNumber, int pageSize, int type)
    {
        var branchData = await GetBranchData(type);
        var assignmentsLookup = await _assignmentRepository.SortAssignmentsByDateTypeAndGetByPage(pageNumber, pageSize, type);
        var waitingAssignmentsCount = await _assignmentRepository.GetWaitingAssignmentsByTypeCount(type);

        return new BranchDashboardDTO(branchData.TotalDuties, branchData.TotalAssignments, branchData.TotalPayments, assignmentsLookup, branchData.TotalAssignedPersonal, waitingAssignmentsCount, branchData.TotalPaymentsDone);
    }
}