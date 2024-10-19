using DutyAssignment.DTOs;
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

    public async Task<IDashboardDTO> GetDashboardData(int pageNumber, int pageSize)
    {
        var totalDuties = await _dutyRepository.GetCountAsync();
        var totalAssignments = await _personalRepository.GetCountAsync();
        var totalPayments = await _personalRepository.GetTotalPaymentsAsync();
        var dashboardInfo = await _dashboardRepository.GetDashboardData();
        var assignmentsLookup = await _assignmentRepository.SortAssignmentsByDateAndGetByPage(pageNumber, pageSize);
        var totalAssignedPersonal = await _personalRepository.GetTotalAssignedPersonal();
        return new DashboardDTO(dashboardInfo, totalDuties, totalAssignments, totalPayments, assignmentsLookup, totalAssignedPersonal);
    }
}