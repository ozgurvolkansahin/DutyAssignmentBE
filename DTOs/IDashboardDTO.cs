using DutyAssignment.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DutyAssignment.DTOs
{
    public interface IDashboardDTO
    {
        IDashboard Dashboard { get; set; }
        int TotalDuties { get; set; }
        int TotalAssignments { get; set; }
        int TotalPayments { get; set; }
        int WaitingAssignmentsCount { get; set; }
        int TotalPaymentsDone { get; set; }
        IEnumerable<IAssignmentLookupDuty> AssignmentLookupDuty { get; set; }
        int TotalAssignedPersonal { get; set; }
    }
}
