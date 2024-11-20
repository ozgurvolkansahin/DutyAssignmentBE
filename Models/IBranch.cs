using DutyAssignment.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DutyAssignment.DTOs
{
    public interface IBranchData
    {
        int TotalDuties { get; set; }
        int TotalAssignments { get; set; }
        int TotalPayments { get; set; }
        int TotalPaymentsDone { get; set; }
        int TotalAssignedPersonal { get; set; }
    }
    public interface IBranchInfo
    {
        public string BranchName { get; set; }
        public IBranchData Data { get; set; }
    }
}
