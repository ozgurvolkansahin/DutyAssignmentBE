using DutyAssignment.Models;

namespace DutyAssignment.Interfaces
{
    public interface IAssignment : IEntity<string>
    {
        string DutyId { get; set; }
        IEnumerable<string> ResponsibleManagers { get; set; }
        IEnumerable<string> PoliceAttendants { get; set; }
        IEnumerable<string> PaidPersonal { get; set; }
        IEnumerable<PreviousAssignments> PreviousAssignments { get; set; }
        DateTime AssignmentDate { get; set; }
        DateTime LastUpdate { get; set; }
        bool IsActive { get; set; }
    }
}
