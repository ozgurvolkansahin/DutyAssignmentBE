using DutyAssignment.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DutyAssignment.Models;

public class DutyAssignments : IDutyAssignments
{
    [BsonElement("duty_id")]
    public string DutyId { get; set; }
    [BsonElement("personal")]
    public required IEnumerable<IPersonalExcel> Personal { get; set; }
    [BsonElement("date")]
    public DateTime Date { get; set; }
    public string Id { get; set; }
}
