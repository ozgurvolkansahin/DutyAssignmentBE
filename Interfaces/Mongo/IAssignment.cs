using DutyAssignment.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

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
    public interface IAssignmentLookupDuty : IEntity<string>
    {
        public string DutyId { get; set; }
        public IDuty Duty { get; set; }
        public DateTime date { get; set; }
    }
    public class AssignmentLookupDuty : IAssignmentLookupDuty
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string Id { get; set; }
        public required string DutyId { get; set; }
        public required IDuty Duty { get; set; }
        public DateTime date { get; set; }
    }
}
