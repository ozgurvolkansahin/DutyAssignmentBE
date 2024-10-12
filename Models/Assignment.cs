using DutyAssignment.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace DutyAssignment.Models;

public class Assignment : IAssignment
{
    public required string DutyId { get; set; }
    public required IEnumerable<string> ResponsibleManagers { get; set; }
    public required IEnumerable<string> PoliceAttendants { get; set; }
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string Id { get; set; }
}