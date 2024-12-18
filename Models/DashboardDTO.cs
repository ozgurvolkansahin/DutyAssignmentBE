using DutyAssignment.DTOs;
using DutyAssignment.Interfaces;

namespace DutyAssignment.Models;

public class BranchDashboardDTO : IBranchDashboardDTO
{
    public int TotalDuties { get; set; }
    public int TotalAssignments { get; set; }
    public int TotalPayments { get; set; }
    public int TotalAssignedPersonal { get; set; }
    public int WaitingAssignmentsCount { get; set; }
    public int TotalPaymentsDone { get; set; }
    public IEnumerable<IAssignmentLookupDuty> AssignmentLookupDuty { get; set; }
    // constructor
    public BranchDashboardDTO(int totalDuties,
    int totalAssignments,
    int totalPayments, IEnumerable<IAssignmentLookupDuty> assignmentLookupDuty,
    int totalAssignedPersonal,
    int waitingAssignmentsCount,
    int totalPaymentsDone)
    {
        TotalDuties = totalDuties;
        TotalAssignments = totalAssignments;
        TotalPayments = totalPayments;
        AssignmentLookupDuty = assignmentLookupDuty;
        TotalAssignedPersonal = totalAssignedPersonal;
        WaitingAssignmentsCount = waitingAssignmentsCount;
        TotalPaymentsDone = totalPaymentsDone;
    }
}