using DutyAssignment.DTOs;
using DutyAssignment.Interfaces;

namespace DutyAssignment.Models;

public class DashboardDTO : IDashboardDTO
{
    public IDashboard Dashboard { get; set; }
    public int TotalDuties { get; set; }
    public int TotalAssignments { get; set; }
    public int TotalPayments { get; set; }
    public int TotalAssignedPersonal { get; set; }
    public IEnumerable<IAssignmentLookupDuty> AssignmentLookupDuty { get; set; }
    // constructor
    public DashboardDTO(IDashboard dashboard, int totalDuties, int totalAssignments, int totalPayments, IEnumerable<IAssignmentLookupDuty> assignmentLookupDuty, int totalAssignedPersonal)
    {
        Dashboard = dashboard;
        TotalDuties = totalDuties;
        TotalAssignments = totalAssignments;
        TotalPayments = totalPayments;
        AssignmentLookupDuty = assignmentLookupDuty;
        TotalAssignedPersonal = totalAssignedPersonal;
    }
}