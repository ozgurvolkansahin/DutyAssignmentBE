using DutyAssignment.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace DutyAssignment.Models;

public class Assignment : IAssignment
{
    public required string DutyId { get; set; }
    public int Type { get; set; }
    public required IEnumerable<string> ResponsibleManagers { get; set; }
    public required IEnumerable<string> PoliceAttendants { get; set; }
    public required IEnumerable<string> PaidPersonal { get; set; }
    public required IEnumerable<PreviousAssignments> PreviousAssignments { get; set; }
    public DateTime AssignmentDate { get; set; }
    public DateTime LastUpdate { get; set; }
    public bool IsActive { get; set; }
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string Id { get; set; }
}

public class PreviousAssignments
{
    public IEnumerable<string> Personal { get; set; }
    public DateTime Date { get; set; }
}

public class PaidPersonnelInDuty
{
    public IEnumerable<string> Personnel { get; set; }
    public string DutyId { get; set; }
    public int Type { get; set; }
}